To compile the codebase:

* Debug configuration - execute *DEV-Build-CodeBase-Debug.bat*
* Release configuration - execute *DEV-Build-CodeBase-Release.bat*

To create deploy packages:

* Azure Emulator - execute *msbuild /t:EmulatorFileSystemPublish Deploy-Application.xml*
* Azure Cloud - execute *msbuild /t:AzureFileSystemPublish Deploy-Application.xml*
* IIS deployment packages are created when the codebase is compiled - *Bin\Deploy\WebDeploy*

To deploy the application:

* IIS 7.5 (C:\inetpub\wwwroot\AppKernel) - execute *msbuild /t:WebFileSystemPublish Deploy-Application.xml*
* 