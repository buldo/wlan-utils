namespace Bld.Libnl.Types;

/// <summary>
/// channel type
/// </summary>
public enum Nl80211ChannelType
{
    /// <summary>
    /// 20 MHz, non-HT channel
    /// </summary>
    NL80211_CHAN_NO_HT,

    /// <summary>
    /// 20 MHz HT channel
    /// </summary>
    NL80211_CHAN_HT20,

    /// <summary>
    /// HT40 channel, secondary channel below the control channel
    /// </summary>
    NL80211_CHAN_HT40MINUS,

    /// <summary>
    /// HT40 channel, secondary channel above the control channel
    /// </summary>
    NL80211_CHAN_HT40PLUS
};
