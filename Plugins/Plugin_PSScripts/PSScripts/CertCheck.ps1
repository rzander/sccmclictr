#return code
#1 - only one valid cert (no old certs)
#2 - more than one valid certs (no old certs)
#3 - valid and old cert
#4 - only old certs
#5 - other issue
$certs=@(Dir Cert:\LocalMachine\My | Select FriendlyName, Issuer, Subject, NotAfter, @{Label="ExpiresInDays";Expression={($_.NotAfter - (Get-Date)).Days}})

$validCert=0

#check if there is cert for new PKI servers.. change info to match your PKI cert
foreach ($cert in $certs) {
    #$cert.friendlyName
    if ($cert.issuer -like "CN=CHANGE-THIS*") {
        $validCert+=1      
    }
     if ($cert.issuer -like "CN=Microsoft Remote Attestation Service") {
        $validCert+=1      
    }
}


#return code
#1 - only one valid cert (no old certs)
#2 - more than one valid certs (no old certs)
#3 - valid and old cert (old removed)
#4 - only old certs
#5 - other issue

if (($validCert -eq 1) -and ($certs.Count -eq 1)) { 
    return 1
    }
elseif ($validCert -eq $certs.Count -eq 1) { 
    #return 2
    return 1
    }
elseif (($validCert -ge 1) -and ($certs.Count -gt $validCert)) {
    #remove old certificate
	$today = Get-Date
	$ConfirmPreference = "None"

#--------------------------------------Parse all stores and removed expired-----------------------------
	$store = New-Object System.Security.Cryptography.x509Certificates.x509Store("My","LocalMachine")
	$store.Open("ReadWrite")
	$certs = $store.Certificates | Where-Object {$_.NotAfter -lt $today}
	ForEach ($cert in $certs)
	{
	$store.Remove($cert)
	}
	$store.Close()
	return 3
    }
elseif (($validCert -eq 0) -and ($certs.Count -ge 1)) {
	#remove old certificate
	$today = Get-Date
	$ConfirmPreference = "None"

#--------------------------------------Parse all stores and removed expired-----------------------------
	$store = New-Object System.Security.Cryptography.x509Certificates.x509Store("My","LocalMachine")
	$store.Open("ReadWrite")
	$certs = $store.Certificates | Where-Object {$_.NotAfter -lt $today}
	ForEach ($cert in $certs)
	{
	$store.Remove($cert)
	}
	$store.Close()
	#Run gpupdate to download new certs
	GPUPDATE /FORCE /SCOPE COMPUTER
    return 4
    }
else {
    return 5
    }
