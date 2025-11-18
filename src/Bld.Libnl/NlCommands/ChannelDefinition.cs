using Bld.Libnl.Types;

namespace Bld.Libnl.NlCommands;

public class ChannelDefinition
{
    public Nl80211ChannelWidth Width { get; set; }
    public uint ControlFreq { get; set; }
    public uint ControlFreqOffset { get; set; }
    public uint CenterFreq1 { get; set; }
    public uint CenterFreq1Offset { get; set; }
    public uint CenterFreq2 { get; set; }
    public uint Punctured { get; set; }
}