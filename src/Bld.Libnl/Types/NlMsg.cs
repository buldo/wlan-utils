namespace Bld.Libnl.Types;

/// <summary>Type-safe wrapper for nl_msg pointer</summary>
public readonly struct NlMsg
{
    public readonly IntPtr Handle;

    public NlMsg(IntPtr handle) => Handle = handle;

    public bool IsValid => Handle != IntPtr.Zero;

    public static implicit operator IntPtr(NlMsg msg) => msg.Handle;
}