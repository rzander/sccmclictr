$ServiceName = 'CCMEXEC'
$arrService = Get-Service -Name $ServiceName
Stop-Service $ServiceName;
Start-Sleep -seconds 5;
while ($arrService.Status -ne 'Stopped')
{
    Stop-Service $ServiceName
    write-host $arrService.status
    write-host 'Service Stopping'
    Start-Sleep -seconds 5
    $arrService.Refresh()
}
if ($arrService.Status -eq 'Stopped')
    {
        Write-Host 'Service is now Stopped'
		remove-item C:\Windows\SMSCFG.ini;
		Remove-Item -Path HKLM:\Software\Microsoft\SystemCertificates\SMS\Certificates\* -Force; 
		Start-Service $ServiceName
    }
