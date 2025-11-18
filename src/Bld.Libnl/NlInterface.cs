using System.Threading.Channels;

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
        using var command = new DumpWiPhyCommand(_isSplitDumpSupperted);
        var result = command.Run();
        return result;
    }

    public List<Dictionary<Nl80211Attribute, INl80211AttributeValue>> DumpInterface()
    {
        using var command = new DumpInterfaceCommand(_isSplitDumpSupperted);
        var result = command.Run();
        return result;
    }

    public void SetInterfaceType(uint interfaceIndex, Nl80211InterfaceType type)
    {
        using var command = new SetInterfaceTypeCommand(interfaceIndex, type);
        command.Run();
    }

    public void SetMonitorFlags(uint interfaceIndex, IEnumerable<Nl80211MonitorFlag> flags)
    {
        using var command = new SetMonitorFlagsCommand(interfaceIndex, flags);
        command.Run();
    }

    public void InterfaceDown(uint interfaceIndex)
    {
        using var linkManager = new LinkManager();

        if (linkManager.IsLinkUp(interfaceIndex))
        {
            linkManager.SetLinkState(interfaceIndex, false);
        }
    }

    public void InterfaceUp(uint interfaceIndex)
    {
        using var linkManager = new LinkManager();

        if (!linkManager.IsLinkUp(interfaceIndex))
        {
            linkManager.SetLinkState(interfaceIndex, true);
        }
    }

    public void SetChannel(uint interfaceIndex, uint chan, Nl80211Band band, ChannelMode mode)
    {
        var freq = ChannelsUtils.ChannelToFrequency(chan, band);
        SetChannel(interfaceIndex, freq, mode);
    }

    public void SetChannel(uint interfaceIndex, uint freq, ChannelMode mode)
    {
        var chanDef = new ChannelDefinition()
        {
            ControlFreq = freq,
            ControlFreqOffset = 0,
            CenterFreq1 = ChannelsUtils.GetCf1(mode, freq),
            CenterFreq1Offset = 0,
            Width = mode.width
        };

        /* For non-S1G frequency */
        if (chanDef.CenterFreq1 > 1000)
        {
            chanDef.CenterFreq1Offset = 0;
        }

        var command = new SwitchChannelCommand(interfaceIndex, chanDef);
        command.Run();
    }
}
