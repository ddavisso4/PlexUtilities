using System;
using Ddavisso4.PlexUtilities.Args;
using Ddavisso4.PlexUtilities.Configuration;
using Ddavisso4.PlexUtilities.Utilities;
using Ddavisso4.PlexUtilities.Utilities.PowerManagement;

namespace Ddavisso4.PlexUtilities
{
    internal class Program
    {
        private const string _version = "0.3.0";

        internal static void Main(string[] args)
        {
            PlexUtilitiesArgs plexUtilitiesArgs = new ArgParser(args).ParseArgs();
            PlexUtilitiesConfiguration configuration = ConfigurationLoader.LoadConfiguration();

            if (plexUtilitiesArgs == null)
            {
                Console.WriteLine($"Plex Utilities v{_version}");
            }

            RunPrimaryAction(plexUtilitiesArgs, configuration);
        }

        private static void RunPrimaryAction(PlexUtilitiesArgs plexUtilitiesArgs, PlexUtilitiesConfiguration configuration)
        {
            // TODO: GetConfig, SetConfig
            switch (plexUtilitiesArgs.PrimaryAction)
            {
                case PrimaryAction.SetupPowerManagement:
                    new PowerManagementTaskScheduler(configuration).SetupPowerManagementTasks();
                    break;
                case PrimaryAction.TrySleep:
                    new TrySleeper(configuration).CheckIfShouldSleep();
                    break;
                case PrimaryAction.DownloadAlbum:
                    new AlbumDownloader(configuration, plexUtilitiesArgs.DownloadAlbumArgs).DownloadAlbum("Picture Frame");
                    break;
            }
        }
    }
}