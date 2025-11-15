using Bld.Libnl.NlCommands;
using Bld.Libnl.Types;

namespace Bld.Libnl;

public class NlInterface
{
    public List<Dictionary<Nl80211Attribute, INl80211AttributeValue>> DumpWiPhy()
    {
        using var command = new DumpWiPhyComman();
        var result = command.Run();
        return result;
    }

    public List<Dictionary<Nl80211Attribute, INl80211AttributeValue>> DumpInterface()
    {
        using var command = new DumpInterfaceCommand();
        var result = command.Run();
        return result;
    }

}
