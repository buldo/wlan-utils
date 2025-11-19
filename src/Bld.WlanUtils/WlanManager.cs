using Bld.Libnl;
using Bld.Libnl.Types;
using Bld.NetworkManager;
using Microsoft.Extensions.Logging;

namespace Bld.WlanUtils;

public class WlanManager
{
    private readonly ILogger<WlanManager> _logger;
    private readonly NlInterface _nlInterface;

    private NetworkManagerManager? _networkManagerManager;

    public WlanManager(ILogger<WlanManager> logger)
    {
        _logger = logger;
        _nlInterface = new NlInterface();
    }

    /// <summary>
    /// Switching to monitor mode
    /// </summary>
    /// <param name="deviceInfo">Device</param>
    /// <remarks>
    /// Alternative to next commands:
    ///   nmcli dev set wlan0 managed no
    ///   ip link set wlan0 down
    ///   iw wlan0 set type monitor
    ///   iw wlan0 set monitor otherbss
    ///   ip link set wlan0 up
    /// </remarks>
    public async Task TrySwitchToMonitorAsync(WlanDeviceInfo deviceInfo)
    {
        _logger.LogDebug("START TrySwitchToMonitorAsync");

        var nmm = await EnsureNetworkManagerAsync();
        var devices = await nmm.GetNmWiFiDevicesAsync();
        var selectedNmDevice = devices.First(p => p.Properties.Interface == deviceInfo.InterfaceName);
        await selectedNmDevice.Device.SetManagedAsync(false);
        _nlInterface.InterfaceDown(deviceInfo.InterfaceIndex);
        _nlInterface.SetInterfaceType(deviceInfo.InterfaceIndex, Nl80211InterfaceType.NL80211_IFTYPE_MONITOR);
        _nlInterface.SetMonitorFlags(deviceInfo.InterfaceIndex, [Nl80211MonitorFlag.NL80211_MNTR_FLAG_OTHER_BSS]);
        _nlInterface.InterfaceUp(deviceInfo.InterfaceIndex);
        _logger.LogDebug("END TrySwitchToMonitorAsync");
    }



    public void SetChannel(WlanDeviceInfo selectedDevice, uint freq, ChannelMode mode)
    {
        _nlInterface.SetChannel(selectedDevice.InterfaceIndex, freq, mode);
    }

    public IReadOnlyList<WlanDeviceInfo> GetWlanInterfaces()
    {
        var interfaces = _nlInterface.DumpInterface();
        var phy = _nlInterface.DumpWiPhy();
        var phyByWiPhy = phy.ToDictionary(
            p => ((Nl80211AttributeValue<uint>)p[Nl80211Attribute.NL80211_ATTR_WIPHY]).Value,
            p => p);

        var ret = new List<WlanDeviceInfo>();
        foreach (var iface in interfaces)
        {
            if(!iface.TryGetValue(Nl80211Attribute.NL80211_ATTR_IFNAME, out var ifaceName))
            {
                continue;
            }

            if (!iface.TryGetValue(Nl80211Attribute.NL80211_ATTR_WIPHY, out var phyIdValue))
            {
                continue;
            }

            var phyId = ((Nl80211AttributeValue<uint>)phyIdValue).Value;
            if (!phyByWiPhy.TryGetValue(phyId, out var phyInfo))
            {
                continue;
            }

            var driverName = GetDriverName(phyId);

            ret.Add(new WlanDeviceInfo(phyInfo, iface, driverName));
        }

        return ret;
    }

    private string GetDriverName(uint phyIndex)
    {
        var target = File.ResolveLinkTarget($"/sys/class/ieee80211/phy{phyIndex}/device/driver", true);
        return target!.Name;
    }

    private async Task<NetworkManagerManager> EnsureNetworkManagerAsync()
    {
        _networkManagerManager ??= await NetworkManagerManager.CreateAsync();
        return _networkManagerManager;
    }
}
