namespace Bld.Libnl.Types;

public enum Nl80211Band
{
    /// <summary>
    /// 2.4 GHz ISM band
    /// </summary>
    NL80211_BAND_2GHZ,

    /// <summary>
    /// around 5 GHz band (4.9 - 5.7 GHz)
    /// </summary>
    NL80211_BAND_5GHZ,

    /// <summary>
    /// around 60 GHz band (58.32 - 69.12 GHz)
    /// </summary>
    NL80211_BAND_60GHZ,

    /// <summary>
    /// around 6 GHz band (5.9 - 7.2 GHz)
    /// </summary>
    NL80211_BAND_6GHZ,

    /// <summary>
    /// around 900MHz, supported by S1G PHYs
    /// </summary>
    NL80211_BAND_S1GHZ,

    /// <summary>
    /// light communication band (placeholder)
    /// </summary>
    NL80211_BAND_LC,
};