namespace Bld.Libnl.Types;

/// <summary>
/// Callback types from libnl
/// </summary>
public enum NetlinkCallbackType
{
    /// <summary>
    /// Message is valid
    /// </summary>
    NL_CB_VALID,

    /// <summary>
    /// Last message in a series of multi part messages received
    /// </summary>
    NL_CB_FINISH,

    /// <summary>
    /// Report received that data was lost
    /// </summary>
    NL_CB_OVERRUN,

    /// <summary>
    /// Message wants to be skipped
    /// </summary>
    NL_CB_SKIPPED,

    /// <summary>
    /// Message is an acknowledgement
    /// </summary>
    NL_CB_ACK,

    /// <summary>
    /// Called for every message received
    /// </summary>
    NL_CB_MSG_IN,

    /// <summary>
    /// Called for every message sent out except for nl_sendto()
    /// </summary>
    NL_CB_MSG_OUT,

    /// <summary>
    /// Message is malformed and invalid
    /// </summary>
    NL_CB_INVALID,

    /// <summary>
    /// Called instead of internal sequence number checking
    /// </summary>
    NL_CB_SEQ_CHECK,

    /// <summary>
    /// Sending of an acknowledge message has been requested
    /// </summary>
    NL_CB_SEND_ACK,

    /// <summary>
    /// Flag NLM_F_DUMP_INTR is set in message
    /// </summary>
    NL_CB_DUMP_INTR,
}
