using System.Collections.Generic;
using System.IO;
using Ddavisso4.PlexUtilities.Api;
using Ddavisso4.PlexUtilities.Args;
using Ddavisso4.PlexUtilities.Configuration;

namespace Ddavisso4.PlexUtilities.Utilities
{
    internal class AlbumDownloader
    {
        private readonly PlaylistsApiClient _playlistsApiClient;
        private readonly DownloadAlbumArgs _args;

        public AlbumDownloader(PlexUtilitiesConfiguration configuration, DownloadAlbumArgs args)
        {
            _playlistsApiClient = new PlaylistsApiClient(configuration);
            _args = args;
        }

        public int DownloadAlbum(string albumName)
        {
            int playlistID = _playlistsApiClient.GetPlaylistIDByName(albumName);
            IEnumerable<string> filePaths = _playlistsApiClient.GetFilePathsForAlbumItems(playlistID);

            // Better: https://<server>/library/parts/31699/1514218084/file.JPG?download=1&X-Plex-Token=eNmfgwb5FKSczVLXpF7i
            foreach (string filePath in filePaths)
            {
                File.Copy(filePath, _args.DestinationDirectory);
            }

            return playlistID;
        }
    }
}
