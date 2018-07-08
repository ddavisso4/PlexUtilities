using System;
using System.Collections.Generic;
using System.IO;

namespace Ddavisso4.PlexUtilities.Args
{
    internal class ArgParser
    {
        private readonly string[] _inputArgs;
        private readonly PlexUtilitiesArgs _plexUtilitiesArgs;

        public ArgParser(string[] inputArgs)
        {
            _inputArgs = inputArgs;
            _plexUtilitiesArgs = new PlexUtilitiesArgs();
        }

        internal PlexUtilitiesArgs ParseArgs()
        {
            if (_inputArgs.Length == 0)
            {
                WriteValidPrimaryActions();
                return null;
            }

            switch (_inputArgs[0])
            {
                case "SetupPowerManagement":
                    _plexUtilitiesArgs.PrimaryAction = PrimaryAction.SetupPowerManagement;
                    break;
                case "TrySleep":
                    _plexUtilitiesArgs.PrimaryAction = PrimaryAction.TrySleep;
                    break;
                case "DownloadAlbum":
                    _plexUtilitiesArgs.PrimaryAction = PrimaryAction.DownloadAlbum;
                    ParseDownloadAlbumArgs();
                    break;
                default:
                    WriteValidPrimaryActions();
                    return null;

            }

            return _plexUtilitiesArgs;
        }

        private bool ParseDownloadAlbumArgs()
        {
            _plexUtilitiesArgs.DownloadAlbumArgs = new DownloadAlbumArgs();

            if(_inputArgs.Length < 2)
            {
                Console.WriteLine("Album name required.");
                return false;
            }

            _plexUtilitiesArgs.DownloadAlbumArgs.AlbumName = _inputArgs[1];

            if (_inputArgs.Length > 2)
            {
                try
                {
                    _plexUtilitiesArgs.DownloadAlbumArgs.DestinationDirectory = Path.GetFullPath(_inputArgs[2]);
                    return true;
                }
                catch (Exception)
                {
                    Console.WriteLine($"Invalid destination directory: {_inputArgs[2]}");
                    return false;
                }
            }
            else
            {
                _plexUtilitiesArgs.DownloadAlbumArgs.DestinationDirectory = Environment.CurrentDirectory;
                return true;
            }
        }

        private static void WriteValidPrimaryActions()
        {
            Console.WriteLine("Valid actions are:");

            IEnumerable<string> primaryActions = Enum.GetNames(typeof(PrimaryAction));

            foreach (string primaryAction in primaryActions)
            {
                Console.WriteLine(" - " + primaryAction);
            }
        }
    }
}
