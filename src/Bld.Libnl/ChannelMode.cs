using Bld.Libnl.Types;

namespace Bld.Libnl;

public class ChannelMode
{
    internal ChannelMode()
    {

    }

    public required string name { get; init; }
    public required Nl80211ChannelWidth width { get; init; }
    public required int freq1_diff { get; init; }
    public required Nl80211ChannelType chantype { get; init; }
}
