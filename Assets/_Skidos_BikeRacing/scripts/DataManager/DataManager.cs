namespace vasundharabikeracing {
using UnityEngine;
//using System; - big no no
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using Data_MainProject;
using CielaSpike; //ninja threads
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using System.IO;
using System.Globalization;



/**
 * statiska klase, kas saturu visus appas datus un prot tos seivot un ieládét 
 * (ieládéśana notiek tikai appas sákumá, seivośana jáveic katru reizi, kad kaut kas izmainits)
 */



public class BikeDataManager : MonoBehaviour
{

    public static string GameDataFilePath = Application.persistentDataPath + "/" + "gamedata_BikeRace.moto";


    /**
	 * @note - visi turpmáie publiskie mainígie ir jáieliek arí:
	 * 		   Save() 
	 * 		   Load()
	 * 		   SetDefaults()
	 *		  metodés
	 */

    public static OrderedList_BikeRace<string, LevelRecord> Levels = new OrderedList_BikeRace<string, LevelRecord>(); // LevelName(filename) => levelRecord
    public static OrderedList_BikeRace<string, AchievementRecord> Achievements = new OrderedList_BikeRace<string, AchievementRecord>(); // AchievementCodeName => AchievementRecord 
    public static OrderedList_BikeRace<string, BikeRecord> Bikes = new OrderedList_BikeRace<string, BikeRecord>(); // BikePrefabName => BikeRecord 
    public static OrderedList_BikeRace<int, PlayerXPLevelRecord> PlayerXPLevels = new OrderedList_BikeRace<int, PlayerXPLevelRecord>(); // level(int) => XPLevelRecord 
    public static OrderedList_BikeRace<int, PlayerMultiplayerLevelRecord> PlayerMultiplayerLevels = new OrderedList_BikeRace<int, PlayerMultiplayerLevelRecord>(); // level(int) => MultiplayerLevelRecord 
    public static OrderedList_BikeRace<string, BoostRecord> Boosts = new OrderedList_BikeRace<string, BoostRecord>(); // boost_name => BoostRecord
    public static OrderedList_BikeRace<int, PresetRecord> Presets = new OrderedList_BikeRace<int, PresetRecord>(); // preset_id => PresetRecord
    public static OrderedList_BikeRace<int, UpgradeRecord> Upgrades = new OrderedList_BikeRace<int, UpgradeRecord>(); // 
    public static OrderedList_BikeRace<int, StyleRecord> Styles = new OrderedList_BikeRace<int, StyleRecord>(); // 
    public static OrderedList_BikeRace<int, LevelGiftRecord> LevelGifts = new OrderedList_BikeRace<int, LevelGiftRecord>(); // 

    public static int Stars; //cik spélétájam pieder
    public static int StarsTotal; //cik spélé nopelnámas
    public static int StarsToUnlockNextWorld; //cik spélé nopelnámas

    private static int _coins; // pietiesá COINS vértíbá
    public static int Coins
    { //interfess, kas progresés achívmentu
        get { return _coins; }
        set
        {
            _coins = value;
            AchievementManager.AchievementProgress("money", PickupManager.CoinsCollected);
            AchievementManager.AchievementProgress("money__2", PickupManager.CoinsCollected);
            AchievementManager.AchievementProgress("money__3", PickupManager.CoinsCollected);
        }
    }
    public static int CoinsWOAchievement
    { //interfeiss, kas neprogresés achívmentu - pérkot koinpakas un ieládéjot datus
        get { return _coins; }
        set { _coins = value; }
    }


    public static int PlayerXP;
    public static int PlayerXPLevel;

    public static bool PaidLevelUnlock = false; // vai ir nopircis iespéju braukt visus límenjus
    public static bool HasBoughtAnything = false; //jebko ir nopircis par ístu naudu

    public static bool SettingsSPGhost = true;
    public static bool SettingsMusic = false;
    public static bool SettingsSfx = true;
    public static bool SettingsHD = true;
    public static bool SettingsAccelerometer = false;

    public static bool FirstProduce = true;
    public static bool FirstUpgrade = true;
    public static bool FirstFinishNow = true;
    public static bool FirstStyle = false; // will be set to true before showing the promo in PostGameBehaviour
    public static bool FirstLong = true;
    public static bool FirstClaim = true;
    //    public static bool FirstInvite = true;
    public static bool FirstFriends = true;

    public static bool CoinsGifted = false;
    //    public static bool StyleGifted = false;
    public static bool FastestBikeShowed = false;

    public static bool ShowedMultiplayerFreeCoins = false;

    public static bool ShowedWoFPointers = false;

    public static bool MultiplayerUnlockedPopupShowed = false;
    public static bool MultiplayerUnlockedPopupShowed2 = false;
    public static bool MultiplayerUnlockedPopupShowed3 = false;
    public static bool MultiplayerUnlockedPopupShowed4 = false;

    public static bool FirstStore = true;

    public static bool FreeCoinsReceived = false;
    public static bool MultiplayerFreeCoinsReceived = false;

    public static int MultiplayerFriendsPopupShowCount = 0;
    public static int UpgradeSupersalePromoShowCount = 0;

    public static int GarageShowCount = 0;

    public static int GiftStyleIndex = 3;
    //    public static int[] GiftStyleIndices = new int[]{1,5,3,4,2,6,9};//TODO will be deprecated by LevelGift structure
    public static bool ShowGiftStyle = false;

    public static bool HasRatedUs = false;
    public static int RateUsShowCount = 0;

    public static string Country = "";

    //    public static System.DateTime LastActiveTimestamp;
    //    public static System.DateTime LastSpinTimestamp; //gets set when the wheel has finished spinnign

    /*****END**seivojamie**parametri***********/

    public static int[] CheckpointCoinBonus = new int[] { 50, 100, 150, 200, 250, 500, 750, 1000, 1500, 2000 };

    public static int CoinsInLastLevel;

    public static string SingleplayerPlayerBikeRecordName = "Regular"; // izvélétais baiks: prefaba nosaukums
    public static string MultiplayerPlayerBikeRecordName = "MPRegular"; // izvélétais baiks: prefaba nosaukums
    public static string SingleplayerGhostBikeRecordName = "SPGhost"; // izvélétais baiks: prefaba nosaukums

    public const float UNITS_TO_METERS = 0.75f; //nebija citur, kur likt :P 

    public static string RawInput = "";

    private static bool SavingInProgress = false;

    public static bool FirstTimer = true; // ja izdodas ieládét gamedata.moto ,tad uzskatu, ka nav pirmo reizi spélé

    public static int MaxMultiplayerLevelReached = 0; //0-N, 

    public static bool WatchedAdToGetAnExtraReplay = false;

    /**
     * misc. prices w/o better place to be defined in
     */
    public static int priceUnlockAll = 50000;//all levels

    public static int PriceUnlockAll
    {
        get
        {
            int totalPrice = priceUnlockAll;
            if (Levels.Count > 0)
            {
                int firstLevelOfWorld = (LevelLineupManager.CountCompletedRegularLevels(Levels) / 10) * 10;
                float percentCompleted = (float)firstLevelOfWorld / LevelLineupManager.regularLevelCount; //normalized
                totalPrice = Mathf.RoundToInt(priceUnlockAll * (1 - percentCompleted));
            }
            return totalPrice;
        }
    }

    public static bool MultiplayerUnlocked
    {
        get
        {
            return ((Levels.ContainsKey("a___014") && Levels["a___014"].Tried) || BikeDataManager.PaidLevelUnlock || BikeGameManager.developmentMode); //multiplayer gets unlocked at level 10
        }
    }

    public static bool ShowGarageButtonNotification = false;
    public static bool ShowMultiplayerButtonNotification = false;

    public static int CountUnclaimedAchievements()
    {
        return AchievementLineupManager.CountUnclaimedAchievements(BikeDataManager.Achievements);
    }

    public static int GetLevelGiftID(string levelName)
    {
        return LevelGiftLineupManager.GetLevelGiftID(levelName, LevelGifts);
    }

    /**
	 * ieládé no JSONa
	 * párbauda cheksummu - ja nepareiza - visam nokluséjuma vértíbas
	 *  neesośiem parametriem nokluséjuma vértíbas
	 */
    public static void Init()
    {

#if UNITY_EDITOR
        LevelLineupManager.CreateLevelListCSV(); //spélé tiks liets pédéjais izveidotais CSV fails
#endif



        LoadData();
        SetupTemporaryBoostExpirationCheck();
    }


    public static void LoadData(bool evenUnnecessaryThings = false)
    {
        SetDefaults();
        //Flush(); // <--- atkomentét, lai vienmér reseto!

        try
        {
            LoadJSON();
        }
        catch (System.Exception e)
        {
            Debug.LogError("DataManager nespeej ielaadeed JSONu; meegjini izdzeest appu vai gamedata.moto failu - tas paliidz 99% gadiijumu!\ne:" + e);
        }

        if (Debug.isDebugBuild)
        {
            if (Coins < 500)
            {
                //CoinsWOAchievement += 5000000;
                print("here, have some free money | DEV only");
            }
        }

        if (Debug.isDebugBuild)
        {
            //SettingsMusic = false; //i hate music
        }

        if (evenUnnecessaryThings)
        {
            BoostManager.UpdateTimeSpans();//śeit vajadzígs péc bekapa ieládés, parastos gad pietiek, ka tas tiek izsaukts garáźas inicializéśaná
        }

    }

    private static void SetDefaults()
    {
        Stars = 0;
        _coins = 0;
        StarsTotal = 0;

        PlayerXP = 0;
        PlayerXPLevel = 0;
        PaidLevelUnlock = false;
        HasBoughtAnything = false;

        MultiplayerManager.NumWins = 0;
        MultiplayerManager.NumGames = 0;

        SettingsSPGhost = true;
        SettingsMusic = false;
        SettingsSfx = true;
        SettingsHD = true;
        SettingsAccelerometer = false;

        Upgrades = UpgradeLineupManager.DefineUpgrades();
        Styles = StyleLineupManager.DefineStyles();//must be defined before Bikes
        Presets = PresetLineupManager.DefinePresets();
        Levels = LevelLineupManager.DefineLevels();
        Achievements = AchievementLineupManager.DefineAchievements();
        Bikes = BikeLineupManager.DefineBikes();
        PlayerXPLevels = PlayerXPLevelLineupManager.DefinePlayerXPLevels();
        PlayerMultiplayerLevels = PlayerMultiplayerLevelLineupManager.DefinePlayerMultiplayerLevels();
        Boosts = BoostLineupManager.DefineBoosts();
        LevelGifts = LevelGiftLineupManager.DefineLevelGifts();
        Country = "";


        SpinManager.DefineSpins();
    }

    /**
	 * pieseivo visus parametrus JSONá, 
	 * jáapréḱina un jápieseivo cheksumma
	 * 
	 * śo izsauc iznícinot spéli
	 * 
	 * @note -- Ar Lielo Burtu JSONá Tiek Glabáti Mainígie
	 * 			ar mazo burtu JSONá tiek glabáti parametri, kas nav mainígie árpus JSONa
	 * 
	 * @todo -- zipot JSONu, ja sanáks failińś tuvosies megabaitam
	 * 
	 * @todo -- neseivot, ja nekas nav mainíjies
	 */
    public static void Flush()
    {

        if (SavingInProgress)
        {
            return;  //lai 2 vienlaicígi neraksta failá
        }
        SavingInProgress = true;

#if UNITY_IOS
        GameObject cam = null; // iosá lietotais IL2CPP nesuporté ninjathreads un asinhronás korutínas
#else
		GameObject cam = null;////////////////////////////////////////test//////////////////GameObject.Find("Main Camera");
#endif

        if (cam == null)
        { //spéles aizvérśanas brídí kamera vairs nav atrodama - nebus bús seivośana ko-rutíná
            _Flush();
        }
        else
        {
            cam.GetComponent<MonoBehaviour>().StartCoroutineAsync(_FlushAsync()); // bús seivośana korutíná (+ citá pavediená )
        }
        PlayerPrefs.Save();


    }

    //asinhroná ko-rutína, izmanto ninja threadus
    private static IEnumerator _FlushAsync()
    {
        _Flush();
        yield break;
    }


    //seivotájfunkcija, ko var izsaukt asinhroni, [citá pavediená], gan sinhroni
    private static void _Flush()
    {
        JSONClass J = new JSONClass();

        //datu struktúras
        foreach (KeyValuePair<string, LevelRecord> level in Levels)
        {
            JSONClass L = new JSONClass();
            L["fileName"] = level.Key;
            L["BestStars"].AsInt = level.Value.BestStars;
            L["BestTime"].AsFloat = level.Value.BestTime;
            L["BestCoins"].AsInt = level.Value.BestCoins;
            L["BestCheckpoints"].AsInt = level.Value.BestCheckpoints;
            L["DiamondEggCollected"].AsInt = level.Value.DiamondEggCollected ? 1 : 0;
            L["BoostIceCrateCollected"].AsInt = level.Value.BoostIceCrateCollected ? 1 : 0;
            L["BoostMagnetCrateCollected"].AsInt = level.Value.BoostMagnetCrateCollected ? 1 : 0;
            L["BoostInvincibilityCrateCollected"].AsInt = level.Value.BoostInvincibilityCrateCollected ? 1 : 0;
            L["BoostFuelCrateCollected"].AsInt = level.Value.BoostFuelCrateCollected ? 1 : 0;
            L["CoinCrateCollected"].AsInt = level.Value.CoinCrateCollected ? 1 : 0;
            L["GhostBikePrefab"] = level.Value.GhostBikePrefab;
            L["Shared"].AsInt = level.Value.Shared ? 1 : 0;
            L["Tried"].AsInt = level.Value.Tried ? 1 : 0;

            J["info"]["levels"][-1] = L;
        }

        foreach (KeyValuePair<string, AchievementRecord> achi in Achievements)
        {
            JSONClass A = new JSONClass();
            A["codename"] = achi.Key;
            A["Progress"].AsFloat = achi.Value.Progress; //achiivementiem seivo tikai progresu - péréjais nák no nokluséjuma vértíbám
            A["Claimed"].AsBool = achi.Value.Claimed; //achiivementiem seivo tikai progresu - péréjais nák no nokluséjuma vértíbám
            J["info"]["achievements"][-1] = A;
        }

        foreach (KeyValuePair<string, BikeRecord> bike in Bikes)
        {

            if (bike.Key == SingleplayerPlayerBikeRecordName ||
                bike.Key == MultiplayerPlayerBikeRecordName)
            {

                JSONClass B = new JSONClass();
                B["prefabName"] = bike.Key;
                B["StyleID"].AsInt = bike.Value.StyleID;

                int i = 0;
                foreach (var riderPresets in bike.Value.StyleGroupPresetIDs)
                {
                    JSONClass R = new JSONClass();
                    foreach (var groupPreset in riderPresets)
                    {
                        R["GroupPresetIDs"][groupPreset.Key].AsInt = groupPreset.Value;  //string->int
                    }
                    B["Style"]["" + i] = R;
                    i++;
                }

                //                foreach(KeyValuePair<int, int> upgrade in bike.Value.Upgrades){
                foreach (KeyValuePair<int, int> upgrade in bike.Value.UpgradesPerm)
                { //only save the permanent upgrades of the player bikes
                    B["Upgrades"]["" + upgrade.Key].AsInt = upgrade.Value;  //int->int
                }

                J["info"]["bikes"][-1] = B;
            }
        }

        foreach (KeyValuePair<int, StyleRecord> style in Styles)
        {
            JSONClass S = new JSONClass();
            S["codename"].AsInt = style.Key;
            S["Locked"].AsBool = style.Value.Locked;
            J["info"]["styles"][-1] = S;
        }

        foreach (KeyValuePair<int, LevelGiftRecord> gift in LevelGifts)
        {
            JSONClass G = new JSONClass();
            G["codename"].AsInt = gift.Key;
            G["Gifted"].AsBool = gift.Value.Gifted;
            J["info"]["levelGifts"][-1] = G;
        }

        foreach (KeyValuePair<string, BoostRecord> boo in Boosts)
        {
            JSONClass B = new JSONClass();
            B["codename"] = boo.Key;
            B["Number"].AsInt = boo.Value.Number;
            B["Discovered"].AsBool = boo.Value.Discovered;
            B["FarmingTimestamp"] = boo.Value.FarmingTimestamp.ToString();
            J["info"]["boosts"][-1] = B;
        }

        //vienkárśie mainígie
        J["info"]["Stars"].AsInt = Stars;
        J["info"]["Coins"].AsInt = _coins;

        J["info"]["PlayerXP"].AsInt = PlayerXP;
        J["info"]["PlayerXPLevel"].AsInt = PlayerXPLevel;
        J["info"]["PaidLevelUnlock"].AsBool = PaidLevelUnlock;
        J["info"]["HasBoughtAnything"].AsBool = HasBoughtAnything;


        J["info"]["SettingsSPGhost"].AsInt = SettingsSPGhost ? 1 : 0;
        J["info"]["SettingsMusic"].AsInt = SettingsMusic ? 1 : 0;
        J["info"]["SettingsSfx"].AsInt = SettingsSfx ? 1 : 0;
        J["info"]["SettingsHD"].AsInt = SettingsHD ? 1 : 0;
        J["info"]["SettingsAccelerometer"].AsInt = SettingsAccelerometer ? 1 : 0;

        J["info"]["FirstProduce"].AsBool = FirstProduce;
        J["info"]["FirstUpgrade"].AsBool = FirstUpgrade;
        J["info"]["FirstFinishNow"].AsBool = FirstFinishNow;
        J["info"]["FirstStyle"].AsBool = FirstStyle;
        J["info"]["FirstLong"].AsBool = FirstLong;
        J["info"]["FirstClaim"].AsBool = FirstClaim;
        J["info"]["FirstFriends"].AsBool = FirstFriends;
        J["info"]["FirstStore"].AsBool = FirstStore;

        J["info"]["MPUnlockedPopupShowed"].AsBool = MultiplayerUnlockedPopupShowed;
        J["info"]["MPUnlockedPopupShowed2"].AsBool = MultiplayerUnlockedPopupShowed2;
        J["info"]["MPUnlockedPopupShowed3"].AsBool = MultiplayerUnlockedPopupShowed3;
        J["info"]["MPUnlockedPopupShowed4"].AsBool = MultiplayerUnlockedPopupShowed4;

        J["info"]["CoinsGifted"].AsBool = CoinsGifted;
        J["info"]["FreeCoinsReceived"].AsBool = FreeCoinsReceived;
        J["info"]["MultiplayerFreeCoinsReceived"].AsBool = MultiplayerFreeCoinsReceived;
        J["info"]["FastestBikeShowed"].AsBool = FastestBikeShowed;
        J["info"]["ShowedMultiplayerFreeCoins"].AsBool = ShowedMultiplayerFreeCoins;
        J["info"]["ShowedWoFPointers"].AsBool = ShowedWoFPointers;

        J["info"]["HasRatedUs"].AsBool = HasRatedUs;
        J["info"]["RateUsShowCount"].AsInt = RateUsShowCount;
        J["info"]["MaxMultiplayerLevelReached"].AsInt = MaxMultiplayerLevelReached;
        J["info"]["MPFriendsPopupShowCount"].AsInt = MultiplayerFriendsPopupShowCount;
        J["info"]["UpgradeSupersalePromoShowCount"].AsInt = UpgradeSupersalePromoShowCount;
        J["info"]["GarageShowCount"].AsInt = GarageShowCount;
        // J["info"]["FacebookShareCount"].AsInt = ShareManager.FacebookShareCount;
        // J["info"]["TransactionsRestored"].AsBool = StoreManager.TransactionsAlreadyRestored;


        try
        {//es nezinu vai te ir probléma
            J["info"]["country"] = Country;
        }
        catch (System.Exception e)
        {
            if (Debug.isDebugBuild) { Debug.Log("exception:" + e.Message); }
            J["info"]["country"] = "";
        }



        //méǵina seivot
        string jString;
        //try {
        jString = J.ToString();
        //} catch(System.Exception e) {

        //Debug.LogError("JSONs nomira meegjinot saglabaat speeleetaajdatus; noresetojaam speeleetaajdatus" + e.Message);
        //SavingInProgress = false;
        //jString = "";//likvidéju slikto JSONu, nákamreiz ieládéjot tiks noresetots!
        //}

        DataBackupManager.Backup(jString);
        System.IO.File.WriteAllText(GameDataFilePath, jString);
        SavingInProgress = false;
        Debug.Log("DataManager::Flush");
    }




    public static void ResetAllPlayerInfo()
    {
        print("ResetAllPlayerInfo");
        SetDefaults();
        PlayerPrefs.DeleteAll();
        Flush();
        LoadJSON();
    }





    private static void LoadJSON()
    {

        //print(GameDataFilePath);
        if (!System.IO.File.Exists(GameDataFilePath))
        {
            System.IO.File.WriteAllText(GameDataFilePath, ""); //jáizveido tukśs JSON fails, ja táds neeksisté
            if (Debug.isDebugBuild)
            {
                print("Nav gamedata.moto faila, nelasam!");
            }
            return;
        }

        string jsontext = System.IO.File.ReadAllText(GameDataFilePath);
        if (jsontext.Length == 0)
        {
            if (Debug.isDebugBuild)
            {
                print("gamedata.moto satur neriktiigu JSONu!");
            }
            return;
        }

        JSONNode N = JSON.Parse(jsontext);

        //ja viss ir kártíbá, tad párraksa nokluséjuma vértíbas ar JSONá esośajám
        for (int i = 0; i < N["info"]["levels"].AsArray.Count; i++)
        {
            string fileName = N["info"]["levels"][i]["fileName"];
            LevelRecord l;
            if (Levels.TryGetValue(fileName, out l))
            {
                l.BestStars = N["info"]["levels"][i]["BestStars"].AsInt;
                l.BestTime = N["info"]["levels"][i]["BestTime"].AsFloat;
                l.BestCoins = N["info"]["levels"][i]["BestCoins"].AsInt;
                l.BestCheckpoints = N["info"]["levels"][i]["BestCheckpoints"].AsInt;
                l.DiamondEggCollected = N["info"]["levels"][i]["DiamondEggCollected"].AsInt == 1 ? true : false;
                l.BoostIceCrateCollected = N["info"]["levels"][i]["BoostIceCrateCollected"].AsInt == 1 ? true : false;
                l.BoostMagnetCrateCollected = N["info"]["levels"][i]["BoostMagnetCrateCollected"].AsInt == 1 ? true : false;
                l.BoostInvincibilityCrateCollected = N["info"]["levels"][i]["BoostInvincibilityCrateCollected"].AsInt == 1 ? true : false;
                l.BoostFuelCrateCollected = N["info"]["levels"][i]["BoostFuelCrateCollected"].AsInt == 1 ? true : false;
                l.CoinCrateCollected = (N["info"]["levels"][i]["CoinCrateCollected"] != null && N["info"]["levels"][i]["CoinCrateCollected"].AsInt == 1) ? true : false;
                l.GhostBikePrefab = N["info"]["levels"][i]["GhostBikePrefab"];
                l.Shared = N["info"]["levels"][i]["Shared"].AsInt == 1 ? true : false;
                l.Tried = N["info"]["levels"][i]["Tried"].AsInt == 1 ? true : false;
            }
            else
            {
                if (Debug.isDebugBuild)
                {
                    Debug.LogWarning("Lietotaajiestatiijumos atrasta info par vecu liimeni: " + fileName + ", skipojam");
                }
                continue;
            }

        }

        for (int i = 0; i < N["info"]["achievements"].AsArray.Count; i++)
        {
            string codeName = N["info"]["achievements"][i]["codename"];
            Achievements[codeName].Progress = N["info"]["achievements"][i]["Progress"].AsFloat;
            Achievements[codeName].Claimed = N["info"]["achievements"][i]["Claimed"].AsBool;

            if (Achievements[codeName].Progress >= Achievements[codeName].Target)
            { //izpíldítajiem achiivmentiem pieraksta, ka tie ir izpildíti
                Achievements[codeName].Done = true;
            }
        }

        //baiki
        //only need to save the player bikes here, all data for ghost bikes will be loaded from files
        for (int i = 0; i < N["info"]["bikes"].AsArray.Count; i++)
        {
            string prefabName = N["info"]["bikes"][i]["prefabName"];

            BikeRecord b;
            if (Bikes.TryGetValue(prefabName, out b))
            {
                if (N["info"]["bikes"][i]["Style"] != null)
                {
                    JSONClass NStyle = (JSONClass)N["info"]["bikes"][i]["Style"];
                    //print(NStyle.Count);
                    for (int j = 0; j < NStyle.Count; j++)
                    {
                        if (NStyle[j]["GroupPresetIDs"] != null)
                        {
                            foreach (KeyValuePair<string, JSONNode> groupPreset in (JSONClass)NStyle[j]["GroupPresetIDs"])
                            { //já-taipkásto no JsonNode uz JsonClass, lai varétu lieto foreach :/
                                b.StyleGroupPresetIDs[j][groupPreset.Key] = groupPreset.Value.AsInt;
                            }
                        }
                    }
                }

                b.StyleID = N["info"]["bikes"][i]["StyleID"].AsInt;

                if (N["info"]["bikes"][i]["Upgrades"].Value != null)
                {
                    foreach (KeyValuePair<string, JSONNode> upgrade in (JSONClass)N["info"]["bikes"][i]["Upgrades"])
                    { //já-taipkásto no JsonNode uz JsonClass, lai varétu lieto foreach :/
                      //                        b.Upgrades[System.Convert.ToInt32(upgrade.Key)] = upgrade.Value.AsInt;
                        b.UpgradesSet(System.Convert.ToInt32(upgrade.Key), upgrade.Value.AsInt); // set the perm upgrades
                    }
                }
                //b.param1 = pagaidám nav citu parametru, ko ieládét no JSONa 

            }
            else
            {
                if (Debug.isDebugBuild)
                {
                    Debug.LogWarning("Lietotaajiestatiijumos atrasta info par vecu baiku: " + prefabName + ", skipojam");
                }
                continue;
            }

        }


        for (int i = 0; i < N["info"]["styles"].AsArray.Count; i++)
        {
            int codeName = N["info"]["styles"][i]["codename"].AsInt;
            if (Styles.ContainsKey(codeName))
            {
                Styles[codeName].Locked = N["info"]["styles"][i]["Locked"].AsBool;
            }
        }


        for (int i = 0; i < N["info"]["boosts"].AsArray.Count; i++)
        {
            string codeName = N["info"]["boosts"][i]["codename"];
            if (Boosts.ContainsKey(codeName))
            {
                Boosts[codeName].Number = N["info"]["boosts"][i]["Number"].AsInt;
                Boosts[codeName].Discovered = N["info"]["boosts"][i]["Discovered"].AsBool;
                if (!System.DateTime.TryParse(N["info"]["boosts"][i]["FarmingTimestamp"], CultureInfo.InvariantCulture, DateTimeStyles.None, out Boosts[codeName].FarmingTimestamp))
                { //tryparse, baby, lai tiktu galá ar NULL vértíbám
                    Boosts[codeName].FarmingTimestamp = new System.DateTime();
                }

            }
        }


        for (int i = 0; i < N["info"]["levelGifts"].AsArray.Count; i++)
        {
            int codeName = N["info"]["levelGifts"][i]["codename"].AsInt;
            if (LevelGifts.ContainsKey(codeName))
            {
                LevelGifts[codeName].Gifted = N["info"]["levelGifts"][i]["Gifted"].AsBool;
            }
        }


        Stars = N["info"]["Stars"].AsInt;
        _coins = N["info"]["Coins"].AsInt;

        PlayerXP = N["info"]["PlayerXP"].AsInt;
        PlayerXPLevel = N["info"]["PlayerXPLevel"].AsInt;
        PaidLevelUnlock = N["info"]["PaidLevelUnlock"].AsBool;
        HasBoughtAnything = N["info"]["HasBoughtAnything"].AsBool;


        SettingsSPGhost = N["info"]["SettingsSPGhost"].AsInt == 1 ? true : false;
        SettingsMusic = N["info"]["SettingsMusic"].AsInt == 1 ? true : false;
        SettingsSfx = N["info"]["SettingsSfx"].AsInt == 1 ? true : false;
        SettingsHD = N["info"]["SettingsHD"].AsInt == 1 ? true : false;
        SettingsAccelerometer = (N["info"]["SettingsAccelerometer"] != null) ? (N["info"]["SettingsAccelerometer"].AsInt == 1 ? true : false) : false;

        FirstProduce = N["info"]["FirstProduce"].AsBool;
        FirstUpgrade = N["info"]["FirstUpgrade"].AsBool;
        FirstFinishNow = N["info"]["FirstFinishNow"].AsBool;
        FirstStyle = N["info"]["FirstStyle"].AsBool;
        FirstLong = (N["info"]["FirstLong"] != null) ? N["info"]["FirstLong"].AsBool : true; //backwards compatible
        FirstClaim = (N["info"]["FirstClaim"] != null) ? N["info"]["FirstClaim"].AsBool : true; //backwards compatible
        FirstFriends = (N["info"]["FirstFriends"] != null) ? N["info"]["FirstFriends"].AsBool : true; //backwards compatible
        FirstStore = (N["info"]["FirstStore"] != null) ? N["info"]["FirstStore"].AsBool : true; //backwards compatible

        MultiplayerUnlockedPopupShowed = (N["info"]["MPUnlockedPopupShowed"] != null) ? N["info"]["MPUnlockedPopupShowed"].AsBool : false;
        MultiplayerUnlockedPopupShowed2 = (N["info"]["MPUnlockedPopupShowed2"] != null) ? N["info"]["MPUnlockedPopupShowed2"].AsBool : false;
        MultiplayerUnlockedPopupShowed3 = (N["info"]["MPUnlockedPopupShowed3"] != null) ? N["info"]["MPUnlockedPopupShowed3"].AsBool : false;
        MultiplayerUnlockedPopupShowed4 = (N["info"]["MPUnlockedPopupShowed4"] != null) ? N["info"]["MPUnlockedPopupShowed4"].AsBool : false;

        CoinsGifted = (N["info"]["CoinsGifted"] != null) ? N["info"]["CoinsGifted"].AsBool : false;
        FreeCoinsReceived = (N["info"]["FreeCoinsReceived"] != null) ? N["info"]["FreeCoinsReceived"].AsBool : false;
        MultiplayerFreeCoinsReceived = (N["info"]["MultiplayerFreeCoinsReceived"] != null) ? N["info"]["MultiplayerFreeCoinsReceived"].AsBool : false;
        FastestBikeShowed = (N["info"]["FastestBikeShowed"] != null) ? N["info"]["FastestBikeShowed"].AsBool : false;
        ShowedMultiplayerFreeCoins = (N["info"]["ShowedMultiplayerFreeCoins"] != null) ? N["info"]["ShowedMultiplayerFreeCoins"].AsBool : false;
        ShowedWoFPointers = (N["info"]["ShowedWoFPointers"] != null) ? N["info"]["ShowedWoFPointers"].AsBool : false;

        HasRatedUs = N["info"]["HasRatedUs"].AsBool;
        RateUsShowCount = (N["info"]["RateUsShowCount"] != null) ? N["info"]["RateUsShowCount"].AsInt : 0;
        MaxMultiplayerLevelReached = (N["info"]["MaxMultiplayerLevelReached"] != null) ? N["info"]["MaxMultiplayerLevelReached"].AsInt : 0;
        MultiplayerFriendsPopupShowCount = (N["info"]["MPFriendsPopupShowCount"] != null) ? N["info"]["MPFriendsPopupShowCount"].AsInt : 0;
        UpgradeSupersalePromoShowCount = (N["info"]["UpgradeSupersalePromoShowCount"] != null) ? N["info"]["UpgradeSupersalePromoShowCount"].AsInt : 0;
        GarageShowCount = (N["info"]["GarageShowCount"] != null) ? N["info"]["GarageShowCount"].AsInt : 0;
        // ShareManager.FacebookShareCount = (N["info"]["FacebookShareCount"] != null) ? N["info"]["FacebookShareCount"].AsInt : 0;
        // StoreManager.TransactionsAlreadyRestored = (N["info"]["TransactionsRestored"] != null) ? N["info"]["TransactionsRestored"].AsBool : false;

        try
        {//es nezinu vai te ir probléma, bet daźi súdzás
            Country = (N["info"]["country"] != null) ? (string)N["info"]["country"] : "";
        }
        catch (System.Exception e)
        {
            Country = "";
            if (Debug.isDebugBuild) { Debug.Log("exception:" + e.Message); }
        }


        FirstTimer = false;
    }


    /**
	 * atgrieź cik unikálas SP trases ir izbrauktas
	 */
    public static int GetNumSP(bool includeSkippedOnes = false)
    {
        int n = 0;
        foreach (KeyValuePair<string, LevelRecord> l in Levels)
        {
            if (l.Value.BestCoins > 0 || (includeSkippedOnes && l.Value.Tried))
            {
                n++;
            }
        }
        return n;
    }

    //--------------------//
    public static void SetPlayerRecordInputJSON(JSONClass inputJSON, bool singlePlayer = true)
    {
        string bikeRecordName = singlePlayer ? SingleplayerPlayerBikeRecordName : MultiplayerPlayerBikeRecordName;
        Bikes[bikeRecordName].InputJSONClass = inputJSON;
    }

    public static JSONClass GetGhostRecordInputJSON(string ghostRecordName = "SPGhost")
    {
        string bikeRecordName = ghostRecordName;
        return Bikes[bikeRecordName].InputJSONClass;
    }
    private static string GetGhostPath(string fileName, bool singlePlayer = true)
    {
        string folder = singlePlayer ? "/sp-rides/" : "/mp-cache/";
        string path = Application.persistentDataPath + folder + fileName + ".json";
        return path;
    }

    private static void CreateDefaultGhostRecord(string ghostRecordName)
    {
        if (ghostRecordName.Contains("MPGhost"))
        {
            Bikes[ghostRecordName] = new BikeRecord(new int[] {
                (int)UpgradeType.Acceleration,
                (int)UpgradeType.AccelerationStart,
                (int)UpgradeType.AdditionalRestarts,
                (int)UpgradeType.BreakSpeed,
                (int)UpgradeType.MaxSpeed
            }, "RegularMP");
        }
        else
        {
            Bikes[ghostRecordName] = new BikeRecord(new int[] { 0 }, ghostRecordName == "SPGhost" ? "Regular" : "RegularMP");
        }
    }
    public static bool LoadLevelGhost(string levelName, string ghostRecordName = "SPGhost")
    {
        string path = GetGhostPath(levelName, ghostRecordName == SingleplayerGhostBikeRecordName);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"❌ Ghost JSON not found at {path}, creating default ghost.");
            try
            {
                CreateDefaultGhostRecord(ghostRecordName);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to create default ghost record: {e.Message}");
            }
            return false;
        }

        string json = File.ReadAllText(path);
        JSONNode data = JSON.Parse(json);
        Bikes[ghostRecordName].LoadJSONNode(data);
        return true;
    }

    public static void SaveLevelGhost(string levelName, bool singlePlayer = true)
    {
        string bikeRecordName = singlePlayer ? SingleplayerPlayerBikeRecordName : MultiplayerPlayerBikeRecordName;
        JSONClass data = Bikes[bikeRecordName].MakeJSONClass();
        string json = data.ToString();

        string path = GetGhostPath(levelName, singlePlayer);
        File.WriteAllText(path, json);
        Debug.Log($"✅ Saved ghost JSON at {path}");
    }

    public static string AssemblePath(string fileName, bool singlePlayer = true)
    {

        string path = "";
        path += Application.persistentDataPath;
        path += singlePlayer ? "/sp-rides/" : "/mp-cache/";
        path += fileName; //LevelManager.CurrentLevelName;
        path += ".bytes";

        return path;
    }

    /// <summary>
    /// Increments the player XP by specified amount.
    /// </summary>
    /// <returns><c>true</c>, if player leveled up, <c>false</c> otherwise.</returns>
    /// <param name="amount">Amount.</param>
    public static bool IncrementPlayerXP(int amount)
    {

        bool leveledUp = false;

        PlayerXP += amount;
        int nextLevelIndex = PlayerXPLevel + 1;

        if (nextLevelIndex < PlayerXPLevels.Count && //within range
            PlayerXP >= PlayerXPLevels[nextLevelIndex].XP) //and reached required xp amount
        {
            PlayerXPLevel++;//increment level
            leveledUp = true;
        }

        return leveledUp;
    }

    //----------------------------------//
    //----------------------------------//
    //----------------------------------//


    public static byte[] LastZippedContent;


    public static void ZipStringToFile(string data, string outputFilePath, bool writeFile = true, string fileExtension = ".bytes")
    {
        if (!writeFile)
        {
            Debug.LogWarning("⚠️ Skipping write due to writeFile = false");
            return;
        }

        string finalPath = Path.ChangeExtension(outputFilePath, fileExtension);
        File.WriteAllText(finalPath, data);
        Debug.Log("✅ Saved JSON to " + finalPath);
    }
      

      public static string UnzipFileToString(string inputFilePath)
    {
        // Now reads plain JSON instead of decompressing
        if (!File.Exists(inputFilePath))
        {
            Debug.LogWarning("❌ File not found at: " + inputFilePath);
            return null;
        }

        return File.ReadAllText(inputFilePath);
    }



    public static System.DateTime ConvertFromUnixTimestamp(double timestamp)
    {
        System.DateTime origin = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
        return origin.AddSeconds(timestamp);
    }

    public static double ConvertToUnixTimestamp(System.DateTime date)
    {
        System.DateTime origin = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
        System.TimeSpan diff = date.ToUniversalTime() - origin;
        return System.Math.Floor(diff.TotalSeconds);
    }
    //--------------------------------



    const string POWER_BOOST_TIMESTAMP_KEY = "PowerBoostTimestamp";
    public const float POWER_BOOST_EXPIRTION_TIME = 30;// minutes

    public static System.DateTime PowerBoostTimestamp;
    public static bool PowerBoostEnabled = false;

    public static int actualPowerIncreaseAmount;

    public static void GiveTemporaryPowerBoost()
    {
        var upgradesPerm = Bikes[MultiplayerPlayerBikeRecordName].UpgradesPerm;

        int accelerationIndex = (int)UpgradeType.Acceleration;
        int accelerationStartIndex = (int)UpgradeType.AccelerationStart;
        int maxSpeedIndex = (int)UpgradeType.MaxSpeed;
        int breakSpeedIndex = (int)UpgradeType.BreakSpeed;

        int desiredPowerIncreaseAmount = 100;

        var powerIncreaseResult = PowerRatingManager.IncreasePowerEvenlyBy(
            desiredPowerIncreaseAmount,
            upgradesPerm[accelerationIndex],
            upgradesPerm[accelerationStartIndex],
            upgradesPerm[maxSpeedIndex],
            upgradesPerm[breakSpeedIndex]
            );

        actualPowerIncreaseAmount = desiredPowerIncreaseAmount + powerIncreaseResult[0];

        Bikes[MultiplayerPlayerBikeRecordName].UpgradesTempSet(accelerationIndex, powerIncreaseResult[1] - upgradesPerm[accelerationIndex]);
        Bikes[MultiplayerPlayerBikeRecordName].UpgradesTempSet(accelerationStartIndex, powerIncreaseResult[2] - upgradesPerm[accelerationStartIndex]);
        Bikes[MultiplayerPlayerBikeRecordName].UpgradesTempSet(maxSpeedIndex, powerIncreaseResult[3] - upgradesPerm[maxSpeedIndex]);
        Bikes[MultiplayerPlayerBikeRecordName].UpgradesTempSet(breakSpeedIndex, powerIncreaseResult[4] - upgradesPerm[breakSpeedIndex]);

        PowerBoostEnabled = true;

        bool powerBoostExpired = true;
        if (PowerBoostTimestamp != System.DateTime.MinValue)
        {
            powerBoostExpired = System.DateTime.Now.Subtract(PowerBoostTimestamp).TotalMinutes > POWER_BOOST_EXPIRTION_TIME; // if timestamp is younger than 30 minutes it hasn't expired yet
        }

        //if there isn't a timestamp younger than 30 min, set timestamp to now
        if (powerBoostExpired)
        {
            PowerBoostTimestamp = System.DateTime.Now;
        }

        //TODO save the timestamp && powerboost state
        PlayerPrefs.SetString(POWER_BOOST_TIMESTAMP_KEY, PowerBoostTimestamp.ToString());
        PlayerPrefs.Save();
    }

    public static void RecalulateTemporaryPowerBoost()
    {
        //        print("---------RecalulateTemporaryPowerBoost");
        var upgradesPerm = Bikes[MultiplayerPlayerBikeRecordName].UpgradesPerm;

        int accelerationIndex = (int)UpgradeType.Acceleration;
        int accelerationStartIndex = (int)UpgradeType.AccelerationStart;
        int maxSpeedIndex = (int)UpgradeType.MaxSpeed;
        int breakSpeedIndex = (int)UpgradeType.BreakSpeed;

        int desiredPowerIncreaseAmount = 100;

        var powerIncreaseResult = PowerRatingManager.IncreasePowerEvenlyBy(
            desiredPowerIncreaseAmount,
            upgradesPerm[accelerationIndex],
            upgradesPerm[accelerationStartIndex],
            upgradesPerm[maxSpeedIndex],
            upgradesPerm[breakSpeedIndex]
            );

        actualPowerIncreaseAmount = desiredPowerIncreaseAmount + powerIncreaseResult[0];

        Bikes[MultiplayerPlayerBikeRecordName].UpgradesTempSet(accelerationIndex, powerIncreaseResult[1] - upgradesPerm[accelerationIndex]);
        Bikes[MultiplayerPlayerBikeRecordName].UpgradesTempSet(accelerationStartIndex, powerIncreaseResult[2] - upgradesPerm[accelerationStartIndex]);
        Bikes[MultiplayerPlayerBikeRecordName].UpgradesTempSet(maxSpeedIndex, powerIncreaseResult[3] - upgradesPerm[maxSpeedIndex]);
        Bikes[MultiplayerPlayerBikeRecordName].UpgradesTempSet(breakSpeedIndex, powerIncreaseResult[4] - upgradesPerm[breakSpeedIndex]);

    }

    public static void SetupTemporaryBoostExpirationCheck()
    {

        //        print("SetupTemporaryBoostExpirationCheck");
        //        print("PowerBoostTimestamp == null " +(PowerBoostTimestamp == null) + " " + PowerBoostTimestamp.ToString());
        if (PowerBoostTimestamp == System.DateTime.MinValue)  //DONE power boost gets reset every launch
        {
            //DONE try to get the timestamp

            System.DateTime timestampDate = System.DateTime.MinValue;
            bool validTimestamp = false;

            if (PlayerPrefs.HasKey(POWER_BOOST_TIMESTAMP_KEY))
            {
                validTimestamp = System.DateTime.TryParseExact(PlayerPrefs.GetString(POWER_BOOST_TIMESTAMP_KEY), "G", CultureInfo.InvariantCulture, DateTimeStyles.None, out timestampDate);
            }

            if (validTimestamp)
                PowerBoostTimestamp = timestampDate;
        }

        //        print("SetupTemporaryBoostExpirationCheck");
        //        print("PowerBoostTimestamp != null " +(PowerBoostTimestamp != null));
        if (PowerBoostTimestamp != System.DateTime.MinValue)
        {
            //if successfully retreived the timestamp
            bool powerBoostExpired = System.DateTime.Now.Subtract(PowerBoostTimestamp).TotalMinutes > POWER_BOOST_EXPIRTION_TIME;

            //            print("powerBoostExpired " + powerBoostExpired);

            if (!powerBoostExpired)
            {

                //                print("PowerBoostEnabled " + PowerBoostEnabled);

                if (!PowerBoostEnabled)
                {
                    GiveTemporaryPowerBoost(); // enable power boost if it isn't yet
                }

                //                print("Launching coroutine");

                GameObject cam = GameObject.Find("Main Camera");
                cam.GetComponent<MonoBehaviour>().StartCoroutine(CheckIfTemporaryPowerBoostExpired());

            }
            else if (PowerBoostEnabled)
            {

                RemoveTemporaryPowerBoost();
            }
        }
        else if (PowerBoostEnabled)
        {
            RemoveTemporaryPowerBoost(); // if there's no timestamp but boost is marked as enabled
        }
    }

    static void RemoveTemporaryPowerBoost()
    {
        //        print("RemoveTemporaryPowerBoost");

        int accelerationIndex = (int)UpgradeType.Acceleration;
        int accelerationStartIndex = (int)UpgradeType.AccelerationStart;
        int maxSpeedIndex = (int)UpgradeType.MaxSpeed;
        int breakSpeedIndex = (int)UpgradeType.BreakSpeed;

        Bikes[MultiplayerPlayerBikeRecordName].UpgradesTempSet(accelerationIndex, 0);
        Bikes[MultiplayerPlayerBikeRecordName].UpgradesTempSet(accelerationStartIndex, 0);
        Bikes[MultiplayerPlayerBikeRecordName].UpgradesTempSet(maxSpeedIndex, 0);
        Bikes[MultiplayerPlayerBikeRecordName].UpgradesTempSet(breakSpeedIndex, 0);

        PowerBoostEnabled = false;
    }

    public static IEnumerator CheckIfTemporaryPowerBoostExpired()
    {

        if (PowerBoostTimestamp != System.DateTime.MinValue)
        {
            bool powerBoostExpired = System.DateTime.Now.Subtract(PowerBoostTimestamp).TotalMinutes > POWER_BOOST_EXPIRTION_TIME;
            //            print("time" + System.DateTime.Now.Subtract(PowerBoostTimestamp).TotalMinutes.ToString());

            while (!powerBoostExpired)
            {

                powerBoostExpired = System.DateTime.Now.Subtract(PowerBoostTimestamp).TotalMinutes > POWER_BOOST_EXPIRTION_TIME;

                yield return null;//new WaitForSeconds(0.1f); //WaitForSeconds is not executed when timescale is 0
            }

            RemoveTemporaryPowerBoost();
        }

        //        print("CheckIfTemporaryPowerBoostExpired");
    }
}




}
