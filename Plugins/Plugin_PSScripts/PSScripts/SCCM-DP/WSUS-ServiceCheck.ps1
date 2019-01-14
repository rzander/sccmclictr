#::WSUS service check
$ServiceName = 'WsusService'
$arrService = Get-Service -Name $ServiceName
while ($arrService.Status -ne 'Running')
{
    Start-Service $ServiceName
    write-host $arrService.status
    write-host 'Service starting'
    Start-Sleep -seconds 5
    $arrService.Refresh()
    if ($arrService.Status -eq 'Running')
    {
        Write-Host 'Service is now Running'
    }
}

$ServiceName = 'CcmExec'
$arrService = Get-Service -Name $ServiceName
while ($arrService.Status -ne 'Running')
{
    Start-Service $ServiceName
    write-host $arrService.status
    write-host 'Service starting'
    Start-Sleep -seconds 5
    $arrService.Refresh()
    if ($arrService.Status -eq 'Running')
    {
        Write-Host 'Service is now Running'
    }
}
$ServiceName = 'MSSQL$MICROSOFT##WID'
$arrService = Get-Service -Name $ServiceName
while ($arrService.Status -ne 'Running')
{
    Start-Service $ServiceName
    write-host $arrService.status
    write-host 'Service starting'
    Start-Sleep -seconds 5
    $arrService.Refresh()
    if ($arrService.Status -eq 'Running')
    {
        Write-Host 'Service is now Running'
    }
}
$ServiceName = 'W3SVC'
$arrService = Get-Service -Name $ServiceName
while ($arrService.Status -ne 'Running')
{
    Start-Service $ServiceName
    write-host $arrService.status
    write-host 'Service starting'
    Start-Sleep -seconds 5
    $arrService.Refresh()
    if ($arrService.Status -eq 'Running')
    {
        Write-Host 'Service is now Running'
    }
}

set-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\SMS\Components\SMS_EXECUTIVE\Threads\SMS_WSUS_CONTROL_MANAGER' -Name 'Requested Operation' -value Start -force
#(Get-WsusServer).GetSubscription().StartSynchronization()
