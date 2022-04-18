try
{
   ([wmiclass]"ROOT\ccm\ClientSDK:CCM_ServiceWindowManager").IsWindowAvailableNow(4, $true, 30).CanProgramRunNow
}
catch
{
   $false
}