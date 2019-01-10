#LEDBAT CHECK AND FIX
$ledbat = Get-NetTCPSetting -SettingName InternetCustom | Select CongestionProvider 
#Get-NetTransportFilter -SettingName InternetCustom
if ($ledbat.CongestionProvider -eq "LEDBAT") {
  $true
} else {
"LEDBAT OFF"
Set-NetTCPSetting -SettingName InternetCustom -CongestionProvider LEDBAT
New-NetTransportFilter -SettingName InternetCustom -LocalPortStart 80 -LocalPortEnd 80 -RemotePortStart 0 -RemotePortEnd 65535
New-NetTransportFilter -SettingName InternetCustom -LocalPortStart 443 -LocalPortEnd 443 -RemotePortStart 0 -RemotePortEnd 65535
}
