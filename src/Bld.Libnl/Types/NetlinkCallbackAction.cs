namespace Bld.Libnl.Types;

/// <summary>
/// Callback return codes
/// </summary>
public enum NetlinkCallbackAction
{
    /// <summary>
    /// Proceed with whatever comes next
    /// </summary>
    NL_OK = 0,

    /// <summary>
    /// Skip this message
    /// </summary>
    NL_SKIP = 1,

    /// <summary>
    /// Stop parsing altogether and discard remaining messages
    /// </summary>
    NL_STOP = 2,
}
