using System;
using System.Collections.Generic;
using System.Linq;

using System.Net;
using System.IO;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Xml;
using RuckZuck_WCF;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Xml.Linq;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Net.Http;

namespace RZUpdate
{
    /// <summary>
    /// Updater Class
    /// </summary>
    public class RZUpdater
    {
        static string sAuthToken = "";

        /// <summary>
        /// Constructor
        /// </summary>
        public RZUpdater() : this("FreeRZ", _getTimeToken())
        {
        }

        public RZUpdater(string Username, string Password)
        {
            AddSoftware oSW = new AddSoftware();
            sAuthToken = RZRestAPI.GetAuthToken(Username, Password);

            SoftwareUpdate = new SWUpdate(oSW);
        }

        public RZUpdater(string sSWFile)
        {
            if (sSWFile.EndsWith(".xml", StringComparison.CurrentCultureIgnoreCase))
            {
                SoftwareUpdate = new SWUpdate(ParseXML(sSWFile));
            }

            if (sSWFile.EndsWith(".json", StringComparison.CurrentCultureIgnoreCase))
            {
                SoftwareUpdate = new SWUpdate(ParseJSON(sSWFile));
            }

            if (!File.Exists(sSWFile))
            {
                SoftwareUpdate = new SWUpdate(Parse(sSWFile));
            }

        }

        /// <summary>
        /// Check if there are Updates for a Software
        /// </summary>
        /// <param name="ProductName">Name of the Software Product (must be in the RuckZuck Repository !)</param>
        /// <param name="Version">>Current Version of the Software</param>
        /// <returns>SWUpdate if an Update is available otherwise null</returns>
        public SWUpdate CheckForUpdate(string ProductName, string Version, string Manufacturer = "")
        {
            try
            {
                AddSoftware oSW = new AddSoftware();

                oSW.ProductName = ProductName; // ;
                oSW.ProductVersion = Version; // ;
                oSW.Manufacturer = Manufacturer ?? "";

                List<AddSoftware> oResult = RZRestAPI.CheckForUpdate(new List<AddSoftware>() { oSW }).ToList();
                if (oResult.Count > 0)
                {
                    foreach (AddSoftware SW in oResult)
                    {
                        if (SW.PSPreReq == null)
                        {
                            //Load all MetaData for the specific SW
                            foreach (AddSoftware SWCheck in RZRestAPI.GetSWDefinitions(SW.ProductName, SW.ProductVersion, SW.Manufacturer))
                            {
                                if (string.IsNullOrEmpty(SW.PSPreReq))
                                    SW.PSPreReq = "$true; ";

                                //Check PreReq for all Installation-types of the Software
                                if ((bool)SWUpdate._RunPS(SWCheck.PSPreReq)[0].BaseObject)
                                {
                                    SoftwareUpdate = new SWUpdate(SWCheck);
                                    return SoftwareUpdate;
                                }
                            }
                        }

                        if ((bool)SWUpdate._RunPS(SW.PSPreReq).Last().BaseObject)
                        {
                            SoftwareUpdate = new SWUpdate(SW);
                            return SoftwareUpdate;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Access to the SWUpdate
        /// </summary>
        public SWUpdate SoftwareUpdate;

        internal static string _getTimeToken()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            return Convert.ToBase64String(time.Concat(key).ToArray());
        }

        internal static XElement stripNS(XElement root)
        {
            return new XElement(
                root.Name.LocalName,
                root.HasElements ?
                    root.Elements().Select(el => stripNS(el)) :
                    (object)root.Value
            );
        }

        internal static AddSoftware ParseXML(string sFile)
        {
            XmlDocument xDoc = new XmlDocument();
            if (File.Exists(sFile))
            {
                xDoc.Load(sFile);
            }
            else
            {
                xDoc.LoadXml(sFile);
            }

            string sRes = stripNS(XElement.Parse(xDoc.InnerXml)).ToString();
            xDoc.InnerXml = sRes;

            AddSoftware oSoftware = new AddSoftware();
            oSoftware.Files = new List<contentFiles>();

            foreach (XmlNode xRoot in xDoc.SelectNodes("AddSoftware"))
            {
                try
                {
                    List<string> lPreReq = new List<string>();
                    foreach (XmlNode xProperty in xRoot.ChildNodes)
                    {
                        try
                        {
                            switch (xProperty.Name.ToLower())
                            {
                                case "architecture":
                                    oSoftware.Architecture = xProperty.InnerText;
                                    break;
                                case "author":
                                    oSoftware.Author = xProperty.InnerText;
                                    break;
                                case "category":
                                    oSoftware.Category = xProperty.InnerText;
                                    break;
                                case "contentid":
                                    oSoftware.ContentID = xProperty.InnerText;
                                    break;
                                case "description":
                                    oSoftware.Description = xProperty.InnerText;
                                    break;
                                case "manufacturer":
                                    oSoftware.Manufacturer = xProperty.InnerText;
                                    break;
                                case "msiproductid":
                                    oSoftware.MSIProductID = xProperty.InnerText;
                                    break;
                                case "productname":
                                    oSoftware.ProductName = xProperty.InnerText;
                                    break;
                                case "producturl":
                                    oSoftware.ProductURL = xProperty.InnerText;
                                    break;
                                case "productversion":
                                    oSoftware.ProductVersion = xProperty.InnerText;
                                    break;
                                case "psdetection":
                                    oSoftware.PSDetection = xProperty.InnerText;
                                    break;
                                case "psinstall":
                                    oSoftware.PSInstall = xProperty.InnerText;
                                    break;
                                case "pspostinstall":
                                    oSoftware.PSPostInstall = xProperty.InnerText;
                                    break;
                                case "pspreinstall":
                                    oSoftware.PSPreInstall = xProperty.InnerText;
                                    break;
                                case "psprereq":
                                    oSoftware.PSPreReq = xProperty.InnerText;
                                    break;
                                case "psuninstall":
                                    oSoftware.PSUninstall = xProperty.InnerText;
                                    break;
                                case "shortname":
                                    oSoftware.Shortname = xProperty.InnerText;
                                    break;
                                case "image":
                                    oSoftware.Image = Convert.FromBase64String(xProperty.InnerText);
                                    break;
                                case "prerequisites":
                                    foreach (XmlNode xPreReq in xProperty.ChildNodes)
                                    {
                                        lPreReq.Add(xPreReq.InnerText);
                                    }
                                    break;
                                case "files":
                                    foreach (XmlNode xFile in xProperty.ChildNodes)
                                    {
                                        contentFiles oFile = new contentFiles();
                                        oFile.HashType = "MD5";
                                        try { oFile.FileHash = xFile["FileHash"].InnerText; } catch { }
                                        try { oFile.FileName = xFile["FileName"].InnerText; } catch { }
                                        try { oFile.HashType = xFile["HashType"].InnerText; } catch { }
                                        try { oFile.URL = xFile["URL"].InnerText; } catch { }
                                        oSoftware.Files.Add(oFile);
                                    }

                                    break;
                            }
                        }
                        catch { }
                    }

                    oSoftware.PreRequisites = lPreReq.ToArray();
                }
                catch { }
            }

            return oSoftware;
        }

        internal static AddSoftware ParseJSON(string sFile)
        {
            if (File.Exists(sFile))
            {
                try
                {
                    JavaScriptSerializer ser = new JavaScriptSerializer();
                    string sJson = File.ReadAllText(sFile);
                    AddSoftware lRes;

                    //Check if it's an Arrya (new in V2)
                    if (sJson.TrimStart().StartsWith("["))
                    {
                        List<AddSoftware> lItems = ser.Deserialize<List<AddSoftware>>(sJson);
                        lRes = lItems[0];
                    }
                    else
                    {
                        lRes = ser.Deserialize<AddSoftware>(sJson);
                    }

                    if (lRes.PreRequisites != null)
                    {
                        lRes.PreRequisites = lRes.PreRequisites.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    }
                    else
                        lRes.PreRequisites = new string[0];
                    return lRes;
                }
                catch { }
            }

            return new AddSoftware();
        }

        internal static AddSoftware Parse(string sJSON)
        {
            try
            {
                JavaScriptSerializer ser = new JavaScriptSerializer();
                AddSoftware lRes = ser.Deserialize<AddSoftware>(sJSON);
                lRes.PreRequisites = lRes.PreRequisites.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                return lRes;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



            return new AddSoftware();
        }
    }

    /// <summary>
    /// SWUpdate Class
    /// </summary>
    public class SWUpdate
    {
        public AddSoftware SW;
        //public GetSoftware GetSW;
        //internal string sToken = "";
        //internal deploymentType UpdDT;
        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler Downloaded;
        private static event EventHandler DLProgress = delegate { };
        public event EventHandler ProgressDetails = delegate { };
        internal DLTask downloadTask;
        private ReaderWriterLockSlim UILock = new ReaderWriterLockSlim();
        public string sUserName = "FreeRZ";
        public bool SendFeedback = true;
        public string ContentPath = "";

        //Constructor
        public SWUpdate(AddSoftware Software)
        {
            SW = Software;
            //downloadTask = new DLTask();
            downloadTask = new DLTask() { ProductName = SW.ProductName, ProductVersion = SW.ProductVersion, Manufacturer = SW.Manufacturer, Shortname = SW.Shortname, Image = SW.Image, Files = SW.Files, UnInstalled = false, Installed = false, Installing = false };
            downloadTask.SWUpd = this;

            try
            {
                if (SW.Image == null)
                {
                    SW.Image = RZRestAPI.GetIcon(SW.SWId);
                    downloadTask.Image = SW.Image;
                }
            }
            catch { }

            if (SW.Files == null)
                SW.Files = new List<contentFiles>();
            if (SW.PreRequisites == null)
                SW.PreRequisites = new string[0];

            foreach (contentFiles vFile in SW.Files)
            {
                if (string.IsNullOrEmpty(vFile.HashType))
                    vFile.HashType = "MD5";
            }
            //sToken = AuthToken;
        }

        //Constructor
        public SWUpdate(string ProductName, string ProductVersion, string Manufacturer, bool NoPreReqCheck = false)
        {
            SW = null;
            SW = new AddSoftware();

            SW.ProductName = ProductName;
            SW.ProductVersion = ProductVersion;
            SW.Manufacturer = Manufacturer;

            downloadTask = new DLTask() { ProductName = SW.ProductName, ProductVersion = SW.ProductVersion, Manufacturer = SW.Manufacturer, UnInstalled = false, Installed = false };
            downloadTask.SWUpd = this;


            //Get Install-type
            if (!GetInstallType(NoPreReqCheck))
            {
                SW = null;
                return;
            }

            downloadTask = new DLTask() { ProductName = SW.ProductName, ProductVersion = SW.ProductVersion, Manufacturer = SW.Manufacturer, Shortname = SW.Shortname, Image = SW.Image, Files = SW.Files, UnInstalled = false, Installed = false };

            if (SW == null)
            {
                SW = RZRestAPI.GetSWDefinitions(ProductName, ProductVersion, Manufacturer).FirstOrDefault();

                try
                {
                    if (SW.Image == null)
                    {
                        SW.Image = RZRestAPI.GetIcon(SW.SWId);
                    }
                }
                catch { }

                if (SW.Files == null)
                    SW.Files = new List<contentFiles>();

                if (string.IsNullOrEmpty(SW.PSPreReq))
                    SW.PSPreReq = "$true; ";
            }

            if (SW.Files != null)
            {
                foreach (contentFiles vFile in SW.Files)
                {
                    if (string.IsNullOrEmpty(vFile.HashType))
                        vFile.HashType = "MD5";
                }
            }

            if (SW.PreRequisites == null)
                SW.PreRequisites = new string[0];


        }

        public SWUpdate(string Shortname)
        {
            SW = null;
            downloadTask = new DLTask();
            downloadTask.SWUpd = this;
            downloadTask.Shortname = Shortname;

            try
            {

                SW = new AddSoftware();

                //Always use local JSON-File if exists
                if (File.Exists(Path.Combine(Environment.ExpandEnvironmentVariables("%TEMP%"), Shortname + ".json")))
                {
                    string sSWFile = Path.Combine(Environment.ExpandEnvironmentVariables("%TEMP%"), Shortname + ".json");
                    SW = new SWUpdate(RZUpdater.ParseJSON(sSWFile)).SW;
                }
                else
                {
                    var oGetSW = RZRestAPI.SWGet(Shortname).FirstOrDefault();
                    if (oGetSW != null)
                    {
                        SW.ProductName = oGetSW.ProductName;
                        SW.ProductVersion = oGetSW.ProductVersion;
                        SW.Manufacturer = oGetSW.Manufacturer;
                        SW.Shortname = Shortname;

                        if (SW.Architecture == null)
                        {
                            SW = RZRestAPI.GetSWDefinitions(oGetSW.ProductName, oGetSW.ProductVersion, oGetSW.Manufacturer).FirstOrDefault();
                            SW.Shortname = Shortname;
                            try
                            {
                                if (SW.Image == null)
                                {
                                    SW.Image = RZRestAPI.GetIcon(SW.SWId);
                                }
                            }
                            catch { }

                            if (SW.Files == null)
                                SW.Files = new List<contentFiles>();

                            if (string.IsNullOrEmpty(SW.PSPreReq))
                                SW.PSPreReq = "$true; ";
                        }
                    }

                    if (string.IsNullOrEmpty(SW.Shortname))
                        return;

                    //Get Install-type
                    GetInstallType();


                }

                downloadTask = new DLTask() { ProductName = SW.ProductName, ProductVersion = SW.ProductVersion, Manufacturer = SW.Manufacturer, Shortname = SW.Shortname, Image = SW.Image, Files = SW.Files };

                foreach (contentFiles vFile in SW.Files)
                {
                    if (string.IsNullOrEmpty(vFile.HashType))
                        vFile.HashType = "MD5";
                }

                if (SW.PreRequisites == null)
                    SW.PreRequisites = new string[0];
            }
            catch { }
        }

        public bool GetInstallType(bool bGetFirst = false)
        {
            //Only get other DeploymentTypes if Architecture is not defined...
            if (string.IsNullOrEmpty(this.SW.Architecture))
            {
                foreach (var DT in RZRestAPI.GetSWDefinitions(SW.ProductName, SW.ProductVersion, SW.Manufacturer))
                {
                    try
                    {
                        //Check PreReqs
                        try
                        {
                            if (!string.IsNullOrEmpty(DT.PSPreReq))
                            {
                                if (!bGetFirst)
                                {
                                    if (!(bool)_RunPS(DT.PSPreReq).Last().BaseObject)
                                        continue;
                                }
                            }
                        }
                        catch { continue; }

                        SW = DT;

                        try
                        {
                            if (SW.Image == null)
                            {
                                SW.Image = RZRestAPI.GetIcon(SW.SWId);
                            }
                        }
                        catch { }

                        return true;
                    }
                    catch { }
                }

                return false;
            }

            return true;
        }

        private bool _Download(bool Enforce, string DLPath)
        {
            bool bError = false;
            ContentPath = DLPath;
            if (!Enforce)
            {
                //Check if it's still required
                try
                {
                    if (CheckIsInstalled(true))
                    {
                        if (Downloaded != null)
                            Downloaded(downloadTask, EventArgs.Empty);
                        return true;
                    }
                }
                catch { }
            }

            if (SW.Files == null)
                SW.Files = new List<contentFiles>();

            //only XML File contains Files
            if (SW.Files.Count() > 0)
            {
                foreach (var vFile in SW.Files)
                {
                    bool bDLSuccess = false;
                    try
                    {
                        if (string.IsNullOrEmpty(vFile.URL))
                        {
                            downloadTask.PercentDownloaded = 100;
                            ProgressDetails(downloadTask, EventArgs.Empty);
                            continue;
                        }

                        string sDir = DLPath; // Path.Combine(Environment.ExpandEnvironmentVariables(DLPath), SW.ContentID);

                        string sFile = Path.Combine(sDir, vFile.FileName);

                        if (!Directory.Exists(sDir))
                            Directory.CreateDirectory(sDir);

                        bool bDownload = true;

                        //Check File-Hash on existing Files...
                        if (File.Exists(sFile))
                        {
                            if (string.IsNullOrEmpty(vFile.FileHash))
                            {
                                File.Delete(sFile);
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(vFile.HashType))
                                    vFile.HashType = "MD5";

                                if (vFile.HashType.ToUpper() == "MD5")
                                {
                                    if (!_checkFileMd5(sFile, vFile.FileHash))
                                    {
                                        Console.WriteLine("Hash mismatch on existing File " + vFile.FileName);
                                        File.Delete(sFile); //Hash mismatch
                                    }
                                    else
                                        bDownload = false; //Do not download, Hash is valid   
                                }
                                if (vFile.HashType.ToUpper() == "SHA1")
                                {
                                    if (!_checkFileSHA1(sFile, vFile.FileHash))
                                        File.Delete(sFile); //Hash mismatch
                                    else
                                        bDownload = false; //Do not download, Hash is valid  
                                }
                                if (vFile.HashType.ToUpper() == "SHA256")
                                {
                                    if (!_checkFileSHA256(sFile, vFile.FileHash))
                                        File.Delete(sFile); //Hash mismatch
                                    else
                                        bDownload = false; //Do not download, Hash is valid  
                                }

                                if (vFile.HashType.ToUpper() == "X509")
                                {
                                    if (!_checkFileX509(sFile, vFile.FileHash))
                                        File.Delete(sFile); //Hash mismatch
                                    else
                                        bDownload = false; //Do not download, Hash is valid  
                                }
                            }
                        }

                        //Call GetContentFiles to count downloaded Files
                        //oAPI.getContentFiles(SW.ContentID);

                        if (bDownload)
                        {
                            downloadTask.PercentDownloaded = 0;
                            downloadTask.Downloading = true;
                            ProgressDetails(downloadTask, EventArgs.Empty);

                            if (!_DownloadFile2(vFile.URL, sFile))
                            {

                                downloadTask.Error = true;
                                downloadTask.PercentDownloaded = 0;
                                downloadTask.ErrorMessage = "ERROR: download failed... " + vFile.FileName;
                                Console.WriteLine("ERROR: download failed... " + vFile.FileName);
                                ProgressDetails(downloadTask, EventArgs.Empty);
                                File.Delete(sFile);
                                return false;
                            }
                            else
                            {
                                bDLSuccess = true;
                            }

                            //Sleep 1s to complete
                            Thread.Sleep(1000);
                            ProgressDetails(downloadTask, EventArgs.Empty);
                            //downloadTask.Downloading = false;

                        }
                        else
                        {
                            downloadTask.PercentDownloaded = 100;
                            downloadTask.Downloading = false;
                        }

                        //Only Check Hash if downloaded
                        if (!string.IsNullOrEmpty(vFile.FileHash) && bDownload)
                        {
                            if (string.IsNullOrEmpty(vFile.HashType))
                                vFile.HashType = "MD5";

                            //Check if there is a File
                            long iFileSize = 0;
                            try
                            {
                                FileInfo fi = new FileInfo(sFile);
                                iFileSize = fi.Length;
                            }
                            catch { }

                            if (iFileSize == 0)
                            {
                                downloadTask.Error = true;
                                downloadTask.PercentDownloaded = 0;
                                downloadTask.ErrorMessage = "ERROR: empty File... " + vFile.FileName;
                                Console.WriteLine("ERROR: empty File... " + vFile.FileName);
                                ProgressDetails(downloadTask, EventArgs.Empty);
                                File.Delete(sFile);
                                return false;
                            }
                            else
                            {


                                //Check default MD5 Hash
                                if (vFile.HashType.ToUpper() == "MD5")
                                {
                                    if (!_checkFileMd5(sFile, vFile.FileHash))
                                    {
                                        downloadTask.Error = true;
                                        downloadTask.PercentDownloaded = 0;
                                        downloadTask.ErrorMessage = "ERROR: Hash mismatch on File " + vFile.FileName;
                                        Console.WriteLine("ERROR: Hash mismatch on File " + vFile.FileName);
                                        File.Delete(sFile);
                                        if (SendFeedback)
                                            RZRestAPI.Feedback(SW.ProductName, SW.ProductVersion, SW.Manufacturer, SW.Architecture, "false", sUserName, "Hash mismatch").ConfigureAwait(false);
                                        bError = true;
                                    }
                                    else
                                    {
                                        downloadTask.PercentDownloaded = 100;
                                    }
                                }

                                //Check default SHA1 Hash
                                if (vFile.HashType.ToUpper() == "SHA1")
                                {
                                    if (!_checkFileSHA1(sFile, vFile.FileHash))
                                    {
                                        downloadTask.Error = true;
                                        downloadTask.PercentDownloaded = 0;
                                        downloadTask.ErrorMessage = "ERROR: Hash mismatch on File " + vFile.FileName;
                                        Console.WriteLine("ERROR: Hash mismatch on File " + vFile.FileName);
                                        File.Delete(sFile);
                                        if (SendFeedback)
                                            RZRestAPI.Feedback(SW.ProductName, SW.ProductVersion, SW.Manufacturer, SW.Architecture, "false", sUserName, "Hash mismatch").ConfigureAwait(false);
                                        bError = true;
                                    }
                                    else
                                    {
                                        downloadTask.PercentDownloaded = 100;
                                    }
                                }

                                //Check default SHA256 Hash
                                if (vFile.HashType.ToUpper() == "SHA256")
                                {
                                    if (!_checkFileSHA256(sFile, vFile.FileHash))
                                    {
                                        downloadTask.Error = true;
                                        downloadTask.PercentDownloaded = 0;
                                        downloadTask.ErrorMessage = "ERROR: Hash mismatch on File " + vFile.FileName;
                                        Console.WriteLine("ERROR: Hash mismatch on File " + vFile.FileName);
                                        File.Delete(sFile);
                                        if (SendFeedback)
                                            RZRestAPI.Feedback(SW.ProductName, SW.ProductVersion, SW.Manufacturer, SW.Architecture, "false", sUserName, "Hash mismatch").ConfigureAwait(false);
                                        bError = true;
                                    }
                                    else
                                    {
                                        downloadTask.PercentDownloaded = 100;
                                    }
                                }

                                if (vFile.HashType.ToUpper() == "X509")
                                {
                                    if (!_checkFileX509(sFile, vFile.FileHash))
                                    {
                                        downloadTask.Error = true;
                                        downloadTask.PercentDownloaded = 0;
                                        downloadTask.ErrorMessage = "ERROR: Signature mismatch on File " + vFile.FileName;
                                        Console.WriteLine("ERROR: Signature mismatch on File " + vFile.FileName);
                                        File.Delete(sFile);
                                        if (SendFeedback)
                                            RZRestAPI.Feedback(SW.ProductName, SW.ProductVersion, SW.Manufacturer, SW.Architecture, "false", sUserName, "Signature mismatch").ConfigureAwait(false);
                                        bError = true;
                                    }
                                    else
                                    {
                                        downloadTask.PercentDownloaded = 100;
                                    }
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        downloadTask.PercentDownloaded = 0;
                        downloadTask.ErrorMessage = ex.Message;
                        Console.WriteLine("ERROR: " + ex.Message);
                        bError = true;
                    }

                    if (SendFeedback && bDLSuccess)
                    {
                        RZRestAPI.TrackDownloads2(SW.SWId, SW.Architecture, SW.Shortname);
                    }
                }


            }
            else
            {
                downloadTask.PercentDownloaded = 100;
            }

            downloadTask.Downloading = false;


            if (bError)
            {
                downloadTask.PercentDownloaded = 0;
                downloadTask.Error = true;
            }
            else
            {
                downloadTask.Error = false;
                downloadTask.ErrorMessage = "";
            }

            ProgressDetails(downloadTask, EventArgs.Empty);

            if (Downloaded != null)
                Downloaded(downloadTask, EventArgs.Empty);

            return !bError;
        }

        /// <summary>
        /// Download all related Files to %TEMP%
        /// </summary>
        /// <returns>true = success</returns>
        public async Task<bool> Download()
        {
            bool bAutoInstall = downloadTask.AutoInstall;
            downloadTask = new DLTask() { ProductName = SW.ProductName, ProductVersion = SW.ProductVersion, Manufacturer = SW.Manufacturer, Shortname = SW.Shortname, Image = SW.Image, Files = SW.Files };

            if (SW.PreRequisites.Length > 0)
            {
                downloadTask.WaitingForDependency = true;
                downloadTask.AutoInstall = false;
            }
            else
            {
                downloadTask.AutoInstall = bAutoInstall;
            }
            downloadTask.Error = false;
            downloadTask.SWUpd = this;
            downloadTask.Downloading = true;
            ProgressDetails += SWUpdate_ProgressDetails;

            bool bResult = await Task.Run(() => _Download(false, Path.Combine(Environment.ExpandEnvironmentVariables("%TEMP%"), SW.ContentID))).ConfigureAwait(false);
            return bResult;
        }

        /// <summary>
        /// Download all related Files to %TEMP%
        /// </summary>
        /// <param name="Enforce">True = do not check if SW is already installed</param>
        /// <returns>true = success</returns>
        public async Task<bool> Download(bool Enforce)
        {
            return await Download(Enforce, Path.Combine(Environment.ExpandEnvironmentVariables("%TEMP%"), SW.ContentID));
            /*bool bAutoInstall = downloadTask.AutoInstall;
            downloadTask = new DLTask() { ProductName = SW.ProductName, ProductVersion = SW.ProductVersion, Manufacturer = SW.Manufacturer, Shortname = SW.Shortname, Image = SW.Image, Files = SW.Files };

            if (SW.PreRequisites.Length > 0)
            {
                downloadTask.WaitingForDependency = true;
                downloadTask.AutoInstall = false;
            }
            else
            {
                downloadTask.AutoInstall = bAutoInstall;
            }
            downloadTask.Error = false;
            downloadTask.SWUpd = this;
            downloadTask.Downloading = true;
            ProgressDetails += SWUpdate_ProgressDetails;

            bool bResult = await Task.Run(() => _Download(Enforce, Path.Combine(Environment.ExpandEnvironmentVariables("%TEMP%"), SW.ContentID))).ConfigureAwait(false);
            return bResult;*/
        }

        public async Task<bool> Download(bool Enforce, string DLPath)
        {
            bool bAutoInstall = downloadTask.AutoInstall;
            downloadTask = new DLTask() { ProductName = SW.ProductName, ProductVersion = SW.ProductVersion, Manufacturer = SW.Manufacturer, Shortname = SW.Shortname, Image = SW.Image, Files = SW.Files };

            if (SW.PreRequisites != null)
            {
                if (SW.PreRequisites.Length > 0)
                {
                    downloadTask.WaitingForDependency = true;
                    downloadTask.AutoInstall = false;
                }
                else
                {
                    downloadTask.AutoInstall = bAutoInstall;
                }
            }
            else
            {
                downloadTask.AutoInstall = bAutoInstall;
            }
            downloadTask.Error = false;
            downloadTask.SWUpd = this;
            downloadTask.Downloading = true;
            ProgressDetails += SWUpdate_ProgressDetails;

            bool bResult = await Task.Run(() => _Download(Enforce, DLPath)).ConfigureAwait(false);
            return bResult;
        }

        private void SWUpdate_ProgressDetails(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(DLStatus))
            {
                try
                {
                    DLStatus dlStatus = sender as DLStatus;
                    downloadTask.Installing = false;
                    downloadTask.Downloading = true;
                    downloadTask.DownloadedBytes = dlStatus.DownloadedBytes;
                    downloadTask.PercentDownloaded = dlStatus.PercentDownloaded;
                    downloadTask.TotalBytes = dlStatus.TotalBytes;
                }
                catch { }
            }
        }

        /// <summary>
        /// Install a SWUpdate
        /// </summary>
        /// <param name="Force">Do not check if SW is already installed.</param>
        /// <returns></returns>
        private bool _Install(bool Force = false)
        {
            bool bError = false;

            //Check if Installer is already running
            if (downloadTask.Installing)
            {
                Thread.Sleep(1500);
                return CheckIsInstalled(true); ;
            }

            downloadTask.Installing = true;

            if (!CheckDTPreReq())
            {
                Console.WriteLine("Requirements not valid. Installation will not start.");
                downloadTask.Installing = false;
                downloadTask.Installed = false;
                downloadTask.Error = true;
                downloadTask.ErrorMessage = "Requirements not valid. Installation will not start.";
                ProgressDetails(this.downloadTask, EventArgs.Empty);

                if (SendFeedback)
                    RZRestAPI.Feedback(SW.ProductName, SW.ProductVersion, SW.Manufacturer, SW.Architecture, "false", sUserName, "Requirements not valid. Installation will not start.").ConfigureAwait(false);

                return false;
            }

            //Is Product already installed ?
            try
            {
                if (!Force)
                {
                    //Already installed ?
                    if (CheckIsInstalled(true))
                    {
                        if (SendFeedback)
                            RZRestAPI.Feedback(SW.ProductName, SW.ProductVersion, SW.Manufacturer, SW.Architecture, "true", sUserName, "Ok..").ConfigureAwait(false); ;
                        return true;
                    }
                }



                downloadTask.Installing = true;


                //Set CurrentDir and $Folder variable
                string sFolder = ContentPath;
                if (string.IsNullOrEmpty(ContentPath))
                {
                    string sLocalPath = Environment.ExpandEnvironmentVariables("%TEMP%");
                    sFolder = Path.Combine(sLocalPath, SW.ContentID.ToString());
                }

                string psPath = string.Format("Set-Location -Path \"{0}\" -ErrorAction SilentlyContinue; $Folder = \"{0}\";", sFolder);
                int iExitCode = -1;

                //Run Install Script
                if (!string.IsNullOrEmpty(SW.PSInstall))
                {
                    try
                    {
                        downloadTask.Installing = true;
                        ProgressDetails(this.downloadTask, EventArgs.Empty);

                        var oResult = _RunPS(psPath + SW.PSPreInstall + ";" + SW.PSInstall + ";" + SW.PSPostInstall + ";$ExitCode", "", new TimeSpan(0, 60, 0));

                        try
                        {
                            iExitCode = ((int)oResult.Last().BaseObject);
                        }
                        catch { }

                        //Wait 1s to let the installer close completely...
                        System.Threading.Thread.Sleep(1100);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("PS ERROR: " + ex.Message);
                    }

                    //InstProgress(this, EventArgs.Empty);
                }

                //is installed ?
                if (CheckIsInstalled(false))
                {
                    ProgressDetails(downloadTask, EventArgs.Empty);
                    if (SendFeedback)
                        RZRestAPI.Feedback(SW.ProductName, SW.ProductVersion, SW.Manufacturer, SW.Architecture, "true", sUserName, "Ok...").ConfigureAwait(false); ;
                    return true;
                }
                else
                {
                    Console.WriteLine("WARNING: Product not detected after installation.");
                    if (iExitCode != 0 && iExitCode != 3010)
                    {
                        if (SendFeedback)
                            RZRestAPI.Feedback(SW.ProductName, SW.ProductVersion, SW.Manufacturer, SW.Architecture, "false", sUserName, "Product not detected after installation.").ConfigureAwait(false); ;
                    }
                    downloadTask.Error = true;
                    downloadTask.ErrorMessage = "WARNING: Product not detected after installation.";
                    downloadTask.Installed = false;
                    downloadTask.Installing = false;
                    ProgressDetails(downloadTask, EventArgs.Empty);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                RZRestAPI.Feedback(SW.ProductName, SW.ProductVersion, SW.Manufacturer, SW.Architecture, "false", sUserName, "ERROR: " + ex.Message).ConfigureAwait(false); ;
                downloadTask.Error = true;
                downloadTask.ErrorMessage = "WARNING: Product not detected after installation.";
                downloadTask.Installed = false;
                downloadTask.Installing = false;
                bError = true;
            }

            //RZRestAPI.Feedback(SW.ProductName, SW.ProductVersion, (!bError).ToString(), "RZUpdate", "");
            ProgressDetails(this.downloadTask, EventArgs.Empty);
            return !bError;
        }

        public async Task<bool> Install(bool Force = false, bool Retry = false)
        {
            bool msiIsRunning = false;
            bool RZisRunning = false;
            do
            {
                //Check if MSI is running...
                try
                {
                    using (var mutex = Mutex.OpenExisting(@"Global\_MSIExecute"))
                    {
                        msiIsRunning = true;
                        if (Retry)
                        {
                            Console.WriteLine("Warning: Windows-Installer setup is already running!... waiting...");
                            Thread.Sleep(new TimeSpan(0, 0, 10));
                        }
                        else
                            return false;
                    }
                    GC.Collect();
                }
                catch
                {
                    msiIsRunning = false;
                }


                //Check if RuckZuckis running...
                try
                {
                    using (var mutex = Mutex.OpenExisting(@"Global\RuckZuck"))
                    {
                        RZisRunning = true;
                        if (Retry)
                        {
                            Console.WriteLine("Warning: RuckZuck setup is already running!... waiting...");
                            Thread.Sleep(new TimeSpan(0, 0, 10));
                        }
                        else
                            return false;
                    }
                    GC.Collect();
                }
                catch
                {
                    RZisRunning = false;
                }
            }
            while (msiIsRunning || RZisRunning);

            bool bMutexCreated = false;
            bool bResult = false;

            using (Mutex mutex = new Mutex(false, "Global\\RuckZuck", out bMutexCreated))
            {
                bResult = await Task.Run(() => _Install(Force)).ConfigureAwait(false);

                if (bMutexCreated)
                    mutex.Close();
            }
            GC.Collect();
            return bResult;


        }

        private bool _UnInstall(bool Force = false)
        {
            //Check if Installer is already running
            if (downloadTask.Installing)
            {
                Thread.Sleep(1500);
                CheckIsInstalled(true);
                //ProgressDetails(this.downloadTask, EventArgs.Empty);
                return true;

            }

            downloadTask.Installing = true;

            var tGetSWRepo = Task.Run(() =>
            {
                bool bError = false;

                if (!CheckDTPreReq() && !Force)
                {

                    Console.WriteLine("Requirements not valid. Installation will not start.");
                    downloadTask.Installing = false;
                    downloadTask.Installed = false;
                    downloadTask.Error = true;
                    downloadTask.ErrorMessage = "Requirements not valid. Installation will not start.";
                    ProgressDetails(this.downloadTask, EventArgs.Empty);

                    if (SendFeedback)
                        RZRestAPI.Feedback(SW.ProductName, SW.ProductVersion, SW.Manufacturer, SW.Architecture, "false", sUserName, "Requirements not valid. Installation will not start.").ConfigureAwait(false); ;

                    return false;
                }

                //Is Product already installed ?
                try
                {
                    if (!Force)
                    {
                        //Already installed ?
                        if (!CheckIsInstalled(false))
                        {
                            downloadTask.Installed = false;
                            downloadTask.Installing = false;
                            downloadTask.UnInstalled = true;
                            downloadTask.Error = false;
                            return true;
                        }
                    }

                    //Check if Installer is already running
                    while (downloadTask.Installing)
                    {
                        Thread.Sleep(1500);
                        if (!CheckIsInstalled(false))
                        {
                            downloadTask.Installed = false;
                            downloadTask.Installing = false;
                            downloadTask.UnInstalled = true;
                            downloadTask.Error = false;
                            return true;
                        }
                    }

                    downloadTask.Installing = true;

                    int iExitCode = -1;

                    //Run Install Script
                    if (!string.IsNullOrEmpty(SW.PSUninstall))
                    {
                        try
                        {
                            downloadTask.Installing = true;
                            ProgressDetails(this.downloadTask, EventArgs.Empty);

                            var oResult = _RunPS(SW.PSUninstall + ";$ExitCode", "", new TimeSpan(0, 30, 0));

                            try
                            {
                                iExitCode = ((int)oResult.Last().BaseObject);
                            }
                            catch { }

                            //Wait 500ms to let the installer close completely...
                            System.Threading.Thread.Sleep(550);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("PS ERROR: " + ex.Message);
                        }

                        downloadTask.Installing = false;
                        //InstProgress(this, EventArgs.Empty);
                    }

                    //is installed ?
                    if (!CheckIsInstalled(false))
                    {
                        downloadTask.Installed = false;
                        downloadTask.Installing = false;
                        downloadTask.UnInstalled = true;
                        downloadTask.Error = false;
                        //RZRestAPI.Feedback(SW.ProductName, SW.ProductVersion, "true", "RZUpdate", "Uninstalled...");
                        ProgressDetails(downloadTask, EventArgs.Empty);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("WARNING: Product is still installed.");
                        downloadTask.Error = true;
                        downloadTask.ErrorMessage = "WARNING: Product is still installed.";
                        downloadTask.Installed = false;
                        downloadTask.Installing = false;
                        ProgressDetails(downloadTask, EventArgs.Empty);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                    downloadTask.Error = true;
                    downloadTask.ErrorMessage = "WARNING: Product is still installed.";
                    downloadTask.Installed = false;
                    downloadTask.Installing = false;
                    bError = true;
                }

                //RZRestAPI.Feedback(SW.ProductName, SW.ProductVersion, (!bError).ToString(), "RZUpdate", "");
                ProgressDetails(this.downloadTask, EventArgs.Empty);
                return !bError;
            });

            return true;
        }

        public async Task<bool> UnInstall(bool Force = false, bool Retry = false)
        {
            bool msiIsRunning = false;
            bool RZisRunning = false;
            do
            {
                //Check if MSI is running...
                try
                {
                    using (var mutex = Mutex.OpenExisting(@"Global\_MSIExecute"))
                    {
                        msiIsRunning = true;
                        if (Retry)
                        {
                            Console.WriteLine("Warning: Windows-Installer setup is already running!... waiting...");
                            Thread.Sleep(new TimeSpan(0, 0, 10));
                        }
                        else
                            return false;
                    }
                    GC.Collect();
                }
                catch
                {
                    msiIsRunning = false;
                }


                //Check if RuckZuckis running...
                try
                {
                    using (var mutex = Mutex.OpenExisting(@"Global\RuckZuck"))
                    {
                        RZisRunning = true;
                        if (Retry)
                        {
                            Console.WriteLine("Warning: RuckZuck setup is already running!... waiting...");
                            Thread.Sleep(new TimeSpan(0, 0, 10));
                        }
                        else
                            return false;
                    }
                    GC.Collect();
                }
                catch
                {
                    RZisRunning = false;
                }
            }
            while (msiIsRunning || RZisRunning);

            bool bMutexCreated = false;
            bool bResult = false;

            using (Mutex mutex = new Mutex(false, "Global\\RuckZuck", out bMutexCreated))
            {
                bResult = await Task.Run(() => _UnInstall(Force)).ConfigureAwait(false);

                if (bMutexCreated)
                    mutex.Close();
            }
            GC.Collect();
            return bResult;
        }

        /// <summary>
        /// Check if Install-Type is installed
        /// </summary>
        /// <returns>true = installed ; false = not installed</returns>
        public bool CheckIsInstalled(bool sendProgressEvent)
        {
            if (SW != null)
            {

                //Is Product already installed ?
                try
                {
                    //Already installed ?
                    if ((bool)_RunPS(SW.PSDetection).Last().BaseObject)
                    {
                        UILock.EnterReadLock();
                        try
                        {
                            downloadTask.Installed = true;
                            downloadTask.Installing = false;
                            downloadTask.Downloading = false;
                            downloadTask.WaitingForDependency = false;
                            downloadTask.Error = false;
                            downloadTask.ErrorMessage = "";
                            downloadTask.PercentDownloaded = 100;

                            if (sendProgressEvent)
                                ProgressDetails(downloadTask, EventArgs.Empty);

                        }
                        finally { UILock.ExitReadLock(); }
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                downloadTask.Installed = false;
                downloadTask.Installing = false;
                downloadTask.Downloading = false;
            }
            else
            {
                downloadTask.Installed = false;
                downloadTask.Installing = false;
                downloadTask.Downloading = false;
                downloadTask.PercentDownloaded = 0;
            }

            if (sendProgressEvent)
                ProgressDetails(downloadTask, EventArgs.Empty);
            return false;
        }

        /// <summary>
        /// Check if PreReq from Install-Type are compliant (true).
        /// </summary>
        /// <returns>true = compliant; false = noncompliant</returns>
        public bool CheckDTPreReq()
        {
            if (SW != null)
            {

                //Is Product already installed ?
                try
                {
                    if (string.IsNullOrEmpty(SW.PSPreReq))
                        SW.PSPreReq = "$true; ";
                    //Already installed ?
                    if ((bool)_RunPS(SW.PSPreReq).Last().BaseObject)
                    {
                        return true;
                    }
                }
                catch { }
            }

            return false;
        }

        /// <summary>
        /// Download a File
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="FileName"></param>
        /// <returns>true = success; false = error</returns>
        public bool _DownloadFile2(string URL, string FileName)
        {
            //Check if URL is HTTP, otherwise it must be a PowerShell
            if (!URL.StartsWith("http", StringComparison.CurrentCultureIgnoreCase) && !URL.StartsWith("ftp", StringComparison.CurrentCultureIgnoreCase))
            {
                var oResults = _RunPS(URL, FileName, new TimeSpan(2, 0, 0)); //2h timeout
                if (File.Exists(FileName))
                {
                    DLProgress((int)100, EventArgs.Empty);
                    ProgressDetails(new DLStatus() { Filename = FileName, URL = URL, PercentDownloaded = 100, DownloadedBytes = 100, TotalBytes = 100 }, EventArgs.Empty);
                    return true;
                }

                URL = oResults.FirstOrDefault().BaseObject.ToString();
            }

            try
            {
                Stream ResponseStream = null;
                WebResponse Response = null;

                Int64 ContentLength = 1;
                Int64 ContentLoaded = 0;
                Int64 ioldProgress = 0;
                Int64 iProgress = 0;

                if (URL.StartsWith("http"))
                {
                    //_DownloadFile(URL, FileName).Result.ToString();
                    var httpRequest = (HttpWebRequest)WebRequest.Create(URL);
                    httpRequest.UserAgent = "chocolatey command line";
                    httpRequest.AllowAutoRedirect = true;
                    httpRequest.MaximumAutomaticRedirections = 5;
                    httpRequest.GetResponse();


                    // Get back the HTTP response for web server
                    Response = (HttpWebResponse)httpRequest.GetResponse();
                    ResponseStream = Response.GetResponseStream();
                }

                if (URL.StartsWith("ftp"))
                {
                    var ftpRequest = (FtpWebRequest)WebRequest.Create(URL);
                    ftpRequest.ContentLength.ToString();
                    ftpRequest.GetResponse();


                    // Get back the HTTP response for web server
                    Response = (FtpWebResponse)ftpRequest.GetResponse();
                    ResponseStream = Response.GetResponseStream();

                    ContentLength = Response.ContentLength;
                }

                if (ResponseStream == null)
                    return false;

                // Define buffer and buffer size
                int bufferSize = 32768; //4096;
                byte[] buffer = new byte[bufferSize];
                int bytesRead = 0;

                // Read from response and write to file
                FileStream fileStream = File.Create(FileName);
                while ((bytesRead = ResponseStream.Read(buffer, 0, bufferSize)) != 0)
                {
                    if (ContentLength == 1) { Int64.TryParse(Response.Headers.Get("Content-Length"), out ContentLength); }

                    fileStream.Write(buffer, 0, bytesRead);
                    ContentLoaded = ContentLoaded + bytesRead;

                    try
                    {
                        iProgress = (100 * ContentLoaded) / ContentLength;
                        //only send status on percent change
                        if (iProgress != ioldProgress)
                        {
                            if ((iProgress % 10) == 5 || (iProgress % 10) == 0)
                            {
                                try
                                {
                                    DLProgress((int)iProgress, EventArgs.Empty);
                                    ProgressDetails(new DLStatus() { Filename = FileName, URL = URL, PercentDownloaded = Convert.ToInt32(iProgress), DownloadedBytes = ContentLoaded, TotalBytes = ContentLength }, EventArgs.Empty);
                                    ioldProgress = iProgress;
                                }
                                catch { }
                            }
                        }
                    }
                    catch { }
                } // end while

                try
                {
                    if (ioldProgress != 100)
                    {
                        iProgress = (100 * ContentLoaded) / ContentLength;
                        DLProgress((int)iProgress, EventArgs.Empty);
                        ProgressDetails(new DLStatus() { Filename = FileName, URL = URL, PercentDownloaded = Convert.ToInt32(iProgress), DownloadedBytes = ContentLoaded, TotalBytes = ContentLength }, EventArgs.Empty);
                        ioldProgress = iProgress;
                    }
                }
                catch { }

                fileStream.Close();
                ResponseStream.Close();
                Response.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        private static async Task<bool> _DownloadFile(string URL, string FileName)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.AllowAutoRedirect = true;
                handler.MaxAutomaticRedirections = 5;

                //DotNetCore2.0
                //handler.CheckCertificateRevocationList = false;
                //handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }; //To prevent Issue with FW

                using (HttpClient oClient = new HttpClient(handler))
                {
                    oClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "chocolatey command line");

                    using (HttpResponseMessage response = await oClient.GetAsync(URL, HttpCompletionOption.ResponseHeadersRead))
                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                    {
                        string fileToWriteTo = FileName; // Path.GetTempFileName();

                        using (Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create))
                        {
                            await streamToReadFrom.CopyToAsync(streamToWriteTo);
                        }
                        Console.WriteLine("Donwloaded: " + URL);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        private bool _checkFileMd5(string FilePath, string MD5)
        {
            try
            {
                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    using (var stream = File.OpenRead(FilePath))
                    {
                        if (MD5.ToLower() != BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower())
                            return false;
                        else
                            return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private bool _checkFileSHA1(string FilePath, string SHA1)
        {
            try
            {
                using (var sha1 = System.Security.Cryptography.SHA1.Create())
                {
                    using (var stream = File.OpenRead(FilePath))
                    {
                        if (SHA1.ToLower() != BitConverter.ToString(sha1.ComputeHash(stream)).Replace("-", "").ToLower())
                            return false;
                        else
                            return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private bool _checkFileSHA256(string FilePath, string SHA256)
        {
            try
            {
                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    using (var stream = File.OpenRead(FilePath))
                    {
                        if (SHA256.ToLower() != BitConverter.ToString(sha256.ComputeHash(stream)).Replace("-", "").ToLower())
                            return false;
                        else
                            return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private bool _checkFileX509(string FilePath, string X509)
        {
            try
            {
                var Cert = X509Certificate.CreateFromSignedFile(FilePath);
                if (Cert.GetCertHashString().ToLower().Replace(" ", "") == X509.ToLower())
                    return true;
                else
                    return false;

            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Run PowerShell
        /// </summary>
        /// <param name="PSScript">PowerShell Script</param>
        /// <returns></returns>
        public static PSDataCollection<PSObject> _RunPS(string PSScript, string WorkingDir = "", TimeSpan? Timeout = null)
        {
            TimeSpan timeout = new TimeSpan(0, 15, 0); //default timeout = 15min

            if (Timeout != null)
                timeout = (TimeSpan)Timeout;

            DateTime dStart = DateTime.Now;
            TimeSpan dDuration = DateTime.Now - dStart;
            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                if (!string.IsNullOrEmpty(WorkingDir))
                {
                    WorkingDir = Path.GetDirectoryName(WorkingDir);
                    PSScript = "Set-Location -Path '" + WorkingDir + "';" + PSScript;
                }

                PowerShellInstance.AddScript(PSScript);
                PSDataCollection<PSObject> outputCollection = new PSDataCollection<PSObject>();

                outputCollection.DataAdding += ConsoleOutput;
                PowerShellInstance.Streams.Error.DataAdding += ConsoleError;

                IAsyncResult async = PowerShellInstance.BeginInvoke<PSObject, PSObject>(null, outputCollection);
                while (async.IsCompleted == false || dDuration > timeout)
                {
                    Thread.Sleep(200);
                    dDuration = DateTime.Now - dStart;
                }

                return outputCollection;
            }

        }

        private static void ConsoleError(object sender, DataAddingEventArgs e)
        {
            if (e.ItemAdded != null)
                Console.WriteLine("ERROR:" + e.ItemAdded.ToString());
        }

        private static void ConsoleOutput(object sender, DataAddingEventArgs e)
        {
            //if (e.ItemAdded != null)
            //    Console.WriteLine(e.ItemAdded.ToString());
        }

        public string GetDLPath()
        {
            return Environment.ExpandEnvironmentVariables("%TEMP%\\" + SW.ContentID.ToString());
        }
    }
}
