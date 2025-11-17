namespace Bld.Libnl.Types;

/// <summary>
/// Monitor configuration flags
/// </summary>
public enum Nl80211MonitorFlag
{
    /// <summary>
    /// pass frames with bad FCS
    /// </summary>
    NL80211_MNTR_FLAG_FCSFAIL = 1,

    /// <summary>
    /// pass frames with bad PLCP
    /// </summary>
    NL80211_MNTR_FLAG_PLCPFAIL = 2,

    /// <summary>
    /// pass control frames
    /// </summary>
    NL80211_MNTR_FLAG_CONTROL = 3,

    /// <summary>
    /// disable BSSID filtering
    /// </summary>
    NL80211_MNTR_FLAG_OTHER_BSS = 4,

    /// <summary>
    /// deprecated will unconditionally be refused
    /// </summary>
    NL80211_MNTR_FLAG_COOK_FRAMES = 5,

    /// <summary>
    /// use the configured MAC address and ACK incoming unicast packetsactive monitor: use no ack, no encryption, etc.
    /// </summary>
    NL80211_MNTR_FLAG_ACTIVE = 6,

    /// <summary>
    /// do not pass local tx packets
    /// </summary>
    NL80211_MNTR_FLAG_SKIP_TX = 7
}
