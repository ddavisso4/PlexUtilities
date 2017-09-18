using System;
using Ddavisso4.PlexUtilities.Api;
using Ddavisso4.PlexUtilities.Args;
using Ddavisso4.PlexUtilities.Configuration;

namespace PlexUtilities
{
    class Program
    {
        static void Main(string[] args)
        {
            PlexUtilitiesArgs parsedArgs = ArgParser.ParseArgs(args);
            PlexUtilitiesConfiguration configuration = ConfigurationLoader.LoadConfiguration();

            PlexApiClient apiClient = new PlexApiClient(configuration.ServerIPAddress, configuration.ServerPort, configuration.XPlexToken);

            DateTimeOffset nextRecordingTime = new RecordingScheduleClient(apiClient).CreateScheduledTaskToWakeComputerAtNextRecordingTime();

            Console.WriteLine(nextRecordingTime);
            Console.ReadKey();
        }
    }
}