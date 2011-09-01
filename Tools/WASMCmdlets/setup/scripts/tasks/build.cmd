@echo off
@REM ----------------------------------------------------------------------------------
@REM Build Windows Azure Service Management Tools Solution
@REM ----------------------------------------------------------------------------------

set verbosity=quiet
set pause=true

:: Check for 64-bit Framework
if exist %SystemRoot%\Microsoft.NET\Framework64\v3.5 (
  set msbuild=%SystemRoot%\Microsoft.NET\Framework64\v3.5\msbuild.exe
  goto :run
)
:: Check for 32-bit Framework
if exist %SystemRoot%\Microsoft.NET\Framework\v3.5 (
  set msbuild=%SystemRoot%\Microsoft.NET\Framework\v3.5\msbuild.exe
  goto :run
)

@echo Building "AzureManagementTools.Cmdlets.csproj"
:run
call %msbuild% "..\..\..\code\AzureManagementTools.Cmdlets\AzureManagementTools.Cmdlets.csproj" /t:ReBuild /verbosity:%verbosity%

@if errorlevel 1 goto :error
@echo Build Complete

@goto :exit

:error
@echo An Error Occured building the Windows Azure Service Management Tools Solution

:exit