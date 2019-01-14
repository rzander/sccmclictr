Set-Service MpsSvc -StartupType Automatic
(Get-Service 'MpsSvc').Start()

$A = New-ScheduledTaskAction -Execute "powershell.exe" -Argument '-command &{Get-BitsTransfer -AllUsers | Where-Object { $_.JobState -like "TransientError" } | Remove-BitsTransfer}'
$T = New-ScheduledTaskTrigger -Once -At (get-date).AddSeconds(10); $t.EndBoundary = (get-date).AddSeconds(20).ToString('s')
$S = New-ScheduledTaskSettingsSet -StartWhenAvailable -DeleteExpiredTaskAfter 00:02:00
Register-ScheduledTask -Force -user SYSTEM -TaskName "Fix Stuck BITS" -Action $A -Trigger $T -Settings $S
schtasks /run /tn "Fix Stuck BITS"

$A1 = New-ScheduledTaskAction -Execute "powershell.exe" -Argument '-command &{Get-BitsTransfer -AllUsers | Where-Object { $_.JobState -like "SUSPENDED" } | Resume-BitsTransfer}'
$T1 = New-ScheduledTaskTrigger -Once -At (get-date).AddSeconds(10); $t.EndBoundary = (get-date).AddSeconds(20).ToString('s')
$S1 = New-ScheduledTaskSettingsSet -StartWhenAvailable -DeleteExpiredTaskAfter 00:02:00
Register-ScheduledTask -Force -user SYSTEM -TaskName "Resume BITS" -Action $A1 -Trigger $T1 -Settings $S1
schtasks /run /tn "Resume BITS"
