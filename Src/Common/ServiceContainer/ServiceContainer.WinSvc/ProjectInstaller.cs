using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Avanade.ServiceContainer.WinSvc
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        #region Constructors

        public ProjectInstaller()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            ServiceController controller = new ServiceController(serviceContainerInstaller.ServiceName);
            controller.Start();
        }

        private void serviceContainerProcessInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
        }

        #endregion Methods
    }
}