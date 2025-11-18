using Bld.Libnl.Types;

namespace Bld.Libnl.NlCommands;

internal sealed class SwitchChannelCommand : NlCommandBase
{
    private readonly uint _interfaceIndex;
    private readonly ChannelDefinition _channelDefinition;

    public SwitchChannelCommand(
        uint interfaceIndex,
        ChannelDefinition channelDefinition)
    {
        _interfaceIndex = interfaceIndex;
        _channelDefinition = channelDefinition;
    }

    protected override void BuildMessage(NlMsg msg)
    {
        msg
            .PutAuto(Nl80211Id, NetlinkMessageFlags.NLM_F_REQUEST, Nl80211Command.NL80211_CMD_CHANNEL_SWITCH)
            .PutU32(Nl80211Attribute.NL80211_ATTR_IFINDEX, _interfaceIndex)
            .PutU32(Nl80211Attribute.NL80211_ATTR_WIPHY_FREQ, _channelDefinition.ControlFreq)
            .PutU32(Nl80211Attribute.NL80211_ATTR_WIPHY_FREQ_OFFSET, _channelDefinition.ControlFreqOffset)
            .PutU32(Nl80211Attribute.NL80211_ATTR_CHANNEL_WIDTH, (uint)_channelDefinition.Width);

        // Set channel type based on width
        switch (_channelDefinition.Width)
        {
            case Nl80211ChannelWidth.NL80211_CHAN_WIDTH_20_NOHT:
                msg.PutU32(Nl80211Attribute.NL80211_ATTR_WIPHY_CHANNEL_TYPE, (uint)Nl80211ChannelType.NL80211_CHAN_NO_HT);
                break;
            case Nl80211ChannelWidth.NL80211_CHAN_WIDTH_20:
                msg.PutU32(Nl80211Attribute.NL80211_ATTR_WIPHY_CHANNEL_TYPE, (uint)Nl80211ChannelType.NL80211_CHAN_HT20);
                break;
            case Nl80211ChannelWidth.NL80211_CHAN_WIDTH_40:
                if (_channelDefinition.ControlFreq > _channelDefinition.CenterFreq1)
                {
                    msg.PutU32(Nl80211Attribute.NL80211_ATTR_WIPHY_CHANNEL_TYPE, (uint)Nl80211ChannelType.NL80211_CHAN_HT40MINUS);
                }
                else
                {
                    msg.PutU32(Nl80211Attribute.NL80211_ATTR_WIPHY_CHANNEL_TYPE, (uint)Nl80211ChannelType.NL80211_CHAN_HT40PLUS);
                }
                break;
        }

        // Add optional center frequencies
        if (_channelDefinition.CenterFreq1 != 0)
        {
            msg.PutU32(Nl80211Attribute.NL80211_ATTR_CENTER_FREQ1, _channelDefinition.CenterFreq1);
        }

        if (_channelDefinition.CenterFreq1Offset != 0)
        {
            msg.PutU32(Nl80211Attribute.NL80211_ATTR_CENTER_FREQ1_OFFSET, _channelDefinition.CenterFreq1Offset);
        }

        if (_channelDefinition.CenterFreq2 != 0)
        {
            msg.PutU32(Nl80211Attribute.NL80211_ATTR_CENTER_FREQ2, _channelDefinition.CenterFreq2);
        }

        if (_channelDefinition.Punctured != 0)
        {
            msg.PutU32(Nl80211Attribute.NL80211_ATTR_PUNCT_BITMAP, _channelDefinition.Punctured);
        }
    }

    public void Run()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);

        using var msg = LibNlNative.nlmsg_alloc();
        if (msg.IsInvalid)
        {
            throw new Exception("Failed to allocate netlink message");
        }

        BuildMessage(msg);

        var sendResult = LibNlNative.nl_send_auto_complete(NlSocket, msg);
        if (sendResult < 0)
        {
            throw new Exception($"Failed to send netlink message: {sendResult}");
        }
    }
}