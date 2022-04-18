#Get-HotFix | Where { $_.InstalledOn -gt (Get-Date).AddDays(-14) } | sort InstalledOn
$Session = New-Object -ComObject "Microsoft.Update.Session"
$Searcher = $Session.CreateUpdateSearcher()
$historyCount = $Searcher.GetTotalHistoryCount()

$UpdateHistory = $Searcher.QueryHistory(0, $historyCount)
$KBs = @()

foreach ($Update in $UpdateHistory) { 
                [regex]::match($Update.Title,'(KB[0-9]{6,7})').value | Where-Object {$_ -ne ""} | foreach { 
                    $KB = New-Object -TypeName PSObject 
                    $KB | Add-Member -MemberType NoteProperty -Name KB -Value $_ 
                    $KB | Add-Member -MemberType NoteProperty -Name Title -Value $Update.Title  
                    $KB | Add-Member -MemberType NoteProperty -Name Description -Value $Update.Description
                    $KB | Add-Member -MemberType NoteProperty -Name Date -Value $Update.Date    
                    $KBs += $KB
                } 
            } 
         
$KBs | Select KB,Date,Title