Function Get-CCMLogDirectory {
        $obj = (Get-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\CCM\Logging\@Global').LogDirectory
        if ($obj -eq $null) { $obj = "C:\WINDOWS\CCM\Logs" }
        Write-Output $obj
    }


function Test-CCMCertificateError {
        #Param([Parameter(Mandatory=$true)]$Log)
        # More checks to come
        $logdir = Get-CCMLogDirectory
        $logFile1 = "$logdir\ClientIDManagerStartup.log"
        $error1 = 'Failed to find the certificate in the store'
        $error2 = '[RegTask] - Server rejected registration 3'
        $content = Get-Content -Path $logFile1

        $ok = $true

        if ($content -match $error1) {
            $ok = $false
            $text = 'ConfigMgr Client Certificate: Error failed to find the certificate in store. Attempting fix.'
            Write-Warning $text
            Stop-Service -Name ccmexec -Force
            # Name is persistant across systems.
            $cert = "$env:ProgramData\Microsoft\Crypto\RSA\MachineKeys\19c5cf9c7b5dc9de3e548adb70398402_50e417e0-e461-474b-96e2-077b80325612"
            # CCM creates new certificate when missing.
            Remove-Item -Path $cert -Force -ErrorAction SilentlyContinue | Out-Null
            # Remove the error from the logfile to avoid double remediations based on false positives
            $newContent = $content | Select-String -pattern $Error1 -notmatch
            Out-File -FilePath $logfile -InputObject $newContent -Encoding utf8 -Force
            Start-Service -Name ccmexec
            
            # Update log object
            #$log.ClientCertificate = $error1
        }

        #$content = Get-Content -Path $logFile2
        if ($content -match $error2) {
            $ok = $false
            $text = 'ConfigMgr Client Certificate: Error! Server rejected client registration. Client Certificate not valid. No auto-remediation.'
            Write-Error $text
            #$log.ClientCertificate = $error2
        }

        if ($ok = $true) {
            $text = 'ConfigMgr Client Certificate: OK'
            Write-Output $text
            #$log.ClientCertificate = 'OK'
        }
    }
    Test-CCMCertificateError