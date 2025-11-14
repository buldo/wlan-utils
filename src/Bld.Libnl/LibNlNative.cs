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
}
