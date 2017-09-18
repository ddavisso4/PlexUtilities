using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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

            return nextRecordingStartTime;
        }
    }
}