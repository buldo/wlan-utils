using System.Collections.ObjectModel;
using Bld.Libnl.Types;

namespace Bld.Libnl;

/// <summary>
/// Information about a WiFi network interface
/// </summary>
public sealed class WiFiInterfaceInfo
{
    public ReadOnlyDictionary<Nl80211Attribute, INl80211AttributeValue> Attributes { get; init; } =
        new ReadOnlyDictionary<Nl80211Attribute, INl80211AttributeValue>(new Dictionary<Nl80211Attribute, INl80211AttributeValue>());
}
