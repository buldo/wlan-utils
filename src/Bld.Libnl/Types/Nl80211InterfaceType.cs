namespace Bld.Libnl.Types;

/// <summary>
/// Interface types from linux/nl80211.h
/// </summary>
public enum Nl80211InterfaceType
{
    /// <summary>
    /// unspecified type, driver decides
    /// </summary>
    NL80211_IFTYPE_UNSPECIFIED,

    /// <summary>
    /// independent BSS member
    /// </summary>
    NL80211_IFTYPE_ADHOC,

    /// <summary>
    /// managed BSS member
    /// </summary>
    NL80211_IFTYPE_STATION,

    /// <summary>
    /// access point
    /// </summary>
    NL80211_IFTYPE_AP,

    /// <summary>
    /// VLAN interface for access points; VLAN interfaces are a bit special in that they must always be tied to a pre-existing AP type interface.
    /// </summary>
    NL80211_IFTYPE_AP_VLAN,

    /// <summary>
    /// wireless distribution interface
    /// </summary>
    NL80211_IFTYPE_WDS,

    /// <summary>
    /// monitor interface receiving all frames
    /// </summary>
    NL80211_IFTYPE_MONITOR,

    /// <summary>
    /// mesh point
    /// </summary>
    NL80211_IFTYPE_MESH_POINT,

    /// <summary>
    /// P2P client
    /// </summary>
    NL80211_IFTYPE_P2P_CLIENT,

    /// <summary>
    /// P2P group owner
    /// </summary>
    NL80211_IFTYPE_P2P_GO,

    /// <summary>
    /// P2P device interface type, this is not a netdev and therefore can't be created in the normal ways, use the %NL80211_CMD_START_P2P_DEVICE and %NL80211_CMD_STOP_P2P_DEVICE commands to create and destroy one
    /// </summary>
    NL80211_IFTYPE_P2P_DEVICE,

    /// <summary>
    /// Outside Context of a BSS This mode corresponds to the MIB variable dot11OCBActivated=true
    /// </summary>
    NL80211_IFTYPE_OCB,

    /// <summary>
    /// NAN device interface type (not a netdev)
    /// </summary>
    NL80211_IFTYPE_NAN,
}
