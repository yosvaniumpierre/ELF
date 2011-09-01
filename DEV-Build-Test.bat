msbuild /t:BuildTests nbuild.xml /l:FileLogger,Microsoft.Build.Engine;logfile=./Artifacts/BuildLog/build-tests.log
pause
