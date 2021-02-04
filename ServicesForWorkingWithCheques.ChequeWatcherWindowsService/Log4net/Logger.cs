using log4net;
using log4net.Config;

namespace ServicesForWorkingWithCheques.ChequeWatcherWindowsService.Log4net
{
    internal static class Logger
    {
        internal static ILog Log { get; } = LogManager.GetLogger( "LOGGER" );

        internal static void InitLogger()
        {
            XmlConfigurator.Configure();
        }
    }
}