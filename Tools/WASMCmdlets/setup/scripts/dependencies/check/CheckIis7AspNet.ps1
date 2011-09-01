$res1 = (Get-ItemProperty -path 'HKLM:SOFTWARE\Wow6432Node\Microsoft\INETSTP\Components').ASPNET;
$res2 = (Get-ItemProperty -path 'HKLM:SOFTWARE\Microsoft\INETSTP\Components').ASPNET;

($res1 -or $res2)