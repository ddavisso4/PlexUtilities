using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Ddavisso4.PlexUtilities.Configuration;

namespace Ddavisso4.PlexUtilities.Api
{
    internal class RecordingScheduleApiClient : PlexApiClientBase
    {
        internal RecordingScheduleApiClient(PlexUtilitiesConfiguration configuration)
            : base(configuration)
        {
        }

        protected override string PlexFeatureUrl => "media/subscriptions/scheduled";

        internal RecordingScheduleInfo GetNextRecordingStartTime()
        {
            XDocument xDocument = SendRequest();

            if (xDocument == null)
            {
                return null;
            }

            IEnumerable<ScheduleData> schedules = xDocument.Root
                .Descendants("Media")
                .Select(m => new ScheduleData
                {
                    BeginsAt = ReadStringAsUnixTime(m.Attribute("beginsAt").Value),
                    EndsAt = ReadStringAsUnixTime(m.Attribute("endsAt").Value)
                })
                .ToArray();

            // Could do some performance improvement in here but as far as I can tell
            // we are generally dealing with very small data sets (less than 10k).
            if (schedules.Any())
            {
                RecordingScheduleInfo recordingScheduleInfo = new RecordingScheduleInfo();

                recordingScheduleInfo.IsCurrentlyRecording = schedules
                    .Where(s => s.BeginsAt >= DateTimeOffset.UtcNow && s.EndsAt <= DateTimeOffset.UtcNow)
                    .Any();

                var futureRecordingStartDates = schedules
                    .Where(s => s.BeginsAt > DateTimeOffset.UtcNow)
                    .Select(schedule => schedule.BeginsAt)
                    .ToArray();

                if(futureRecordingStartDates.Any())
                {
                    recordingScheduleInfo.NextRecordingStartTime = futureRecordingStartDates.Min();
                }
                else
                {
                    recordingScheduleInfo.NextRecordingStartTime = null;
                }

                return recordingScheduleInfo;
            }

            return null;
        }

        private DateTimeOffset ReadStringAsUnixTime(string value)
        {
            return DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(value));
        }

        internal class RecordingScheduleInfo
        {
            internal DateTimeOffset? NextRecordingStartTime { get; set; }
            internal bool IsCurrentlyRecording { get; set; }
        }

        private class ScheduleData
        {
            public DateTimeOffset BeginsAt { get; set; }
            public DateTimeOffset EndsAt { get; set; }
        }
    }
}