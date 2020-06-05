using System.Collections.Generic;
using System.IO;
using Ddavisso4.PlexUtilities.Api;
using Ddavisso4.PlexUtilities.Configuration;

namespace Ddavisso4.PlexUtilities.Actions.DownloadAlbum
{
    internal class DownloadAlbumHandler : IActionHandler<DownloadAlbumActionOptions>
    {
        private readonly PlaylistsApiClient _playlistsApiClient;
        private readonly FileDownloadApiClient _fileDownloadApiClient;

        public DownloadAlbumHandler(PlexUtilitiesConfiguration configuration)
        {
            _playlistsApiClient = new PlaylistsApiClient(configuration);
            _fileDownloadApiClient = new FileDownloadApiClient(configuration);
        }

        public void Handle(DownloadAlbumActionOptions options)
        {
            int playlistID = _playlistsApiClient.GetPlaylistIDByName(options.AlbumName);
            IEnumerable<PlaylistsApiClient.AlbumFile> albumFiles = _playlistsApiClient.GetDownloadUrlsForAlbumItems(playlistID);

            if (!Directory.Exists(options.DestinationDirectory))
            {
                Directory.CreateDirectory(options.DestinationDirectory);
            }

            foreach (PlaylistsApiClient.AlbumFile albumFile in albumFiles)
            {
                _fileDownloadApiClient.DownloadFile(albumFile.DonwloadUrl, $"{options.DestinationDirectory}{Path.DirectorySeparatorChar}{albumFile.FileName}.{albumFile.FileExtension}");
            }
        }
    }
}
