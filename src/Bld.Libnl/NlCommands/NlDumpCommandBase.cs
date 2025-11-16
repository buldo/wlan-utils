using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Bld.Libnl.Types;

namespace Bld.Libnl.NlCommands;

internal abstract class NlDumpCommandBase : NlCommandBase<List<Dictionary<Nl80211Attribute, INl80211AttributeValue>>>
{
    private readonly Nl80211Command _command;
    private readonly bool _isSplitDumpSupported;
    private readonly Nl80211Attribute _indexingAttribute;

    private readonly ConcurrentDictionary<uint, Dictionary<Nl80211Attribute, INl80211AttributeValue>> _pendingEntities =
        new();

    public NlDumpCommandBase(Nl80211Command command, bool isSplitDumpSupported, Nl80211Attribute indexingAttribute) :
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
                storage[kvp.Key] = kvp.Value;
            }

            return (int)NetlinkCallbackAction.NL_SKIP;
        }
        catch
        {
            return (int)NetlinkCallbackAction.NL_SKIP;
        }
    }
}
