Function Get-ComputerUpTime
{
    $DObject = New-Object PSObject
    $DObject | Add-Member -MemberType NoteProperty -Name "ComputerName" -Value $env:ComputerName 
    Try{
        $BootTime = Get-WmiObject -Class Win32_OperatingSystem -ErrorAction STOP
        $LastBootUpTime = $BootTime.ConvertToDateTime($BootTime.LastBootUpTime)
        $DObject | Add-Member -MemberType NoteProperty -Name "LastBootUpTime" -Value $LastBootUpTime
        $DObject | Add-Member -MemberType NoteProperty -Name "Days" -Value (((Get-Date) - (Get-Date $LastBootUpTime)).Days)
        $DObject | Add-Member -MemberType NoteProperty -Name "Hours" -Value (((Get-Date) - (Get-Date $LastBootUpTime)).Hours)
    }
    Catch{
        $DObject | Add-Member -MemberType NoteProperty -Name "LastBootUpTime" -Value "N/A"
        $DObject | Add-Member -MemberType NoteProperty -Name "Days" -Value "N/A"
        $DObject | Add-Member -MemberType NoteProperty -Name "Hours" -Value "N/A"
    }
    $DObject
}
Get-ComputerUpTime