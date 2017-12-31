using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Ddavisso4.PlexUtilities.Api;
using Ddavisso4.PlexUtilities.Configuration;
using Microsoft.Win32.TaskScheduler;

namespace Ddavisso4.PlexUtilities.PowerManagement
{
    internal class TrySleeper
    {
        private readonly RecordingScheduleApiClient _recordingScheduleApiClient;
        private readonly BackgroundSessionsApiClient _backgroundSessionsApiClient;
        private readonly SessionsApiClient _sessionsApiClient;
        private readonly string _wakeTaskName;
        private readonly int _minutesBeforeRecordingAllowSleep;
        private readonly int _minutesBeforeRecordingToWake;

        public TrySleeper(PowerManagementConfiguration configuration)
        {
            _recordingScheduleApiClient = new RecordingScheduleApiClient(configuration);
            _backgroundSessionsApiClient = new BackgroundSessionsApiClient(configuration);
            _sessionsApiClient = new SessionsApiClient(configuration);

            _wakeTaskName = configuration.WakeTaskName;
            _minutesBeforeRecordingAllowSleep = configuration.MinutesBeforeRecordingAllowSleep;
            _minutesBeforeRecordingToWake = configuration.MinutesBeforeRecordingToWake;
        }

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
            using (Process psshutdown = Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\External\\psshutdown.exe", "-d -t 7"))
            {
                psshutdown.WaitForExit((int)TimeSpan.FromSeconds(15).TotalMilliseconds);
            }
        }
    }
}
