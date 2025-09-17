namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.IO;
using Data_MainProject;

/**
 * automátiski uzzin visus pieejamos límeńus no iepriekśizveidota (veidojas tikai redaktorá) CSV faila
 * pagaidám HelperMenedźeris - galvená datu struktúra "Levels" ir lietojama caur DataManager 
 */
public class LevelLineupManager : MonoBehaviour
{

    //#if UNITY_EDITOR
    /**
	 * ja spéle tiek palaista redaktorá, tad tiks izveidots CSV fails ar visiem resursdirá esośajiem límeńim (neatbalsta subdirektorijas :P)
	 * śo CSV vérs vaĺá ieríces, kur nav iespéjams apskatít resursdiras saturu
	 */
    public static void CreateLevelListCSV()
    {
        //DirectoryInfo levelDirectoryPath = new DirectoryInfo(Application.dataPath + "/Resources/Levels");
        //FileInfo[] fileInfo = levelDirectoryPath.GetFiles("*.*", SearchOption.AllDirectories);
        //string namesCSV = "";
        //for (int i = 0; i < fileInfo.Length; i++)
        //{
        //    if (!fileInfo[i].Name.Contains(".bytes"))
        //    { //nav límeńfails, skipojam
        //        continue;
        //    }
        //    if (fileInfo[i].Name.Contains(".bytes.meta"))
        //    { //ir meta, jáskipo
        //        continue;
        //    }
        //    if (fileInfo[i].Name.Contains(".json"))
        //    { //debug JSON, arí jáskipo
        //        continue;
        //    }
        //    namesCSV += fileInfo[i].Name.Replace(".bytes", "") + "\n"; // pliku límeńa nosakumu pievieno CSV stringa galá
        //}

        //System.IO.File.WriteAllText(Application.dataPath + "/Resources/Levels/superlevellist.csv", namesCSV); //lévellisti noliek blakus paśiem límeńiem
    }
    //#endif


    /**
	 * atgriezís sakártotu Dictu ar visiem CSV atrastasto límeńiem nosaukumiem
	 */
    public static OrderedList_BikeRace<string, LevelRecord> DefineLevels()
    {

        OrderedList_BikeRace<string, LevelRecord> Levels = new OrderedList_BikeRace<string, LevelRecord>();

        string csvLevelList = "";
#if UNITY_EDITOR
        //csvLevelList = System.IO.File.ReadAllText(Application.dataPath + "/Resources/Levels/superlevellist.csv"); //redaktorá veru vaĺá failu ar NORMÁLO metodi, kas nekeśo (unity ríks ignoré faila izmaińas, kamér redaktorá nav pabakstíts atveramais fails)

        TextAsset csvAsset = (TextAsset)LoadAddressable_Vasundhara.Instance.GetTextAsset_Resources("superlevellist");
        Debug.Log("<color=yellow>Text File Loaded Text = " + csvAsset.ToString());
        if (csvAsset != null)
        {
            csvLevelList = csvAsset.ToString();
        }
        else
        {
            Debug.Log("Nav atrodams iepriekshizveidotais liimenju CSV");
        }

#else
		TextAsset csvAsset = (TextAsset)LoadAddressable_Vasundhara.Instance.GetTextAsset_Resources("superlevellist");
        Debug.Log("<color=yellow>Text File Loaded Text = " + csvAsset.ToString());
        if (csvAsset != null)
        {
            csvLevelList = csvAsset.ToString();
        }
        else
        {
            Debug.Log("Nav atrodams iepriekshizveidotais liimenju CSV");
        }
#endif

        BikeDataManager.StarsTotal = 0;
        string[] names = csvLevelList.Split('\n');
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i].Length > 0)
            {
                Levels[names[i]] = new LevelRecord();

                //izskaitís cik zvaigznes kopá var savákt (tikai parastajos/bonusa límeńos )
                string ln = names[i].ToLower();
                if (!ln.Contains("long") && !ln.Contains("mp") && !ln.Contains("new_"))
                {
                    BikeDataManager.StarsTotal += 3;
                }

                if (!ln.Contains("long") &&
                   !ln.Contains("bonuss") &&
                   !ln.Contains("a_w") &&
                   !ln.Contains("new_") &&
                   !ln.Contains("mp"))
                { //finished this level successfully
                    regularLevelCount++;
                }
            }
        }

        return Levels;

    }

    public static int regularLevelCount = 0;

    //this is for the case where player didn't buy all levels
    public static int CountCompletedRegularLevels(OrderedList_BikeRace<string, LevelRecord> Levels)
    {
        //unlocked level count = successfully finished level count + 1

        int completedLevelCount = 0;
        foreach (var level in Levels)
        {
            string levelName = level.Key.ToLower();
            if (!levelName.Contains("long") &&
               !levelName.Contains("bonuss") &&
               !levelName.Contains("new_") &&
               !levelName.Contains("mp") &&
               level.Value.BestTime > 0)
            { //finished this level successfully, cause who needs stars to advance
              //               level.Value.BestStars > 0) { //finished this level successfully
                completedLevelCount++;
            }
        }

        return completedLevelCount;
    }
}


/**
 * JSONá saglabájama datu struktúra
 */
public class LevelRecord
{
    public LevelRecord()
    {
        BestStars = 0;
        BestCoins = 0;
        BestTime = 0;
        BestCheckpoints = 0;
        DiamondEggCollected = false;
        BoostIceCrateCollected = false;
        BoostMagnetCrateCollected = false;
        BoostInvincibilityCrateCollected = false;
        BoostBombplusCrateCollected = false;
        BoostBombminusCrateCollected = false;
        BoostFuelCrateCollected = false;
        CoinCrateCollected = false;
        BulletTimeUsed = false;
        GhostBikePrefab = "Regular";
        Shared = false;
        Tried = false;
    }

    public int BestStars; //cik zvaigznes ir noplenítas śajá límení (ja 0, tad nav izbraukts)
    public float BestTime; //labákais laiks śajá límení
    public int BestCoins; //max monétu daudzums, kas nopelníts
    public int BestCheckpoints; //checkpoints reached
    public bool DiamondEggCollected; // vai ir savákta śajá límení pasléptá dimanta ola
    public bool BoostIceCrateCollected; //vai límeni ir savákta kastíte ar konkrétu bústińu
    public bool BoostMagnetCrateCollected;
    public bool BoostInvincibilityCrateCollected;
    public bool BoostBombplusCrateCollected;
    public bool BoostBombminusCrateCollected;
    public bool BoostFuelCrateCollected;
    public bool CoinCrateCollected;
    public bool BulletTimeUsed;
    public string GhostBikePrefab; //ar kádu baiku tika iebraukts SP ghostińś
    public bool Shared; //vai ir śérojis, lia nopelnítu śo límeni (aktuáls tikai BONUSA límenjiem)
    public bool Tried; //has the player tried playing this level
}

}
