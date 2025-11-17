using Bld.Libnl.Types;

namespace Bld.Libnl.NlCommands;

internal sealed class SetInterfaceTypeCommand : NlCommandBase
{
    private readonly uint _interfaceIndex;
    private readonly Nl80211InterfaceType _type;

    public SetInterfaceTypeCommand(uint interfaceIndex, Nl80211InterfaceType type)
    {
        _interfaceIndex = interfaceIndex;
        _type = type;
    }

    protected override void BuildMessage(NlMsg msg)
    {
        var hdr = LibNlNative.genlmsg_put(
            msg,
            0, // portid (automatic)
            0, // sequence (automatic)
            Nl80211Id, // nl80211 family id
            0, // header length
            NetlinkMessageFlags.NLM_F_REQUEST,
            (byte)Nl80211Command.NL80211_CMD_SET_INTERFACE,
            0 // version
        );

        if (hdr == IntPtr.Zero)
        {
            throw new Exception("Failed to build netlink message");
        }

        var ret = LibNlNative.nla_put_u32(msg, (int)Nl80211Attribute.NL80211_ATTR_IFINDEX, _interfaceIndex);
        if (ret != 0)
        {
            throw new Exception("Failed to set NL80211_ATTR_IFINDEX");
        }

        ret = LibNlNative.nla_put_u32(msg, (int)Nl80211Attribute.NL80211_ATTR_IFTYPE, (uint)_type);
        if (ret != 0)
        {
            throw new Exception("Failed to set NL80211_ATTR_IFTYPE");
        }
    }

    public void Run()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);

        using var msg = LibNlNative.nlmsg_alloc();
        if (msg.IsInvalid)
        {
            throw new Exception("Failed to allocate netlink message");
        }

        BuildMessage(msg);

        var sendResult = LibNlNative.nl_send_auto_complete(NlSocket, msg);
        if (sendResult < 0)
        {
            throw new Exception($"Failed to send netlink message: {sendResult}");
        }
    }
}
