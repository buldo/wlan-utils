using Microsoft.Extensions.Logging;
using RunProcessAsTask;

namespace Bld.WlanUtils;

public class WlanManager
{
    private readonly ILogger<WlanManager> _logger;

    public WlanManager(ILogger<WlanManager> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Switching to monitor mode
    /// </summary>
    /// <param name="deviceName">Dev name</param>
    /// <remarks>
    /// Using:
    ///   * nmcli
    ///   * rfkill
    ///   * iw
    ///   * ip
    /// </remarks>
    public async Task TrySwitchToMonitorAsync(string deviceName)
    {
        _logger.LogDebug("START TrySwitchToMonitorAsync");

        await NmcliSetDeviceManagedStatus(deviceName, false);
        await RfkillUnblockAll();
        await Task.Delay(TimeSpan.FromSeconds(1));
        await IpLinkSetCardState(deviceName, false);
        await IwEnableMonitorMode(deviceName);
        await IpLinkSetCardState(deviceName, true);

        _logger.LogDebug("END TrySwitchToMonitorAsync");
    }

    private async Task NmcliSetDeviceManagedStatus(string deviceName, bool managed)
    {
        _logger.LogDebug("Trying to call 'nmcli device set @dev managed @yes/@no'");
        await RunWithLog("nmcli", $"device set {deviceName} managed {(managed ? "yes" : "no")}");
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

    private async Task RunWithLog(string fileName, string arguments)
    {
        ProcessResults result;
        try
        {
            result = await ProcessEx.RunAsync(fileName, arguments);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while calling {fileName}", fileName);
            return;
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
    }
}
