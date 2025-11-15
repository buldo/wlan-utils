using Bld.Libnl.Types;

namespace Bld.Libnl.NlCommands;

internal class DumpInterfaceCommand : NlDumpCommandBase
{
    public DumpInterfaceCommand() : base(Nl80211Command.NL80211_CMD_GET_INTERFACE)
    {
    }
}
