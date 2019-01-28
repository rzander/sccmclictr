#Change setup options to match yours.
#SCCM UNINSTALL
Start-Process -Wait C:\Windows\ccmsetup\ccmsetup.exe /uninstall; Start-Sleep 30;
remove-item C:\Windows\CCM -force -recurse;
remove-item C:\Windows\SMSCFG.ini;
kill 'ccmsetup';
#INSTALL
C:\windows\ccmsetup\ccmsetup.exe /service /forceinstall /retry:1 /MP:SERVER /BITSPriority:FOREGROUND /Source:"\\SERVER\SCCM-CLIENT" SMSSITECODE=DP1 RESETKEYINFORMATION=TRUE
Start-Sleep 20; 
schtasks /Run /TN "Microsoft\Configuration Manager\Configuration Manager Client Retry Task"