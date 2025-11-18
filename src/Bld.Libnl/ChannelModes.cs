using Bld.Libnl.Types;

namespace Bld.Libnl;

public static class ChannelModes
{
    public static IReadOnlyCollection<ChannelMode> Modes { get; } =
    [
        new()
        {
            name = "HT20",
            width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_20,
            freq1_diff = 0,
            chantype = Nl80211ChannelType.NL80211_CHAN_HT20
        },
        new()
        {
            name = "HT40+",
            width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_40,
            freq1_diff = 10,
            chantype = Nl80211ChannelType.NL80211_CHAN_HT40PLUS
        },
        new()
        {
            name = "HT40-",
            width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_40,
            freq1_diff = -10,
            chantype = Nl80211ChannelType.NL80211_CHAN_HT40MINUS
        },
        new()
        {
            name = "NOHT",
            width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_20_NOHT,
            freq1_diff = 0,
            chantype = Nl80211ChannelType.NL80211_CHAN_NO_HT
        },
        new()
        {
            name = "5MHz",
            width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_5,
            freq1_diff = 0,
            chantype = (Nl80211ChannelType)(-1)
        },
        new()
        {
            name = "10MHz",
            width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_10,
            freq1_diff = 0,
            chantype = (Nl80211ChannelType)(-1)
        },
        new()
        {
            name = "80MHz",
            width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_80,
            freq1_diff = 0,
            chantype = (Nl80211ChannelType)(-1)
        },
        new()
        {
            name = "160MHz",
            width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_160,
            freq1_diff = 0,
            chantype = (Nl80211ChannelType)(-1)
        },
        new()
        {
            name = "320MHz",
            width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_320,
            freq1_diff = 0,
            chantype = (Nl80211ChannelType)(-1)
        },
        new()
        {
            name = "1MHz",
            width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_1,
            freq1_diff = 0,
            chantype = (Nl80211ChannelType)(-1)
        },
        new()
        {
            name = "2MHz",
            width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_2,
            freq1_diff = 0,
            chantype = (Nl80211ChannelType)(-1)
        },
        new()
        {
            name = "4MHz",
            width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_4,
            freq1_diff = 0,
            chantype = (Nl80211ChannelType)(-1)
        },
        new()
        {
            name = "8MHz",
            width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_8,
            freq1_diff = 0,
            chantype = (Nl80211ChannelType)(-1)
        },
        new()
        {
            name = "16MHz",
            width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_16,
            freq1_diff = 0,
            chantype = (Nl80211ChannelType)(-1)
        }
    ];
}