using Avanade.ServiceContainer.Impl;
using System.ServiceProcess;

namespace Avanade.ServiceContainer.WinSvc
{
    public partial class ContainerService : ServiceBase
    {
        #region Fields

        private readonly IServiceContainer serviceContainer;

        #endregion Fields

        #region Constructors

        public ContainerService()
        {
            InitializeComponent();
            serviceContainer = new DefaultServiceContainerImpl();
        }

        #endregion Constructors

        #region Methods

        protected override void OnStart(string[] args)
        {
            serviceContainer.Start();
        }

        protected override void OnStop()
        {
            serviceContainer.Stop();
        }

        #endregion Methods
    }
}