using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

using Bld.Libnl.Types;

namespace Bld.Libnl;

internal class LinkManager
{
    private NlSock _socket;
    private bool _disposed = false;

    public LinkManager()
    {
        _socket = LibNlNative.nl_socket_alloc();
        if (_socket == IntPtr.Zero)
            throw new Exception("Failed to allocate netlink socket");

        int err = LibNlNative.nl_connect(_socket, NetlinkProtocol.NETLINK_ROUTE);
        if (err < 0)
        {
            LibNlNative.nl_socket_free(_socket);
            throw new Exception($"Failed to connect to NETLINK_ROUTE: {err}");
        }
    }

    public void SetLinkState(int ifindex, bool up)
    {
        IntPtr link = IntPtr.Zero;
        IntPtr change = IntPtr.Zero;

        try
        {
            int err = LibNlRouteNative.rtnl_link_get_kernel(_socket, ifindex, null, out link);
            if (err < 0)
            {
                throw new Exception($"Failed to get link info for ifindex {ifindex}: {err}");
            }

            change = LibNlRouteNative.rtnl_link_alloc();
            if (change == IntPtr.Zero)
            {
                throw new Exception("Failed to allocate change object");
            }

            if (up)
            {
                LibNlRouteNative.rtnl_link_set_flags(change, (uint)NetDeviceFlag.IFF_UP);
            }
            else
            {
                LibNlRouteNative.rtnl_link_unset_flags(change, (uint)NetDeviceFlag.IFF_UP);
            }

            err = LibNlRouteNative.rtnl_link_change(_socket, link, change, 0);
            if (err < 0)
            {
                throw new Exception($"Failed to change link state: {err}");
            }

            Console.WriteLine($"Interface {ifindex} is now {(up ? "UP" : "DOWN")}");
        }
        finally
        {
            if (change != IntPtr.Zero)
            {
                LibNlRouteNative.rtnl_link_put(change);
            }

            if (link != IntPtr.Zero)
            {
                LibNlRouteNative.rtnl_link_put(link);
            }
        }
    }


    /// <summary>
    /// Получить текущие флаги интерфейса
    /// </summary>
    public NetDeviceFlag GetLinkFlags(int ifindex)
    {
        IntPtr link = IntPtr.Zero;

        try
        {
            int err = LibNlRouteNative.rtnl_link_get_kernel(_socket, ifindex, null, out link);
            if (err < 0)
            {
                throw new Exception($"Failed to get link info for ifindex {ifindex}: {err}");
            }

            uint flags = LibNlRouteNative.rtnl_link_get_flags(link);
            return (NetDeviceFlag)flags;
        }
        finally
        {
            if (link != IntPtr.Zero)
            {
                LibNlRouteNative.rtnl_link_put(link);
            }
        }
    }

    /// <summary>
    /// Проверить, что интерфейс в состоянии UP
    /// </summary>
    public bool IsLinkUp(int ifindex)
    {
        var flags = GetLinkFlags(ifindex);
        return flags.HasFlag(NetDeviceFlag.IFF_UP);
    }

    public void Dispose()
    {
        if (!_disposed && _socket != IntPtr.Zero)
        {
            LibNlNative.nl_socket_free(_socket);
            _socket = new NlSock(IntPtr.Zero);
            _disposed = true;
        }
    }

    ~LinkManager()
    {
        Dispose();
    }
}