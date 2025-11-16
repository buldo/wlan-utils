using Bld.Libnl.Types;

namespace Bld.Libnl.NlCommands;

internal class DumpInterfaceCommand : NlDumpCommandBase
{
    public DumpInterfaceCommand(bool isSplitDumpSupported)
        : base(Nl80211Command.NL80211_CMD_GET_INTERFACE, isSplitDumpSupported, Nl80211Attribute.NL80211_ATTR_IFINDEX)
    {
    }
}
