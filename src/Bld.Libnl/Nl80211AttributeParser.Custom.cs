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

        var supportedTypes = new List<Nl80211InterfaceType>();

        // Get nested data pointer and length
        var data = LibNlNative.nla_data(nla);
        var len = LibNlNative.nla_len(nla);

        if (data == IntPtr.Zero || len <= 0)
            return Nl80211AttributeValue.FromBinary(Array.Empty<byte>());

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

        // Convert list to binary format (as array of integers)
        var resultBytes = new byte[supportedTypes.Count * sizeof(int)];
        for (int i = 0; i < supportedTypes.Count; i++)
        {
            BitConverter.GetBytes((int)supportedTypes[i]).CopyTo(resultBytes, i * sizeof(int));
        }

        return Nl80211AttributeValue.FromBinary(resultBytes);
    }
}
