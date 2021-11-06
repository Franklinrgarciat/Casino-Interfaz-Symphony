using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Servicio
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
		private ServiceInstaller serviceInstaller;
		private ServiceProcessInstaller processInstaller;
        public Installer()
        {
            InitializeComponent();


			processInstaller = new ServiceProcessInstaller();
			serviceInstaller = new ServiceInstaller();

			//The services will run under the system account.

			processInstaller.Account = ServiceAccount.LocalSystem;

			//The services will be started manually.

			serviceInstaller.StartType = ServiceStartMode.Automatic;

			//ServiceName must equal those on ServiceBase derived classes.

			serviceInstaller.ServiceName = "ServicioSimphonyCasino";
			serviceInstaller.Description = "Seincomp";

			//Add installers to collection. Order is not important.

			Installers.Add(serviceInstaller);

			Installers.Add(processInstaller);
		}
    }
}
