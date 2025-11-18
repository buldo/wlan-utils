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
        msg
            .PutAuto(Nl80211Id, NetlinkMessageFlags.NLM_F_REQUEST, Nl80211Command.NL80211_CMD_SET_INTERFACE)
            .PutU32(Nl80211Attribute.NL80211_ATTR_IFINDEX, _interfaceIndex)
            .PutU32(Nl80211Attribute.NL80211_ATTR_IFTYPE, (uint)_type);
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

        var sendResult = LibNlNative.nl_send_auto(NlSocket, msg);
        if (sendResult < 0)
        {
            throw new Exception($"Failed to send netlink message: {sendResult}");
        }
    }
}
