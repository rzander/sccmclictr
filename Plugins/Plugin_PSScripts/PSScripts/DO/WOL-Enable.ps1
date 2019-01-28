#Wake On LAN
$Adapters = gwmi MSPower_DeviceWakeEnable -namespace 'root\wmi'
if ($Adapters.count -gt 0){
foreach ($Adapter in $Adapters){$Adapter.enable = "$True"}
}else{$Adapters.enable = "$True"}

$Adapters = gwmi MSNdis_DeviceWakeOnMagicPacketOnly -namespace 'root\wmi'
if ($Adapters.count -gt 0){
foreach ($Adapter in $Adapters){$Adapter.enablewakeonmagicpacketonly = "$True"}
}else{$Adapters.enablewakeonmagicpacketonly = "$True"}