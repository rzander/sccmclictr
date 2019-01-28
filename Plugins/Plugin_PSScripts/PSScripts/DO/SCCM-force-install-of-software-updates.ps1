## https://timmyit.com/2016/08/01/sccm-and-powershell-force-install-of-software-updates-thats-available-on-client-through-wmi/
## use: Trigger-AvailableSupInstall -Computername DESKTOP-CIH3QS1 -Supname All
function trigger-AvailableSupInstall
{
 Param
(
 [String][Parameter(Mandatory=$True, Position=1)] $Computername,
 [String][Parameter(Mandatory=$True, Position=2)] $SupName
 
)
Begin
{
 $AppEvalState0 = "0"
 $AppEvalState1 = "1"
 $ApplicationClass = [WmiClass]"root\ccm\clientSDK:CCM_SoftwareUpdatesManager"
}
 
Process
{
If ($SupName -Like "All" -or $SupName -like "all")
{
 Foreach ($Computer in $Computername)
{
 $Application = (Get-WmiObject -Namespace "root\ccm\clientSDK" -Class CCM_SoftwareUpdate -ComputerName $Computer | Where-Object { $_.EvaluationState -like "*$($AppEvalState0)*" -or $_.EvaluationState -like "*$($AppEvalState1)*"})
 Invoke-WmiMethod -Class CCM_SoftwareUpdatesManager -Name InstallUpdates -ArgumentList (,$Application) -Namespace root\ccm\clientsdk -ComputerName $Computer
 
}
 
}
 Else
 
{
 Foreach ($Computer in $Computername)
{
 $Application = (Get-WmiObject -Namespace "root\ccm\clientSDK" -Class CCM_SoftwareUpdate -ComputerName $Computer | Where-Object { $_.EvaluationState -like "*$($AppEvalState)*" -and $_.Name -like "*$($SupName)*"})
 Invoke-WmiMethod -Class CCM_SoftwareUpdatesManager -Name InstallUpdates -ArgumentList (,$Application) -Namespace root\ccm\clientsdk -ComputerName $Computer
 
}
 
}
}
End {}
}
Trigger-AvailableSupInstall -Computername localhost -Supname All
