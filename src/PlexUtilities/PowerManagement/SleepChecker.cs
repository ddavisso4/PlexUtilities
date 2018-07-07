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
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        internal void CheckIfShouldSleep()
        {
            RecordingScheduleApiClient.RecordingScheduleInfo scheduleInfo = _recordingScheduleApiClient.GetNextRecordingStartTime();

            if (scheduleInfo.IsCurrentlyRecording)
            {
                Console.WriteLine("Currently recording.");
                return;
            }

            if (!scheduleInfo.NextRecordingStartTime.HasValue)
            {
                Console.WriteLine("No recording found.");
                Sleep();
            }
            else if (scheduleInfo.NextRecordingStartTime.Value.AddMinutes(-_minutesBeforeRecordingAllowSleep) > DateTimeOffset.UtcNow)
            {
                Console.WriteLine($"Next recording start time: {scheduleInfo.NextRecordingStartTime.Value.LocalDateTime}");

                using (TaskService taskService = new TaskService())
                {
                    Task wakeTask = taskService.FindTask(_wakeTaskName);
                    wakeTask.Definition.Triggers.Clear();
                    wakeTask.Definition.Triggers.Add(new TimeTrigger(scheduleInfo.NextRecordingStartTime.Value.AddMinutes(-_minutesBeforeRecordingToWake).LocalDateTime));
                    wakeTask.RegisterChanges();
                }

                Sleep();
            }
        }

        private void Sleep()
        {
            Console.WriteLine("Sleeping in 3 seconds...");
            Thread.Sleep((int)TimeSpan.FromSeconds(3).TotalMilliseconds);
            SetSuspendState(false, true, false);
        }
    }
}
