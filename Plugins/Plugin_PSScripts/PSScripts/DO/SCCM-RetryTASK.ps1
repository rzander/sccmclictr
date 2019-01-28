#schtasks /Run /TN "Microsoft\Configuration Manager\Configuration Manager Client Retry Task"
$error.clear()
try { schtasks /Run /TN "Microsoft\Configuration Manager\Configuration Manager Client Retry Task" }
catch { C:\Windows\ccmsetup\ccmsetup.exe }
if (!$error) {
"Retry Task Ran"
}