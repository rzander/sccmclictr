schtasks /create /RU SYSTEM /f /tn "Reboot" /tr "powershell.exe Restart-Computer -Force" /sc once /st 20:00:00
