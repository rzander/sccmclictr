Function Get-CMClientWSUSContentLocation
{
    $DObject = New-Object PSObject
    $DObject | Add-Member -MemberType NoteProperty -Name "ComputerName" -Value $env:ComputerName  
        Try{
            $WUA = Get-WmiObject -Namespace "Root\CCM\SoftwareUpdates\WUAHandler" -Class CCM_UpdateSource -ErrorAction STOP
            $DObject | Add-Member -MemberType NoteProperty -Name "Status" -Value "OK"
            $DObject | Add-Member -MemberType NoteProperty -Name "ContentLocation" -Value $WUA.ContentLocation
            $DObject | Add-Member -MemberType NoteProperty -Name "ContentVersion" -Value $WUA.ContentVersion
        }
        Catch{
            Try{
                $WUA = Get-ItemProperty "HKLM:SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate" -ErrorAction STOP
                if($WUA.WUServer.Length -eq 0){
                    $WuServer = "No Server"
                }
                Else{
                    $WuServer = $WUA.WUServer
                }
                $DObject | Add-Member -MemberType NoteProperty -Name "Status" -Value "OK"
                $DObject | Add-Member -MemberType NoteProperty -Name "ContentLocation" -Value $WuServer
                $DObject | Add-Member -MemberType NoteProperty -Name "ContentVersion" -Value "N/A"
            }
            Catch{
                    $DObject | Add-Member -MemberType NoteProperty -Name "Status" -Value $_.Exception.Message
                    $DObject | Add-Member -MemberType NoteProperty -Name "ContentLocation" -Value "N/A"
                    $DObject | Add-Member -MemberType NoteProperty -Name "ContentVersion" -Value "N/A"
            }
            $DObject | Add-Member -MemberType NoteProperty -Name "Status" -Value $_.Exception.Message
            $DObject | Add-Member -MemberType NoteProperty -Name "ContentLocation" -Value "N/A"
            $DObject | Add-Member -MemberType NoteProperty -Name "ContentVersion" -Value "N/A"
        }
    $DObject
}
Get-CMClientWSUSContentLocation
