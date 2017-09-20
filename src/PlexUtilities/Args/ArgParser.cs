using System;
using System.Collections.Generic;

namespace Ddavisso4.PlexUtilities.Args
{
    internal static class ArgParser
    {
        internal static PlexUtilitiesArgs ParseArgs(string[] args)
        {
            if (args.Length == 0)
            {
                WriteValidPrimaryActions();
                return null;
            }

            PlexUtilitiesArgs plexUtilitiesArgs = new PlexUtilitiesArgs();

            switch (args[0])
            {
                case "setup-power-management":
                    plexUtilitiesArgs.PrimaryAction = PrimaryAction.SetupPowerManagement;
                    break;
                case "try-sleep":
                    plexUtilitiesArgs.PrimaryAction = PrimaryAction.TrySleep;
                    break;
                default:
                    WriteValidPrimaryActions();
                    return null;

            }

            return plexUtilitiesArgs;
        }

        private static void WriteValidPrimaryActions()
        {
            Console.WriteLine("Valid primary actions are:");

            IEnumerable<string> primaryActions = Enum.GetNames(typeof(PrimaryAction));

            foreach (string primaryAction in primaryActions)
            {
                Console.WriteLine(" - " + primaryAction);
            }
        }
    }
}
