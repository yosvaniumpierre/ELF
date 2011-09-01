msbuild /t:FileSystemPublish Build-Central.xml /l:FileLogger,Microsoft.Build.Engine;logfile=./Artifacts/BuildLog/build-deploy-package.log
pause
