## Get-BatteryStats test
Function Get-BatteryStats{
$batteryStatus = gwmi batterystatus -name root\wmi
$batterystats  = gwmi win32_portablebattery
$missing = [Math]::Round(((1-($batteryStatus.RemainingCapacity[0]/$batterystats.DesignCapacity))*100), 2)
 
$batteryReport = [pscustomobject]@{'Charging'=$batteryStatus.Charging[0];'PluggedIn'=$batteryStatus.PowerOnline[0];'ListedCapacity'=$batterystats.DesignCapacity;'ActualCapacity'=$batteryStatus.RemainingCapacity[0];'Percent Remaining'=[Math]::Round((($batteryStatus.RemainingCapacity[0]/$batterystats.DesignCapacity)*100),2);MissingCapacity=if ([math]::Sign($missing) -eq '-1'){"ReRunOnBattery"}ELSE{$missing}}
$batteryReport | ft 
 
if ($batteryReport.ActualCapacity -gt $batteryReport.ListedCapacity){Write-Warning "Battery Reports greater than possible capacity, try rerunning this code when unit is not plugged in to AC power"}
if ($batteryReport.MissingCapacity -eq 'ReRunOnBattery'){Write-Warning "Can't obtain missing power, rerun while unit is not plugged in to AC power"}ELSE{"You're missing out on $missing percent of your battery, time to replace!"}
} 
Get-BatteryStats