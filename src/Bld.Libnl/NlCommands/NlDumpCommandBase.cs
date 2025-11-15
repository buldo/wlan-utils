using Bld.Libnl.Types;

namespace Bld.Libnl.NlCommands;

internal abstract class NlDumpCommandBase : NlCommandBase<List<Dictionary<Nl80211Attribute, INl80211AttributeValue>>>
{
    private readonly Nl80211Command _command;
    private readonly List<Dictionary<Nl80211Attribute, INl80211AttributeValue>> _result = new();

    public NlDumpCommandBase(Nl80211Command command) : base()
    {
        _command = command;
    }

    protected override void BuildMessage(NlMsg msg)
    {
        var hdr = LibNlNative.genlmsg_put(
            msg,
            0, // portid (automatic)
            0, // sequence (automatic)
            Nl80211Id, // nl80211 family id
            0, // header length
            (NetlinkMessageFlags.NLM_F_REQUEST | NetlinkMessageFlags.NLM_F_DUMP),
            (byte)_command,
            0 // version
        );

        if (hdr == IntPtr.Zero)
        {
            throw new Exception("Failed to build netlink message");
        }
    }

    protected override List<Dictionary<Nl80211Attribute, INl80211AttributeValue>> GetResult()
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

            _result.Add(attributes);
            return (int)NetlinkCallbackAction.NL_SKIP;
        }
        catch
        {
            return (int)NetlinkCallbackAction.NL_SKIP;
        }
    }
}
