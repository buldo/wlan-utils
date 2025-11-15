using Bld.Libnl;
using Bld.Libnl.Types;

namespace Bld.WlanUtils;

public class WlanDeviceInfo
{
    public required WiFiInterfaceInfo NlInterfaceInfo { get; init; }
    public string Interface => ((Nl80211AttributeValue<string>)(NlInterfaceInfo.Attributes[Nl80211Attribute.NL80211_ATTR_IFNAME])).Value;
}