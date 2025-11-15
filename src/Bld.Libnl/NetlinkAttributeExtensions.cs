namespace Bld.Libnl;

/// <summary>
/// Extension methods for iterating over netlink attributes
/// Provides C#-idiomatic equivalents to nla_for_each_nested and nla_for_each_attr macros
/// </summary>
public static unsafe class NetlinkAttributeExtensions
{
    /// <summary>
    /// Iterate over nested attributes
    /// Equivalent to nla_for_each_nested(pos, nla, rem) macro
    /// </summary>
    /// <param name="nla">Attribute containing the nested attributes</param>
    /// <returns>Enumerable sequence of nested attribute pointers</returns>
    public static IEnumerable<IntPtr> EnumerateNested(this IntPtr nla)
    {
        if (nla == IntPtr.Zero)
            return Enumerable.Empty<IntPtr>();

        var data = LibNlNative.nla_data(nla);
        var len = LibNlNative.nla_len(nla);

        return EnumerateAttributes(data, len);
    }

    /// <summary>
    /// Iterate over attributes in a stream
    /// Equivalent to nla_for_each_attr(pos, head, len, rem) macro
    /// </summary>
    /// <param name="head">Head of attribute stream</param>
    /// <param name="len">Length of attribute stream</param>
    /// <returns>Enumerable sequence of attribute pointers</returns>
    public static IEnumerable<IntPtr> EnumerateAttributes(IntPtr head, int len)
    {
        if (head == IntPtr.Zero || len <= 0)
            return Enumerable.Empty<IntPtr>();

        var result = new List<IntPtr>();
        var remaining = len;
        var current = head;

        while (LibNlNative.nla_ok(current, remaining))
        {
            result.Add(current);
            current = LibNlNative.nla_next(current, &remaining);
        }

        return result;
    }
}
