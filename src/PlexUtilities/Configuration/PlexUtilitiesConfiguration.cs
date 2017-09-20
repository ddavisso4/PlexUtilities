namespace Ddavisso4.PlexUtilities.Configuration
{
    internal class PlexUtilitiesConfiguration
    {
        public string ServerIPAddress { get; set; }
        public int ServerPort { get; set; }
        public string XPlexToken { get; set; }
        public string TaskSchedulerFolderName { get; set; }
        public string SleepTaskName { get; set; }
        public string WakeTaskName { get; set; }
        public int MinutesBeforeRecordingAllowSleep { get; set; }
        public int MinutesBeforeRecordingToWake { get; set; }

        internal static PlexUtilitiesConfiguration GetDefault()
        {
            return new PlexUtilitiesConfiguration
            {
                ServerIPAddress = "127.0.0.1",
                ServerPort = 32400,
                TaskSchedulerFolderName = "PlexUtilities",
                SleepTaskName = "Auto-Sleep for Recording",
                WakeTaskName = "Auto-Wake for Recording",
                MinutesBeforeRecordingAllowSleep = 15,
                MinutesBeforeRecordingToWake = 2
            };
        }
    }
}