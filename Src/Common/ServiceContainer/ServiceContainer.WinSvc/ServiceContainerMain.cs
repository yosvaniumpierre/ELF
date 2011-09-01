using System.ServiceProcess;

namespace Avanade.ServiceContainer.WinSvc
{
    static class ServiceContainerMain
    {
        #region Methods

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] servicesToRun = new ServiceBase[]
                                              {
                                                  new ContainerService()
                                              };
            ServiceBase.Run(servicesToRun);
        }

        #endregion Methods
    }
}