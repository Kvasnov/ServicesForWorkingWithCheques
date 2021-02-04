using System.ServiceProcess;

namespace ServicesForWorkingWithCheques.ChequeWatcherWindowsService
{
    internal sealed partial class ServiceChequeWatcher : ServiceBase
    {
        internal ServiceChequeWatcher()
        {
            InitializeComponent();
        }

        private ChequeWatcher watcher;

        protected override void OnStart( string[] args )
        {
            watcher = new ChequeWatcher();
            watcher.Start();
        }

        protected override void OnStop()
        {
            watcher.Stop();
        }
    }
}