namespace Ddavisso4.PlexUtilities.Configuration
{
    internal class ServerConfiguration
    {
        public string ServerIPAddress { get; set; }
        public int ServerPort { get; set; }

        internal static ServerConfiguration GetDefault()
        {
            return new ServerConfiguration
            {
                ServerIPAddress = "127.0.0.1",
                ServerPort = 32400
            };
        }
    }
}