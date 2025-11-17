using Bld.Libnl.Types;

namespace Bld.Libnl;

internal abstract class NlCommandBase : IDisposable
{
    protected readonly NlSock NlSocket;
    protected readonly int Nl80211Id;

    protected bool Disposed;

    protected NlCommandBase()
    {
        NlSocket = LibNlNative.nl_socket_alloc();
        if (!NlSocket.IsValid)
        {
            throw new Exception("Failed to allocate socket");
        }

        var connectResult = LibNlNative.genl_connect(NlSocket);
        if (connectResult != 0)
        {
            LibNlNative.nl_socket_free(NlSocket);
            throw new Exception($"Failed to genl_connect: {connectResult}");
        }

        Nl80211Id = LibNlNative.genl_ctrl_resolve(NlSocket, "nl80211");
        if (Nl80211Id < 0)
        {
            LibNlNative.nl_socket_free(NlSocket);
            throw new Exception($"Failed to resolve nl80211 family: {Nl80211Id}");
        }
    }

    protected abstract void BuildMessage(NlMsg msg);

    protected virtual void Dispose(bool disposing)
    {
        if (!Disposed)
        {
            if (NlSocket.IsValid)
            {
                LibNlNative.nl_socket_free(NlSocket);
            }

            Disposed = true;
        }
    }

    ~NlCommandBase()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
