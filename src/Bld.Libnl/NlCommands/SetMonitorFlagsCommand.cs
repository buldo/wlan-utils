using Bld.Libnl.Types;

namespace Bld.Libnl.NlCommands;

/// <summary>
/// NL command to set monitor mode flags (NL80211_ATTR_MNTR_FLAGS) for an interface.
/// Does not change interface type; use SetInterfaceTypeCommand separately if needed.
/// </summary>
internal sealed class SetMonitorFlagsCommand : NlCommandBase
{
    private readonly uint _interfaceIndex;
    private readonly IReadOnlyCollection<Nl80211MonitorFlag> _flags;

    public SetMonitorFlagsCommand(uint interfaceIndex, IEnumerable<Nl80211MonitorFlag> flags)
    {
        _interfaceIndex = interfaceIndex;
        _flags = flags?.ToArray() ?? Array.Empty<Nl80211MonitorFlag>();
    }

    protected override void BuildMessage(NlMsg msg)
    {
        msg
            .PutAuto(Nl80211Id, NetlinkMessageFlags.NLM_F_REQUEST, Nl80211Command.NL80211_CMD_SET_INTERFACE)
            .PutU32(Nl80211Attribute.NL80211_ATTR_IFINDEX, _interfaceIndex)
            .NestStart(Nl80211Attribute.NL80211_ATTR_MNTR_FLAGS)
            .PutFlags(_flags)
            .NestEnd();
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
