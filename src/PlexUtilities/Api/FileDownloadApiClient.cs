﻿using Ddavisso4.PlexUtilities.Configuration;

namespace Ddavisso4.PlexUtilities.Api
{
    internal class FileDownloadApiClient : PlexApiClientBase
    {
        protected override string PlexFeatureRootUrl => "";

        internal FileDownloadApiClient(PlexUtilitiesConfiguration configuration) : base(configuration)
        {
        }
    }
}