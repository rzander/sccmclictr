#Checks to see matches the PC name
$SN = (gwmi win32_bios).SerialNumber
if ($SN -ne $env:ComputerName) { write "WARNING PC Name not = $SN"}