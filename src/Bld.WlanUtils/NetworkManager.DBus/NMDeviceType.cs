// ReSharper disable once CheckNamespace
namespace NetworkManager.DBus;

public enum NMDeviceType : uint
{
    /// <summary>
    /// unknown device
    /// </summary>
    NM_DEVICE_TYPE_UNKNOWN = 0,

    /// <summary>
    /// a wired ethernet device
    /// </summary>
    NM_DEVICE_TYPE_ETHERNET = 1,

    /// <summary>
    /// an 802.11 WiFi device
    /// </summary>
    NM_DEVICE_TYPE_WIFI = 2,

    /// <summary>
    /// not used
    /// </summary>
    NM_DEVICE_TYPE_UNUSED1 = 3,

    /// <summary>
    /// not used
    /// </summary>
    NM_DEVICE_TYPE_UNUSED2 = 4,

    /// <summary>
    /// a Bluetooth device supporting PAN or DUN access protocols
    /// </summary>
    NM_DEVICE_TYPE_BT = 5,

    /// <summary>
    /// an OLPC XO mesh networking device
    /// </summary>
    NM_DEVICE_TYPE_OLPC_MESH = 6,

    /// <summary>
    /// an 802.16e Mobile WiMAX broadband device
    /// </summary>
    NM_DEVICE_TYPE_WIMAX = 7,

    /// <summary>
    /// a modem supporting analog telephone, CDMA/EVDO, GSM/UMTS, or LTE network access protocols
    /// </summary>
    NM_DEVICE_TYPE_MODEM = 8,

    /// <summary>
    /// an IP-over-InfiniBand device
    /// </summary>
    NM_DEVICE_TYPE_INFINIBAND = 9,

    /// <summary>
    /// a bond master interface
    /// </summary>
    NM_DEVICE_TYPE_BOND = 10,

    /// <summary>
    /// an 802.1Q VLAN interface
    /// </summary>
    NM_DEVICE_TYPE_VLAN = 11,

    /// <summary>
    /// ADSL modem
    /// </summary>
    NM_DEVICE_TYPE_ADSL = 12,

    /// <summary>
    /// a bridge master interface
    /// </summary>
    NM_DEVICE_TYPE_BRIDGE = 13,

    /// <summary>
    /// generic support for unrecognized device types
    /// </summary>
    NM_DEVICE_TYPE_GENERIC = 14,

    /// <summary>
    /// a team master interface
    /// </summary>
    NM_DEVICE_TYPE_TEAM = 15,

    /// <summary>
    /// a TUN or TAP interface
    /// </summary>
    NM_DEVICE_TYPE_TUN = 16,

    /// <summary>
    /// a IP tunnel interface
    /// </summary>
    NM_DEVICE_TYPE_IP_TUNNEL = 17,

    /// <summary>
    /// a MACVLAN interface
    /// </summary>
    NM_DEVICE_TYPE_MACVLAN = 18,

    /// <summary>
    /// a VXLAN interface
    /// </summary>
    NM_DEVICE_TYPE_VXLAN = 19,

    /// <summary>
    /// a VETH interface
    /// </summary>
    NM_DEVICE_TYPE_VETH = 20,
}
