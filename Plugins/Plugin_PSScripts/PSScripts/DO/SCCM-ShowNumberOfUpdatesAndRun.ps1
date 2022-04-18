# Get list of all instances of CCM_SoftwareUpdate from root\CCM\ClientSDK for missing updates https://msdn.microsoft.com/en-us/library/jj155450.aspx?f=255&MSPPError=-2147217396
$SCCMUpdatesStore = New-Object -ComObject Microsoft.CCM.UpdatesStore
$SCCMUpdatesStore.RefreshServerComplianceState()
New-EventLog -LogName Application -Source SyncStateScript -ErrorAction SilentlyContinue
Write-EventLog -LogName Application -Source SyncStateScript -EventId 555 -EntryType Information -Message "Sync State ran successfully"

$TargetedUpdates= Get-WmiObject -Namespace root\CCM\ClientSDK -Class CCM_SoftwareUpdate -Filter ComplianceState=0
$approvedUpdates= ($TargetedUpdates |Measure-Object).count
$pendingpatches=($TargetedUpdates |Where-Object {$TargetedUpdates.EvaluationState -ne 8} |Measure-Object).count
$rebootpending=($TargetedUpdates |Where-Object {$TargetedUpdates.EvaluationState -eq 8} |Measure-Object).count
if ($pendingpatches -gt 0) 
{
  try {
	$MissingUpdatesReformatted = @($TargetedUpdates | ForEach-Object {if($_.ComplianceState -eq 0){[WMI]$_.__PATH}}) 
	# The following is the invoke of the CCM_SoftwareUpdatesManager.InstallUpdates with our found updates 
	$InstallReturn = Invoke-WmiMethod -Class CCM_SoftwareUpdatesManager -Name InstallUpdates -ArgumentList (,$MissingUpdatesReformatted) -Namespace root\ccm\clientsdk 
	"Targeted Patches :$approvedUpdates,Pending patches:$pendingpatches,Reboot Pending patches :$rebootpending,initiated $pendingpatches patches for install"  }
	catch {"pending patches - $pendingpatches but unable to install them ,please check Further"}
}
else {"Targeted Patches :$approvedUpdates,Pending patches:$pendingpatches,Reboot Pending patches :$rebootpending,Compliant" }