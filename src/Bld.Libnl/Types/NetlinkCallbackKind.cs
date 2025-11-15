// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Bld.Libnl.Types;

/// <summary>
/// Callback kinds from libnl
/// </summary>
public enum NetlinkCallbackKind
{
    /// <summary>
    /// Default handlers (quiet)
    /// </summary>
    NL_CB_DEFAULT,

    /// <summary>
    /// Verbose default handlers (error messages printed)
    /// </summary>
    NL_CB_VERBOSE,

    /// <summary>
    /// Debug handlers for debugging
    /// </summary>
    NL_CB_DEBUG,

    /// <summary>
    /// Customized handler specified by the user
    /// </summary>
    NL_CB_CUSTOM,
}
