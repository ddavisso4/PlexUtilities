using System;
using Ddavisso4.PlexUtilities.Api;
using Ddavisso4.PlexUtilities.Args;
using Ddavisso4.PlexUtilities.Configuration;
using Ddavisso4.PlexUtilities.PowerManagement;

namespace Ddavisso4.PlexUtilities
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            PlexUtilitiesArgs plexUtilitiesArgs = ArgParser.ParseArgs(args);
            PowerManagementConfiguration configuration = ConfigurationLoader.LoadConfiguration();

            if (plexUtilitiesArgs == null)
            {
                Console.WriteLine("Plex Utilities v0");
            }

            RunPrimaryAction(plexUtilitiesArgs, configuration);
        }

        private static void RunPrimaryAction(PlexUtilitiesArgs plexUtilitiesArgs, PowerManagementConfiguration configuration)
        {
            // TODO: GetConfig, SetConfig
            switch (plexUtilitiesArgs.PrimaryAction)
            {
                case PrimaryAction.SetupPowerManagement:
                    new PowerManagementTaskScheduler(configuration)
                        .SetupPowerManagementTasks();
                    break;
                case PrimaryAction.TrySleep:
                    new TrySleeper(configuration)
                        .CheckIfShouldSleep();
                    break;
                case PrimaryAction.DownloadAlbum:

                    break;
            }
        }
    }
}