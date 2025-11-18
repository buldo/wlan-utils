namespace Bld.Libnl.Types;

/// <summary>
/// channel width definitions
/// These values are used with the NL80211_ATTR_CHANNEL_WIDTH attribute.
/// </summary>
public enum ChannelWidth
{
    /// <summary>
    /// 20 MHz, non-HT channel
    /// </summary>
    NL80211_CHAN_WIDTH_20_NOHT,

    /// <summary>
    /// 20 MHz HT channel
    /// </summary>
    NL80211_CHAN_WIDTH_20,

    /// <summary>
    /// 40 MHz channel, the NL80211_ATTR_CENTER_FREQ1 attribute must be provided as well
    /// </summary>
    NL80211_CHAN_WIDTH_40,

    /// <summary>
    /// 80 MHz channel, the %NL80211_ATTR_CENTER_FREQ1 attribute must be provided as well
    /// </summary>
    NL80211_CHAN_WIDTH_80,

    /// <summary>
    /// 80+80 MHz channel, the %NL80211_ATTR_CENTER_FREQ1 and %NL80211_ATTR_CENTER_FREQ2 attributes must be provided as well
    /// </summary>
    NL80211_CHAN_WIDTH_80P80,

    /// <summary>
    /// 160 MHz channel, the %NL80211_ATTR_CENTER_FREQ1 attribute must be provided as well
    /// </summary>
    NL80211_CHAN_WIDTH_160,

    /// <summary>
    /// 5 MHz OFDM channel
    /// </summary>
    NL80211_CHAN_WIDTH_5,

    /// <summary>
    /// 10 MHz OFDM channel
    /// </summary>
    NL80211_CHAN_WIDTH_10,

    /// <summary>
    /// 1 MHz OFDM channel
    /// </summary>
    NL80211_CHAN_WIDTH_1,

    /// <summary>
    /// 2 MHz OFDM channel
    /// </summary>
    NL80211_CHAN_WIDTH_2,

    /// <summary>
    /// 4 MHz OFDM channel
    /// </summary>
    NL80211_CHAN_WIDTH_4,

    /// <summary>
    /// 8 MHz OFDM channel
    /// </summary>
    NL80211_CHAN_WIDTH_8,

    /// <summary>
    /// 16 MHz OFDM channel
    /// </summary>
    NL80211_CHAN_WIDTH_16,

    /// <summary>
    /// 320 MHz channel, the NL80211_ATTR_CENTER_FREQ1 attribute must be provided as well
    /// </summary>
    NL80211_CHAN_WIDTH_320,
}