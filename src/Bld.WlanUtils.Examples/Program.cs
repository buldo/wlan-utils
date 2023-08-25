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
        var manager = new WlanManager(loggerFactory.CreateLogger<WlanManager>());
        var deviceName = "wlx00c0caa98097";
        await manager.TrySwitchToMonitorAsync(deviceName);
        await manager.IwSetFrequencyAndChannelWidth(deviceName, 5180, ChannelWidth._20MHz);
    }
}