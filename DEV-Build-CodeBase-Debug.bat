REM /p:DeployOnBuild=True - This will pass the property to every project and based on typical MsBuild semantics the projects who does not honor the property will ignore it
REM (msbuild cannot do so for targets). So, for web projects, the Deployment zip package will be created, while class library will create the assembly DLL files.
msbuild /t:BuildDebug /p:DeployOnBuild=True Build-Central.xml /l:FileLogger,Microsoft.Build.Engine;logfile=./Artifacts/BuildLog/build-codebase-debug.log
pause