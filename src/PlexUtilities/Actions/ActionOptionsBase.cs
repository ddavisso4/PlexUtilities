using CommandLine;

namespace Ddavisso4.PlexUtilities.Actions
{
    public abstract class ActionOptionsBase
    {
        [Option]
        public string UserName { get; set; }

        [Option]
        public string Password { get; set; }
    }
}