del .\Signed\CommonLibrary.* /F
"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\ildasm.exe" .\CommonLibrary.dll /out:.\Signed\CommonLibrary.il
"C:\Windows\Microsoft.NET\Framework\v4.0.30319\ilasm.exe" .\Signed\CommonLibrary.il /dll /key=.\CodeBase.snk /output=.\Signed\CommonLibrary.dll

pause