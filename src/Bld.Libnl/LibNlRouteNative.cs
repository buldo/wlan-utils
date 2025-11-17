using System.Runtime.InteropServices;
using Bld.Libnl.Types;

namespace Bld.Libnl;

public static unsafe partial class LibNlRouteNative
{
    private const string LibraryName = "libnl-route-3.so";

    /// <summary>
    /// Allocate link object
    /// </summary>
    /// <returns></returns>
    [LibraryImport(LibraryName, EntryPoint = "rtnl_link_alloc")]
    public static partial IntPtr rtnl_link_alloc();

    [LibraryImport(LibraryName, EntryPoint = "rtnl_link_get_kernel", StringMarshalling = StringMarshalling.Utf8)]
    public static partial int rtnl_link_get_kernel(
        NlSock sk,
        int ifindex,
        string name,
        out IntPtr result);


    /// <summary>
    /// Set flags of link object
    /// </summary>
    /// <param name="link">Link object</param>
    /// <param name="flags">Flags</param>
    /// <seealso cref="rtnl_link_unset_flags"/>
    [LibraryImport(LibraryName, EntryPoint = "rtnl_link_set_flags")]
    public static partial void rtnl_link_set_flags(IntPtr link, uint flags);

    /// <summary>
    /// Unset flags of link object
    /// </summary>
    /// <param name="link">Link object</param>
    /// <param name="flags">Flags</param>
    /// <seealso cref="rtnl_link_set_flags"/>
    [LibraryImport(LibraryName, EntryPoint = "rtnl_link_unset_flags")]
    public static partial void rtnl_link_unset_flags(IntPtr link, uint flags);

    /// <summary>
    /// Builds a RTM_NEWLINK netlink message requesting the change of
    /// a network link. If -EOPNOTSUPP is returned by the kernel, the
    /// message type will be changed to RTM_SETLINK and the message is
    /// resent to work around older kernel versions.
    ///
    /// The link to be changed is looked up based on the interface index
    /// supplied in the orig link. Optionaly the link name is used but
    /// only if no interface index is provided, otherwise providing an
    /// link name will result in the link name being changed.
    ///
    /// If no matching link exists, the function will return -NLE_OBJ_NOTFOUND.
    ///
    /// After sending, the function will wait for the ACK or an eventual
    /// error message to be received and will therefore block until the
    /// operation has been completed.
    /// </summary>
    /// <remarks>
    /// The link name can only be changed if the link has been put in opertional down state. (~IF_UP)
    /// </remarks>
    /// <param name="sk">netlink socket</param>
    /// <param name="orig">original link to be changed</param>
    /// <param name="changes">link containing the changes to be made</param>
    /// <param name="flags">additional netlink message flags</param>
    /// <returns>0 on success or a negative error code</returns>
    [LibraryImport(LibraryName, EntryPoint = "rtnl_link_change")]
    public static partial int rtnl_link_change(NlSock sk, IntPtr orig, IntPtr changes, int flags);

    /// <summary>
    /// Return a link object reference
    /// </summary>
    /// <param name="link">Link object</param>
    [LibraryImport(LibraryName, EntryPoint = "rtnl_link_put")]
    public static partial void rtnl_link_put(IntPtr link);

    /// <summary>
    /// Return flags of link object
    /// </summary>
    /// <param name="link">Link object</param>
    /// <returns>Link flags or 0 if none have been set.</returns>
    /// <seealso cref="rtnl_link_set_flags"/>
    /// <seealso cref="rtnl_link_unset_flags"/>
    [LibraryImport(LibraryName, EntryPoint = "rtnl_link_get_flags")]
    public static partial uint rtnl_link_get_flags(IntPtr link);
}