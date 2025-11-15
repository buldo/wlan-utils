namespace Bld.Libnl.Types;

/// <summary>
/// Netlink message flags from linux/netlink.h
/// </summary>
[Flags]
public enum NetlinkMessageFlags
{
    /// <summary>
    /// It is request message.
    /// </summary>
    NLM_F_REQUEST = 0x01,

    /// <summary>
    /// Multipart message, terminated by NLMSG_DONE
    /// </summary>
    NLM_F_MULTI = 0x02,

    /// <summary>
    /// Reply with ack, with zero or error code
    /// </summary>
    NLM_F_ACK = 0x04,

    /// <summary>
    /// Receive resulting notifications
    /// </summary>
    NLM_F_ECHO = 0x08,

    /// <summary>
    /// Dump was inconsistent due to sequence change
    /// </summary>
    NLM_F_DUMP_INTR = 0x10,

    /// <summary>
    /// Dump was filtered as requested
    /// </summary>
    NLM_F_DUMP_FILTERED = 0x20,

    /* Modifiers to GET request */

    /// <summary>
    /// specify tree root
    /// </summary>
    NLM_F_ROOT = 0x100,
    /// <summary>
    /// return all matching
    /// </summary>
    NLM_F_MATCH = 0x200,
    /// <summary>
    /// atomic GET
    /// </summary>
    NLM_F_ATOMIC = 0x400,
    NLM_F_DUMP = (NLM_F_ROOT | NLM_F_MATCH),

    /* Modifiers to NEW request */
    /// <summary>
    /// Override existing
    /// </summary>
    NLM_F_REPLACE = 0x100,

    /// <summary>
    /// Do not touch, if it exists
    /// </summary>
    NLM_F_EXCL = 0x200,

    /// <summary>
    /// Create, if it does not exist
    /// </summary>
    NLM_F_CREATE = 0x400,

    /// <summary>
    /// Add to end of list
    /// </summary>
    NLM_F_APPEND = 0x800,

    /* Modifiers to DELETE request */
    /// <summary>
    /// Do not delete recursively
    /// </summary>
    NLM_F_NONREC = 0x100,

    /// <summary>
    /// Delete multiple objects
    /// </summary>
    NLM_F_BULK = 0x200,

    /* Flags for ACK message */
    /// <summary>
    /// request was capped
    /// </summary>
    NLM_F_CAPPED = 0x100,

    /// <summary>
    /// extended ACK TVLs were included
    /// </summary>
    NLM_F_ACK_TLVS = 0x200

/*
   4.4BSD ADD		NLM_F_CREATE|NLM_F_EXCL
   4.4BSD CHANGE	NLM_F_REPLACE

   True CHANGE		NLM_F_CREATE|NLM_F_REPLACE
   Append		NLM_F_CREATE
   Check		NLM_F_EXCL
 */
}
