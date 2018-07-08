using System;
using System.Runtime.InteropServices;
using System.Threading;
using Ddavisso4.PlexUtilities.Api;
using Ddavisso4.PlexUtilities.Configuration;
using Microsoft.Win32.TaskScheduler;

namespace Ddavisso4.PlexUtilities.Utilities.PowerManagement
{
    internal class TrySleeper
    {
        private readonly RecordingScheduleApiClient _recordingScheduleApiClient;
        private readonly BackgroundSessionsApiClient _backgroundSessionsApiClient;
        private readonly SessionsApiClient _sessionsApiClient;
        private readonly string _wakeTaskName;
        private readonly int _minutesBeforeRecordingAllowSleep;
        private readonly int _minutesBeforeRecordingToWake;

        public TrySleeper(PlexUtilitiesConfiguration configuration)
        {
            _recordingScheduleApiClient = new RecordingScheduleApiClient(configuration);
            _backgroundSessionsApiClient = new BackgroundSessionsApiClient(configuration);
            _sessionsApiClient = new SessionsApiClient(configuration);

            _wakeTaskName = configuration.PowerManagementConfiguration.WakeTaskName;
            _minutesBeforeRecordingAllowSleep = configuration.PowerManagementConfiguration.MinutesBeforeRecordingAllowSleep;
            _minutesBeforeRecordingToWake = configuration.PowerManagementConfiguration.MinutesBeforeRecordingToWake;
        }

        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        internal void CheckIfShouldSleep()
        {
            RecordingScheduleApiClient.RecordingScheduleInfo scheduleInfo = _recordingScheduleApiClient.GetNextRecordingStartTime();
            bool areThereActiveBackgroundSessions = _backgroundSessionsApiClient.AreThereActiveBackgroundSessions();
            bool areThereActiveSessions = _sessionsApiClient.AreThereActiveSessions();

            if (scheduleInfo.IsCurrentlyRecording || areThereActiveBackgroundSessions || areThereActiveSessions)
            {
                Console.WriteLine("Currently doing something.");
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
