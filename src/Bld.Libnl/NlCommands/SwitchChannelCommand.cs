using Bld.Libnl.Types;

namespace Bld.Libnl.NlCommands;

internal sealed class SwitchChannelCommand : NlCommandBase
{
    public SwitchChannelCommand(ChannelDefinition channelDefinition)
    {

    }

    protected override void BuildMessage(NlMsg msg)
    {
        msg
            .PutAuto(Nl80211Id, NetlinkMessageFlags.NLM_F_REQUEST, Nl80211Command.NL80211_CMD_CHANNEL_SWITCH);

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