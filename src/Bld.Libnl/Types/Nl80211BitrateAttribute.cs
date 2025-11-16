namespace Bld.Libnl.Types;

/// <summary>
/// Bitrate nested attributes, mirror of nl80211_bitrate_attr from linux/nl80211.h
/// Only a subset is defined.
/// </summary>
public enum Nl80211BitrateAttribute
{
    __NL80211_BITRATE_ATTR_INVALID = 0,
    NL80211_BITRATE_ATTR_RATE = 1,
    NL80211_BITRATE_ATTR_2GHZ_SHORTPREAMBLE = 2,
}
