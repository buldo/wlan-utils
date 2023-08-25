# Bld.WlanUtils
This library is just wrapper around console utils.  
Commands are oriented for swithing wlan cards to monitor mode and managing paremeters.  
Command sequences are from OpenHD project.

# Examples
```csharp
var manager = new WlanManager(loggerFactory.CreateLogger<WlanManager>());
var deviceName = "wlx00c0caa98097";
await manager.TrySwitchToMonitorAsync(deviceName);
await manager.IwSetFrequencyAndChannelWidth(deviceName, 5180, ChannelWidth._20MHz);
```

# Future plans
I plan to replace command line calls to OS API calls.