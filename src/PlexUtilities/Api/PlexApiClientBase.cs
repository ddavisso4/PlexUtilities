using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using System.Xml.Linq;
using Ddavisso4.PlexUtilities.Configuration;

namespace Ddavisso4.PlexUtilities.Api
{
    internal abstract class PlexApiClientBase
    {
        private readonly UriBuilder _requestUriBuilder;

        protected abstract string PlexFeatureUrl { get; }

        internal PlexApiClientBase(PowerManagementConfiguration configuration)
            : this(configuration.ServerIPAddress, configuration.ServerPort, configuration.XPlexToken)
        {
        }

        private PlexApiClientBase(string serverIpAddress, int serverPort, string xPlexToken)
        {
            UriBuilder _requestUriBuilder = new UriBuilder();
            _requestUriBuilder.Scheme = "http";
            _requestUriBuilder.Host = serverIpAddress;
            _requestUriBuilder.Port = serverPort;
            _requestUriBuilder.Path = PlexFeatureUrl;

            NameValueCollection queryStringBuilder = HttpUtility.ParseQueryString(string.Empty);
            queryStringBuilder["X-Plex-Token"] = xPlexToken;

            _requestUriBuilder.Query = queryStringBuilder.ToString();
        }

        protected XDocument SendRequest()
        {
            return SendRequest(PlexFeatureUrl);
        }

        protected XDocument SendRequest(string url)
        {
            _requestUriBuilder.Path = url;

            HttpWebRequest request = HttpWebRequest.CreateHttp(_requestUriBuilder.Uri);
            request.Timeout = (int)TimeSpan.FromSeconds(3).TotalMilliseconds;

            try
            {
                WebResponse response = request.GetResponse();
                Stream responseStream = response.GetResponseStream();

                if (responseStream.CanRead)
                {
                    using (StreamReader streamReader = new StreamReader(responseStream))
                    {
                        string responseString = streamReader.ReadToEnd();
                        XDocument xDocument = XDocument.Parse(responseString);
                        return xDocument;
                    }
                }
            }
            catch (WebException)
            {
                Console.WriteLine("Error connecting to Plex server.");
            }

            return null;
        }
    }
}