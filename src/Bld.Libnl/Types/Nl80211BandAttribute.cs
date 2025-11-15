namespace Bld.Libnl.Types;

/// <summary>
/// Band attributes from linux/nl80211.h
/// </summary>
public enum Nl80211BandAttribute
{
    /// <summary>
    /// attribute number 0 is reserved
    /// </summary>
    __NL80211_BAND_ATTR_INVALID,

    /// <summary>
    /// supported frequencies in this band, an array of nested frequency attributes
    /// </summary>
    NL80211_BAND_ATTR_FREQS,

    /// <summary>
    /// supported bitrates in this band, an array of nested bitrate attributes
    /// </summary>
    NL80211_BAND_ATTR_RATES,

    /// <summary>
    /// 16-byte attribute containing the MCS set as defined in 802.11n
    /// </summary>
    NL80211_BAND_ATTR_HT_MCS_SET,

    /// <summary>
    /// HT capabilities, as in the HT information IE
    /// </summary>
    NL80211_BAND_ATTR_HT_CAPA,

    /// <summary>
    /// A-MPDU factor, as in 11n
    /// </summary>
    NL80211_BAND_ATTR_HT_AMPDU_FACTOR,

    /// <summary>
    /// A-MPDU density, as in 11n
    /// </summary>
    NL80211_BAND_ATTR_HT_AMPDU_DENSITY,

    /// <summary>
    /// 32-byte attribute containing the MCS set as defined in 802.11ac
    /// </summary>
    NL80211_BAND_ATTR_VHT_MCS_SET,

    /// <summary>
    /// VHT capabilities, as in the HT information IE
    /// </summary>
    NL80211_BAND_ATTR_VHT_CAPA,

    /// <summary>
    /// nested array attribute, with each entry using attributes from &enum nl80211_band_iftype_attr
    /// </summary>
    NL80211_BAND_ATTR_IFTYPE_DATA,

    /// <summary>
    /// bitmap that indicates the 2.16 GHz channel(s) that are allowed to be used for EDMG transmissions.
    /// Defined by IEEE P802.11ay/D4.0 section 9.4.2.251.
    /// </summary>
    NL80211_BAND_ATTR_EDMG_CHANNELS,

    /// <summary>
    /// Channel BW Configuration subfield encodes the allowed channel bandwidth configurations.
    /// Defined by IEEE P802.11ay/D4.0 section 9.4.2.251, Table 13.
    /// </summary>
    NL80211_BAND_ATTR_EDMG_BW_CONFIG,

    /// <summary>
    /// S1G capabilities, supported S1G-MCS and NSS set subfield, as in the S1G information IE, 5 bytes
    /// </summary>
    NL80211_BAND_ATTR_S1G_MCS_NSS_SET,

    /// <summary>
    /// S1G capabilities information subfield as in the S1G information IE, 10 bytes
    /// </summary>
    NL80211_BAND_ATTR_S1G_CAPA,
}
