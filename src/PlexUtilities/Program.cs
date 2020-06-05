using System.Reflection;
using Autofac;
using CommandLine;
using Ddavisso4.PlexUtilities.Actions;
using Ddavisso4.PlexUtilities.Actions.TrySleep;

namespace Ddavisso4.PlexUtilities
{
    class Program
    {        
        static void Main(string[] args)
        {   
            var builder = new ContainerBuilder();
            builder
                .RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.IsClosedTypeOf)

            var container = builder.Build();
            container.Resolve<IActionHandler<TrySleepActionOptions>>();

            var types = LoadVerbs();			

            Parser.Default.ParseArguments(args, types)
                .WithParsed(Run)
                .WithNotParsed(HandleErrors);

            // PlexUtilitiesArgs plexUtilitiesArgs = ArgParser.ParseArgs(args);
            // PlexUtilitiesConfiguration configuration = ConfigurationLoader.LoadConfiguration();

            // if (plexUtilitiesArgs == null)
            // {
            //     return;
            // }

            // switch (plexUtilitiesArgs.PrimaryAction)
            // {
            //     case PrimaryAction.SetupPowerManagement:
            //         new PowerManagementTaskScheduler(configuration)
            //             .SetupPowerManagementTasks();
            //         break;
            //     case PrimaryAction.TrySleep:
            //         new SleepChecker(configuration)
            //             .CheckIfShouldSleep();
            //         break;
            // }
        }

        private static void HandleErrors()
        {
            
        }

        private static void Run()
        {
             
        }
    }
}