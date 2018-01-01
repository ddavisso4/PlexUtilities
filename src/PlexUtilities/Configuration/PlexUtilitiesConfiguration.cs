namespace Ddavisso4.PlexUtilities.Configuration
{
    internal class PlexUtilitiesConfiguration
    {
        public ApiConfiguration ApiConfiguration { get; set; }
        public ServerConfiguration ServerConfiguration { get; set; }
        public PowerManagementConfiguration PowerManagementConfiguration { get; set; }

        internal static PlexUtilitiesConfiguration GetDefault()
        {
            return new PlexUtilitiesConfiguration
            {
                ServerConfiguration = ServerConfiguration.GetDefault(),
                PowerManagementConfiguration = PowerManagementConfiguration.GetDefault(),
                ApiConfiguration = new ApiConfiguration()
            };
        }
    }
}