using System;
using System.Xml.Linq;
using Ddavisso4.PlexUtilities.Configuration;

namespace Ddavisso4.PlexUtilities.Api
{
    internal class BackgroundSessionsApiClient : PlexApiClientBase
    {
        protected override string PlexFeatureRootUrl => "status/sessions/background";

        internal BackgroundSessionsApiClient(PlexUtilitiesConfiguration configuration) : base(configuration)
        {
        }

        internal bool AreThereActiveBackgroundSessions()
        {
            XDocument response = SendRequest();

            int size = Convert.ToInt32(response.Root
                .Attribute("size")
                .Value);

            return size > 0;
        }
    }
}