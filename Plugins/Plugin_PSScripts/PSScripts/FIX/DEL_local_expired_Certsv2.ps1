# Script:	Remove_Local_Expired_v2.ps1
# Purpose:  Parse all local Windows cert stores and remove any certificate that is expired
# Author:   Paperclips
# Email:	pwd9000@hotmail.co.uk
# Date:     March 2015 (Updated May 2015)
# Comments: Requires Powershell 3 or later
#			Tested on Win7, 8, Server 2008, Server 2012
#			Must be run locally on machine (Do not run on server that hosts a PKI Certificate Authority)

#--------------------------------------Get todays date--------------------------------------------------
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
#--------------------------------------END--------------------------------------------------------------