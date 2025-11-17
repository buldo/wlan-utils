namespace Bld.Libnl.Types;

public sealed class BitrateInfo
{
    /// <summary>
    /// Bitrate in Mbps (non-HT rate)
    /// </summary>
    public double Mbps { get; set; }

    /// <summary>
    /// For 2.4 GHz, whether short preamble is supported at this rate
    /// </summary>
    public bool ShortPreamble2GHz { get; set; }
}