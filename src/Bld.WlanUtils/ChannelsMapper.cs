namespace Bld.WlanUtils;

public static class ChannelsMapper
{
    private static readonly HashSet<WlanChannel> ChannelsList = new()
    {
        new(1, 2412),
        new(2, 2417),
        new(3, 2422),
        new(4, 2427),
        new(5, 2432),
        new(6, 2437),
        new(7, 2442),
        new(8, 2447),
        new(9, 2452),
        new(10, 2457),
        new(11, 2462),
        new(12, 2467),
        new(13, 2472),
        new(14, 2484),

        new(32, 5160),
        new(36, 5180),
        new(40, 5200),
        new(44, 5220),
        new(48, 5240),
        new(52, 5260),
        new(56, 5280),
        new(60, 5300),
        new(64, 5320),
        new(68, 5340),
        new(72, 5360),
        new(76, 5380),
        new(80, 5400),
        new(84, 5420),
        new(88, 5440),
        new(92, 5460),
        new(96, 5480),
        new(100, 5500),
        new(104, 5520),
        new(108, 5540),
        new(112, 5560),
        new(116, 5580),
        new(120, 5600),
        new(124, 5620),
        new(128, 5640),
        new(132, 5660),
        new(136, 5680),
        new(140, 5700),
        new(144, 5720),
        new(149, 5745),
        new(153, 5765),
        new(157, 5785),
        new(161, 5805),
        new(165, 5825),
        new(169, 5845),
        new(173, 5865),
        new(177, 5885),
    };

    private static readonly Dictionary<int, WlanChannel> ChannelsByNum = new ();

    private static readonly Dictionary<int, WlanChannel> ChannelsByFreq = new();

    static ChannelsMapper()
    {
        foreach (var channel in ChannelsList)
        {
            ChannelsByFreq[channel.ChannelFrequencyMHz] = channel;
            ChannelsByNum[channel.ChannelNumber] = channel;
        }
    }

    public static WlanChannel GetByNumber(int channelNumber) => ChannelsByNum[channelNumber];

    public static WlanChannel GetByFrequency(int channelFrequencyMHz) => ChannelsByFreq[channelFrequencyMHz];
}