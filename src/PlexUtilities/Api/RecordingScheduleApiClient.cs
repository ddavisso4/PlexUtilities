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

        internal DateTimeOffset? GetNextRecordingStartTime()
        {
            XDocument xDocument = SendRequest();

            if (xDocument == null)
            {
                return null;
            }

            IEnumerable<string> beginsAtAttributeValues = xDocument.Root
                .Descendants("Media")
                .Attributes()
                .Where(a => a.Name == "beginsAt")
                .Select(a => a.Value)
                .ToArray();

            if (beginsAtAttributeValues.Any())
            {
                return beginsAtAttributeValues
                    .Select(a => Convert.ToInt64(a))
                    .Select(a => DateTimeOffset.FromUnixTimeSeconds(a))
                    .Where(date => date > DateTimeOffset.UtcNow)
                    .Min();
            }

            return null;
        }
    }
}