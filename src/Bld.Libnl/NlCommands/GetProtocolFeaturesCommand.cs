using Bld.Libnl.Types;

namespace Bld.Libnl.NlCommands;

internal class GetProtocolFeaturesCommand : NlCommandBaseResult<HashSet<Nl80211ProtocolFeature>>
{
    private HashSet<Nl80211ProtocolFeature> _result = new();

    protected override void BuildMessage(NlMsg msg)
    {
        var hdr = LibNlNative.genlmsg_put(
            msg,
            0, // portid (automatic)
            0, // sequence (automatic)
            Nl80211Id, // nl80211 family id
            0, // header length
            NetlinkMessageFlags.NLM_F_REQUEST,
            (byte)Nl80211Command.NL80211_CMD_GET_PROTOCOL_FEATURES,
            0 // version
        );

        if (hdr == IntPtr.Zero)
        {
            throw new Exception("Failed to build netlink message");
        }
    }

    protected override HashSet<Nl80211ProtocolFeature> GetResult()
    {
        return _result;
    }

    protected override unsafe int ProcessMessage(IntPtr msgPtr, IntPtr arg)
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
                return (int)NetlinkCallbackAction.NL_SKIP;
            }

            // Get PROTOCOL_FEATURES attribute
            var protocolFeaturesAttr = tb[(int)Nl80211Attribute.NL80211_ATTR_PROTOCOL_FEATURES];
            if (protocolFeaturesAttr != IntPtr.Zero)
            {
                var featuresValue = LibNlNative.nla_get_u32(protocolFeaturesAttr);

                // Parse flags from U32 value
                foreach (Nl80211ProtocolFeature feature in Enum.GetValues(typeof(Nl80211ProtocolFeature)))
                {
                    if ((featuresValue & (uint)feature) != 0)
                    {
                        _result.Add(feature);
                    }
                }
            }

            return (int)NetlinkCallbackAction.NL_SKIP;
        }
        catch
        {
            return (int)NetlinkCallbackAction.NL_SKIP;
        }
    }
}
