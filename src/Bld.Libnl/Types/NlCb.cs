using System.Runtime.InteropServices;

namespace Bld.Libnl.Types;

public sealed class NlCb : SafeHandle
{
    public NlCb() : base(IntPtr.Zero, true)
    {
    }

    public override bool IsInvalid => handle <= IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            LibNlNative.nl_cb_put(handle);
        }

        return true;
    }
}