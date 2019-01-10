try { schtasks /Run /TN "Microsoft\Configuration Manager\Configuration Manager Client Upgrade Task" }
catch { "C:\Windows\ccmsetup\ccmsetup.exe" /AutoUpgrade
Start-Sleep -Seconds 30; schtasks /Run /TN "Microsoft\Configuration Manager\Configuration Manager Client Upgrade Task" }
