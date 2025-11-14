namespace Bld.Libnl;

public class NlInterface : IDisposable
{
    private readonly IntPtr _nlSocket;
    private readonly int _nl80211Id;
    private bool _disposed;

    private NlInterface(IntPtr nlSocket, int nl80211Id)
    {
        _nlSocket = nlSocket;
        _nl80211Id = nl80211Id;
    }

    public static NlInterface Open()
    {
        var nlSocket = LibNlNative.nl_socket_alloc();
        if (nlSocket == IntPtr.Zero)
        {
            throw new Exception("Failed to allocate socket");
        }

        var connectResult = LibNlNative.genl_connect(nlSocket);
        if (connectResult != 0)
        {
            LibNlNative.nl_socket_free(nlSocket);
            throw new Exception($"Failed to genl_connect: {connectResult}");
        };

        var nl80211Id = LibNlNative.genl_ctrl_resolve(nlSocket, "nl80211");

        return new NlInterface(nlSocket, nl80211Id);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (_nlSocket != IntPtr.Zero)
            {
                LibNlNative.nl_socket_free(_nlSocket);
            }

            _disposed = true;
        }
    }

    ~NlInterface()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
