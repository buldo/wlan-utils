namespace Bld.Libnl;

internal enum NetlinkProtocol
{
    /// <summary>
    /// Routing/device hook
    /// </summary>
    NETLINK_ROUTE = 0,

    /// <summary>
    /// Unused number
    /// </summary>
    NETLINK_UNUSED = 1,

    /// <summary>
    /// Reserved for user mode socket protocols
    /// </summary>
    NETLINK_USERSOCK = 2,

    /// <summary>
    /// Unused number, formerly ip_queue
    /// </summary>
    NETLINK_FIREWALL = 3,

    /// <summary>
    /// socket monitoring
    /// </summary>
    NETLINK_SOCK_DIAG = 4,

    /// <summary>
    /// netfilter/iptables ULOG
    /// </summary>
    NETLINK_NFLOG = 5,

    /// <summary>
    /// ipsec
    /// </summary>
    NETLINK_XFRM = 6,

    /// <summary>
    /// SELinux event notifications
    /// </summary>
    NETLINK_SELINUX = 7,

    /// <summary>
    /// Open-iSCSI
    /// </summary>
    NETLINK_ISCSI = 8,

    /// <summary>
    /// auditing
    /// </summary>
    NETLINK_AUDIT = 9,
    NETLINK_FIB_LOOKUP = 10,
    NETLINK_CONNECTOR = 11,

    /// <summary>
    /// netfilter subsystem
    /// </summary>
    NETLINK_NETFILTER = 12,
    NETLINK_IP6_FW = 13,

    /// <summary>
    /// DECnet routing messages (obsolete)
    /// </summary>
    NETLINK_DNRTMSG = 14,

    /// <summary>
    /// Kernel messages to userspace
    /// </summary>
    NETLINK_KOBJECT_UEVENT = 15,
    NETLINK_GENERIC = 16,

    /* leave room for NETLINK_DM (DM Events) */

    /// <summary>
    /// SCSI Transports
    /// </summary>
    NETLINK_SCSITRANSPORT = 18,

    NETLINK_ECRYPTFS = 19,

    NETLINK_RDMA = 20,

    /// <summary>
    /// Crypto layer
    /// </summary>
    NETLINK_CRYPTO = 21,

    /// <summary>
    /// SMC monitoring
    /// </summary>
    NETLINK_SMC = 22,

    NETLINK_INET_DIAG = NETLINK_SOCK_DIAG
}