using Bld.Libnl;
using Bld.Libnl.Types;
using Bld.WlanUtils;
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(
    loggingBuilder => loggingBuilder
        .AddConsole()
        .SetMinimumLevel(LogLevel.Trace));
var logger = loggerFactory.CreateLogger<Program>();
var manager = new WlanManager(loggerFactory.CreateLogger<WlanManager>());
var devices = manager.GetWlanInterfaces();
var selectedDevice = devices
    .Where(d => d.SupportedInterfaceTypes?.Contains(Nl80211InterfaceType.NL80211_IFTYPE_MONITOR) ?? false)
    .First(d => d.DriverName.Contains("8812") || d.DriverName.Contains("rtw88"));
logger.LogInformation(
    """
    Device: {Interface}
        Driver: {Driver}
        Monitor support: {Monitor}
    """,
    selectedDevice.InterfaceName,
    selectedDevice.DriverName,
    selectedDevice.CurrentInterfaceMode
    );

await manager.TrySwitchToMonitorAsync(selectedDevice);
logger.LogInformation("Switched to monitor mode");

// 5660MHz == 132channel
var freq = 5660u;
var mode = ChannelModes.ModeHt20;
manager.SwitchChannel(selectedDevice, freq, mode);
logger.LogInformation("Switched to {Freq}MHz, {ModeName}", freq, mode.name);
