﻿using System;
using System.Xml.Linq;
using Ddavisso4.PlexUtilities.Configuration;

namespace Ddavisso4.PlexUtilities.Api
{
    internal class SessionsApiClient : PlexApiClientBase
    {
        protected override string PlexFeatureUrl => "status/sessions";

        internal SessionsApiClient(PowerManagementConfiguration configuration) : base(configuration)
        {
        }

        internal bool AreThereActiveSessions()
        {
            XDocument response = SendRequest();

            int size = Convert.ToInt32(response.Root
                .Attribute("size")
                .Value);

            return size > 0;
        }
    }
}