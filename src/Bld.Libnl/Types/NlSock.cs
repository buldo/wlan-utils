namespace Bld.Libnl.Types;

/// <summary>Type-safe wrapper for nl_sock pointer</summary>
public readonly struct NlSock
{
    public readonly IntPtr Handle;

    public NlSock(IntPtr handle) => Handle = handle;

    public bool IsValid => Handle != IntPtr.Zero;

    public static implicit operator IntPtr(NlSock sock) => sock.Handle;
}