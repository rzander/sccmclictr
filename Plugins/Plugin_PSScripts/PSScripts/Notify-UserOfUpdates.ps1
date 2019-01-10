#notify user of updates
$updates = Get-WmiObject -Namespace Root\ccm\clientSDK -Class CCM_softwareupdate 
If ($updates -ne $null){
$dead = $updates.deadline[0]
$UTC = [System.Management.ManagementDateTimeConverter]::ToDateTime($dead)
$Timespan = New-TimeSpan -Start $utc -End (get-date)
$countdown = ($Timespan.Days.ToString()).TrimStart('-')
Set-Location $env:windir\CCM
#c:\windows\ccm\SCToastNotification.exe "Notice: Pending Updates" "You have Software Updates available to be installed.  You have $countdown days until the updates will install automatically"
msg * "Notice: Pending Updates" "You have Software Updates available to be installed.  You have $countdown days until the updates will install automatically"
}