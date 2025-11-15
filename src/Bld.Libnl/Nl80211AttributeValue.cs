using Bld.Libnl.Types;

namespace Bld.Libnl;

/// <summary>
/// Base interface for nl80211 attribute values
/// </summary>
public interface INl80211AttributeValue
{
    NlAttributeType Type { get; }
}

/// <summary>
/// Generic nl80211 attribute value - stores only the actual value type
/// </summary>
public sealed class Nl80211AttributeValue<T> : INl80211AttributeValue
{
    public NlAttributeType Type { get; init; }
    public T Value { get; init; }

    public Nl80211AttributeValue(NlAttributeType attributeType, T value)
    {
        Type = attributeType;
        Value = value;
    }
}

/// <summary>
/// Helper methods for creating attribute values
/// </summary>
public static class Nl80211AttributeValue
{
    public static INl80211AttributeValue FromU8(byte value) => new Nl80211AttributeValue<byte>(NlAttributeType.NLA_U8, value);
    public static INl80211AttributeValue FromU16(ushort value) => new Nl80211AttributeValue<ushort>(NlAttributeType.NLA_U16, value);
    public static INl80211AttributeValue FromU32(uint value) => new Nl80211AttributeValue<uint>(NlAttributeType.NLA_U32, value);
    public static INl80211AttributeValue FromU64(ulong value) => new Nl80211AttributeValue<ulong>(NlAttributeType.NLA_U64, value);
    public static INl80211AttributeValue FromS8(sbyte value) => new Nl80211AttributeValue<sbyte>(NlAttributeType.NLA_S8, value);
    public static INl80211AttributeValue FromS16(short value) => new Nl80211AttributeValue<short>(NlAttributeType.NLA_S16, value);
    public static INl80211AttributeValue FromS32(int value) => new Nl80211AttributeValue<int>(NlAttributeType.NLA_S32, value);
    public static INl80211AttributeValue FromS64(long value) => new Nl80211AttributeValue<long>(NlAttributeType.NLA_S64, value);
    public static INl80211AttributeValue FromMsecs(ulong value) => new Nl80211AttributeValue<ulong>(NlAttributeType.NLA_MSECS, value);
    public static INl80211AttributeValue FromString(string value) => new Nl80211AttributeValue<string>(NlAttributeType.NLA_NUL_STRING, value);
    public static INl80211AttributeValue FromBinary(byte[] value) => new Nl80211AttributeValue<byte[]>(NlAttributeType.NLA_BINARY, value);
    public static INl80211AttributeValue FromNested(IntPtr value) => new Nl80211AttributeValue<IntPtr>(NlAttributeType.NLA_NESTED, value);
    public static INl80211AttributeValue FromInterfaceTypes(HashSet<Nl80211InterfaceType> value) => new Nl80211AttributeValue<HashSet<Nl80211InterfaceType>>(NlAttributeType.NLA_NESTED, value);
    public static INl80211AttributeValue FromBands(List<BandInfo> value) => new Nl80211AttributeValue<List<BandInfo>>(NlAttributeType.NLA_NESTED, value);
}

/// <summary>
/// Extension methods for working with attribute values
/// </summary>
public static class Nl80211AttributeValueExtensions
{
    public static byte? AsU8(this INl80211AttributeValue attr) => attr is Nl80211AttributeValue<byte> typed ? typed.Value : null;
    public static ushort? AsU16(this INl80211AttributeValue attr) => attr is Nl80211AttributeValue<ushort> typed ? typed.Value : null;
    public static uint? AsU32(this INl80211AttributeValue attr) => attr is Nl80211AttributeValue<uint> typed ? typed.Value : null;
    public static ulong? AsU64(this INl80211AttributeValue attr) => attr is Nl80211AttributeValue<ulong> typed ? typed.Value : null;
    public static sbyte? AsS8(this INl80211AttributeValue attr) => attr is Nl80211AttributeValue<sbyte> typed ? typed.Value : null;
    public static short? AsS16(this INl80211AttributeValue attr) => attr is Nl80211AttributeValue<short> typed ? typed.Value : null;
    public static int? AsS32(this INl80211AttributeValue attr) => attr is Nl80211AttributeValue<int> typed ? typed.Value : null;
    public static long? AsS64(this INl80211AttributeValue attr) => attr is Nl80211AttributeValue<long> typed ? typed.Value : null;
    public static ulong? AsMsecs(this INl80211AttributeValue attr) => attr is Nl80211AttributeValue<ulong> typed ? typed.Value : null;
    public static string? AsString(this INl80211AttributeValue attr) => attr is Nl80211AttributeValue<string> typed ? typed.Value : null;
    public static byte[]? AsBinary(this INl80211AttributeValue attr) => attr is Nl80211AttributeValue<byte[]> typed ? typed.Value : null;
    public static IntPtr AsNested(this INl80211AttributeValue attr) => attr is Nl80211AttributeValue<IntPtr> typed ? typed.Value : IntPtr.Zero;

    public static string? AsMacAddress(this INl80211AttributeValue attr)
    {
        if (attr is Nl80211AttributeValue<byte[]> typed && typed.Value.Length == 6)
        {
            return BitConverter.ToString(typed.Value).Replace('-', ':');
        }
        return null;
    }

    public static bool? AsBoolean(this INl80211AttributeValue attr) => attr switch
    {
        Nl80211AttributeValue<byte> u8 => u8.Value != 0,
        Nl80211AttributeValue<ushort> u16 => u16.Value != 0,
        Nl80211AttributeValue<uint> u32 => u32.Value != 0,
        Nl80211AttributeValue<ulong> u64 => u64.Value != 0,
        Nl80211AttributeValue<sbyte> s8 => s8.Value != 0,
        Nl80211AttributeValue<short> s16 => s16.Value != 0,
        Nl80211AttributeValue<int> s32 => s32.Value != 0,
        Nl80211AttributeValue<long> s64 => s64.Value != 0,
        _ => null
    };

    public static uint? AsUInt32(this INl80211AttributeValue attr) => attr switch
    {
        Nl80211AttributeValue<byte> u8 => u8.Value,
        Nl80211AttributeValue<ushort> u16 => u16.Value,
        Nl80211AttributeValue<uint> u32 => u32.Value,
        Nl80211AttributeValue<ulong> u64 => u64.Value <= uint.MaxValue ? (uint)u64.Value : null,
        Nl80211AttributeValue<sbyte> s8 => s8.Value >= 0 ? (uint)s8.Value : null,
        Nl80211AttributeValue<short> s16 => s16.Value >= 0 ? (uint)s16.Value : null,
        Nl80211AttributeValue<int> s32 => s32.Value >= 0 ? (uint)s32.Value : null,
        Nl80211AttributeValue<long> s64 => s64.Value is >= 0 and <= uint.MaxValue ? (uint)s64.Value : null,
        _ => null
    };

    /// <summary>
    /// Extract interface types from NL80211_ATTR_SUPPORTED_IFTYPES or NL80211_ATTR_SOFTWARE_IFTYPES attribute
    /// </summary>
    public static HashSet<Nl80211InterfaceType>? AsInterfaceTypes(this INl80211AttributeValue attr)
    {
        return attr is Nl80211AttributeValue<HashSet<Nl80211InterfaceType>> typed ? typed.Value : null;
    }

    /// <summary>
    /// Extract band information from NL80211_ATTR_WIPHY_BANDS attribute
    /// </summary>
    public static List<BandInfo>? AsBands(this INl80211AttributeValue attr)
    {
        return attr is Nl80211AttributeValue<List<BandInfo>> typed ? typed.Value : null;
    }
}
