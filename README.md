# Bld.WlanUtils
This library is just wrapper around console utils.  
Commands are oriented for swithing wlan cards to monitor mode and managing paremeters.  
Command sequences are from OpenHD project.

# Examples

## Switch to monitor and channel
```csharp
var manager = new WlanManager(loggerFactory.CreateLogger<WlanManager>());
var deviceName = "wlx00c0caa98097";
await manager.TrySwitchToMonitorAsync(deviceName);
await manager.IwSetFrequencyAndChannelWidth(deviceName, Channels.Ch048, ChannelWidth._20MHz);
```

## Map channel
```csharp
// Use channels list to get one of defined channels
var ch = Channels.Ch048;

// Use channels mapper to get channel by num or freq
var byFreq = ChannelsMapper.GetByFrequency(ch.ChannelFrequencyMHz);
var byNum  = ChannelsMapper.GetByNumber(ch.ChannelNumber);
```

# Future plans
I plan to replace command line calls to OS API calls.