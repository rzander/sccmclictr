# Script:	DEL-AllUserTemp.ps1
# Purpose:  This will clean up all temp data for all users. Similar to running CCleaner
# Author:   Russ Ring
# Email:	russr123@yahoo.com
# Date:     Jan 2016 (Updated Jan 2019)


GCI 'C:\Users\*\AppData\Local\Temp\*' | remove-item -Force -recurse -ErrorAction SilentlyContinue
GCI 'C:\Users\*\AppData\Local\CrashDumps\*' | remove-item -Force -recurse -ErrorAction SilentlyContinue
GCI 'C:\Users\*\AppData\Local\Microsoft\Windows\WER*' | remove-item -Force -recurse -ErrorAction SilentlyContinue
#(GP 'C:\Users\*\AppData\Local\Microsoft\Outlook\*.ost').length/1MB -name "Name"
GP 'C:\Users\*\AppData\Local\Microsoft\Outlook\*.ost' |select length/1MB

GP 'C:\Users\*\AppData\Local\Microsoft\Outlook\*.ost' | Select-Object -Property LastWriteTime, @{N='Size';E={[math]::Round(($_.length /1MB)+"MB")}}, Name | Sort-Object -Property Size;


Remove-Item 'C:\Windows\Temp\*' -Force -recurse
Clear-RecycleBin -force

Remove-Item 'C:\$Recycle.Bin\*' -Force -recurse
Remove-Item 'C:\Windows\*.dmp' -Force -recurse
Remove-Item 'C:\Windows\Debug\*.log' -Force -recurse
Remove-Item 'C:\Windows\security\logs\*.log' -Force -recurse
Remove-Item 'C:\Windows\Logs\CBS\*.log' -Force -recurse
Remove-Item 'C:\Windows\Logs\DISM\*.log' -Force -recurse
Remove-Item 'C:\Windows\Logs\DPX\*.log' -Force -recurse
Remove-Item 'C:\Windows\ServiceProfiles\NetworkService\AppData\Local\Temp\*.log' -Force -recurse
Remove-Item 'C:\ProgramData\Microsoft\Windows\WER\ReportQueue\*' -Force -recurse
Remove-Item 'C:\ProgramData\Microsoft\Windows\WER\Temp\*' -Force -recurse
Remove-Item 'C:\Windows\CCM\Temp\*' -Force -recurse


# Delete all Files in C:\inetpub\logs\LogFiles\ older than 30 day(s) per https://docs.microsoft.com/en-us/iis/manage/provisioning-and-managing-iis/managing-iis-log-file-storage#02
GCI 'C:\inetpub\logs\LogFiles\*' -recurse | where {$_.lastwritetime -lt (get-date).adddays(-15) -and -not $_.psiscontainer} |% {remove-item $_.fullname -force}

# Delete all Files in C:\Windows\SoftwareDistribution\Download\ older than 15 day(s)
GCI 'C:\Windows\SoftwareDistribution\Download\*' -recurse | where {$_.lastwritetime -lt (get-date).adddays(-15) -and -not $_.psiscontainer} |% {remove-item $_.fullname -force}


#SCCM Cache Cleanup
#get CCMCache path
$Cachepath = ([wmi]"ROOT\ccm\SoftMgmtAgent:CacheConfig.ConfigKey='Cache'").Location
#Get Items not referenced for more than 30 days
$OldCache = get-wmiobject -query "SELECT * FROM CacheInfoEx" -namespace "ROOT\ccm\SoftMgmtAgent" | Where-Object { ([datetime](Date) - ([System.Management.ManagementDateTimeConverter]::ToDateTime($_.LastReferenced))).Days -gt 15  }
#delete Items on Disk
$OldCache | % { Remove-Item -Path $_.Location -Recurse -Force -ea SilentlyContinue }
#delete Items on WMI
$OldCache | Remove-WmiObject
#Get all cached Items from Disk
$CacheFoldersDisk = (GCI $Cachepath).FullName
#Get all cached Items from WMI
$CacheFoldersWMI = get-wmiobject -query "SELECT * FROM CacheInfoEx" -namespace "ROOT\ccm\SoftMgmtAgent"
#Remove orphaned Folders from Disk
$CacheFoldersDisk | % { if($_ -notin $CacheFoldersWMI.Location) { remove-item -path $_ -recurse -force -ea SilentlyContinue} }
#Remove orphaned WMI Objects
$CacheFoldersWMI| % { if($_.Location -notin $CacheFoldersDisk) { $_ | Remove-WmiObject }}


#Adobe Flash old file "60 day" remediation.
IF (Test-Path '$env:windir\SysWOW64\Macromed\Flash'){
GCI '$env:windir\SysWOW64\Macromed\Flash' -recurse | where {$_.lastwritetime -lt (get-date).adddays(-60) -and -not $_.psiscontainer} |% {remove-item $_.fullname -force}}
IF (Test-Path '$env:windir\System32\Macromed\Flash'){
GCI '$env:windir\System32\Macromed\Flash' -recurse | where {$_.lastwritetime -lt (get-date).adddays(-60) -and -not $_.psiscontainer} |% {remove-item $_.fullname -force}}

##get all user dir size
# dir C:\users | foreach -begin {} -process{ 
# $size=(dir $_.FullName -recurse -force -ea silentlycontinue | Measure-Object 'length' -sum -Maximum).sum
# write-host("{0:n2}" -f ($size/1MB) +" MB",$_.fullname)
#}

function Get-FriendlySize {
    param($Bytes)
    $sizes='MB,GB' -split ','
    for($i=0; ($Bytes -ge 1kb) -and 
        ($i -lt $sizes.Count); $i++) {$Bytes/=1kb}
    $N=2; if($i -eq 0) {$N=0}
    "{0:N$($N)} {1}" -f $Bytes, $sizes[$i]
}


#List all OST's in profiles
#GP 'C:\Users\*\AppData\Local\Microsoft\Outlook\*.ost' | Select-Object -Property LastWriteTime, @{N='Size';E={[math]::Round(($_.length /1MB))}}, Name | Sort -Property Size;

#Remove old OST's that haven't been used for X days
get-itemproperty 'C:\Users\*\AppData\Local\Microsoft\Outlook\*.ost' | where {$_.lastwritetime -lt (get-date).adddays(-60) -and -not $_.psiscontainer} |% {remove-item $_.fullname -force}

#List all OST's in profiles, display size, user and last date used.
# get-itemproperty 'C:\Users\*\AppData\Local\Microsoft\Outlook\*.ost' | foreach -begin {} -process{ 
# $size=(dir $_.FullName -recurse -force -ea silentlycontinue | Measure-Object 'length' -sum -Maximum).sum
# write-host($_.LastWriteTime, "{0:n2}" -f ($size/1MB) +" MB",$_.fullname )
# };

#Display free drive space
(([wmi]"root\cimv2:Win32_logicalDisk.DeviceID='C:'").FreeSpace/1GB).ToString("N2")+"GB"
