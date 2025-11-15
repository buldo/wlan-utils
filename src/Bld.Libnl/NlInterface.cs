using Bld.Libnl.NlCommands;
using Bld.Libnl.Types;

namespace Bld.Libnl;

public class NlInterface
{
    private readonly bool _isSplitDumpSupperted;

    public NlInterface()
    {
        using var featuresCommand = new GetProtocolFeaturesCommand();
        var result = featuresCommand.Run();
        if (result.Contains(Nl80211ProtocolFeature.NL80211_PROTOCOL_FEATURE_SPLIT_WIPHY_DUMP))
        {
            _isSplitDumpSupperted = true;
        }
    }

    public List<Dictionary<Nl80211Attribute, INl80211AttributeValue>> DumpWiPhy()
    {
        using var command = new DumpWiPhyComman();
        var result = command.Run();
        return result;
    }

    public List<Dictionary<Nl80211Attribute, INl80211AttributeValue>> DumpInterface()
    {
        using var command = new DumpInterfaceCommand();
        var result = command.Run();
        return result;
    }

}
