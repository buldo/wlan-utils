// ReSharper disable UnusedMember.Global

using System.Runtime.InteropServices;
using Bld.Libnl.Types;

namespace Bld.Libnl;

public static unsafe partial class LibNlNative
{
    [LibraryImport("libnl-3.so", EntryPoint = "nl_socket_alloc")]
    public static partial NlSock nl_socket_alloc();

    [LibraryImport("libnl-3.so", EntryPoint = "nl_socket_free")]
    public static partial void nl_socket_free(NlSock sock);

    [LibraryImport("libnl-genl-3.so", EntryPoint = "genl_connect")]
    public static partial int genl_connect(NlSock sock);

    [LibraryImport("libnl-genl-3.so", EntryPoint = "genl_ctrl_resolve", StringMarshalling = StringMarshalling.Utf8)]
    public static partial int genl_ctrl_resolve(NlSock sock, string name);

    [LibraryImport("libnl-3.so", EntryPoint = "nl_send_auto")]
    public static partial int nl_send_auto(NlSock sock, NlMsg msg);

    // Netlink message allocation and deallocation
    [LibraryImport("libnl-3.so", EntryPoint = "nlmsg_alloc")]
    public static partial NlMsg nlmsg_alloc();

    [LibraryImport("libnl-3.so", EntryPoint = "nlmsg_free")]
    public static partial void nlmsg_free(IntPtr msg);

    // Generic netlink message construction
    [LibraryImport("libnl-genl-3.so", EntryPoint = "genlmsg_put")]
    public static partial IntPtr genlmsg_put(
        NlMsg msg,
        uint portid,
        uint seq,
        int family,
        int hdrlen,
        NetlinkMessageFlags flags,
        byte cmd,
        byte version);

    // Netlink attribute addition
    [LibraryImport("libnl-3.so", EntryPoint = "nla_put_u32")]
    public static partial int nla_put_u32(NlMsg msg, int attrtype, uint value);

    // Callback handling
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int NlRecvmsgCallback(IntPtr msg, IntPtr arg);

    [LibraryImport("libnl-3.so", EntryPoint = "nl_socket_modify_cb")]
    public static partial int nl_socket_modify_cb(NlSock sock, int type, int kind, NlRecvmsgCallback callback, IntPtr arg);

    [LibraryImport("libnl-3.so", EntryPoint = "nl_recvmsgs_default")]
    public static partial int nl_recvmsgs_default(NlSock sock);

    // Netlink message parsing
    [LibraryImport("libnl-genl-3.so", EntryPoint = "genlmsg_attrdata")]
    public static partial IntPtr genlmsg_attrdata(IntPtr gnlh, int hdrlen);

    [LibraryImport("libnl-genl-3.so", EntryPoint = "genlmsg_attrlen")]
    public static partial int genlmsg_attrlen(IntPtr gnlh, int hdrlen);

    [LibraryImport("libnl-3.so", EntryPoint = "nla_parse")]
    public static partial int nla_parse(IntPtr* tb, int maxtype, IntPtr head, int len, IntPtr policy);

    [LibraryImport("libnl-3.so", EntryPoint = "nla_type")]
    public static partial NlAttributeType nla_type(IntPtr nla);

    [LibraryImport("libnl-3.so", EntryPoint = "nla_get_u8")]
    public static partial byte nla_get_u8(IntPtr nla);

    [LibraryImport("libnl-3.so", EntryPoint = "nla_get_u16")]
    public static partial ushort nla_get_u16(IntPtr nla);

    [LibraryImport("libnl-3.so", EntryPoint = "nla_get_u32")]
    public static partial uint nla_get_u32(IntPtr nla);

    [LibraryImport("libnl-3.so", EntryPoint = "nla_get_u64")]
    public static partial ulong nla_get_u64(IntPtr nla);

    [LibraryImport("libnl-3.so", EntryPoint = "nla_get_s8")]
    public static partial sbyte nla_get_s8(IntPtr nla);

    [LibraryImport("libnl-3.so", EntryPoint = "nla_get_s16")]
    public static partial short nla_get_s16(IntPtr nla);

    [LibraryImport("libnl-3.so", EntryPoint = "nla_get_s32")]
    public static partial int nla_get_s32(IntPtr nla);

    [LibraryImport("libnl-3.so", EntryPoint = "nla_get_s64")]
    public static partial long nla_get_s64(IntPtr nla);

    [LibraryImport("libnl-3.so", EntryPoint = "nla_get_msecs")]
    public static partial ulong nla_get_msecs(IntPtr nla);

    [LibraryImport("libnl-3.so", EntryPoint = "nla_get_string")]
    public static partial IntPtr nla_get_string(IntPtr nla);

    [LibraryImport("libnl-3.so", EntryPoint = "nla_data")]
    public static partial IntPtr nla_data(IntPtr nla);

    [LibraryImport("libnl-3.so", EntryPoint = "nla_len")]
    public static partial int nla_len(IntPtr nla);

    [LibraryImport("libnl-3.so", EntryPoint = "nlmsg_hdr")]
    public static partial IntPtr nlmsg_hdr(IntPtr msg);

    [LibraryImport("libnl-genl-3.so", EntryPoint = "genlmsg_hdr")]
    public static partial IntPtr genlmsg_hdr(IntPtr nlh);
}
