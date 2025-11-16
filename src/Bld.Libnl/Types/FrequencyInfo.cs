namespace Bld.Libnl.Types;

public sealed class FrequencyInfo
{
    /// <summary>
    /// Center frequency in MHz
    /// </summary>
    public uint FrequencyMHz { get; set; }

    /// <summary>
    /// Optional frequency offset in kHz (printed as fractional MHz)
    /// </summary>
    public uint? OffsetKHz { get; set; }

    /// <summary>
    /// Max TX power in dBm (if provided)
    /// </summary>
    public double? MaxTxPowerDbm { get; set; }

    public bool Disabled { get; set; }
    public bool NoIR { get; set; }
    public bool RadarDetection { get; set; }

    /// <summary>
    /// Convenience combined frequency including offset, in MHz
    /// </summary>
    public double FrequencyWithOffsetMHz => FrequencyMHz + (OffsetKHz.GetValueOrDefault() / 1000.0);
}
