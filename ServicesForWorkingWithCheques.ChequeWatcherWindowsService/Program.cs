using System.ServiceProcess;

namespace ServicesForWorkingWithCheques.ChequeWatcherWindowsService
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            var servicesToRun = new ServiceBase[] { new ServiceChequeWatcher() };
            ServiceBase.Run( servicesToRun );
        }
    }
}