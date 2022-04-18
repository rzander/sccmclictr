#Remove SG Lock
$query = "select * from CCM_PrePostActions"; gwmi -Namespace ROOT\ccm\Policy\Machine\RequestedConfig -Query $query | rwmi; gwmi -Namespace ROOT\ccm\Policy\Machine\ActualConfig -Query $query | rwmi; $sms = [WmiClass]'root\ccm:sms_client'; $SMS.TriggerSchedule('{00000000-0000-0000-0000-000000000108}')
