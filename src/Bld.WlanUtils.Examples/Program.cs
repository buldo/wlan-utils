using Microsoft.Extensions.Logging;

namespace Bld.WlanUtils.Examples;

internal class Program
{
    static async Task Main(string[] args)
    {
        var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.AddConsole());
        var manager = new WlanManager(loggerFactory.CreateLogger<WlanManager>());
        await manager.TrySwitchToMonitorAsync("wlx00c0caa98097");
    }
}