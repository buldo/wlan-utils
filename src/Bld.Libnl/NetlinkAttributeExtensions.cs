namespace Bld.Libnl;

/// <summary>
/// Extension methods for iterating over netlink attributes
/// Provides C#-idiomatic equivalents to nla_for_each_nested and nla_for_each_attr macros
/// </summary>
public static unsafe class NetlinkAttributeExtensions
{
    /// <summary>
    /// Iterate over nested attributes
    /// Equivalent to nla_for_each_nested(pos, nla, rem) macro:
    /// for (pos = nla_data(nla), rem = nla_len(nla); nla_ok(pos, rem); pos = nla_next(pos, &rem))
    /// </summary>
    /// <param name="nla">Attribute containing the nested attributes</param>
    /// <returns>Enumerable sequence of nested attribute pointers</returns>
    public static IEnumerable<IntPtr> EnumerateNested(this IntPtr nla)
    {
        // Direct translation: pos = nla_data(nla), rem = nla_len(nla)
        var pos = LibNlNative.nla_data(nla);
        var rem = LibNlNative.nla_len(nla);

        return EnumerateAttributesImpl(pos, rem);
    }

    /// <summary>
    /// Iterate over attributes in a stream
    /// Equivalent to nla_for_each_attr(pos, head, len, rem) macro:
    /// for (pos = head, rem = len; nla_ok(pos, rem); pos = nla_next(pos, &rem))
    /// </summary>
    /// <param name="head">Head of attribute stream</param>
    /// <param name="len">Length of attribute stream</param>
    /// <returns>Enumerable sequence of attribute pointers</returns>
    public static IEnumerable<IntPtr> EnumerateAttributes(IntPtr head, int len)
    {
        return EnumerateAttributesImpl(head, len);
    }

    private static IEnumerable<IntPtr> EnumerateAttributesImpl(IntPtr pos, int rem)
    {
        // Direct translation of: for (; nla_ok(pos, rem); pos = nla_next(pos, &rem))
        var result = new List<IntPtr>();

        while (LibNlNative.nla_ok(pos, rem))
        {
            result.Add(pos);
            pos = LibNlNative.nla_next(pos, &rem);
        }

        return result;
    }
}
