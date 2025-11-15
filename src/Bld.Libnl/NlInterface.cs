using System.Runtime.InteropServices;
using Bld.Libnl.Types;

namespace Bld.Libnl;

public class NlInterface : IDisposable
{
    private readonly NlSock _nlSocket;
    private readonly int _nl80211Id;
    private readonly LibNlNative.NlRecvmsgCallback _interfaceCallback;
    private readonly GCHandle _interfaceCallbackHandle;
    private bool _disposed;

    private NlInterface(NlSock nlSocket, int nl80211Id)
    {
        _nlSocket = nlSocket;
        _nl80211Id = nl80211Id;

        // Keep callback alive for the lifetime of this instance
        _interfaceCallback = HandleNl80211MessageCallback;
        _interfaceCallbackHandle = GCHandle.Alloc(_interfaceCallback);
    }

    public static NlInterface Open()
    {
        var nlSocket = LibNlNative.nl_socket_alloc();
        if (!nlSocket.IsValid)
        {
            throw new Exception("Failed to allocate socket");
        }

        var connectResult = LibNlNative.genl_connect(nlSocket);
        if (connectResult != 0)
        {
            LibNlNative.nl_socket_free(nlSocket);
            throw new Exception($"Failed to genl_connect: {connectResult}");
        }

        var nl80211Id = LibNlNative.genl_ctrl_resolve(nlSocket, "nl80211");
        if (nl80211Id < 0)
        {
            LibNlNative.nl_socket_free(nlSocket);
            throw new Exception($"Failed to resolve nl80211 family: {nl80211Id}");
        }

        return new NlInterface(nlSocket, nl80211Id);
    }

    public List<Dictionary<Nl80211Attribute, INl80211AttributeValue>> DumpWiPhy() => Nl80211Dump(Nl80211Command.NL80211_CMD_GET_WIPHY);
    public List<Dictionary<Nl80211Attribute, INl80211AttributeValue>> DumpInterface() => Nl80211Dump(Nl80211Command.NL80211_CMD_GET_INTERFACE);

    private List<Dictionary<Nl80211Attribute, INl80211AttributeValue>> Nl80211Dump(Nl80211Command command)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var responses = new List<Dictionary<Nl80211Attribute, INl80211AttributeValue>>();

        using var msg = LibNlNative.nlmsg_alloc();
        if (msg.IsInvalid)
        {
            throw new Exception("Failed to allocate netlink message");
        }

        // Build the message: nl80211 GET_INTERFACE command with DUMP flag to get all interfaces
        var hdr = LibNlNative.genlmsg_put(
            msg,
            0, // portid (automatic)
            0, // sequence (automatic)
            _nl80211Id, // nl80211 family id
            0, // header length
            (NetlinkMessageFlags.NLM_F_REQUEST | NetlinkMessageFlags.NLM_F_DUMP),
            (byte)command,
            0 // version
        );

        if (hdr == IntPtr.Zero)
        {
            throw new Exception("Failed to build netlink message");
        }

        var interfacesHandle = GCHandle.Alloc(responses);
        try
        {
            // Set the callback for valid messages
            var cbResult = LibNlNative.nl_socket_modify_cb(
                _nlSocket,
                (int)NetlinkCallbackType.NL_CB_VALID,
                (int)NetlinkCallbackKind.NL_CB_CUSTOM,
                _interfaceCallback,
                GCHandle.ToIntPtr(interfacesHandle)
            );

            if (cbResult != 0)
            {
                throw new Exception($"Failed to set callback: {cbResult}");
            }

            var sendResult = LibNlNative.nl_send_auto(_nlSocket, msg);
            if (sendResult < 0)
            {
                throw new Exception($"Failed to send netlink message: {sendResult}");
            }

            var recvResult = LibNlNative.nl_recvmsgs_default(_nlSocket);
            if (recvResult < 0)
            {
                throw new Exception($"Failed to receive netlink messages: {recvResult}");
            }

            return responses;
        }
        finally
        {
            interfacesHandle.Free();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (_nlSocket.IsValid)
            {
                LibNlNative.nl_socket_free(_nlSocket);
            }

            if (_interfaceCallbackHandle.IsAllocated)
            {
                _interfaceCallbackHandle.Free();
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

    private int HandleNl80211MessageCallback(IntPtr msgPtr, IntPtr arg)
    {
        var interfacesHandle = GCHandle.FromIntPtr(arg);
        var interfaces = (List<Dictionary<Nl80211Attribute, INl80211AttributeValue>>)interfacesHandle.Target!;
        return HandleNl80211Message(msgPtr, interfaces);
    }

    private unsafe int HandleNl80211Message(IntPtr msgPtr, List<Dictionary<Nl80211Attribute, INl80211AttributeValue>> interfaces)
    {
        try
        {
            var nlMessageHeader = LibNlNative.nlmsg_hdr(msgPtr);
            var genNlMessageHeader = LibNlNative.genlmsg_hdr(nlMessageHeader);

            // Parse attributes
            var attrData = LibNlNative.genlmsg_attrdata(genNlMessageHeader, 0);
            var attrLen = LibNlNative.genlmsg_attrlen(genNlMessageHeader, 0);

            int maxAttr = Enum.GetValues<Nl80211Attribute>().Cast<int>().Max();
            var tb = stackalloc IntPtr[maxAttr + 1];

            var parseResult = LibNlNative.nla_parse(tb, maxAttr, attrData, attrLen, IntPtr.Zero);
            if (parseResult != 0)
            {
                return 0; // Skip this message
            }

            var attributes = new Dictionary<Nl80211Attribute, INl80211AttributeValue>();

            for (int i = 0; i <= maxAttr; i++)
            {
                if (tb[i] == IntPtr.Zero)
                {
                    continue;
                }

                var nl80211Attr = (Nl80211Attribute)i;
                var attrValue = Nl80211AttributeParser.ParseAttribute(nl80211Attr, tb[i]);
                if (attrValue != null)
                {
                    attributes[nl80211Attr] = attrValue;
                }
            }

            interfaces.Add(attributes);
            return 0; // NL_OK
        }
        catch
        {
            return 0; // Skip on error, don't break the entire operation
        }
    }
}
