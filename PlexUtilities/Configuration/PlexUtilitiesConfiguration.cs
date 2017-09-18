namespace Ddavisso4.PlexUtilities.Configuration
{
    internal class PlexUtilitiesConfiguration
    {
        public string ServerIPAddress { get; set; }
        public int ServerPort { get; set; }
        public string XPlexToken { get; set; }

        internal static PlexUtilitiesConfiguration GetDefault()
        {
            return new PlexUtilitiesConfiguration
            {
                ServerIPAddress = "127.0.0.1",
                ServerPort = 32400
            };
        }
    }
}