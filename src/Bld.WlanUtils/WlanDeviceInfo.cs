using Bld.Libnl;
using Bld.Libnl.Types;

namespace Bld.WlanUtils;

public class WlanDeviceInfo
{
    public WlanDeviceInfo(
        Dictionary<Nl80211Attribute, INl80211AttributeValue> phyAttributes,
        Dictionary<Nl80211Attribute, INl80211AttributeValue> interfaceAttributes)
    {
        PhyAttributes = phyAttributes;
        InterfaceAttributes = interfaceAttributes;
    }

    public IReadOnlyDictionary<Nl80211Attribute, INl80211AttributeValue> PhyAttributes { get; }
    public IReadOnlyDictionary<Nl80211Attribute, INl80211AttributeValue> InterfaceAttributes { get; }

    public string InterfaceName => ((Nl80211AttributeValue<string>)InterfaceAttributes[Nl80211Attribute.NL80211_ATTR_IFNAME]).Value;

    public HashSet<Nl80211InterfaceType>? SupportedInterfaceTypes => PhyAttributes[Nl80211Attribute.NL80211_ATTR_SUPPORTED_IFTYPES].AsInterfaceTypes();
}
