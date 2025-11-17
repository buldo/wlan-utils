using Bld.Libnl;
using Bld.Libnl.Types;
using Bld.NetworkManager;
using Microsoft.Extensions.Logging;
using RunProcessAsTask;

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


        // if (useRfKill)
        // {
        //     await RfkillUnblockAll();
        // }

        // await Task.Delay(TimeSpan.FromSeconds(1));
        // await IpLinkSetCardState(deviceName, false);
        // await IwEnableMonitorMode(deviceName);
        // await IpLinkSetCardState(deviceName, true);

        _logger.LogDebug("END TrySwitchToMonitorAsync");
    }

    /// <summary>
    /// Set frequency and channel width via iw
    /// </summary>
    /// <param name="deviceName">Dev name</param>
    /// <param name="channel">Channel</param>
    /// <param name="channelWidth">Channel width</param>
    /// <returns></returns>
    /// <remarks>
    /// Using:
    ///   * iw
    /// </remarks>
    public async Task<bool> IwSetFrequencyAndChannelWidth(
        string deviceName,
        WlanChannel channel,
        ChannelWidth channelWidth)
    {
        _logger.LogDebug("Trying to call 'iw dev @dev set freq @freq @width'");
        var widthString = ChannelWidthAsIwString(channelWidth);
        var result = await RunWithLog("iw", $"dev {deviceName} set freq {channel.ChannelFrequencyMHz} {widthString}");

        if (result == null)
        {
            return false;
        }

        return result.ExitCode == 0;
    }

    private static string ChannelWidthAsIwString(ChannelWidth channelWidth)
    {
        return channelWidth switch
        {
            ChannelWidth._05MHz => "5MHz",
            ChannelWidth._10MHz => "10Mhz",
            ChannelWidth._20MHz => "HT20",
            ChannelWidth._40MHz => "HT40+",
            _ => throw new ArgumentOutOfRangeException(nameof(channelWidth), channelWidth, null)
        };
    }

    private async Task RfkillUnblockAll()
    {
        _logger.LogDebug("Trying to call 'rfkill unblock all'");
        await RunWithLog("rfkill", "unblock all");
    }

    private async Task IwEnableMonitorMode(string deviceName)
    {
        _logger.LogDebug("Trying to call 'iw dev @dev set monitor otherbss'");
        await RunWithLog("iw", $"dev {deviceName} set monitor otherbss");
    }

    private async Task IpLinkSetCardState(string deviceName, bool up)
    {
        _logger.LogDebug("Trying to call 'ip link set dev @dev @up/@down'");
        await RunWithLog("ip", $"link set dev {deviceName} {(up ? "up" : "down")}");
    }

    private async Task<ProcessResults?> RunWithLog(string fileName, string arguments)
    {
        ProcessResults result;
        try
        {
            result = await ProcessEx.RunAsync(fileName, arguments);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while calling {fileName}", fileName);
            return null;
        }


        if (result.StandardOutput.Length != 0)
        {
            foreach (var s in result.StandardOutput)
            {
                _logger.LogDebug(s);
            }
        }

        if (result.StandardError.Length != 0)
        {
            foreach (var s in result.StandardError)
            {
                _logger.LogDebug(s);
            }
        }

        return result;
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
