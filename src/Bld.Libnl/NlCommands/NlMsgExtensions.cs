using Bld.Libnl.Types;

namespace Bld.Libnl.NlCommands;

internal static class NlMsgExtensions
{
    public static NestedPart NestStart(
        this NlMsg msg,
        Nl80211Attribute attribute)
    {
        var start = LibNlNative.nla_nest_start(msg, (int)attribute);
        if (start == IntPtr.Zero)
        {
            throw new Exception("Failed to start nested part");
        }
        return new NestedPart { Message = msg, Start = start };
    }

    public static NlMsg NestEnd(this NestedPart nestedPart)
    {
        LibNlNative.nla_nest_end(nestedPart.Message, nestedPart.Start);
        return nestedPart.Message;
    }

    private static NestedPart PutFlag(this NestedPart nestedPart, int flag)
    {
        var ret = LibNlNative.nla_put_flag(nestedPart.Message, flag);
        if (ret != 0)
        {
            throw new Exception($"Failed to add flag {flag}");
        }
        return nestedPart;
    }

    private static NlMsg PutFlag(this NlMsg message, int flag)
    {
        var ret = LibNlNative.nla_put_flag(message, flag);
        if (ret != 0)
        {
            throw new Exception($"Failed to add flag {flag}");
        }
        return message;
    }

    public static NlMsg PutFlag(this NlMsg message, Nl80211Attribute flag)
    {
        return message.PutFlag((int)flag);
    }

    public static NestedPart PutFlag(this NestedPart nestedPart, Nl80211MonitorFlag flag)
    {
        return nestedPart.PutFlag((int)flag);
    }

    public static NestedPart PutFlags(this NestedPart nestedPart, IEnumerable<Nl80211MonitorFlag> flags)
    {
        foreach (var flag in flags)
        {
            nestedPart.PutFlag(flag);
        }

        return nestedPart;
    }

    public static NlMsg PutU32(
        this NlMsg msg,
        Nl80211Attribute attribute,
        uint value)
    {
        var ret = LibNlNative.nla_put_u32(msg, (int)attribute, value);
        if (ret != 0)
        {
            throw new Exception($"Failed to set {attribute} to {value} in message");
        }

        return msg;
    }

    public static NlMsg PutAuto(
        this NlMsg msg,
        int family,
        NetlinkMessageFlags flags,
        Nl80211Command command)
    {
        var hdr = LibNlGenlNative.genlmsg_put(
            msg,
            0, // portid (automatic)
            0, // sequence (automatic)
            family, // nl80211 family id
            0, // header length
            flags,
            (byte)command,
            0 // version
        );

        if (hdr == IntPtr.Zero)
        {
            throw new Exception($"Failed to build netlink message. Family:{family}; Flags:{flags}; Command:{command}");
        }

        return msg;
    }

    public static NlMsg PutAuto(
        this NlMsg msg,
        int family,
        Nl80211Command command)
    {
        var hdr = LibNlGenlNative.genlmsg_put(
            msg,
            0, // portid (automatic)
            0, // sequence (automatic)
            family, // nl80211 family id
            0, // header length
            0,
            (byte)command,
            0 // version
        );

        if (hdr == IntPtr.Zero)
        {
            throw new Exception($"Failed to build netlink message. Family:{family}; Command:{command}");
        }

        return msg;
    }
}

internal class NestedPart
{
    public NlMsg Message { get; set; }

    public IntPtr Start { get; set; }
}
