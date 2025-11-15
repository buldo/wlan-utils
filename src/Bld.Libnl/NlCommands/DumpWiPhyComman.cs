using Bld.Libnl.Types;

namespace Bld.Libnl.NlCommands;

internal class DumpWiPhyComman : NlDumpCommandBase
{
    public DumpWiPhyComman() : base(Nl80211Command.NL80211_CMD_GET_WIPHY)
    {
    }
}
