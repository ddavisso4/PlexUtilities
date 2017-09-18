using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;

namespace Ddavisso4.PlexUtilities.Api
{
    internal class PlexApiClient
    {
        private readonly UriBuilder _requestUriBuilder;

        internal PlexApiClient(string serverIpAddress, int serverPort, string xPlexToken)
        {
            _requestUriBuilder = new UriBuilder();
            _requestUriBuilder.Scheme = "http";
            _requestUriBuilder.Host = serverIpAddress;
            _requestUriBuilder.Port = serverPort;

            NameValueCollection queryStringBuilder = HttpUtility.ParseQueryString(string.Empty);
            queryStringBuilder["X-Plex-Token"] = xPlexToken;

            _requestUriBuilder.Query = queryStringBuilder.ToString();
        }

        internal string SendRequest(string plexUrl)
        {
            _requestUriBuilder.Path = plexUrl;

            HttpWebRequest request = HttpWebRequest.CreateHttp(_requestUriBuilder.Uri);
            WebResponse response = request.GetResponse();

            Stream responseStream = response.GetResponseStream();

            if (responseStream.CanRead)
            {
                using (StreamReader streamReader = new StreamReader(responseStream))
                {
                    return streamReader.ReadToEnd();
                }
            }

            return null;
        }
    }
}
