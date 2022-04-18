try
{
   ([wmiclass]"ROOT\ccm\ClientSDK:CCM_ServiceWindowManager").IsWindowAvailableNow(2, $true, 5).CanProgramRunNow
}
catch
{
   $false
}