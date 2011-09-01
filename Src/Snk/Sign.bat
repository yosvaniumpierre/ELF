del .\Signed\WebActivator.* /F
"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\ildasm.exe" .\WebActivator.dll /out:.\Signed\WebActivator.il
"C:\Windows\Microsoft.NET\Framework\v4.0.30319\ilasm.exe" .\Signed\WebActivator.il /dll /key=.\CodeBase.snk /output=.\Signed\WebActivator.dll

pause