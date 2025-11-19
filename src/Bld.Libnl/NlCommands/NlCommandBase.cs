using System.Runtime.InteropServices;
using Bld.Libnl.NlCommands;
using Bld.Libnl.Types;

namespace Bld.Libnl;

public abstract class NlCommandBase
{
    private readonly Nl80211Command _command;
    private readonly NetlinkMessageFlags _flags;
    private readonly uint? _ifIndex;

    private bool _inProcess = true;

    protected NlCommandBase(Nl80211Command command, NetlinkMessageFlags flags, uint? ifIndex = null)
    {
        _command = command;
        _flags = flags;
        _ifIndex = ifIndex;
    }

    public bool IsError { get; private set; }

    protected void RunInternal()
    {
        using var nlSocket = LibNlNative.nl_socket_alloc();
        if (nlSocket.IsInvalid)
        {
            throw new Exception("Failed to allocate socket");
        }

        var connectResult = LibNlGenlNative.genl_connect(nlSocket);
        if (connectResult != 0)
        {
            throw new Exception($"Failed to genl_connect: {connectResult}");
        }

        var nl80211Id = LibNlGenlNative.genl_ctrl_resolve(nlSocket, "nl80211");
        if (nl80211Id < 0)
        {
            throw new Exception($"Failed to resolve nl80211 family: {nl80211Id}");
        }

        using var msg = LibNlNative.nlmsg_alloc();
        if (msg.IsInvalid)
        {
            throw new Exception("Failed to allocate netlink message");
        }

#if DEBUG
        var cbFlag = NlCbKind.NL_CB_DEBUG;
#else
        var cbFlag = NlCbKind.NL_CB_DEFAULT;
#endif

        using var cb = LibNlNative.nl_cb_alloc(cbFlag);
        using var s_cb = LibNlNative.nl_cb_alloc(cbFlag);
        if (cb.IsInvalid || s_cb.IsInvalid)
        {
            throw new Exception("Failed to allocate netlink callbacks");
        }

        msg.PutAuto(nl80211Id, _flags, _command);

        // 	switch (command_idby) {
        // 	case CIB_PHY:
        // 		NLA_PUT_U32(msg, NL80211_ATTR_WIPHY, devidx);
        // 		break;
        // 	case CIB_NETDEV:
        // 		NLA_PUT_U32(msg, NL80211_ATTR_IFINDEX, devidx);
        // 		break;
        // 	case CIB_WDEV:
        // 		NLA_PUT_U64(msg, NL80211_ATTR_WDEV, devidx);
        // 		break;
        // 	default:
        // 		break;
        // 	}
        if (_ifIndex.HasValue)
        {
            msg.PutU32(Nl80211Attribute.NL80211_ATTR_IFINDEX, _ifIndex.Value);
        }

        BuildMessage(msg);

        nlSocket.SetCb(s_cb);

        var err = LibNlNative.nl_send_auto(nlSocket, msg);
        if (err < 0)
        {
            throw new Exception($"error sending message {err}");
        }

        LibNlNative.NlRecvmsgCb valid_handler = ProcessValidMessage;
        var valid_handlerHandle = GCHandle.Alloc(valid_handler);

        LibNlNative.NlRecvmsgCb ack_handler = ProcessAckMessage;
        var ack_handlerHandle = GCHandle.Alloc(ack_handler);

        LibNlNative.NlRecvmsgCb finish_handler = ProcessFinishMessage;
        var finish_handlerHandle = GCHandle.Alloc(finish_handler);

        LibNlNative.NlRecvmsgErrCb error_handler = ProcessErrorMessage;
        var error_handlerHandle = GCHandle.Alloc(error_handler);

        try
        {
            LibNlNative.nl_cb_err(cb, NlCbKind.NL_CB_CUSTOM, error_handler, IntPtr.Zero);
            LibNlNative.nl_cb_set(cb, NlCbType.NL_CB_FINISH, NlCbKind.NL_CB_CUSTOM, finish_handler, IntPtr.Zero);
            LibNlNative.nl_cb_set(cb, NlCbType.NL_CB_ACK, NlCbKind.NL_CB_CUSTOM, ack_handler, IntPtr.Zero);
            LibNlNative.nl_cb_set(cb, NlCbType.NL_CB_VALID, NlCbKind.NL_CB_CUSTOM, valid_handler, IntPtr.Zero);

            _inProcess = true;
            while (_inProcess)
            {
                nlSocket.RecvMsgs(cb);
            }
        }
        finally
        {
            error_handlerHandle.Free();
            finish_handlerHandle.Free();
            ack_handlerHandle.Free();
            valid_handlerHandle.Free();
        }
    }

    protected abstract int ProcessValidMessage(IntPtr msgPtr, IntPtr arg);

    private int ProcessAckMessage(IntPtr msgPtr, IntPtr arg)
    {
        _inProcess = false;
        return (int)NetlinkCallbackAction.NL_STOP;
    }

    private int ProcessFinishMessage(IntPtr msgPtr, IntPtr arg)
    {
        _inProcess = false;
        return (int)NetlinkCallbackAction.NL_SKIP;
    }

    private int ProcessErrorMessage(IntPtr nla, IntPtr err, IntPtr arg)
    {
        IsError = true;
        return (int)NetlinkCallbackAction.NL_STOP;
    }

    protected abstract void BuildMessage(NlMsg msg);
}
