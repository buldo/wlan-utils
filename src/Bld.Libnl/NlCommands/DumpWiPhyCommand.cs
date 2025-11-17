using Bld.Libnl.Types;

namespace Bld.Libnl.NlCommands;

internal class DumpWiPhyCommand : NlDumpCommandBaseResult
{
    public DumpWiPhyCommand(bool isSplitDumpSupported)
        : base(Nl80211Command.NL80211_CMD_GET_WIPHY, isSplitDumpSupported, Nl80211Attribute.NL80211_ATTR_WIPHY)
    {
    }
}
