using Bld.Libnl.Types;

namespace Bld.Libnl;

public static class ChannelModes
{
    public static ChannelMode ModeHt20 { get; } = new()
    {
        name = "HT20",
        width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_20,
        freq1_diff = 0,
        chantype = Nl80211ChannelType.NL80211_CHAN_HT20
    };

    public static ChannelMode ModeHt40P { get; } = new()
    {
        name = "HT40+",
        width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_40,
        freq1_diff = 10,
        chantype = Nl80211ChannelType.NL80211_CHAN_HT40PLUS
    };

    public static ChannelMode ModeHt40M { get; } = new()
    {
        name = "HT40-",
        width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_40,
        freq1_diff = -10,
        chantype = Nl80211ChannelType.NL80211_CHAN_HT40MINUS
    };
    public static ChannelMode ModeNoHt { get; } = new()
    {
        name = "NOHT",
        width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_20_NOHT,
        freq1_diff = 0,
        chantype = Nl80211ChannelType.NL80211_CHAN_NO_HT
    };

    public static ChannelMode Mode5Mhz { get; } = new()
    {
        name = "5MHz",
        width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_5,
        freq1_diff = 0,
        chantype = (Nl80211ChannelType)(-1)
    };

    public static ChannelMode Mode10MHz { get; } = new()
    {
        name = "10MHz",
        width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_10,
        freq1_diff = 0,
        chantype = (Nl80211ChannelType)(-1)
    };

    public static ChannelMode Mode80MHz { get; } = new()
    {
        name = "80MHz",
        width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_80,
        freq1_diff = 0,
        chantype = (Nl80211ChannelType)(-1)
    };

    public static ChannelMode Mode160MHz { get; } = new()
    {
        name = "160MHz",
        width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_160,
        freq1_diff = 0,
        chantype = (Nl80211ChannelType)(-1)
    };

    public static ChannelMode Mode320MHz { get; } = new()
    {
        name = "320MHz",
        width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_320,
        freq1_diff = 0,
        chantype = (Nl80211ChannelType)(-1)
    };

    public static ChannelMode Mode1MHz { get; } = new()
    {
        name = "1MHz",
        width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_1,
        freq1_diff = 0,
        chantype = (Nl80211ChannelType)(-1)
    };

    public static ChannelMode Mode2MHz { get; } = new()
    {
        name = "2MHz",
        width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_2,
        freq1_diff = 0,
        chantype = (Nl80211ChannelType)(-1)
    };

    public static ChannelMode Mode4MHz { get; } = new()
    {
        name = "4MHz",
        width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_4,
        freq1_diff = 0,
        chantype = (Nl80211ChannelType)(-1)
    };

    public static ChannelMode Mode8MHz { get; } = new()
    {
        name = "8MHz",
        width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_8,
        freq1_diff = 0,
        chantype = (Nl80211ChannelType)(-1)
    };

    public static ChannelMode Mode16MHz { get; } = new()
    {
        name = "16MHz",
        width = Nl80211ChannelWidth.NL80211_CHAN_WIDTH_16,
        freq1_diff = 0,
        chantype = (Nl80211ChannelType)(-1)
    };
}