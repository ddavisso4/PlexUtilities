using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Ddavisso4.PlexUtilities.Configuration;

namespace Ddavisso4.PlexUtilities.Api
{
    internal class PlaylistsApiClient : PlexApiClientBase
    {
        internal PlaylistsApiClient(PlexUtilitiesConfiguration configuration) : base(configuration)
        {
        }

        protected override string PlexFeatureRootUrl => "playlists";

        public int GetPlaylistIDByName(string playlistName)
        {
            XDocument xDocument = SendRequest();

            return xDocument.Root
                .Elements("Playlist")
                .Where(x => x.Attribute("title").Value == playlistName)
                //.Where(x => x.Attribute("playlistType").Value == playlistType.ToString())
                .Select(x => Convert.ToInt32(x.Attribute("ratingKey").Value))
                .SingleOrDefault();
        }

        public IEnumerable<AlbumFile> GetDownloadUrlsForAlbumItems(int playlistID)
        {
            XDocument xDocument = SendRequest($"{playlistID}/items");

            return xDocument
                .Descendants("Photo")
                .Select(x => new
                {
                    Photo = x,
                    Part = x.Element("Media").Element("Part")
                })
                .Select(x => new AlbumFile
                {
                    DonwloadUrl = x.Part.Attribute("key").Value,
                    FileName = x.Photo.Attribute("title").Value,
                    FileExtension = x.Part.Attribute("container").Value
                })
                .ToArray();
        }

        public class AlbumFile
        {
            public string DonwloadUrl { get; set; }
            public string FileName { get; internal set; }
            public string FileExtension { get; set; }
        }

        public sealed class PlaylistType
        {
            private readonly string _name;
            private readonly int _value;

            public static readonly PlaylistType Photo = new PlaylistType(1, "photo");

            private PlaylistType(int value, string name)
            {
                _name = name;
                _value = value;
            }

            public override string ToString()
            {
                return _name;
            }
        }
    }
}