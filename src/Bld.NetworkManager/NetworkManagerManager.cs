using Tmds.DBus.Protocol;
using DbusConnection = Tmds.DBus.Protocol.Connection;

namespace Bld.NetworkManager;

public class NetworkManagerManager
{
    private readonly DbusConnection _connection;
    private readonly NetworkManagerService _networkManagerService;
    private readonly NetworkManager _networkManager;

    private NetworkManagerManager(
        DbusConnection connection,
        NetworkManagerService networkManagerService,
        NetworkManager networkManager)
    {
        _connection = connection;
        _networkManagerService = networkManagerService;
        _networkManager = networkManager;
    }

    public static async Task<NetworkManagerManager> CreateAsync()
    {
        string? systemBusAddress = Address.System;
        if (systemBusAddress is null)
        {
            throw new Exception("Can not determine system bus address");
        }
        var connection = new DbusConnection(Address.System!);
        await connection.ConnectAsync();

        var service = new NetworkManagerService(connection, "org.freedesktop.NetworkManager");
        var networkManager = service.CreateNetworkManager("/org/freedesktop/NetworkManager");
        return new NetworkManagerManager(connection, service, networkManager);
    }
    public async Task<IReadOnlyList<(Device Device, DeviceProperties Properties)>> GetNmWiFiDevicesAsync()
    {
        var devices = await _networkManager.GetDevicesAsync();

        var wlanDevices = new List<(Device Device, DeviceProperties Properties)>();
        foreach (var objectPath in devices)
        {
            var dbusDevice = _networkManagerService.CreateDevice(objectPath);
            var props = await dbusDevice.GetPropertiesAsync();

            if (props.DeviceType == NMDeviceType.NM_DEVICE_TYPE_WIFI)
            {
                wlanDevices.Add((dbusDevice, props));
            }
        }

        return wlanDevices;
    }
}
