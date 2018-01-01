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
                    ParseDownloadAlbumDestination();
                    break;
                default:
                    WriteValidPrimaryActions();
                    return null;

            }

            return _plexUtilitiesArgs;
        }

        private bool ParseDownloadAlbumDestination()
        {
            _plexUtilitiesArgs.DownloadAlbumArgs = new DownloadAlbumArgs();

            if (_inputArgs.Length > 1)
            {
                try
                {
                    _plexUtilitiesArgs.DownloadAlbumArgs.DestinationDirectory = Path.GetFullPath(_inputArgs[1]);
                    return true;
                }
                catch (Exception)
                {
                    Console.WriteLine($"Invalid destination directory: {_inputArgs[1]}");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Please specify destination directory as 2nd argument.");
                return false;
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
