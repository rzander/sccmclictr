﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClientCenter.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.10.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string Username {
            get {
                return ((string)(this["Username"]));
            }
            set {
                this["Username"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string Password {
            get {
                return ((string)(this["Password"]));
            }
            set {
                this["Password"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string DefaultHostName {
            get {
                return ((string)(this["DefaultHostName"]));
            }
            set {
                this["DefaultHostName"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"$a = 0
$timespan = New-Object System.TimeSpan(0, 0, 1)
$scope = New-Object System.Management.ManagementScope(""\\.\root\ccm\Events"")
$query = New-Object System.Management.WQLEventQuery(""CCM_Event"", $timespan)
$watcher = New-Object System.Management.ManagementEventWatcher($scope,$query)
do 
    {
        $b = $watcher.WaitForNextEvent()
        $b
    } 
while ($a -ne 1)")]
        public string PSEventQuery {
            get {
                return ((string)(this["PSEventQuery"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("O:BAG:BAD:(A;;CCDCSW;;;WD)(A;;CCDCLCSWRP;;;BA)(A;;CCDCLCSWRP;;;LU)(A;;CCDCLCSWRP;" +
            ";;S-1-5-32-562)")]
        public string MachineLaunchRestriction {
            get {
                return ((string)(this["MachineLaunchRestriction"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("O:BAG:BAD:(A;;CCDCLC;;;WD)(A;;CCDCLC;;;LU)(A;;CCDCLC;;;S-1-5-32-562)(A;;CCDCLC;;;" +
            "AN)")]
        public string MachineAccessRestriction {
            get {
                return ((string)(this["MachineAccessRestriction"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("O:BAG:BAD:(A;;CCDCLCSWRP;;;SY)(A;;CCDCLCSWRP;;;BA)(A;;CCDCLCSWRP;;;IU)")]
        public string DefaultLaunchPermission {
            get {
                return ((string)(this["DefaultLaunchPermission"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool NoLocalAdminCheck {
            get {
                return ((bool)(this["NoLocalAdminCheck"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5985")]
        public string WinRMPort {
            get {
                return ((string)(this["WinRMPort"]));
            }
            set {
                this["WinRMPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool WinRMSSL {
            get {
                return ((bool)(this["WinRMSSL"]));
            }
            set {
                this["WinRMSSL"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool showPingButton {
            get {
                return ((bool)(this["showPingButton"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string AgentInstallSiteCode {
            get {
                return ((string)(this["AgentInstallSiteCode"]));
            }
            set {
                this["AgentInstallSiteCode"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string AgentInstallMP {
            get {
                return ((string)(this["AgentInstallMP"]));
            }
            set {
                this["AgentInstallMP"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool HideTSAdvertisements {
            get {
                return ((bool)(this["HideTSAdvertisements"]));
            }
            set {
                this["HideTSAdvertisements"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool HideNonUserUIExperienceApplicattions {
            get {
                return ((bool)(this["HideNonUserUIExperienceApplicattions"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool HideShutdownPane {
            get {
                return ((bool)(this["HideShutdownPane"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("$CMMP = \'Management Point FQDN\' \r\n$CMSiteCode = \'xxx\'  \r\n\r\n$ErrorActionPreference" +
            " = \"SilentlyContinue\" \r\n\r\ntry \r\n{ \r\n#Get ccm cache path for later cleanup... \r\n " +
            "   try \r\n    { \r\n        $ccmcache = ([wmi]\"ROOT\\ccm\\SoftMgmtAgent:CacheConfig.C" +
            "onfigKey=\'Cache\'\").Location \r\n    } catch {} \r\n\r\n#download ccmsetup.exe from MP " +
            "\r\n    $webclient = New-Object System.Net.WebClient \r\n    $url = \"http://$($CMMP)" +
            "/CCM_Client/ccmsetup.exe\" \r\n    $file = \"c:\\windows\\temp\\ccmsetup.exe\" \r\n    $we" +
            "bclient.DownloadFile($url,$file) \r\n\r\n#stop the old sms agent service \r\n    stop-" +
            "service \'ccmexec\' -ErrorAction SilentlyContinue \r\n\r\n#Cleanup cache \r\n    if($ccm" +
            "cache -ne $null) \r\n    { \r\n        try \r\n        { \r\n        dir $ccmcache \'*\' -" +
            "directory | % { [io.directory]::delete($_.fullname, $true)  } -ErrorAction Silen" +
            "tlyContinue \r\n        } catch {} \r\n    } \r\n\r\n#Cleanup Execution History \r\n    #R" +
            "emove-Item -Path \'HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\SMS\\Mobile Client\\*\' -Rec" +
            "urse -ErrorAction SilentlyContinue \r\n    #Remove-Item -Path \'HKLM:\\SOFTWARE\\Micr" +
            "osoft\\SMS\\Mobile Client\\*\' -Recurse -ErrorAction SilentlyContinue \r\n\r\n#kill exis" +
            "ting instances of ccmsetup.exe \r\n    $ccm = (Get-Process \'ccmsetup\' -ErrorAction" +
            " SilentlyContinue) \r\n    if($ccm -ne $null) \r\n    { \r\n            $ccm.kill(); \r" +
            "\n    } \r\n\r\n#run ccmsetup \r\n    $proc = Start-Process -FilePath \'c:\\windows\\temp\\" +
            "ccmsetup.exe\' -PassThru -Wait -ArgumentList \"/mp:$($CMMP) /source:http://$($CMMP" +
            ")/CCM_Client CCMHTTPPORT=80 RESETKEYINFORMATION=TRUE SMSSITECODE=$($CMSiteCode) " +
            "SMSSLP=$($CMMP) FSP=$($CMMP)\" \r\n   Sleep(5) \r\n   \"ccmsetup started...\" \r\n} \r\n\r\nc" +
            "atch \r\n{ \r\n        \"an Error occured...\" \r\n        $error[0] \r\n} ")]
        public string AgentInstallPS {
            get {
                return ((string)(this["AgentInstallPS"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool HideBusinessHours {
            get {
                return ((bool)(this["HideBusinessHours"]));
            }
            set {
                this["HideBusinessHours"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Enter-PSSession {0}  -port {1} -SessionOption (New-PSSessionOption -NoMachineProf" +
            "ile:$true)")]
        public string OpenPSConsoleCommand {
            get {
                return ((string)(this["OpenPSConsoleCommand"]));
            }
            set {
                this["OpenPSConsoleCommand"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>CcmExec</string>
  <string>WinRM</string>
  <string>wuauserv</string>
  <string>LanmanWorkstation</string>
  <string>LanmanServer</string>
  <string>RemoteRegistry</string>
  <string>BITS</string>
  <string>CmRcService</string>
  <string>Winmgmt</string>
  <string>PeerDistSvc</string>
  <string>smstsmgr</string>
  <string>RpcSs</string>
  <string>ccmsetup</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection ServicesHighlited {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["ServicesHighlited"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <s" +
            "tring>%systemroot%\\system32\\actxprxy.dll</string>\r\n  <string>%systemroot%\\system" +
            "32\\atl.dll</string>\r\n  <string>%systemroot%\\system32\\Bitsprx2.dll</string>\r\n  <s" +
            "tring>%systemroot%\\system32\\Bitsprx3.dll</string>\r\n  <string>%systemroot%\\system" +
            "32\\browseui.dll</string>\r\n  <string>%systemroot%\\system32\\cryptdlg.dll</string>\r" +
            "\n  <string>%systemroot%\\system32\\dssenh.dll</string>\r\n  <string>%systemroot%\\sys" +
            "tem32\\gpkcsp.dll</string>\r\n  <string>%systemroot%\\system32\\initpki.dll</string>\r" +
            "\n  <string>%systemroot%\\system32\\jscript.dll</string>\r\n  <string>%systemroot%\\sy" +
            "stem32\\mshtml.dll</string>\r\n  <string>%systemroot%\\system32\\msi.dll</string>\r\n  " +
            "<string>%systemroot%\\system32\\mssip32.dll</string>\r\n  <string>%systemroot%\\syste" +
            "m32\\msxml3.dll</string>\r\n  <string>%systemroot%\\system32\\msxml3r.dll</string>\r\n " +
            " <string>%systemroot%\\system32\\msxml6.dll</string>\r\n  <string>%systemroot%\\syste" +
            "m32\\msxml6r.dll</string>\r\n  <string>%systemroot%\\system32\\muweb.dll</string>\r\n  " +
            "<string>%systemroot%\\system32\\ole32.dll</string>\r\n  <string>%systemroot%\\system3" +
            "2\\oleaut32.dll</string>\r\n  <string>%systemroot%\\system32\\Qmgr.dll</string>\r\n  <s" +
            "tring>%systemroot%\\system32\\Qmgrprxy.dll</string>\r\n  <string>%systemroot%\\system" +
            "32\\rsaenh.dll</string>\r\n  <string>%systemroot%\\system32\\sccbase.dll</string>\r\n  " +
            "<string>%systemroot%\\system32\\scrrun.dll</string>\r\n  <string>%systemroot%\\system" +
            "32\\shdocvw.dll</string>\r\n  <string>%systemroot%\\system32\\shell32.dll</string>\r\n " +
            " <string>%systemroot%\\system32\\slbcsp.dll</string>\r\n  <string>%systemroot%\\syste" +
            "m32\\softpub.dll</string>\r\n  <string>%systemroot%\\system32\\urlmon.dll</string>\r\n " +
            " <string>%systemroot%\\system32\\userenv.dll</string>\r\n  <string>%systemroot%\\syst" +
            "em32\\vbscript.dll</string>\r\n  <string>%systemroot%\\system32\\Winhttp.dll</string>" +
            "\r\n  <string>%systemroot%\\system32\\wintrust.dll</string>\r\n  <string>%systemroot%\\" +
            "system32\\wuapi.dll</string>\r\n  <string>%systemroot%\\system32\\wuaueng.dll</string" +
            ">\r\n  <string>%systemroot%\\system32\\wuaueng1.dll</string>\r\n  <string>%systemroot%" +
            "\\system32\\wucltui.dll</string>\r\n  <string>%systemroot%\\system32\\wucltux.dll</str" +
            "ing>\r\n  <string>%systemroot%\\system32\\wups.dll</string>\r\n  <string>%systemroot%\\" +
            "system32\\wups2.dll</string>\r\n  <string>%systemroot%\\system32\\wuweb.dll</string>\r" +
            "\n  <string>%systemroot%\\system32\\wuwebv.dll</string>\r\n  <string>%systemroot%\\sys" +
            "tem32\\wbem\\wmisvc.dll</string>\r\n  <string>%systemroot%\\system32\\Xpob2res.dll</st" +
            "ring>\r\n</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection RegisterDLLs {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["RegisterDLLs"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <s" +
            "tring>Password</string>\r\n  <string>PWD</string>\r\n</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection CollectionVariablesFilter {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["CollectionVariablesFilter"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <s" +
            "tring>Operating System|root\\cimv2|SELECT Caption FROM Win32_OperatingSystem</str" +
            "ing>\r\n  <string>OS Version|root\\cimv2|SELECT Version FROM Win32_OperatingSystem<" +
            "/string>\r\n  <string>OS Architecture|root\\cimv2|SELECT VariableValue FROM  Win32_" +
            "Environment WHERE Name=\'PROCESSOR_ARCHITECTURE\' AND UserName=\'&lt;SYSTEM&gt;\'</s" +
            "tring>\r\n  <string>PC Manufacturer|root\\cimv2|SELECT Manufacturer FROM Win32_Comp" +
            "uterSystem</string>\r\n  <string>LastHWInv|Root\\CCM\\Scheduler|SELECT LastTriggerTi" +
            "me FROM CCM_Scheduler_History WHERE ScheduleID=\'{00000000-0000-0000-0000-0000000" +
            "00001}\' and UserSID=\'Machine\'</string>\r\n  <string>LastSWInv|Root\\CCM\\Scheduler|S" +
            "ELECT LastTriggerTime FROM CCM_Scheduler_History WHERE ScheduleID=\'{00000000-000" +
            "0-0000-0000-000000000002}\' and UserSID=\'Machine\'</string>\r\n  <string>LastDDR|Roo" +
            "t\\CCM\\Scheduler|SELECT LastTriggerTime FROM CCM_Scheduler_History WHERE Schedule" +
            "ID=\'{00000000-0000-0000-0000-000000000003}\' and UserSID=\'Machine\'</string>\r\n  <s" +
            "tring>LastMachinePolicyRequest|Root\\CCM\\Scheduler|SELECT LastTriggerTime FROM CC" +
            "M_Scheduler_History WHERE ScheduleID=\'{00000000-0000-0000-0000-000000000021}\' an" +
            "d UserSID=\'Machine\'</string>\r\n  <string>LastReboot|root\\cimv2|SELECT LastBootUpT" +
            "ime FROM Win32_OperatingSystem</string>\r\n  <string>InstallDate|root\\cimv2|SELECT" +
            " InstallDate FROM Win32_OperatingSystem</string>\r\n  <string>CurrentUser|root\\cim" +
            "v2|SELECT Username FROM Win32_ComputerSystem</string>\r\n  <string>PrimaryUsers|RO" +
            "OT\\ccm\\Policy\\Machine\\ActualConfig|SELECT * FROM CCM_UserAffinity WHERE IsUserAf" +
            "finitySet = \'True\'</string>\r\n  <string>FreeDiskSpace on C: (Bytes)|ROOT\\cimv2|SE" +
            "LECT FreeSpace FROM Win32_LogicalDisk WHERE DeviceID=\'C:\'</string>\r\n  <string>Di" +
            "skSpace on C: (Bytes)|ROOT\\cimv2|SELECT Size FROM Win32_LogicalDisk WHERE Device" +
            "ID=\'C:\'</string>\r\n  <string>Running Executions|root\\CCM\\SoftMgmtAgent|select MIF" +
            "PackageName, RequestID, programID, ReceivedTime, SuspendReboot, advertid, conten" +
            "tid, RunningState, State from ccm_executionrequestex</string>\r\n  <string>BIOS Ve" +
            "rsion|root\\cimv2|SELECT SMBIOSBIOSVersion from Win32_Bios</string>\r\n</ArrayOfStr" +
            "ing>")]
        public global::System.Collections.Specialized.StringCollection AdhocInv {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["AdhocInv"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>2b646eff-442b-410e-adf3-d4ec699e0ab4</string>
  <string>3dde85c4-ce0e-4999-ab84-698a569dfcac</string>
  <string>3fd01cd1-9e01-461e-92cd-94866b8d1f39</string>
  <string>9b73a906-6908-4316-b61e-cbab300c9791</string>
  <string>64db983c-10bc-4b47-8f2d-cfff48f34faf</string>
  <string>ed9dee86-eadd-4ac8-82a1-7234a4646e62</string>
  <string>f7cc4bbb-e70e-43e1-978c-1c263d946fff</string>
  <string>fb04b7a5-bc4c-4468-8eb8-937d8eb90efb</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection ConsoleExtensionGUIDs {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["ConsoleExtensionGUIDs"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <s" +
            "tring>127.0.0.1</string>\r\n</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection recentlyUsedComputers {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["recentlyUsedComputers"]));
            }
            set {
                this["recentlyUsedComputers"] = value;
            }
        }
    }
}
