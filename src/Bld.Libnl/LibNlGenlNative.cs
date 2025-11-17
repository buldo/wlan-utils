using System.Runtime.InteropServices;
using Bld.Libnl.Types;

namespace Bld.Libnl;

public static unsafe partial class LibNlGenlNative
{
    private const string LibraryName = "libnl-genl-3.so";

    [LibraryImport(LibraryName, EntryPoint = "genlmsg_hdr")]
    public static partial IntPtr genlmsg_hdr(IntPtr nlh);

    // Netlink message parsing
    [LibraryImport(LibraryName, EntryPoint = "genlmsg_attrdata")]
    public static partial IntPtr genlmsg_attrdata(IntPtr gnlh, int hdrlen);

    [LibraryImport(LibraryName, EntryPoint = "genlmsg_attrlen")]
    public static partial int genlmsg_attrlen(IntPtr gnlh, int hdrlen);

    // Generic netlink message construction
    [LibraryImport(LibraryName, EntryPoint = "genlmsg_put")]
    public static partial IntPtr genlmsg_put(
        NlMsg msg,
        uint portid,
        uint seq,
        int family,
        int hdrlen,
        NetlinkMessageFlags flags,
        byte cmd,
        byte version);

    [LibraryImport(LibraryName, EntryPoint = "genl_connect")]
    public static partial int genl_connect(NlSock sock);

    [LibraryImport(LibraryName, EntryPoint = "genl_ctrl_resolve", StringMarshalling = StringMarshalling.Utf8)]
    public static partial int genl_ctrl_resolve(NlSock sock, string name);
}