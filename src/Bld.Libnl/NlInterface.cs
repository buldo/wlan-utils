using System.Runtime.InteropServices;
using Bld.Libnl.Types;

namespace Bld.Libnl;

public class NlInterface : IDisposable
{
    private readonly NlSock _nlSocket;
    private readonly int _nl80211Id;
    private bool _disposed;

    private NlInterface(NlSock nlSocket, int nl80211Id)
    {
        _nlSocket = nlSocket;
        _nl80211Id = nl80211Id;
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

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (_nlSocket.IsValid)
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

    /// <summary>
    /// Gets list of all WiFi interfaces in the system
    /// </summary>
    /// <returns>List of WiFi interface information</returns>
    public List<WiFiInterfaceInfo> Nl80211GetInterfaces()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var interfaces = new List<WiFiInterfaceInfo>();

        var msg = LibNlNative.nlmsg_alloc();
        if (!msg.IsValid)
        {
            throw new Exception("Failed to allocate netlink message");
        }

        try
        {
            // Build the message: nl80211 GET_INTERFACE command with DUMP flag to get all interfaces
            var hdr = LibNlNative.genlmsg_put(
                msg,
                0, // portid (automatic)
                0, // sequence (automatic)
                _nl80211Id, // nl80211 family id
                0, // header length
                (NetlinkMessageFlags.NLM_F_REQUEST | NetlinkMessageFlags.NLM_F_DUMP),
                (byte)Nl80211Command.NL80211_CMD_GET_INTERFACE,
                0 // version
            );

            if (hdr == IntPtr.Zero)
            {
                throw new Exception("Failed to build netlink message");
            }

            // Setup callback to handle responses
            LibNlNative.NlRecvmsgCallback callback = (msgPtr, arg) =>
            {
                return HandleInterfaceMessage(msgPtr, interfaces);
            };

            // Pin the delegate to prevent GC from collecting it during native calls
            var callbackHandle = GCHandle.Alloc(callback);
            try
            {
                // Set the callback for valid messages
                var cbResult = LibNlNative.nl_socket_modify_cb(
                    _nlSocket,
                    (int)NetlinkCallbackType.NL_CB_VALID,
                    (int)NetlinkCallbackKind.NL_CB_CUSTOM,
                    callback,
                    IntPtr.Zero
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

                return interfaces;
            }
            finally
            {
                callbackHandle.Free();
            }
        }
        finally
        {
            LibNlNative.nlmsg_free(msg);
        }
    }

    private static INl80211AttributeValue? GetStringAttribute(Nl80211Attribute attr, IntPtr nla)
    {
        var strPtr = LibNlNative.nla_get_string(nla);
        if (strPtr != IntPtr.Zero)
        {
            var str = Marshal.PtrToStringUTF8(strPtr);
            if (!string.IsNullOrEmpty(str))
            {
                return Nl80211AttributeValue.FromString(str);
            }
        }
        return null;
    }

    private static INl80211AttributeValue? GetBinaryAttribute(Nl80211Attribute attr, IntPtr nla, int dataLen)
    {
        if (dataLen <= 0)
            return null;

        var dataPtr = LibNlNative.nla_data(nla);
        if (dataPtr != IntPtr.Zero)
        {
            var rawData = new byte[dataLen];
            Marshal.Copy(dataPtr, rawData, 0, dataLen);
            return Nl80211AttributeValue.FromBinary(rawData);
        }
        return null;
    }

    private unsafe int HandleInterfaceMessage(IntPtr msgPtr, List<WiFiInterfaceInfo> interfaces)
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

                // Get the netlink attribute type (NLA_U8, NLA_U32, NLA_STRING, etc.)
                var nlaType = LibNlNative.nla_type(tb[i]);
                var dataLen = LibNlNative.nla_len(tb[i]);

                // Extract value using appropriate nla_get_* method based on type
                INl80211AttributeValue? attrValue = nlaType switch
                {
                    NlAttributeType.NLA_U8 => Nl80211AttributeValue.FromU8(LibNlNative.nla_get_u8(tb[i])),
                    NlAttributeType.NLA_U16 => Nl80211AttributeValue.FromU16(LibNlNative.nla_get_u16(tb[i])),
                    NlAttributeType.NLA_U32 => Nl80211AttributeValue.FromU32(LibNlNative.nla_get_u32(tb[i])),
                    NlAttributeType.NLA_U64 => Nl80211AttributeValue.FromU64(LibNlNative.nla_get_u64(tb[i])),
                    NlAttributeType.NLA_S8 => Nl80211AttributeValue.FromS8(LibNlNative.nla_get_s8(tb[i])),
                    NlAttributeType.NLA_S16 => Nl80211AttributeValue.FromS16(LibNlNative.nla_get_s16(tb[i])),
                    NlAttributeType.NLA_S32 => Nl80211AttributeValue.FromS32(LibNlNative.nla_get_s32(tb[i])),
                    NlAttributeType.NLA_S64 => Nl80211AttributeValue.FromS64(LibNlNative.nla_get_s64(tb[i])),
                    NlAttributeType.NLA_STRING or NlAttributeType.NLA_NUL_STRING => GetStringAttribute(nl80211Attr, tb[i]),
                    NlAttributeType.NLA_MSECS => Nl80211AttributeValue.FromMsecs(LibNlNative.nla_get_msecs(tb[i])),
                    NlAttributeType.NLA_FLAG => Nl80211AttributeValue.FromU8(1),
                    NlAttributeType.NLA_NESTED or NlAttributeType.NLA_NESTED_COMPAT => Nl80211AttributeValue.FromNested(tb[i]),
                    NlAttributeType.NLA_UNSPEC or NlAttributeType.NLA_BINARY => dataLen switch
                    {
                        _ => GetBinaryAttribute(nl80211Attr, tb[i], dataLen)
                    },

                    _ => GetBinaryAttribute(nl80211Attr, tb[i], dataLen)
                };

                if (attrValue != null)
                {
                    attributes[nl80211Attr] = attrValue;
                }
            }

            // Create object with all attributes
            var info = new WiFiInterfaceInfo
            {
                Attributes = new System.Collections.ObjectModel.ReadOnlyDictionary<Nl80211Attribute, INl80211AttributeValue>(attributes)
            };

            interfaces.Add(info);
            return 0; // NL_OK
        }
        catch
        {
            return 0; // Skip on error, don't break the entire operation
        }
    }
}
