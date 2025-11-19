// ReSharper disable UnusedMember.Global

using System.Runtime.InteropServices;
using Bld.Libnl.Types;

namespace Bld.Libnl;

internal static unsafe partial class LibNlNative
{
    private const string LibraryName = "libnl-3.so";

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int NlRecvmsgCb(IntPtr msg, IntPtr arg);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int NlRecvmsgErrCb(IntPtr nla, IntPtr nlerr, IntPtr arg);

    [LibraryImport(LibraryName, EntryPoint="nl_connect")]
    public static partial int nl_connect(IntPtr sock, NetlinkProtocol protocol);

    [LibraryImport(LibraryName, EntryPoint="nl_recvmsgs")]
    public static partial int nl_recvmsgs(NlSock sk, NlCb cb);

    [LibraryImport(LibraryName, EntryPoint = "nl_cb_alloc")]
    public static partial NlCb nl_cb_alloc(NlCbKind kind);

    [LibraryImport(LibraryName, EntryPoint = "nl_cb_put")]
    public static partial void nl_cb_put(IntPtr kind);

    [LibraryImport(LibraryName, EntryPoint = "nl_cb_err")]
    public static partial int nl_cb_err(NlCb cb, NlCbKind kind, NlRecvmsgErrCb func, IntPtr arg);

    [LibraryImport(LibraryName, EntryPoint = "nl_cb_set")]
    public static partial int nl_cb_set(NlCb cb, NlCbType type, NlCbKind kind, NlRecvmsgCb func, IntPtr arg);

    [LibraryImport(LibraryName, EntryPoint = "nl_socket_alloc")]
    public static partial NlSock nl_socket_alloc();

    [LibraryImport(LibraryName, EntryPoint = "nl_socket_free")]
    public static partial void nl_socket_free(IntPtr sock);

    [LibraryImport(LibraryName, EntryPoint = "nl_socket_set_cb")]
    public static partial void nl_socket_set_cb(NlSock sk, NlCb cb);

    [LibraryImport(LibraryName, EntryPoint = "nl_send_auto")]
    public static partial int nl_send_auto(NlSock sock, NlMsg msg);

    [LibraryImport(LibraryName, EntryPoint = "nlmsg_alloc")]
    public static partial NlMsg nlmsg_alloc();

    [LibraryImport(LibraryName, EntryPoint = "nlmsg_free")]
    public static partial void nlmsg_free(IntPtr msg);

    [LibraryImport(LibraryName, EntryPoint = "nla_put_u32")]
    public static partial int nla_put_u32(NlMsg msg, int attrtype, uint value);

    [LibraryImport(LibraryName, EntryPoint = "nla_put_flag")]
    public static partial int nla_put_flag(NlMsg msg, int attrtype);

    [LibraryImport(LibraryName, EntryPoint = "nla_nest_start")]
    public static partial IntPtr nla_nest_start(NlMsg msg, int attrtype);

    [LibraryImport(LibraryName, EntryPoint = "nla_nest_end")]
    public static partial void nla_nest_end(NlMsg msg, IntPtr start);

    [LibraryImport(LibraryName, EntryPoint = "nl_socket_modify_cb")]
    public static partial int nl_socket_modify_cb(NlSock sock, int type, int kind, NlRecvmsgCb callback, IntPtr arg);

    [LibraryImport(LibraryName, EntryPoint = "nl_recvmsgs_default")]
    public static partial int nl_recvmsgs_default(NlSock sock);

    [LibraryImport(LibraryName, EntryPoint = "nla_parse")]
    public static partial int nla_parse(IntPtr* tb, int maxtype, IntPtr head, int len, IntPtr policy);

    [LibraryImport(LibraryName, EntryPoint = "nla_type")]
    public static partial NlAttributeType nla_type(IntPtr nla);

    [LibraryImport(LibraryName, EntryPoint = "nla_get_u8")]
    public static partial byte nla_get_u8(IntPtr nla);

    [LibraryImport(LibraryName, EntryPoint = "nla_get_u16")]
    public static partial ushort nla_get_u16(IntPtr nla);

    [LibraryImport(LibraryName, EntryPoint = "nla_get_u32")]
    public static partial uint nla_get_u32(IntPtr nla);

    [LibraryImport(LibraryName, EntryPoint = "nla_get_u64")]
    public static partial ulong nla_get_u64(IntPtr nla);

    [LibraryImport(LibraryName, EntryPoint = "nla_get_s8")]
    public static partial sbyte nla_get_s8(IntPtr nla);

    [LibraryImport(LibraryName, EntryPoint = "nla_get_s16")]
    public static partial short nla_get_s16(IntPtr nla);

    [LibraryImport(LibraryName, EntryPoint = "nla_get_s32")]
    public static partial int nla_get_s32(IntPtr nla);

    [LibraryImport(LibraryName, EntryPoint = "nla_get_s64")]
    public static partial long nla_get_s64(IntPtr nla);

    [LibraryImport(LibraryName, EntryPoint = "nla_get_msecs")]
    public static partial ulong nla_get_msecs(IntPtr nla);

    [LibraryImport(LibraryName, EntryPoint = "nla_get_string")]
    public static partial IntPtr nla_get_string(IntPtr nla);

    [LibraryImport(LibraryName, EntryPoint = "nla_data")]
    public static partial IntPtr nla_data(IntPtr nla);

    [LibraryImport(LibraryName, EntryPoint = "nla_len")]
    public static partial int nla_len(IntPtr nla);

    [LibraryImport(LibraryName, EntryPoint = "nlmsg_hdr")]
    public static partial IntPtr nlmsg_hdr(IntPtr msg);

    [LibraryImport(LibraryName, EntryPoint = "nla_next")]
    public static partial IntPtr nla_next(IntPtr nla, int* remaining);

    [LibraryImport(LibraryName, EntryPoint = "nla_ok")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool nla_ok(IntPtr nla, int remaining);
}
