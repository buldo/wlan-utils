using System.Runtime.InteropServices;

namespace Bld.Libnl.Types;

public sealed class NlSock : SafeHandle
{
    public NlSock() : base(IntPtr.Zero, true)
    {
    }

    public override bool IsInvalid => handle <= IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            LibNlNative.nl_socket_free(handle);
        }

        return true;
    }

    public void SetCb(NlCb nlCb)
    {
        LibNlNative.nl_socket_set_cb(this, nlCb);
    }

    public void RecvMsgs(NlCb cb)
    {
        LibNlNative.nl_recvmsgs(this, cb);
    }
}
