##get user dir size
dir C:\users | foreach -begin {} -process{ 
$size=(dir $_.FullName -recurse -force -ea silentlycontinue | Measure-Object 'length' -sum -Maximum).sum
write-host("{0:n2}" -f ($size/1MB) +" MB",$_.fullname)
}
