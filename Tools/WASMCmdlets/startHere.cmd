@echo off

setlocal 
%~d0
cd "%~dp0"
pushd .\setup
@Start DependencyChecker\ConfigurationWizard.exe dependencies.dep
popd