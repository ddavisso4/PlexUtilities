using CommandLine;

namespace Ddavisso4.PlexUtilities.Actions.TrySleep
{
    [Verb(ActionName, HelpText = "Put host in standby but wake for next recording.")]
    public class TrySleepActionOptions : ActionOptionsBase
    {
        internal const string ActionName = "try-sleep";   
    }
}