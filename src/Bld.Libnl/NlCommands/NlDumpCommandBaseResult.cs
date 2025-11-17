using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Bld.Libnl.Types;
using Bld.Libnl;

namespace Bld.Libnl.NlCommands;

internal abstract class NlDumpCommandBaseResult : NlCommandBaseResult<List<Dictionary<Nl80211Attribute, INl80211AttributeValue>>>
{
    private readonly Nl80211Command _command;
    private readonly bool _isSplitDumpSupported;
    private readonly Nl80211Attribute _indexingAttribute;

    private readonly ConcurrentDictionary<uint, Dictionary<Nl80211Attribute, INl80211AttributeValue>> _pendingEntities =
        new();

    public NlDumpCommandBaseResult(Nl80211Command command, bool isSplitDumpSupported, Nl80211Attribute indexingAttribute) :
        base()
    {
        _command = command;
        _isSplitDumpSupported = isSplitDumpSupported;
        _indexingAttribute = indexingAttribute;
    }

    protected override void BuildMessage(NlMsg msg)
    {
        var hdr = LibNlNative.genlmsg_put(
            msg,
            0, // portid (automatic)
            0, // sequence (automatic)
            Nl80211Id, // nl80211 family id
            0, // header length
            (NetlinkMessageFlags.NLM_F_REQUEST | NetlinkMessageFlags.NLM_F_DUMP),
            (byte)_command,
            0 // version
        );

        if (hdr == IntPtr.Zero)
        {
            throw new Exception("Failed to build netlink message");
        }

        // Request split dump if supported
        if (_isSplitDumpSupported)
        {
            var ret = LibNlNative.nla_put_flag(msg, (int)Nl80211Attribute.NL80211_ATTR_SPLIT_WIPHY_DUMP);
            if (ret != 0)
            {
                throw new Exception("Failed to set NL80211_ATTR_SPLIT_WIPHY_DUMP flag");
            }
        }
    }

    protected override List<Dictionary<Nl80211Attribute, INl80211AttributeValue>> GetResult()
    {
        return _pendingEntities.Values.ToList();
    }

    protected override unsafe int ProcessMessage(IntPtr msgPtr, IntPtr arg)
    {
        try
        {
            var nlMessageHeader = LibNlNative.nlmsg_hdr(msgPtr);
            var genNlMessageHeader = LibNlNative.genlmsg_hdr(nlMessageHeader);

            // Parse attributes
            var attrData = LibNlNative.genlmsg_attrdata(genNlMessageHeader, 0);
            var attrLen = LibNlNative.genlmsg_attrlen(genNlMessageHeader, 0);

            int maxAttr = Enum.GetValues<Nl80211Attribute>().Cast<int>().Max();
            var tb = stackalloc IntPtr[maxAttr + 1];
            var parseResult = LibNlNative.nla_parse(tb, maxAttr, attrData, attrLen, IntPtr.Zero);
            if (parseResult != 0)
            {
                return (int)NetlinkCallbackAction.NL_SKIP;
            }

            var attributes = new Dictionary<Nl80211Attribute, INl80211AttributeValue>();

            for (int i = 0; i <= maxAttr; i++)
            {
                if (tb[i] == IntPtr.Zero)
                {
                    continue;
                }

                var nl80211Attr = (Nl80211Attribute)i;
                var attrValue = Nl80211AttributeParser.ParseAttribute(nl80211Attr, tb[i]);
                if (attrValue != null)
                {
                    attributes[nl80211Attr] = attrValue;
                }
            }

            // Handle split dump aggregation
            // Determine entity ID based on command type

            if (!attributes.TryGetValue(_indexingAttribute, out var indexAttributeValue))
            {
                return (int)NetlinkCallbackAction.NL_SKIP; // No WIPHY attribute, skip
            }

            var entityId = indexAttributeValue.AsU32()!.Value;

            var storage = _pendingEntities.GetOrAdd(entityId,
                _ => new Dictionary<Nl80211Attribute, INl80211AttributeValue>());

            foreach (var kvp in attributes)
            {
                    // Merge strategy: for bands we need to accumulate data across split messages,
                    // for other attributes the last value wins (as before).
                    if (IsBandAttribute(kvp.Key) && storage.TryGetValue(kvp.Key, out var existingVal))
                    {
                        storage[kvp.Key] = MergeBandAttributes(existingVal, kvp.Value);
                    }
                    else
                    {
                        storage[kvp.Key] = kvp.Value;
                    }
            }

            return (int)NetlinkCallbackAction.NL_SKIP;
        }
        catch
        {
            return (int)NetlinkCallbackAction.NL_SKIP;
        }
    }

    private static bool IsBandAttribute(Nl80211Attribute attribute)
    {
        return attribute == Nl80211Attribute.NL80211_ATTR_WIPHY_BANDS
               || attribute == Nl80211Attribute.NL80211_ATTR_BANDS;
    }

    private static INl80211AttributeValue MergeBandAttributes(INl80211AttributeValue existing, INl80211AttributeValue incoming)
    {
        var existingBands = existing.AsBands() ?? new List<Bld.Libnl.Types.BandInfo>();
        var incomingBands = incoming.AsBands() ?? new List<Bld.Libnl.Types.BandInfo>();

        // Result map by band id
        var result = new Dictionary<Nl80211Band, BandInfo>();

        // Helper to get/create target band entry
        BandInfo Ensure(Nl80211Band band)
        {
            if (!result.TryGetValue(band, out var b))
            {
                b = new BandInfo { Band = band, Attributes = new Dictionary<Nl80211BandAttribute, INl80211AttributeValue>() };
                result[band] = b;
            }
            return b;
        }

        // Seed with existing
        foreach (var b in existingBands)
        {
            var target = Ensure(b.Band);
            foreach (var kv in b.Attributes)
            {
                target.Attributes[kv.Key] = kv.Value;
            }
        }

        // Merge incoming
        foreach (var b in incomingBands)
        {
            var target = Ensure(b.Band);

            foreach (var kv in b.Attributes)
            {
                var attr = kv.Key;
                var val = kv.Value;

                if (attr == Nl80211BandAttribute.NL80211_BAND_ATTR_FREQS)
                {
                    // Merge frequency lists de-duplicating by (FrequencyMHz, OffsetKHz)
                    var existingFreqs = target.Attributes.TryGetValue(attr, out var exVal)
                        ? (exVal.AsFrequencies() ?? new List<FrequencyInfo>())
                        : new List<FrequencyInfo>();
                    var incomingFreqs = val.AsFrequencies() ?? new List<FrequencyInfo>();

                    var map = new Dictionary<(uint freq, uint? offset), FrequencyInfo>();
                    foreach (var f in existingFreqs)
                    {
                        map[(f.FrequencyMHz, f.OffsetKHz)] = f;
                    }

                    foreach (var f in incomingFreqs)
                    {
                        var key = (f.FrequencyMHz, f.OffsetKHz);
                        if (map.TryGetValue(key, out var existingF))
                        {
                            // Combine flags and max power conservatively
                            existingF.Disabled |= f.Disabled;
                            existingF.NoIR |= f.NoIR;
                            existingF.RadarDetection |= f.RadarDetection;
                            if (f.MaxTxPowerDbm.HasValue)
                            {
                                if (!existingF.MaxTxPowerDbm.HasValue)
                                    existingF.MaxTxPowerDbm = f.MaxTxPowerDbm;
                                else
                                    existingF.MaxTxPowerDbm = Math.Max(existingF.MaxTxPowerDbm.Value, f.MaxTxPowerDbm.Value);
                            }
                        }
                        else
                        {
                            map[key] = f;
                        }
                    }

                    target.Attributes[attr] = Nl80211AttributeValue.FromFrequencies(map.Values.ToList());
                }
                else if (attr == Nl80211BandAttribute.NL80211_BAND_ATTR_RATES)
                {
                    // Merge bitrate lists by Mbps
                    var existingRates = target.Attributes.TryGetValue(attr, out var exVal)
                        ? (exVal.AsBitrates() ?? new List<BitrateInfo>())
                        : new List<BitrateInfo>();
                    var incomingRates = val.AsBitrates() ?? new List<BitrateInfo>();

                    var map = new Dictionary<double, BitrateInfo>();
                    foreach (var r in existingRates)
                    {
                        map[r.Mbps] = r;
                    }
                    foreach (var r in incomingRates)
                    {
                        if (map.TryGetValue(r.Mbps, out var existingR))
                        {
                            existingR.ShortPreamble2GHz |= r.ShortPreamble2GHz;
                        }
                        else
                        {
                            map[r.Mbps] = r;
                        }
                    }

                    target.Attributes[attr] = Nl80211AttributeValue.FromBitrates(map.Values.ToList());
                }
                else
                {
                    // For scalar or blob attributes, prefer existing when present; otherwise take new
                    if (!target.Attributes.ContainsKey(attr))
                    {
                        target.Attributes[attr] = val;
                    }
                }
            }
        }

        var merged = result.Values.ToList();
        return Nl80211AttributeValue.FromBands(merged);
    }
}
