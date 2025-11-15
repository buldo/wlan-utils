using System.Runtime.InteropServices;
using Bld.Libnl.Types;

namespace Bld.Libnl;

internal abstract class NlCommandBase<TReturn> : IDisposable
{
    private readonly NlSock _nlSocket;
    private readonly LibNlNative.NlRecvmsgCallback _callback;
    private readonly GCHandle _callbackHandle;
    protected readonly int Nl80211Id;

    private bool _disposed;

    protected NlCommandBase()
    {
        _nlSocket = LibNlNative.nl_socket_alloc();
        if (!_nlSocket.IsValid)
        {
            throw new Exception("Failed to allocate socket");
        }

        var connectResult = LibNlNative.genl_connect(_nlSocket);
        if (connectResult != 0)
        {
            LibNlNative.nl_socket_free(_nlSocket);
            throw new Exception($"Failed to genl_connect: {connectResult}");
        }

        Nl80211Id = LibNlNative.genl_ctrl_resolve(_nlSocket, "nl80211");
        if (Nl80211Id < 0)
        {
            LibNlNative.nl_socket_free(_nlSocket);
            throw new Exception($"Failed to resolve nl80211 family: {Nl80211Id}");
        }

        _callback = ProcessMessage;
        _callbackHandle = GCHandle.Alloc(_callback);

        var cbResult = LibNlNative.nl_socket_modify_cb(
            _nlSocket,
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
        ObjectDisposedException.ThrowIf(_disposed, this);
        using var msg = LibNlNative.nlmsg_alloc();
        if (msg.IsInvalid)
        {
            throw new Exception("Failed to allocate netlink message");
        }

        BuildMessage(msg);

        var sendResult = LibNlNative.nl_send_auto(_nlSocket, msg);
        if (sendResult < 0)
        {
            throw new Exception($"Failed to send netlink message: {sendResult}");
        }

        var recvResult = LibNlNative.nl_recvmsgs_default(_nlSocket);
        if (recvResult < 0)
        {
            throw new Exception($"Failed to receive netlink messages: {recvResult}");
        }

        return GetResult();
    }

    protected abstract void BuildMessage(NlMsg msg);

    protected abstract int ProcessMessage(IntPtr msgPtr, IntPtr arg);

    protected abstract TReturn GetResult();

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (_nlSocket.IsValid)
            {
                LibNlNative.nl_socket_free(_nlSocket);
            }

            if (_callbackHandle.IsAllocated)
            {
                _callbackHandle.Free();
            }

            _disposed = true;
        }
    }

    ~NlCommandBase()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
