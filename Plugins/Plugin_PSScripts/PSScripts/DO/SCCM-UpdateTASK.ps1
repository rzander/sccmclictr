#schtasks /Run /TN "Microsoft\Configuration Manager\Configuration Manager Client Upgrade Task"
#$error.clear()
try { schtasks /Run /TN "Microsoft\Configuration Manager\Configuration Manager Client Upgrade Task" }
catch { "C:\Windows\ccmsetup\ccmsetup.exe" /AutoUpgrade }
#if (!$error) {
#"Task Ran"
#}