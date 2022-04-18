#this will run the health script if installed, if not installed, it will install it then run it... 
#just point the path to your local server share
#https://www.andersrodland.com/configmgr-client-health-0-8-1-bugfixes/

if (test-path 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Schedule\TaskCache\Tree\ConfigMgr Client Health') {schtasks /Run /TN "ConfigMgr Client Health"}
else {schtasks /create /RU SYSTEM /sc once /st 20:00:00 /tn "Install Client Health" /tr "schtasks.exe /create /f /xml \\LOCAL-SERVER_HERE\ConfigMgr-Client-Health.xml /tn 'ConfigMgr Client Health'";
schtasks /run /tn "Install Client Health";
schtasks /delete /f /tn "Install Client Health";
schtasks /Run /TN "ConfigMgr Client Health"}









