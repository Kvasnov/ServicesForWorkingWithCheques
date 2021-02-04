using System;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using ServicesForWorkingWithCheques.ChequeWatcherWindowsService.ChequeServiceReference;
using ServicesForWorkingWithCheques.ChequeWatcherWindowsService.Log4net;
using ServicesForWorkingWithCheques.Domain.Core;

namespace ServicesForWorkingWithCheques.ChequeWatcherWindowsService
{
    internal sealed class ChequeWatcher
    {
        public ChequeWatcher()
        {
            Logger.InitLogger();
            CreateDirectoriesIfNotExists();
            watcher = new FileSystemWatcher( chequePath );
            watcher.Created += Watcher_Created;
        }

        private readonly string chequePath = ConfigurationManager.AppSettings[ "chequePath" ];
        private readonly string completePath = ConfigurationManager.AppSettings[ "completePath" ];
        private readonly string garbagePath = ConfigurationManager.AppSettings[ "garbagPath" ];

        private readonly FileSystemWatcher watcher;

        public void Start()
        {
            Logger.Log.Info( "Запуск ChequeWatcherWindowsService" );
            watcher.EnableRaisingEvents = true;
            watcher.Created += Watcher_Created;
        }

        private void CreateDirectoriesIfNotExists()
        {
            if ( !Directory.Exists( chequePath ) )
            {
                Logger.Log.Info( $"Создание папки {chequePath}" );
                Directory.CreateDirectory( chequePath );
            }

            if ( !Directory.Exists( completePath ) )
            {
                Logger.Log.Info( $"Создание папки {completePath}" );
                Directory.CreateDirectory( completePath );
            }

            if ( !Directory.Exists( garbagePath ) )
            {
                Logger.Log.Info( $"Создание папки {garbagePath}" );
                Directory.CreateDirectory( garbagePath );
            }
        }

        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            watcher.Created -= Watcher_Created;
            Logger.Log.Info( "Остановка ChequeWatcherWindowsService" );
        }

        private void Watcher_Created( object sender, FileSystemEventArgs e )
        {
            WaitToReadFile( e.FullPath );

            if ( Path.GetExtension( e.Name ) == ".txt" )
            {
                try
                {
                    using ( var stream = new StreamReader( e.FullPath ) )
                    {
                        Logger.Log.Info( $"Появление нового файла {e.Name} в папке {e.FullPath}" );
                        var jsonObject = stream.ReadToEnd();
                        Logger.Log.Info( $"Данные в файле {jsonObject}" );
                        var cheque = JsonConvert.DeserializeObject< Cheque >( jsonObject );
                        var client = new ChequeServiceClient( "BasicHttpBinding_IChequeService" );
                        client.SaveCheque( cheque );
                        client.Close();
                    }

                    CreateDirectoriesIfNotExists();
                    Directory.Move( e.FullPath, $"{completePath}\\{e.Name}" );
                    Logger.Log.Info( "Данные успешно отправлены на ChequeService, файл перемещен в папку Complete" );
                }
                catch ( JsonReaderException )
                {
                    CreateDirectoriesIfNotExists();
                    Directory.Move( e.FullPath, $"{garbagePath}\\{e.Name}" );
                    Logger.Log.Error( $"Неверный формат JSON-бъекта в файле {e.Name}, файл перемещен в папку Garbage" );
                }
                catch ( Exception ex )
                {
                    Logger.Log.Error( $"Непредвиденная ошибка {ex.Message}" );
                    Logger.Log.Error( ex.StackTrace );
                }
            }
            else
            {
                CreateDirectoriesIfNotExists();
                Directory.Move( e.FullPath, $"{garbagePath}\\{e.Name}" );
                Logger.Log.Error( $"Неверный формат файла {e.Name}, файл перемещен в папку Garbage" );
            }
        }

        private static void WaitToReadFile( string filename )
        {
            var isFileReady = false;

            while ( !isFileReady )
                try
                {
                    using ( var inputStream = File.Open( filename, FileMode.Open, FileAccess.Read, FileShare.None ) )
                        isFileReady = inputStream.Length >= 0;
                }
                catch ( Exception )
                {
                    isFileReady = false;
                }
        }
    }
}