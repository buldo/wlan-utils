using Bld.Libnl;
using Bld.Libnl.Types;
using Microsoft.Extensions.Logging;

namespace Bld.WlanUtils.Examples;

internal class Program
{
    static async Task Main(string[] args)
    {
        var loggerFactory = LoggerFactory.Create(
            loggingBuilder => loggingBuilder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Trace));
        var logger = loggerFactory.CreateLogger<Program>();
        var manager = new WlanManager(loggerFactory.CreateLogger<WlanManager>());
        var devices = manager.GetWlanInterfaces();
        foreach (var device in devices)
        {
            logger.LogInformation(
                """
                Device: {Interface}
                    Driver: {Driver}
                    Monitor support: {Monitor}
                    Current mode: {Mode}
                    Bands: {Bands}
                """,
                device.InterfaceName,
                device.DriverName,
                device.SupportedInterfaceTypes?.Contains(Nl80211InterfaceType.NL80211_IFTYPE_MONITOR),
                device.CurrentInterfaceMode,
                device.PhyAttributes[Nl80211Attribute.NL80211_ATTR_WIPHY_BANDS].AsBands().Select(b => b.Band).ToList());
        }
        //var deviceName = "wlx00c0caa98097";
        //await manager.TrySwitchToMonitorAsync(deviceName);
        //await manager.IwSetFrequencyAndChannelWidth(deviceName, Channels.Ch048, ChannelWidth._20MHz);

        //var ch = Channels.Ch048;
        //var byFreq = ChannelsMapper.GetByFrequency(ch.ChannelFrequencyMHz);
        //var byNum = ChannelsMapper.GetByNumber(ch.ChannelNumber);
    }
}
