using System.Runtime.InteropServices;
using Bld.Libnl.Types;

namespace Bld.Libnl;

internal abstract class NlCommandBaseResult<TReturn> : NlCommandBase
{
    protected NlCommandBaseResult(Nl80211Command command, NetlinkMessageFlags flags, uint? ifIndex = null)
        : base(command, flags, ifIndex)
    {
    }

    public TReturn Run()
    {
        RunInternal();
        return GetResult();
    }

    protected abstract TReturn GetResult();
}
