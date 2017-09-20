using System;
using Ddavisso4.PlexUtilities.Api;
using Ddavisso4.PlexUtilities.Args;
using Ddavisso4.PlexUtilities.Configuration;
using Ddavisso4.PlexUtilities.PowerManagement;

namespace Ddavisso4.PlexUtilities
{
    class Program
    {
        static void Main(string[] args)
        {
            PlexUtilitiesArgs plexUtilitiesArgs = ArgParser.ParseArgs(args);
            PlexUtilitiesConfiguration configuration = ConfigurationLoader.LoadConfiguration();

            if (plexUtilitiesArgs == null)
            {
                return;
            }

            switch (plexUtilitiesArgs.PrimaryAction)
            {
                case PrimaryAction.SetupPowerManagement:
                    new PowerManagementTaskScheduler(configuration)
                        .SetupPowerManagementTasks();
                    break;
                case PrimaryAction.TrySleep:
                    new SleepChecker(configuration)
                        .CheckIfShouldSleep();
                    break;
            }

            Console.ReadKey();
        }
    }
}