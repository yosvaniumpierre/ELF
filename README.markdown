Introduction
================

A framework with the intent of providing an abstraction over IIS 7.5 and Azure. In other words, components built for this framework can be deployed to either 
environment with **zero** change to the code or configuration.  

Environment
================

1. .NET Framework 4
2. ASP.NET MVC 3
3. Visual Studio 2010
4. Windows Azure SDK 1.4
5. PowerShell

Visual Studio Solution Configuration
================

There are 4 kinds of Configuration - 2 standard and 2 additional ones:

1. Debug - targeted for IIS 7.5 and assemblies are built using the Debug configuration
2. Release - targeted for IIS 7.5 and assemblies are built using the Debug configuration
3. Emulator - targeted for the Azure Emulator and will use the Debug-configured assemblies
4. Azure - targeted for the Azure Cloud and will use the Release-configured assemblies

Continuous Integration
================

To compile the codebase:

* Debug configuration - execute *DEV-Build-CodeBase-Debug.bat*
* Release configuration - execute *DEV-Build-CodeBase-Release.bat*

To create deploy packages:

* Azure Emulator - execute *msbuild /t:EmulatorFileSystemPublish Deploy-Application.xml*
* Azure Cloud - execute *msbuild /t:AzureFileSystemPublish Deploy-Application.xml*
* **Note:** IIS deployment packages are created when the codebase is compiled

Location of deployment packages:

* IIS 7.5 - *Bin\Deploy\WebDeploy*
* Azure - *Bin\Deploy\Azure*

To deploy the application:

* IIS 7.5 (*C:\inetpub\wwwroot\AppKernel*) - execute *msbuild /t:WebFileSystemPublish Deploy-Application.xml*
* Azure Emulator - execute *msbuild /t:EmulatorUploadToBlogStorage Deploy-Application.xml* (**note:** no MSBuild target exists to run the package in the Azure Emulator)
* Azure Cloud - execute *msbuild /t:AzureUploadToBlogStorage Deploy-Application.xml* **and followed by** *msbuild /t:AzureDeployToStaging Deploy-Application.xml*