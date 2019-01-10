if(Select-String "c:\windows\ccm\logs\CAS.log" -pattern "IsCacheCopyNeeded" -quiet){
	Remove-ItemProperty 'hklm:\Software\Microsoft\SMS\Mobile Client\Software Distribution\' 'IsCacheCopyNeededCallBack' -ea SilentlyContinue
	Restart-service 'CCMEXEC';
	([wmiclass]'ROOT\ccm\ClientSDK:CCM_SoftwareUpdatesManager').InstallUpdates([System.Management.ManagementObject[]] (get-wmiobject -query 'SELECT * FROM CCM_SoftwareUpdate' -namespace 'ROOT\ccm\ClientSDK'))
    "FIXING ERROR"
}else{
	#install all
	([wmiclass]'ROOT\ccm\ClientSDK:CCM_SoftwareUpdatesManager').InstallUpdates([System.Management.ManagementObject[]] (get-wmiobject -query 'SELECT * FROM CCM_SoftwareUpdate' -namespace 'ROOT\ccm\ClientSDK'))
    "NO IsCacheCopyNeeded ERROR FOUND"
}
