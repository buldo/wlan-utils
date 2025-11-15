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
    /// Get frequencies nested attribute (NL80211_BAND_ATTR_FREQS)
    /// </summary>
    public INl80211AttributeValue? Frequencies =>
        Attributes.TryGetValue(Nl80211BandAttribute.NL80211_BAND_ATTR_FREQS, out var freq) ? freq : null;

    /// <summary>
    /// Get rates nested attribute (NL80211_BAND_ATTR_RATES)
    /// </summary>
    public INl80211AttributeValue? Rates =>
        Attributes.TryGetValue(Nl80211BandAttribute.NL80211_BAND_ATTR_RATES, out var rates) ? rates : null;

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
