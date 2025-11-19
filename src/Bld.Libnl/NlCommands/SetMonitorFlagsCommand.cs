using Bld.Libnl.Types;

namespace Bld.Libnl.NlCommands;

internal sealed class SetMonitorFlagsCommand : NlCommandNoResultBase
{
    private readonly uint _interfaceIndex;
    private readonly IReadOnlyCollection<Nl80211MonitorFlag> _flags;

    public SetMonitorFlagsCommand(uint interfaceIndex, IEnumerable<Nl80211MonitorFlag> flags)
    : base(Nl80211Command.NL80211_CMD_SET_INTERFACE, new NetlinkMessageFlags(), interfaceIndex)
    {
        _interfaceIndex = interfaceIndex;
        _flags = flags?.ToArray() ?? Array.Empty<Nl80211MonitorFlag>();
    }

    protected override void BuildMessage(NlMsg msg)
    {
        msg
            .NestStart(Nl80211Attribute.NL80211_ATTR_MNTR_FLAGS)
            .PutFlags(_flags)
            .NestEnd();
    }
}
