using System;
using System.Runtime.InteropServices;
using System.Threading;
using Ddavisso4.PlexUtilities.Api;
using Ddavisso4.PlexUtilities.Configuration;
using Microsoft.Win32.TaskScheduler;

namespace Ddavisso4.PlexUtilities.PowerManagement
{
    internal class SleepChecker
    {
        private readonly RecordingScheduleApiClient _recordingScheduleApiClient;
        private readonly string _wakeTaskName;
        private readonly int _minutesBeforeRecordingAllowSleep;
        private readonly int _minutesBeforeRecordingToWake;

        public SleepChecker(PlexUtilitiesConfiguration configuration)
        {
            _recordingScheduleApiClient = new RecordingScheduleApiClient(configuration);
            _wakeTaskName = configuration.WakeTaskName;
            _minutesBeforeRecordingAllowSleep = configuration.MinutesBeforeRecordingAllowSleep;
            _minutesBeforeRecordingToWake = configuration.MinutesBeforeRecordingToWake;
        }

        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        internal void CheckIfShouldSleep()
        {
            DateTimeOffset? nextRecordingStartTime = _recordingScheduleApiClient.GetNextRecordingStartTime();

            if (!nextRecordingStartTime.HasValue)
            {
                Console.WriteLine("No recording found.");
                Sleep();
            }
            else if (nextRecordingStartTime.Value.AddMinutes(-_minutesBeforeRecordingAllowSleep) > DateTimeOffset.UtcNow)
            {
                Console.WriteLine($"Next recording start time: {nextRecordingStartTime.Value.LocalDateTime}");

                using (TaskService taskService = new TaskService())
                {
                    Task wakeTask = taskService.FindTask(_wakeTaskName);
                    wakeTask.Definition.Triggers.Clear();
                    wakeTask.Definition.Triggers.Add(new TimeTrigger(nextRecordingStartTime.Value.AddMinutes(-_minutesBeforeRecordingToWake).LocalDateTime));
                    wakeTask.RegisterChanges();
                }

                // Task doesn't seem to get updated if we sleep to quickly.
                Thread.Sleep((int)TimeSpan.FromSeconds(2).TotalMilliseconds);
                Sleep();
            }
        }

        private void Sleep()
        {
            SetSuspendState(false, true, true);
        }
    }
}
