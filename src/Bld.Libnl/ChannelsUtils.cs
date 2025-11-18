using Bld.Libnl.Types;

namespace Bld.Libnl;

public static class ChannelsUtils
{
    public static uint ChannelToFrequency(uint chan, Nl80211Band band)
    {
        /* see 802.11 17.3.8.3.2 and Annex J
         * there are overlapping channel numbers in 5GHz and 2GHz bands */
        if (chan <= 0)
        {
            return 0; /* not supported */
        }

        switch (band) {
            case Nl80211Band.NL80211_BAND_2GHZ:
                if (chan == 14)
                {
                    return 2484;
                }
                else if (chan < 14)
                {
                    return 2407 + chan* 5;
                }
                break;
            case Nl80211Band.NL80211_BAND_5GHZ:
                if (chan >= 182 && chan <= 196)
                {
                    return 4000 + chan* 5;
                }
                else
                {
                    return 5000 + chan* 5;
                }
                break;
            case Nl80211Band.NL80211_BAND_6GHZ:
                /* see 802.11ax D6.1 27.3.23.2 */
                if (chan == 2)
                {
                    return 5935;
                }
                if (chan <= 253)
                {
                    return 5950 + chan* 5;
                }
                break;
            case Nl80211Band.NL80211_BAND_60GHZ:
                if (chan < 7)
                {
                    return 56160 + chan* 2160;
                }
                break;
            default:
                break;
        }

        return 0; /* not supported */
    }

    public static uint GetCf1(ChannelMode chanmode, uint freq)
    {

        uint cf1 = freq, j;
        uint[] bw80 =
        {
            5180, 5260, 5500, 5580, 5660, 5745,
            5955, 6035, 6115, 6195, 6275, 6355,
            6435, 6515, 6595, 6675, 6755, 6835,
            6195, 6995
        };
        uint[] bw160 =
        {
            5180, 5500, 5955, 6115, 6275, 6435,
            6595, 6755, 6915
        };
        /* based on 11be D2 E.1 Country information and operating classes */
        uint[] bw320 = { 5955, 6115, 6275, 6435, 6595, 6755 };

        switch (chanmode.width)
        {
            case Nl80211ChannelWidth.NL80211_CHAN_WIDTH_80:
                /* setup center_freq1 */
                for (j = 0; j < bw80.Length; j++)
                {
                    if (freq >= bw80[j] && freq < bw80[j] + 80)
                        break;
                }

                if (j == bw80.Length)
                    break;

                cf1 = bw80[j] + 30;
                break;
            case Nl80211ChannelWidth.NL80211_CHAN_WIDTH_160:
                /* setup center_freq1 */
                for (j = 0; j < bw160.Length; j++)
                {
                    if (freq >= bw160[j] && freq < bw160[j] + 160)
                        break;
                }

                if (j == bw160.Length)
                    break;

                cf1 = bw160[j] + 70;
                break;

            case Nl80211ChannelWidth.NL80211_CHAN_WIDTH_320:
                /* setup center_freq1 */
                for (j = 0; j < bw320.Length; j++)
                {
                    if (freq >= bw320[j] && freq < bw320[j] + 160)
                        break;
                }

                if (j == bw320.Length)
                    break;

                cf1 = bw320[j] + 150;
                break;
            default:
                cf1 = (uint)(freq + chanmode.freq1_diff);
                break;
        }

        return cf1;
    }
}