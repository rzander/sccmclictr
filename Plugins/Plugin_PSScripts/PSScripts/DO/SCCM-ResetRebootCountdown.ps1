#Reset SCCM reboot countdown timer.
#For more info, see my Blog post https://sccmf12twice.com/2019/05/sccm-reboot-decoded-how-to-make-a-pc-cancel-start-extend-or-change-mandatory-reboot-to-non-mandatory-on-the-fly/

$time = [DateTimeOffset]::Now.ToUnixTimeSeconds()
Set-Itemproperty -path 'HKLM:\SOFTWARE\Microsoft\SMS\Mobile Client\Reboot Management\RebootData' -name 'RebootBy' -value $time;
Restart-Service ccmexec -force

