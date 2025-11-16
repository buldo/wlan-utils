namespace Bld.Libnl.Types;

public sealed class BandInfo
{
    /// <summary>
    /// Band identifier (2GHz, 5GHz, 6GHz, etc.)
    /// </summary>
    public Nl80211Band Band { get; init; }

    /// <summary>
    /// Raw nested attributes for this band
    /// Contains frequencies, rates, and other band-specific data
    /// </summary>
    public Dictionary<Nl80211BandAttribute, INl80211AttributeValue> Attributes { get; init; } = new();

    /// <summary>
    /// Get frequencies nested attribute (NL80211_BAND_ATTR_FREQS) as managed model list
    /// </summary>
    public List<FrequencyInfo>? Frequencies =>
        Attributes.TryGetValue(Nl80211BandAttribute.NL80211_BAND_ATTR_FREQS, out var freq) ? freq.AsFrequencies() : null;

    /// <summary>
    /// Get rates nested attribute (NL80211_BAND_ATTR_RATES) as managed model list
    /// </summary>
    public List<BitrateInfo>? Rates =>
        Attributes.TryGetValue(Nl80211BandAttribute.NL80211_BAND_ATTR_RATES, out var rates) ? rates.AsBitrates() : null;

    /// <summary>
    /// Get HT capabilities (NL80211_BAND_ATTR_HT_CAPA)
    /// </summary>
    public ushort? HtCapabilities =>
        Attributes.TryGetValue(Nl80211BandAttribute.NL80211_BAND_ATTR_HT_CAPA, out var capa)
            ? capa.AsU16() : null;

    /// <summary>
    /// Get VHT capabilities (NL80211_BAND_ATTR_VHT_CAPA)
    /// </summary>
    public uint? VhtCapabilities =>
        Attributes.TryGetValue(Nl80211BandAttribute.NL80211_BAND_ATTR_VHT_CAPA, out var capa)
            ? capa.AsU32() : null;
}
