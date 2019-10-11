Set-Location (Get-ItemProperty("HKLM:\SOFTWARE\Microsoft\SMS\Client\Configuration\Client Properties")).$("Local SMS Path")
.\SCToastNotification.exe "Notice: Pending Reboot Needed" "Your PC Needs to restart as soon as you can."
msg * "Notice: Pending Reboot Needed...." "Your PC Needs to restart as soon as you can."
<#
[void][System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms")
[void][System.Reflection.Assembly]::LoadWithPartialName("System.Drawing")
[System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms") | out-null
[System.Reflection.Assembly]::LoadWithPartialName("System.Drawing") | out-null
$TimeStart = Get-Date
$TimeEnd = $timeStart.addminutes(480)
Do
{
$TimeNow = Get-Date
if ($TimeNow -ge $TimeEnd)
{
	
	Unregister-Event -SourceIdentifier click_event -ErrorAction SilentlyContinue
	Remove-Event click_event -ErrorAction SilentlyContinue
	[void][System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms")
	[void][System.Reflection.Assembly]::LoadWithPartialName("System.Drawing")
	Exit
}
else
{
	$Balloon = new-object System.Windows.Forms.NotifyIcon
	$Balloon.Icon = [System.Drawing.SystemIcons]::Information
	$Balloon.BalloonTipText = "A reboot is required in order to complete Security patching. Please reboot at your earliest convenience."
	$Balloon.BalloonTipTitle = "Reboot Required"
	$Balloon.BalloonTipIcon = "Warning"
	$Balloon.Visible = $true;
	$Balloon.ShowBalloonTip(20000);
	$Balloon_MouseOver = [System.Windows.Forms.MouseEventHandler]{ $Balloon.ShowBalloonTip(20000) }
	$Balloon.add_MouseClick($Balloon_MouseOver)
	Unregister-Event -SourceIdentifier click_event -ErrorAction SilentlyContinue
	Register-ObjectEvent $Balloon BalloonTipClicked -sourceIdentifier click_event -Action {
		Add-Type -AssemblyName Microsoft.VisualBasic
		
		If ([Microsoft.VisualBasic.Interaction]::MsgBox('Would you like to reboot your machine now?', 'YesNo,MsgBoxSetForeground,Question', 'System Maintenance') -eq "NO")
		{ }
		else
		{
			#shutdown -r -f
            Import-Module BitLocker; Get-BitlockerVolume
            $OutputVariable = (Get-BitlockerVolume -MountPoint "C:" | Select ProtectionStatus)
            If ($OutputVariable -like "Off") { Restart-Computer -Force }
            Else {$error.clear()
            try { Suspend-BitLocker -MountPoint C: -RebootCount 1 }
            catch { manage-bde -protectors -disable c:
            $RegROPath = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce"
            Set-ItemProperty $RegROPath "Bitlocker on" -Value "manage-bde -protectors -enable c:" -type String }
            }
			Restart-Computer -Force
            
		}
		
	} | Out-Null
	
	Wait-Event -timeout 3600 -sourceIdentifier click_event > $null
	Unregister-Event -SourceIdentifier click_event -ErrorAction SilentlyContinue
	$Balloon.Dispose()
}
}
Until ($TimeNow -ge $TimeEnd)
#>