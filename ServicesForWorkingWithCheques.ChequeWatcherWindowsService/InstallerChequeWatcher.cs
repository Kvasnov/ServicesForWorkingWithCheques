using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace ServicesForWorkingWithCheques.ChequeWatcherWindowsService
{
    [RunInstaller( true )]
    public partial class InstallerChequeWatcher : Installer
    {
        public InstallerChequeWatcher()
        {
            InitializeComponent();
            var serviceInstaller = new ServiceInstaller();
            var processInstaller = new ServiceProcessInstaller { Account = ServiceAccount.LocalSystem };
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.ServiceName = "ChequeWatcherService";
            serviceInstaller.Description = "Test task Manzana Group";
            Installers.Add( processInstaller );
            Installers.Add( serviceInstaller );
        }
    }
}