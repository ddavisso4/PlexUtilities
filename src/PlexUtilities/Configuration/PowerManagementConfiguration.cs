namespace Ddavisso4.PlexUtilities.Configuration
{
    internal class PowerManagementConfiguration
    {
        public string TaskSchedulerFolderName { get; set; }
        public string SleepTaskName { get; set; }
        public string WakeTaskName { get; set; }
        public int MinutesBeforeRecordingAllowSleep { get; set; }
        public int MinutesBeforeRecordingToWake { get; set; }

        internal static PowerManagementConfiguration GetDefault()
        {
            return new PowerManagementConfiguration
            {
                TaskSchedulerFolderName = "PlexUtilities",
                SleepTaskName = "Auto-Sleep for Recording",
                WakeTaskName = "Auto-Wake for Recording",
                MinutesBeforeRecordingAllowSleep = 15,
                MinutesBeforeRecordingToWake = 2
            };
        }
    }
}