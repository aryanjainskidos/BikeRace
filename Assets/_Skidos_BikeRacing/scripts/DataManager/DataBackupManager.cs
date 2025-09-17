namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using System.IO;


/*
 * sútís sazipotu gamedata.moto uz serveri, lai tas pieglabá, lídz rodas nepiecieśamíba lokálo failu nomainít pret severí esośo
 */
public class DataBackupManager : MonoBehaviour
{


    static public bool BackupAwailable = false;
    static public bool HasCheckedForBackup = false; //tika 1x sesijá skatísies vai vajag prasít serverim, vai tam ir kas labs
    static public System.DateTime BackupTime;
    static public int BackupLevels;

    static string URL = MultiplayerManager.ServerUrlMP + "/proc/gamedatamoto_backup.php";
    static System.DateTime lastBackupTime;

    static string unq
    {
        get
        {
#if UNITY_IOS
            string u = "";//UnityEngine.iOS.Device.advertisingIdentifier; //uz iOSa deviceUniqueIdentifier mainás katru reizi izdzéśot appu, tápéc jálieto śis, ja ir pieejams
            if (u == null || u.Length == 0)
            {
                return SystemInfo.deviceUniqueIdentifier;
            }
            else
            {
                return u;
            }
#else
			return SystemInfo.deviceUniqueIdentifier;
#endif
        }
    }




    //śo izsauc startups
    public static void Init()
    {


        DownloadBackupIfNeeded();
    }

    //śo izsauc katru reizi seivojot 
    public static void Backup(string gamedatamotoContent)
    {


        if (lastBackupTime == System.DateTime.MinValue || lastBackupTime < System.DateTime.Now - System.TimeSpan.FromMinutes(3))
        {
            //print("DataBackupManager::laiks bekapot");
            int numSP = BikeDataManager.GetNumSP(true);//cik unikáli SP límeńi izbraukti, ieskaitot bonusus-n-shit			

            if (numSP >= 3)
            {//nebekapojam, ja nav vismaz 3 límeńi izbraukti
             //print("DataBackupManager::jaabekapo: ir 3+ liimenji ");
                GameObject.Find("Main Camera").GetComponent<MonoBehaviour>().StartCoroutine(_backup(gamedatamotoContent, numSP));
                lastBackupTime = System.DateTime.Now;
            }
            else
            {
                //print("DataBackupManager::nav jaabekapo: nav 3+ liimenji ");
            }

        }
        else
        {
            //print("DataBackupManager::nav laiks bekapot");
        }
    }




    public static void DownloadBackupIfNeeded()
    {

        //izdzéś iepriekśéjo bekapfailu, if any
        System.IO.File.Delete(BikeDataManager.GameDataFilePath + "_backup");


        int numSP = BikeDataManager.GetNumSP(true);//cik unikáli SP límeńi izbraukti, ieskaitot bonusus-n-shit
                                                   //print("DataBackupManager::GetNumSP="+numSP);
        if (HasCheckedForBackup)
        { //tikai 1x sesijá (rait?)
          //print("DataBackupManager::hasCheckedForBackup skipojam");
        }
        else
        {

            if (numSP == 0)
            { //tikai, ja lokáli nav neviena izbraukta límeńa (rait?)
                print("DataBackupManager::numSP==0 prasam bekapu");
                GameObject.Find("Main Camera").GetComponent<MonoBehaviour>().StartCoroutine(_downloadBackupIfNeeded());

                //} else {
                //print("DataBackupManager::numSP<>0 skipojam");
            }
        }

    }

    //
    public static void Restore()
    {
        if (BackupAwailable)
        {


            try
            {
                if (!System.IO.File.Exists(BikeDataManager.GameDataFilePath))
                {
                    System.IO.File.WriteAllText(BikeDataManager.GameDataFilePath, "xxx");//vienreiz iOS nokárás méǵinot repleisot [man śḱiet] neeksistéjośu failu, better safe than sorry
                }
                System.IO.File.Replace(BikeDataManager.GameDataFilePath + "_backup", BikeDataManager.GameDataFilePath, BikeDataManager.GameDataFilePath + "_");
                BackupAwailable = false;
            }
            catch (System.Exception e)
            {
                if (Debug.isDebugBuild) { Debug.Log("DataBackupManager::Restore::Exc=" + e.Message); }
            }

        }
    }


    private static IEnumerator _backup(string gamedatamotoContent, int numSP)
    {

        BikeDataManager.ZipStringToFile(gamedatamotoContent, "", false); //sazipos bet neseivos failá




        WWWForm data = new WWWForm();
        
        // Check if LastZippedContent is not null before adding to form
        if (BikeDataManager.LastZippedContent != null && BikeDataManager.LastZippedContent.Length > 0)
        {
            data.AddBinaryData("MotoFile", BikeDataManager.LastZippedContent); //sazipotais strings
        }
        else
        {
            Debug.LogError("DataBackupManager: LastZippedContent is null or empty, cannot backup data");
            yield break;
        }
        
        data.AddField("UID", unq);
        data.AddField("Levels", numSP);

        WWW www = new WWW(URL, data);
        yield return www;

        //print("DataBackupManager::resp:\n"+www.text);
    }


    private static IEnumerator _downloadBackupIfNeeded()
    {

        WWWForm data = new WWWForm();
        data.AddField("UID", unq);
        WWW www = new WWW(URL, data);
        yield return www;

        HasCheckedForBackup = true;

        if (www.error == null)
        {

            //foreach(var h in www.responseHeaders){
            //	print(h.Key + " => " + h.Value);
            //}

            if (www.bytes.Length > 100)
            {

                try
                {
                    using (MemoryStream mem = new MemoryStream(www.bytes)) //servera atgrieztos baitus (kas attélo saspiesto JSonu - gamedata.moto) atspiedís un ieliks failá
                    using (ZipInputStream zipStream = new ZipInputStream(mem))
                    {
                        ZipEntry currentEntry;
                        while ((currentEntry = zipStream.GetNextEntry()) != null)
                        {
                            byte[] bdata = new byte[currentEntry.Size];
                            zipStream.Read(bdata, 0, bdata.Length);
                            string jsontext = System.Text.Encoding.UTF8.GetString(bdata);
                            if (jsontext.Length > 100)
                            {
                                System.IO.File.WriteAllText(BikeDataManager.GameDataFilePath + "_backup", jsontext);
                                //print("jsontext=\n"+jsontext);
                                BackupAwailable = true;
                                //print("DataBackupManager::success");
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    if (Debug.isDebugBuild) { Debug.Log("exc:" + ex.Message); }
                }

                string backupTimestamp;
                if (www.responseHeaders.TryGetValue("HTTP_X_BACKUP_TIMESTAMP", out backupTimestamp))
                {
                    //print("DataBackupManager::backupTimestamp="+backupTimestamp );
                    BackupTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    BackupTime = BackupTime.AddSeconds(int.Parse(backupTimestamp, System.Globalization.CultureInfo.InvariantCulture)).ToLocalTime();
                    //print("DataBackupManager::BackupTime="+BackupTime);
                }

                string backupLevels;
                if (www.responseHeaders.TryGetValue("HTTP_X_BACKUP_LEVELS", out backupLevels))
                {
                    //print("DataBackupManager::backupLevels="+backupLevels);
                    BackupLevels = int.Parse(backupLevels, System.Globalization.CultureInfo.InvariantCulture);
                    //print("DataBackupManager::BackupLevels="+BackupLevels);
                }




            }
            else
            {
                //print("DataBackupManager::failure");
            }


        }
        else
        {
            //print("DataBackupManager::err:"+www.error);
        }

    }


}

}
