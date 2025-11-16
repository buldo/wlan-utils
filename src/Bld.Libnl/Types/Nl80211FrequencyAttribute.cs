namespace Bld.Libnl.Types;

/// <summary>
/// Frequency nested attributes, mirror of nl80211_frequency_attr from linux/nl80211.h
/// Only a subset is defined, enough for our parsing needs.
/// </summary>
public enum Nl80211FrequencyAttribute
{
    __NL80211_FREQUENCY_ATTR_INVALID = 0,
    NL80211_FREQUENCY_ATTR_FREQ = 1,
    NL80211_FREQUENCY_ATTR_DISABLED = 2,
    NL80211_FREQUENCY_ATTR_NO_IR = 3,
    // Alias in the kernel headers; keep the same numeric value
    __NL80211_FREQUENCY_ATTR_NO_IBSS = NL80211_FREQUENCY_ATTR_NO_IR,
    NL80211_FREQUENCY_ATTR_RADAR = 4,
    NL80211_FREQUENCY_ATTR_MAX_TX_POWER = 5,
    // Skipping many fields we don't need right now
    NL80211_FREQUENCY_ATTR_OFFSET = 26,
}
