$netfwk = "HKLM:SOFTWARE\Microsoft\NET Framework Setup\NDP\"
$versions = ls -path $netfwk;
$found = $FALSE;

foreach($version in  $versions)
{
   if ($version.toString().EndsWith("v3.5"))
   {
       $properties = Get-ItemProperty -path registry::$version
       if($properties.SP -eq 1)
        {       
           $found = $TRUE;
           break;
        }       
    }
}

$found;


