using Bld.Libnl.Types;

namespace Bld.Libnl.NlCommands;

/// <summary>
/// Set the channel (using %NL80211_ATTR_WIPHY_FREQ and the attributes determining channel width) the given interface (identified by %NL80211_ATTR_IFINDEX) shall operate on.
/// In case multiple channels are supported by the device, the mechanism with which it switches channels is implementation-defined.
/// When a monitor interface is given, it can only switch channel while no other interfaces are operating to avoid disturbing the operation of any other interfaces, and other interfaces will again take precedence when they are used.
/// </summary>
internal sealed class SetChannelCommand : NlCommandNoResultBase
{
    private readonly ChannelDefinition _channelDefinition;

    public SetChannelCommand(uint interfaceIndex, ChannelDefinition channelDefinition)
        : base(Nl80211Command.NL80211_CMD_SET_CHANNEL, new NetlinkMessageFlags(), interfaceIndex)
    {
        _channelDefinition = channelDefinition;
    }

    protected override void BuildMessage(NlMsg msg)
    {
        msg
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
}
