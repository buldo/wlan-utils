using Microsoft.Win32.SafeHandles;

namespace Bld.Libnl.Types;

/// <summary>Type-safe wrapper for nl_msg pointer with automatic lifetime management</summary>
public sealed class NlMsg : SafeHandleZeroOrMinusOneIsInvalid
{
    public NlMsg() : base(ownsHandle: true)
    {
    }

    public NlMsg(IntPtr handle, bool ownsHandle = true) : base(ownsHandle)
    {
        SetHandle(handle);
    }

    protected override bool ReleaseHandle()
    {
        if (handle != IntPtr.Zero)
        {
            LibNlNative.nlmsg_free(handle);
        }
        return true;
    }

    public static implicit operator IntPtr(NlMsg msg) => msg.DangerousGetHandle();
}