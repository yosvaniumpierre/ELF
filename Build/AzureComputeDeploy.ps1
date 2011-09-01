$error.clear()
$sub = $args[0]
$certThumbprint = $args[1].ToUpper()
# One possible gotcha - if CurrentUser path does not work after you are SURE that the cert is created then try LocalService
$certPath = "cert:\CurrentUser\MY\" + $certThumbprint
$cert = get-item $certPath
$buildPath = $args[2]
$packagename = $args[3]
$serviceconfig = $args[4]
$servicename = $args[5]
$storageAccount = $args[6]
$buildLabel = $args[7]
$isProduction = ($args[8].ToUpper() -eq "PRODUCTION")

$package = join-path $buildPath $packageName
$config = join-path $buildPath $serviceconfig

if ((Get-PSSnapin | ?{$_.Name -eq "AzureManagementToolsSnapIn"}) -eq $null)
{
  Add-PSSnapin AzureManagementToolsSnapIn
}
 
$hostedService = Get-HostedService $servicename -Certificate $cert -SubscriptionId $sub | Get-Deployment -Slot Staging

if ($isProduction)
{
   $hostedService = Get-HostedService $servicename -Certificate $cert -SubscriptionId $sub | Get-Deployment -Slot Production
}
 
if ($hostedService.Status -ne $null)
{
    $hostedService |
      Set-DeploymentStatus 'Suspended' |
      Get-OperationStatus -WaitToComplete
    $hostedService |
      Remove-Deployment |
      Get-OperationStatus -WaitToComplete
}

if ($isProduction)
{
    Get-HostedService $servicename -Certificate $cert -SubscriptionId $sub |
        New-Deployment Production -package $package -configuration $config -label $buildLabel -serviceName $servicename -StorageServiceName $storageAccount |
        Get-OperationStatus -WaitToComplete
        
    Get-HostedService $servicename -Certificate $cert -SubscriptionId $sub |
        Get-Deployment -Slot Production |
        Set-DeploymentStatus 'Running' |
        Get-OperationStatus -WaitToComplete
}
else
{
    Get-HostedService $servicename -Certificate $cert -SubscriptionId $sub |
        New-Deployment Staging -package $package -configuration $config -label $buildLabel -serviceName $servicename -StorageServiceName $storageAccount |
        Get-OperationStatus -WaitToComplete
        
    Get-HostedService $servicename -Certificate $cert -SubscriptionId $sub |
        Get-Deployment -Slot Staging |
        Set-DeploymentStatus 'Running' |
        Get-OperationStatus -WaitToComplete
}

if ($error) { exit 888 }