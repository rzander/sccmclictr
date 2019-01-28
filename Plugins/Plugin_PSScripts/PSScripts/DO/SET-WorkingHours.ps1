#sets working hours on client to 8am-5pm m->F
$startTime = 8
$endTime = 17
$workingDays = 62
$cmClientUserSettings = [WmiClass]"\\.\ROOT\ccm\ClientSDK:CCM_ClientUXSettings"
$businessHours = $cmClientUserSettings.PSBase.GetMethodParameters("SetBusinessHours")
$businessHours.StartTime = $StartTime
$businessHours.EndTime = $EndTime
$businessHours.WorkingDays = $WorkingDays

Try {
$result = $cmClientUserSettings.PSBase.InvokeMethod("SetBusinessHours", $businessHours, $Null)
If ($result.ReturnValue -eq 0 ) {
"Success."
}
Else {
"Failed to set SCCM client business hours."
}
}
Catch {
"Failed to set SCCM client business hours."
}
