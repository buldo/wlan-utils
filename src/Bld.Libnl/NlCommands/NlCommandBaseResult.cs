using System.Runtime.InteropServices;
using Bld.Libnl.Types;

namespace Bld.Libnl;

internal abstract class NlCommandBaseResult<TReturn> : NlCommandBase
{
    private readonly LibNlNative.NlRecvmsgCallback _callback;
    private readonly GCHandle _callbackHandle;

    protected NlCommandBaseResult() : base()
    {
        _callback = ProcessMessage;
        _callbackHandle = GCHandle.Alloc(_callback);

        var cbResult = LibNlNative.nl_socket_modify_cb(
            NlSocket,
            (int)NetlinkCallbackType.NL_CB_VALID,
            (int)NetlinkCallbackKind.NL_CB_CUSTOM,
            _callback,
            IntPtr.Zero
        );

        if (cbResult != 0)
        {
            throw new Exception($"Failed to set callback: {cbResult}");
        }
    }

    public TReturn Run()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        using var msg = LibNlNative.nlmsg_alloc();
        if (msg.IsInvalid)
        {
            throw new Exception("Failed to allocate netlink message");
        }

        BuildMessage(msg);

        var sendResult = LibNlNative.nl_send_auto(NlSocket, msg);
        if (sendResult < 0)
        {
            throw new Exception($"Failed to send netlink message: {sendResult}");
        }

        var recvResult = LibNlNative.nl_recvmsgs_default(NlSocket);
        if (recvResult < 0)
        {
            throw new Exception($"Failed to receive netlink messages: {recvResult}");
        }

        return GetResult();
    }

    protected abstract int ProcessMessage(IntPtr msgPtr, IntPtr arg);

    protected abstract TReturn GetResult();

    protected override void Dispose(bool disposing)
    {
        if (_callbackHandle.IsAllocated)
        {
            _callbackHandle.Free();
        }

        base.Dispose(disposing);
    }

    ~NlCommandBaseResult()
    {
        Dispose(disposing: false);
    }
}
