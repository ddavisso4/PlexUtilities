﻿using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Ddavisso4.PlexUtilities.Configuration
{
    internal static class ConfigurationLoader
    {
        internal static PlexUtilitiesConfiguration LoadConfiguration()
        {
            PlexUtilitiesConfiguration configuration = PlexUtilitiesConfiguration.GetDefault();

            string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string settingsFileLocation = Path.Combine(assemblyLocation, "settings.txt");

            if (File.Exists(settingsFileLocation))
            {
                string settingsString = File.ReadAllText(settingsFileLocation, Encoding.ASCII);
                string[] configurationLines = settingsString.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                bool xPlexTokenFound = false;

                foreach (string configurationLine in configurationLines)
                {
                    string[] keyAndValue = configurationLine.Split(new string[] { " = ", "=", " =", "= " }, StringSplitOptions.None);

                    if (keyAndValue.Length != 2)
                    {
                        throw new Exception($"Unexpected line in settings file {configurationLine}.");
                    }

                    string key = keyAndValue[0];
                    string value = keyAndValue[1];
                    int intValue;

                    int.TryParse(value, out intValue);

                    switch (key)
                    {
                        case "ServerIPAddress":
                            configuration.ServerIPAddress = value;
                            break;
                        case "ServerPort":
                            configuration.ServerPort = CheckInt(intValue);
                            break;
                        case "XPlexToken":
                            configuration.XPlexToken = value;
                            xPlexTokenFound = true;
                            break;
                        case "TaskSchedulerFolderName":
                            configuration.TaskSchedulerFolderName = value;
                            break;
                        case "SleepTaskName":
                            configuration.SleepTaskName = value;
                            break;
                        case "WakeTaskName":
                            configuration.WakeTaskName = value;
                            break;
                        case "MinutesBeforeRecordingAllowSleep":
                            configuration.MinutesBeforeRecordingAllowSleep = CheckInt(intValue);
                            break;
                        case "MinutesBeforeRecordingToWake":
                            configuration.MinutesBeforeRecordingToWake = CheckInt(intValue);
                            break;
                    }
                }

                if (!xPlexTokenFound)
                {
                    throw new Exception("X-Plex-Token is required. You can find this by monitoring network requests from web player.");
                }
            }

            return configuration;
        }

        private static int CheckInt(int intValue)
        {
            return intValue == default(int) ? throw new Exception("ServerPort must be a number.") : intValue;
        }
    }
}