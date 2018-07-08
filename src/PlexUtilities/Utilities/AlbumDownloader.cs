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
        private readonly FileDownloadApiClient _fileDownloadApiClient;
        private readonly DownloadAlbumArgs _args;

        public AlbumDownloader(PlexUtilitiesConfiguration configuration, DownloadAlbumArgs args)
        {
            _playlistsApiClient = new PlaylistsApiClient(configuration);
            _fileDownloadApiClient = new FileDownloadApiClient(configuration);
            _args = args;
        }

        public int DownloadAlbum()
        {
            int playlistID = _playlistsApiClient.GetPlaylistIDByName(_args.AlbumName);
            IEnumerable<PlaylistsApiClient.AlbumFile> albumFiles = _playlistsApiClient.GetDownloadUrlsForAlbumItems(playlistID);

            if (!Directory.Exists(_args.DestinationDirectory))
            {
                Directory.CreateDirectory(_args.DestinationDirectory);
            }

            foreach (PlaylistsApiClient.AlbumFile albumFile in albumFiles)
            {
                _fileDownloadApiClient.DownloadFile(albumFile.DonwloadUrl, $"{_args.DestinationDirectory}{Path.DirectorySeparatorChar}{albumFile.FileName}.{albumFile.FileExtension}");
            }

            return playlistID;
        }
    }
}
