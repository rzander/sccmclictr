If ((Get-WmiObject -Class Win32_ComputerSystem).Manufacturer -eq "Lenovo") {
    (Get-WmiObject -Class Win32_ComputerSystemProduct).version
  } else {
    (Get-WmiObject -Class Win32_ComputerSystem).model
  }