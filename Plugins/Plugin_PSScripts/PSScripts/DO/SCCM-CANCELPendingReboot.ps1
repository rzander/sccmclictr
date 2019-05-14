#CANCEL a pending reboot
#For more info, see my Blog post https://sccmf12twice.com/2019/05/sccm-reboot-decoded-how-to-make-a-pc-cancel-start-extend-or-change-mandatory-reboot-to-non-mandatory-on-the-fly/

Remove-Item -path 'HKLM:\SOFTWARE\Microsoft\SMS\Mobile Client\Reboot Management\RebootData'; 
Restart-Service ccmexec -force
