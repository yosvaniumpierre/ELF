function SearchUninstall($SearchFor, $SearchVersion, $UninstallKey)
{
  $uninstallObjects = ls -path $UninstallKey;
  $found = $FALSE;

  foreach($uninstallEntry in  $uninstallObjects)
  {
    $entryProperty = Get-ItemProperty -LiteralPath registry::$uninstallEntry
    if($entryProperty.DisplayName -like $searchFor)
    {
      $entryVer = New-Object System.Version($entryProperty.DisplayVersion)
      $searchVer = New-Object System.Version($SearchVersion)
      
      if($entryVer -ge $searchVer)
      {
         $found = $TRUE;
         break;
      }
    }
  }

  $found;
}

$res1 = SearchUninstall -SearchFor 'Windows Azure SDK*' -SearchVersion '1.4.20227.1419' -UninstallKey 'HKLM:SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\';
$res2 = SearchUninstall -SearchFor 'Windows Azure SDK*' -SearchVersion '1.4.20227.1419' -UninstallKey 'HKLM:SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\';

($res1 -or $res2)