using Bld.Libnl.Types;

namespace Bld.Libnl.NlCommands;

internal sealed class SetInterfaceTypeCommand : NlCommandNoResultBase
{
    private readonly Nl80211InterfaceType _type;

    public SetInterfaceTypeCommand(uint interfaceIndex, Nl80211InterfaceType type)
    : base(Nl80211Command.NL80211_CMD_SET_INTERFACE, new NetlinkMessageFlags(), interfaceIndex)
    {
        _type = type;
    }

    protected override void BuildMessage(NlMsg msg)
    {
        msg.PutU32(Nl80211Attribute.NL80211_ATTR_IFTYPE, (uint)_type);
    }
}
