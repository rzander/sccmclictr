$ServiceName = 'PeerDistSvc'
$arrService = Get-Service -Name $ServiceName
 if ($arrService.Status -ne "Running"){
 netsh branchcache reset 
 netsh branchcache set service mode=DISTRIBUTED
 sc.exe config PeerDistSvc start= delayed-auto
 Start-Service $ServiceName
 ([wmiclass]'ROOT\ccm:SMS_Client').TriggerSchedule('{00000000-0000-0000-0000-000000000024}')
 ([wmiclass]'ROOT\ccm:SMS_Client').TriggerSchedule('{00000000-0000-0000-0000-000000000023}')
 ([wmiclass]'ROOT\ccm:SMS_Client').TriggerSchedule('{00000000-0000-0000-0000-000000000021}')
 ([wmiclass]'ROOT\ccm:SMS_Client').TriggerSchedule('{00000000-0000-0000-0000-000000000108}')
 Start-Service $ServiceName
 Write-Host "Starting " $ServiceName " service" 
 " ---------------------- " 
 " Service is now started"
 }
 if ($arrService.Status -eq "running"){ 
 Write-Host "$ServiceName service is already started"
 }
