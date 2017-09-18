using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Win32.TaskScheduler;

namespace Ddavisso4.PlexUtilities.Api
{
    internal class RecordingScheduleClient
    {
        private const string _subscriptionsUrl = "media/subscriptions/scheduled";
        private readonly PlexApiClient _apiClient;

        public RecordingScheduleClient(PlexApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public DateTimeOffset CreateScheduledTaskToWakeComputerAtNextRecordingTime()
        {
            string response = _apiClient.SendRequest(_subscriptionsUrl);
            XDocument xDocument = XDocument.Parse(response);

            IEnumerable<string> beginsAtAttributeValues = xDocument.Root
                .Descendants("Media")
                .Attributes()
                .Where(a => a.Name == "beginsAt")
                .Select(a => a.Value)
                .ToArray();

            DateTimeOffset nextRecordingStartTime = beginsAtAttributeValues
                .Select(a => Convert.ToInt64(a))
                .Select(a => DateTimeOffset.FromUnixTimeSeconds(a))
                .Where(date => date > DateTimeOffset.Now)
                .Min()
                .ToLocalTime();

            CreateOrUpdateScheduledTask(nextRecordingStartTime);

            return nextRecordingStartTime;
        }

        private void CreateOrUpdateScheduledTask(DateTimeOffset nextRecordingStartTime)
        {
            const string WakeTaskName = "Auto-Wake for Recording";
            const string PlexUtilitesTaskFolderName = "PlexUtilities";

            using (TaskService taskService = new TaskService())
            {
                Task wakeTask = taskService.FindTask(WakeTaskName);

                if (wakeTask == null)
                {
                    // Create a new task definition and assign properties
                    TaskDefinition taskDefinition = taskService.NewTask();
                    taskDefinition.RegistrationInfo.Description = "Wakes up the computer so that it can record a show.";
                    taskDefinition.Triggers.Add(new TimeTrigger(nextRecordingStartTime.DateTime));
                    taskDefinition.Actions.Add(new ExecAction("cmd.exe", "/c \"exit\""));
                    taskDefinition.Settings.WakeToRun = true;
                    taskDefinition.Settings.MultipleInstances = TaskInstancesPolicy.IgnoreNew;

                    TaskFolder taskFolder = taskService.RootFolder.SubFolders
                        .Where(f => f.Name == PlexUtilitesTaskFolderName)
                        .SingleOrDefault();

                    if (taskFolder == null)
                    {
                        taskFolder = taskService.RootFolder.CreateFolder(PlexUtilitesTaskFolderName);
                    }

                    taskFolder.RegisterTaskDefinition(taskFolder.Path, taskDefinition);
                }
            }
        }
    }
}