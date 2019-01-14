#To fix remote communication issues, fixed remote access denied issues
Set-Service MpsSvc -StartupType Automatic
(Get-Service 'MpsSvc').Start()
netsh firewall set service REMOTEADMIN enable
Set-Service WinRM -StartMode Automatic
Set-Item WSMan:localhost\client\trustedhosts -value "*" -force

reg add HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\system /v LocalAccountTokenFilterPolicy /t REG_DWORD /d "1" /f
reg add HKLM\SOFTWARE\Microsoft\Ole /v LegacyAuthenticationLevel /t REG_DWORD /d "2" /f
reg add HKLM\SOFTWARE\Microsoft\Ole /v EnableRemoteConnect /t REG_DWORD /d "Y" /f
reg add HKLM\SOFTWARE\Microsoft\Ole /v LegacyImpersonationLevel /t REG_DWORD /d "2" /f

REG add "HKLM\SOFTWARE\Policies\Microsoft\Windows NT\DNSClient" /v RegisterReverseLookup /t REG_DWORD /d 2 /f
REG add "HKLM\SOFTWARE\Policies\Microsoft\Windows NT\DNSClient" /v RegistrationEnabled /t REG_DWORD /d 1 /f
REG add "HKLM\SOFTWARE\Policies\Microsoft\Windows NT\DNSClient" /v RegistrationOverwritesInConflict /t REG_DWORD /d 1 /f
REG add "HKLM\SOFTWARE\Policies\Microsoft\Windows NT\DNSClient" /v RegistrationRefreshInterval /t REG_DWORD /d 1800 /f
REG add "HKLM\SOFTWARE\Policies\Microsoft\Windows NT\DNSClient" /v EnableMulticast /t REG_DWORD /d 0 /f
netsh firewall set service type=remoteadmin mode=enable
netsh advfirewall firewall set rule group="Windows Remote Management" new enable=yes
netsh advfirewall firewall set rule group="remote desktop" new enable=Yes
netsh advfirewall firewall set rule group="windows management instrumentation (WMI)" new enable=Yes 
netsh advfirewall firewall set rule group="remote administration" new enable=yes
Set-NetFirewallProfile -Profile Domain,Public,Private -Enabled False
ipconfig /flushdns
ipconfig /registerdns
Enable-PSRemoting -force
