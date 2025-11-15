namespace Bld.Libnl.Types;

/// <summary>
/// nl80211 protocol features
/// </summary>
[Flags]
public enum Nl80211ProtocolFeature : uint
{
    /// <summary>
    /// nl80211 supports splitting wiphy dumps (if requested by the application with the attribute %NL80211_ATTR_SPLIT_WIPHY_DUMP.
    /// Also supported is filtering the wiphy dump by %NL80211_ATTR_WIPHY, %NL80211_ATTR_IFINDEX or %NL80211_ATTR_WDEV.
    /// </summary>
    NL80211_PROTOCOL_FEATURE_SPLIT_WIPHY_DUMP = 1 << 0,
}
