namespace Avanade.ServiceContainer.WinSvc
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceContainerProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceContainerInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceContainerProcessInstaller
            // 
            this.serviceContainerProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceContainerProcessInstaller.Password = null;
            this.serviceContainerProcessInstaller.Username = null;
            this.serviceContainerProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceContainerProcessInstaller_AfterInstall);
            // 
            // serviceContainerInstaller
            // 
            this.serviceContainerInstaller.Description = "CrimsonLogic generic container for hosting pluggable services";
            this.serviceContainerInstaller.DisplayName = "Pluggable Service Container";
            this.serviceContainerInstaller.ServiceName = "ServiceContainer";
            this.serviceContainerInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceContainerProcessInstaller,
            this.serviceContainerInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceContainerProcessInstaller;
        private System.ServiceProcess.ServiceInstaller serviceContainerInstaller;
    }
}