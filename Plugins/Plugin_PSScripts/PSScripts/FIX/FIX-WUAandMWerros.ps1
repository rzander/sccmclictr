#FIX bad WUA location and missing MW's and "Waiting for turn to start updates." errors
if(Select-String "c:\windows\ccm\logs\UpdatesDeployment.log" -pattern "Waiting for turn to start updates." -quiet){
	Remove-Item 'C:\Windows\System32\GroupPolicy\*' -Force -recurse
	Rename-Item -path "c:\windows\ccm\logs\UpdatesDeployment.log" -NewName "UpdatesDeployment-old.log" -force
	Restart-Service 'ccmexec'
	#Remove SG Lock
	$query = "select * from CCM_PrePostActions"; gwmi -Namespace ROOT\ccm\Policy\Machine\RequestedConfig -Query $query | rwmi; gwmi -Namespace ROOT\ccm\Policy\Machine\ActualConfig -Query $query | rwmi
	
	Get-TroubleshootingPack -Path "C:\Windows\diagnostics\system\WindowsUpdate" | Invoke-TroubleshootingPack -Unattended
	Restart-Service 'wuauserv'
	#location refreash
	([wmiclass]'ROOT\ccm:SMS_Client').TriggerSchedule('{00000000-0000-0000-0000-000000000012}')
	([wmiclass]'ROOT\ccm:SMS_Client').TriggerSchedule('{00000000-0000-0000-0000-000000000024}')
	([wmiclass]'ROOT\ccm:SMS_Client').TriggerSchedule('{00000000-0000-0000-0000-000000000023}')

	#MP Refreash
	([wmiclass]'ROOT\ccm:SMS_Client').TriggerSchedule('{00000000-0000-0000-0000-000000000021}')
	([wmiclass]'ROOT\ccm:SMS_Client').TriggerSchedule('{00000000-0000-0000-0000-000000000022}')
	([wmiclass]'ROOT\ccm:SMS_Client').TriggerSchedule('{00000000-0000-0000-0000-000000000042}')
	 
	wuauclt.exe /ResetAuthorization /DetectNow
	wuauclt /reportnow

	#update scan
	([wmiclass]'ROOT\ccm:SMS_Client').TriggerSchedule('{00000000-0000-0000-0000-000000000113}')
	([wmiclass]'ROOT\ccm:SMS_Client').TriggerSchedule('{00000000-0000-0000-0000-000000000108}')
	 (New-Object -ComObject Microsoft.CCM.UpdatesStore).RefreshServerComplianceState()

	#install all
	 ([wmiclass]'ROOT\ccm\ClientSDK:CCM_SoftwareUpdatesManager').InstallUpdates([System.Management.ManagementObject[]] (get-wmiobject -query 'SELECT * FROM CCM_SoftwareUpdate' -namespace 'ROOT\ccm\ClientSDK'))
	 
    "FIXING ERROR"
}else{
	#install all
	([wmiclass]'ROOT\ccm\ClientSDK:CCM_SoftwareUpdatesManager').InstallUpdates([System.Management.ManagementObject[]] (get-wmiobject -query 'SELECT * FROM CCM_SoftwareUpdate' -namespace 'ROOT\ccm\ClientSDK'))
    "NO ERROR FOUND"
}

 