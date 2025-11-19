using Bld.Libnl.Types;

namespace Bld.Libnl.NlCommands;

public abstract class NlCommandNoResultBase : NlCommandBase
{
    protected NlCommandNoResultBase(Nl80211Command command, NetlinkMessageFlags flags, uint? ifIndex = null) : base(command, flags, ifIndex)
    {
    }

    public void Run()
    {
        RunInternal();
    }

    protected override int ProcessValidMessage(IntPtr msgPtr, IntPtr arg)
    {
        return (int)NetlinkCallbackAction.NL_SKIP;
    }
}
