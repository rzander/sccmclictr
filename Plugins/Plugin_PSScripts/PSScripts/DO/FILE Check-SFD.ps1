# SDF file check. If these are missing, the client wont work and will need to reinstall.
$ccmdir = (Get-ItemProperty("HKLM:\SOFTWARE\Microsoft\SMS\Client\Configuration\Client Properties"))."Local SMS Path"
$files = Get-ChildItem "$ccmdir\*.sdf"
if ($files.Count -lt 7) { Start-Process $ccmdir\ccmrepair.exe }
else { "SDF Files OK"}

