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
    private static unsafe INl80211AttributeValue? ParseSupportedIfTypes(IntPtr nla)
    {
        if (nla == IntPtr.Zero)
            return null;

        var supportedTypes = new HashSet<Nl80211InterfaceType>();

        // Get nested data pointer and length
        var data = LibNlNative.nla_data(nla);
        var len = LibNlNative.nla_len(nla);

        if (data == IntPtr.Zero || len <= 0)
            return Nl80211AttributeValue.FromInterfaceTypes(supportedTypes);

        // Iterate through nested attributes
        var remaining = len;
        var current = data;

        while (LibNlNative.nla_ok(current, remaining))
        {
            // Get attribute type (which represents the interface type index)
            var attrType = LibNlNative.nla_type(current);

            // The attribute type is the interface type value
            // Flags just indicate presence, so if we see it, it's supported
            if (Enum.IsDefined(typeof(Nl80211InterfaceType), (int)attrType))
            {
                supportedTypes.Add((Nl80211InterfaceType)(int)attrType);
            }

            // Move to next attribute
            current = LibNlNative.nla_next(current, &remaining);
        }

        return Nl80211AttributeValue.FromInterfaceTypes(supportedTypes);
    }

    /// <summary>
    /// Parse NL80211_ATTR_WIPHY_BANDS - nested array containing band information
    /// Each nested element represents a band (2.4GHz, 5GHz, etc.) with its attributes
    /// </summary>
    private static unsafe INl80211AttributeValue? ParseWiphyBands(IntPtr nla)
    {
        if (nla == IntPtr.Zero)
            return null;

        var bands = new List<BandInfo>();

        // Get nested data pointer and length
        var data = LibNlNative.nla_data(nla);
        var len = LibNlNative.nla_len(nla);

        if (data == IntPtr.Zero || len <= 0)
            return Nl80211AttributeValue.FromBands(bands);

        // Iterate through nested band attributes
        var remaining = len;
        var current = data;

        while (LibNlNative.nla_ok(current, remaining))
        {
            // Get band type (2GHz, 5GHz, etc.) from attribute type
            var bandType = LibNlNative.nla_type(current);

            if (Enum.IsDefined(typeof(Nl80211Band), (int)bandType))
            {
                var bandInfo = new BandInfo
                {
                    Band = (Nl80211Band)(int)bandType
                };

                // Parse nested band attributes
                var bandData = LibNlNative.nla_data(current);
                var bandLen = LibNlNative.nla_len(current);

                if (bandData != IntPtr.Zero && bandLen > 0)
                {
                    var bandRemaining = bandLen;
                    var bandCurrent = bandData;

                    while (LibNlNative.nla_ok(bandCurrent, bandRemaining))
                    {
                        var attrType = LibNlNative.nla_type(bandCurrent);

                        if (Enum.IsDefined(typeof(Nl80211BandAttribute), (int)attrType))
                        {
                            var bandAttr = (Nl80211BandAttribute)(int)attrType;

                            // Parse band attribute based on type
                            INl80211AttributeValue? attrValue = bandAttr switch
                            {
                                Nl80211BandAttribute.NL80211_BAND_ATTR_FREQS => Nl80211AttributeValue.FromNested(bandCurrent),
                                Nl80211BandAttribute.NL80211_BAND_ATTR_RATES => Nl80211AttributeValue.FromNested(bandCurrent),
                                Nl80211BandAttribute.NL80211_BAND_ATTR_HT_MCS_SET => ParseBinary(bandCurrent),
                                Nl80211BandAttribute.NL80211_BAND_ATTR_HT_CAPA => Nl80211AttributeValue.FromU16(LibNlNative.nla_get_u16(bandCurrent)),
                                Nl80211BandAttribute.NL80211_BAND_ATTR_HT_AMPDU_FACTOR => Nl80211AttributeValue.FromU8(LibNlNative.nla_get_u8(bandCurrent)),
                                Nl80211BandAttribute.NL80211_BAND_ATTR_HT_AMPDU_DENSITY => Nl80211AttributeValue.FromU8(LibNlNative.nla_get_u8(bandCurrent)),
                                Nl80211BandAttribute.NL80211_BAND_ATTR_VHT_MCS_SET => ParseBinary(bandCurrent),
                                Nl80211BandAttribute.NL80211_BAND_ATTR_VHT_CAPA => Nl80211AttributeValue.FromU32(LibNlNative.nla_get_u32(bandCurrent)),
                                Nl80211BandAttribute.NL80211_BAND_ATTR_IFTYPE_DATA => Nl80211AttributeValue.FromNested(bandCurrent),
                                Nl80211BandAttribute.NL80211_BAND_ATTR_EDMG_CHANNELS => Nl80211AttributeValue.FromU8(LibNlNative.nla_get_u8(bandCurrent)),
                                Nl80211BandAttribute.NL80211_BAND_ATTR_EDMG_BW_CONFIG => Nl80211AttributeValue.FromU8(LibNlNative.nla_get_u8(bandCurrent)),
                                Nl80211BandAttribute.NL80211_BAND_ATTR_S1G_MCS_NSS_SET => ParseBinary(bandCurrent),
                                Nl80211BandAttribute.NL80211_BAND_ATTR_S1G_CAPA => ParseBinary(bandCurrent),
                                _ => null
                            };

                            if (attrValue != null)
                            {
                                bandInfo.Attributes[bandAttr] = attrValue;
                            }
                        }

                        bandCurrent = LibNlNative.nla_next(bandCurrent, &bandRemaining);
                    }
                }

                bands.Add(bandInfo);
            }

            // Move to next band
            current = LibNlNative.nla_next(current, &remaining);
        }

        return Nl80211AttributeValue.FromBands(bands);
    }
}
