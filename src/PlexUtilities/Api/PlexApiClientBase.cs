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
        protected abstract string PlexFeatureRootUrl { get; }

        internal PlexApiClientBase(PlexUtilitiesConfiguration configuration)
            : this(configuration.ServerConfiguration, configuration.ApiConfiguration)
        {
        }

        private PlexApiClientBase(ServerConfiguration serverConfiguration, ApiConfiguration apiConfiguration)
            : this(serverConfiguration.ServerIPAddress, serverConfiguration.ServerPort, apiConfiguration.XPlexToken)
        {
        }

        private PlexApiClientBase(string serverIpAddress, int serverPort, string xPlexToken)
        {
            _requestUriBuilder = new UriBuilder();
            _requestUriBuilder.Scheme = "http";
            _requestUriBuilder.Host = serverIpAddress;
            _requestUriBuilder.Port = serverPort;

            NameValueCollection queryStringBuilder = HttpUtility.ParseQueryString(string.Empty);
            queryStringBuilder["X-Plex-Token"] = xPlexToken;

            _requestUriBuilder.Query = queryStringBuilder.ToString();
        }

        protected XDocument SendRequest()
        {
            _requestUriBuilder.Path = PlexFeatureRootUrl;
            return PrivateSendRequest();
        }

        protected XDocument SendRequest(string additionalUrl)
        {
            string slash;

            if (!PlexFeatureRootUrl.EndsWith("/") && !additionalUrl.StartsWith("/"))
            {
                slash = "/";
            }
            else
            {
                slash = string.Empty;
            }

            _requestUriBuilder.Path = string.Concat(PlexFeatureRootUrl, slash, additionalUrl);
            return PrivateSendRequest();
        }

        private XDocument PrivateSendRequest()
        {
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