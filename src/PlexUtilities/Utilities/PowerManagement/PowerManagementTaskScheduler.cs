using System;
using System.Linq;
using System.Reflection;
using Ddavisso4.PlexUtilities.Args;
using Ddavisso4.PlexUtilities.Configuration;
using Microsoft.Win32.TaskScheduler;

namespace Ddavisso4.PlexUtilities.Utilities.PowerManagement
{
    internal class PowerManagementTaskScheduler
    {
        private readonly string _taskSchedulerFolderName;
        private readonly string _sleepTaskName;
        private readonly string _wakeTaskName;

        internal PowerManagementTaskScheduler(PlexUtilitiesConfiguration configuration)
        {
            _taskSchedulerFolderName = configuration.PowerManagementConfiguration.TaskSchedulerFolderName;
            _sleepTaskName = configuration.PowerManagementConfiguration.SleepTaskName;
            _wakeTaskName = configuration.PowerManagementConfiguration.WakeTaskName;
        }

        internal void SetupPowerManagementTasks()
        {
            using (TaskService taskService = new TaskService())
            {
                CreateSleepScheduledTask(taskService);
                CreateWakeScheduledTask(taskService);
            }
        }

        private void CreateSleepScheduledTask(TaskService taskService)
        {
            Task sleepTask = taskService.FindTask(_sleepTaskName, true);

            if (sleepTask != null)
            {
                sleepTask.Folder.DeleteTask(sleepTask.Name);
            }

            TaskDefinition taskDefinition = taskService.NewTask();

            taskDefinition.RegistrationInfo.Author = "PlexUtilities";
            taskDefinition.RegistrationInfo.Description = "Sleeps the computer if idle and no recording upcoming.";

            taskDefinition.Triggers.Add(new IdleTrigger());

            taskDefinition.Actions.Add(new ExecAction(Assembly.GetExecutingAssembly().Location, PrimaryAction.TrySleep.ToString()));

            taskDefinition.Principal.LogonType = TaskLogonType.S4U;
            taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;

            taskDefinition.Settings.DisallowStartOnRemoteAppSession = false;
            taskDefinition.Settings.IdleSettings.IdleDuration = TimeSpan.FromMinutes(10);
            taskDefinition.Settings.IdleSettings.StopOnIdleEnd = true;
            taskDefinition.Settings.MultipleInstances = TaskInstancesPolicy.IgnoreNew;

            TaskFolder taskFolder = taskService.RootFolder.SubFolders
                .Where(f => f.Name == _taskSchedulerFolderName)
                .SingleOrDefault();

            if (taskFolder == null)
            {
                taskFolder = taskService.RootFolder.CreateFolder(_taskSchedulerFolderName);
            }

            taskFolder.RegisterTaskDefinition(_sleepTaskName, taskDefinition);
        }

        private void CreateWakeScheduledTask(TaskService taskService)
        {
            Task wakeTask = taskService.FindTask(_wakeTaskName);

            if (wakeTask != null)
            {
                wakeTask.Folder.DeleteTask(wakeTask.Name);
            }

            TaskDefinition taskDefinition = taskService.NewTask();

            taskDefinition.RegistrationInfo.Author = "PlexUtilities";
            taskDefinition.RegistrationInfo.Description = "Wakes up the computer so that it can record a show.";

            taskDefinition.Actions.Add(new ExecAction("cmd.exe", "/c \"exit\""));

            taskDefinition.Principal.LogonType = TaskLogonType.S4U;
            taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;

            taskDefinition.Settings.DisallowStartOnRemoteAppSession = false;
            taskDefinition.Settings.MultipleInstances = TaskInstancesPolicy.IgnoreNew;
            taskDefinition.Settings.WakeToRun = true;

            TaskFolder taskFolder = taskService.RootFolder.SubFolders
                .Where(f => f.Name == _taskSchedulerFolderName)
                .SingleOrDefault();

            if (taskFolder == null)
            {
                taskFolder = taskService.RootFolder.CreateFolder(_taskSchedulerFolderName);
            }

            taskFolder.RegisterTaskDefinition(_wakeTaskName, taskDefinition);
        }
    }
}