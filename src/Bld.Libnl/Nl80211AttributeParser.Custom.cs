using System.Runtime.InteropServices;
using Bld.Libnl.Types;

namespace Bld.Libnl;

/// <summary>
/// Custom parsers for nl80211 attributes
/// This file is not auto-generated and can be modified manually
/// </summary>
public static partial class Nl80211AttributeParser
{
    private delegate INl80211AttributeValue? AttributeParser(IntPtr nla);

    private static readonly Dictionary<Nl80211Attribute, AttributeParser> CustomParsers = new();

    static Nl80211AttributeParser()
    {
        // Register custom parsers here
        CustomParsers[Nl80211Attribute.NL80211_ATTR_SOFTWARE_IFTYPES] = ParseSupportedIfTypes;
        CustomParsers[Nl80211Attribute.NL80211_ATTR_SUPPORTED_IFTYPES] = ParseSupportedIfTypes;
        CustomParsers[Nl80211Attribute.NL80211_ATTR_WIPHY_BANDS] = ParseWiphyBands;
        CustomParsers[Nl80211Attribute.NL80211_ATTR_BANDS] = ParseWiphyBands;
    }

    private static partial bool HaveCustomParser(Nl80211Attribute attr)
    {
        return CustomParsers.ContainsKey(attr);
    }

    private static partial INl80211AttributeValue? ParseAttributeCustom(Nl80211Attribute attr, IntPtr nla)
    {
        if (CustomParsers.TryGetValue(attr, out var parser))
        {
            return parser(nla);
        }
        return null;
    }

    // Add custom parsing methods here

    /// <summary>
    /// Parse NL80211_ATTR_SUPPORTED_IFTYPES - nested attribute containing flags
    /// Each flag represents a supported interface type (index = interface type value)
    /// </summary>
    private static INl80211AttributeValue? ParseSupportedIfTypes(IntPtr nla)
    {
        if (nla == IntPtr.Zero)
            return null;

        var supportedTypes = new HashSet<Nl80211InterfaceType>();

        // Iterate through nested attributes
        foreach (var attr in nla.EnumerateNested())
        {
            // Get attribute type (which represents the interface type index)
            var attrType = (int)LibNlNative.nla_type(attr);

            // The attribute type is the interface type value
            // Flags just indicate presence, so if we see it, it's supported
            if (Enum.IsDefined(typeof(Nl80211InterfaceType), attrType))
            {
                supportedTypes.Add((Nl80211InterfaceType)attrType);
            }
        }

        return Nl80211AttributeValue.FromInterfaceTypes(supportedTypes);
    }

    private static INl80211AttributeValue? ParseWiphyBands(IntPtr nla)
    {
        if (nla == IntPtr.Zero)
            return null;

        var bands = new List<BandInfo>();

        // Iterate through nested band attributes
        foreach (var bandAttr in nla.EnumerateNested())
        {
            // Get band type (2GHz, 5GHz, etc.) from attribute type
            var bandType = (int)LibNlNative.nla_type(bandAttr);

            // Always add band info, even if we don't recognize the band type
            // This ensures we don't skip bands that might be valid
            var bandInfo = new BandInfo
            {
                Band = (Nl80211Band)bandType
            };

            // Parse nested band attributes
            foreach (var attr in bandAttr.EnumerateNested())
            {
                // Get attribute type
                var attrType = (int)LibNlNative.nla_type(attr);

                if (Enum.IsDefined(typeof(Nl80211BandAttribute), attrType))
                {
                    var bandAttrType = (Nl80211BandAttribute)attrType;

                    // Parse band attribute based on type
                    INl80211AttributeValue? attrValue = bandAttrType switch
                    {
                        // Parse into managed models instead of IntPtr
                        Nl80211BandAttribute.NL80211_BAND_ATTR_FREQS =>
                            Nl80211AttributeValue.FromFrequencies(ParseFrequencies(attr)),
                        Nl80211BandAttribute.NL80211_BAND_ATTR_RATES =>
                            Nl80211AttributeValue.FromBitrates(ParseBitrates(attr)),
                        Nl80211BandAttribute.NL80211_BAND_ATTR_HT_MCS_SET => ParseBinary(attr),
                        Nl80211BandAttribute.NL80211_BAND_ATTR_HT_CAPA => Nl80211AttributeValue.FromU16(LibNlNative.nla_get_u16(attr)),
                        Nl80211BandAttribute.NL80211_BAND_ATTR_HT_AMPDU_FACTOR => Nl80211AttributeValue.FromU8(LibNlNative.nla_get_u8(attr)),
                        Nl80211BandAttribute.NL80211_BAND_ATTR_HT_AMPDU_DENSITY => Nl80211AttributeValue.FromU8(LibNlNative.nla_get_u8(attr)),
                        Nl80211BandAttribute.NL80211_BAND_ATTR_VHT_MCS_SET => ParseBinary(attr),
                        Nl80211BandAttribute.NL80211_BAND_ATTR_VHT_CAPA => Nl80211AttributeValue.FromU32(LibNlNative.nla_get_u32(attr)),
                        // IFTYPE_DATA is complex (HE/EHT), keep nested for now; can be modeled later
                        Nl80211BandAttribute.NL80211_BAND_ATTR_IFTYPE_DATA => Nl80211AttributeValue.FromNested(attr),
                        Nl80211BandAttribute.NL80211_BAND_ATTR_EDMG_CHANNELS => Nl80211AttributeValue.FromU8(LibNlNative.nla_get_u8(attr)),
                        Nl80211BandAttribute.NL80211_BAND_ATTR_EDMG_BW_CONFIG => Nl80211AttributeValue.FromU8(LibNlNative.nla_get_u8(attr)),
                        Nl80211BandAttribute.NL80211_BAND_ATTR_S1G_MCS_NSS_SET => ParseBinary(attr),
                        Nl80211BandAttribute.NL80211_BAND_ATTR_S1G_CAPA => ParseBinary(attr),
                        _ => null
                    };

                    if (attrValue != null)
                    {
                        bandInfo.Attributes[bandAttrType] = attrValue;
                    }
                }
            }

            bands.Add(bandInfo);
        }

        return Nl80211AttributeValue.FromBands(bands);
    }

    private static List<FrequencyInfo> ParseFrequencies(IntPtr nla)
    {
        var result = new List<FrequencyInfo>();

        // Each nested attribute here is a frequency entry with its own nested attributes
        foreach (var nlFreq in nla.EnumerateNested())
        {
            var info = new FrequencyInfo();

            foreach (var sub in nlFreq.EnumerateNested())
            {
                var t = (int)LibNlNative.nla_type(sub);

                if (!Enum.IsDefined(typeof(Nl80211FrequencyAttribute), t))
                    continue;

                switch ((Nl80211FrequencyAttribute)t)
                {
                    case Nl80211FrequencyAttribute.NL80211_FREQUENCY_ATTR_FREQ:
                        info.FrequencyMHz = LibNlNative.nla_get_u32(sub);
                        break;
                    case Nl80211FrequencyAttribute.NL80211_FREQUENCY_ATTR_OFFSET:
                        info.OffsetKHz = LibNlNative.nla_get_u32(sub);
                        break;
                    case Nl80211FrequencyAttribute.NL80211_FREQUENCY_ATTR_MAX_TX_POWER:
                        // Convert mBm (0.01 dBm units) to dBm
                        info.MaxTxPowerDbm = 0.01 * LibNlNative.nla_get_u32(sub);
                        break;
                    case Nl80211FrequencyAttribute.NL80211_FREQUENCY_ATTR_DISABLED:
                        info.Disabled = true;
                        break;
                    case Nl80211FrequencyAttribute.NL80211_FREQUENCY_ATTR_NO_IR:
                        info.NoIR = true;
                        break;
                    case Nl80211FrequencyAttribute.NL80211_FREQUENCY_ATTR_RADAR:
                        info.RadarDetection = true;
                        break;
                    default:
                        // Ignore other attributes for now
                        break;
                }
            }

            // Only add valid entries that have a frequency set
            if (info.FrequencyMHz != 0)
            {
                result.Add(info);
            }
        }

        return result;
    }

    private static List<BitrateInfo> ParseBitrates(IntPtr nla)
    {
        var result = new List<BitrateInfo>();

        foreach (var nlRate in nla.EnumerateNested())
        {
            var info = new BitrateInfo();

            foreach (var sub in nlRate.EnumerateNested())
            {
                var t = (int)LibNlNative.nla_type(sub);

                if (!Enum.IsDefined(typeof(Nl80211BitrateAttribute), t))
                    continue;

                switch ((Nl80211BitrateAttribute)t)
                {
                    case Nl80211BitrateAttribute.NL80211_BITRATE_ATTR_RATE:
                        // Stored as 100 kbps units -> 0.1 Mbps per unit
                        info.Mbps = 0.1 * LibNlNative.nla_get_u32(sub);
                        break;
                    case Nl80211BitrateAttribute.NL80211_BITRATE_ATTR_2GHZ_SHORTPREAMBLE:
                        info.ShortPreamble2GHz = true;
                        break;
                    default:
                        break;
                }
            }

            if (info.Mbps > 0)
            {
                result.Add(info);
            }
        }

        return result;
    }
}
