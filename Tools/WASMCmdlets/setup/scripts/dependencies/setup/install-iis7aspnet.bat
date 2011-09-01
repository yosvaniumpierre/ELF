@ECHO OFF
ECHO Installing ASP.NET role...

REM ServerManagerCmd -install Web-Asp-Net
start /w pkgmgr /iu:IIS-WebServerRole;IIS-WebServer;IIS-CommonHttpFeatures;IIS-StaticContent;IIS-DefaultDocument;IIS-DirectoryBrowsing;IIS-HttpErrors;IIS-HttpRedirect;IIS-ApplicationDevelopment;IIS-ASPNET;IIS-NetFxExtensibility;IIS-ASP;IIS-ISAPIExtensions;IIS-ISAPIFilter;IIS-HealthAndDiagnostics;IIS-HttpLogging;IIS-RequestMonitor;IIS-HttpTracing;IIS-Security;IIS-URLAuthorization;IIS-RequestFiltering;IIS-Performance;IIS-HttpCompressionStatic;IIS-HttpCompressionDynamic;WAS-WindowsActivationService;WAS-ProcessModel;WAS-NetFxEnvironment;

IF EXIST %WINDIR%\Microsoft.NET\Framework\v4.0.30128 (

                call %WINDIR%\Microsoft.NET\Framework\v4.0.30128\aspnet_regiis.exe -i

)