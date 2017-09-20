using System;
using System.Runtime.InteropServices;
using Ddavisso4.PlexUtilities.Api;
using Ddavisso4.PlexUtilities.Configuration;
using Microsoft.Win32.TaskScheduler;

namespace Ddavisso4.PlexUtilities.PowerManagement
{
    internal class SleepChecker
    {
        private readonly RecordingScheduleApiClient _recordingScheduleApiClient;
        private readonly string _wakeTaskName;

        public SleepChecker(PlexUtilitiesConfiguration configuration)
        {
            _recordingScheduleApiClient = new RecordingScheduleApiClient(configuration);
            _wakeTaskName = configuration.WakeTaskName;
        }

        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        internal void CheckIfShouldSleep()
        {
            DateTimeOffset? nextRecordingStartTime = _recordingScheduleApiClient.GetNextRecordingStartTime();

            if (!nextRecordingStartTime.HasValue)
            {
                Sleep();
            }
            else if (nextRecordingStartTime.Value.AddMinutes(-30) > DateTimeOffset.UtcNow)
            {
                using (TaskService taskService = new TaskService())
                {
                    Task wakeTask = taskService.FindTask(_wakeTaskName);
                    wakeTask.Definition.Triggers.Add(new TimeTrigger(nextRecordingStartTime.Value.AddMinutes(-2).LocalDateTime));
                }

                Sleep();
            }
        }

        private void Sleep()
        {
            //SetSuspendState(false, true, true);
        }
    }
}
