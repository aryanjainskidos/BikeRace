namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SimpleJSON;


/**
 * pieder LevelContainer objektam
 * tiek seivots JSONá kopá ar katru límeni
 */
public class LevelInfo : MonoBehaviour, ISaveable
{

    public LevelDataContainer levelData;

    //seivojamie parametri (śie parametri ir manuáli jápievieno Save() un Load() metodés śajá klasé!)
    public float TimePar = 0; //katram límenim jánoráda cik átri tas ir jáizbrauc, lai nopelnítu zvaigzníti
    public int CoinPar = 100; //.. un cik monétińas jásavác
    public float TimePar2 = 0;
    public int CoinPar2 = 100;
    public float TimePar3 = 0;
    public int CoinPar3 = 100;

    //izskaita un pieraksta statistikai 
    public int CoinsInLevel;
    public int Coins2InLevel;

    public int CheckpointsInLevel;

    //turpmákos tikai izskaita, pat nepieraksta
    public int DiamondEggs;
    public int BoostMagnetCrates;
    public int BoostInvincibilityCrates;
    public int BoostIceCrates;
    public int BoostFuelCrates;
    public int CoinCrates;

    public string LevelName; //śo neseivo śajá skriptá - ieládéjot límeni, nosaukums tiks iedots no faila várda

    //kuri bústińi ir ieteicami śajá límení, seivos  
    public bool SuggBoostIce;
    public bool SuggBoostMagnet;
    public bool SuggBoostInvincibility;
    public bool SuggBoostFuel;

    //
    public int StarsToUnlock = 0;
    public int AllowedRestartCount = -1;

    public string type;

    /**
	 * ievác info par límeni - izsauc, kad límenis tiek saglabáts vai ieládéts (tikai redaktorá, protams)
	 */
    private void GatherInfo()
    {

        CoinsInLevel = 0;
        Coins2InLevel = 0;
        DiamondEggs = 0;
        BoostMagnetCrates = 0;
        BoostInvincibilityCrates = 0;
        BoostIceCrates = 0;
        BoostFuelCrates = 0;
        CoinCrates = 0;

        CheckpointsInLevel = 0;

        /**
		 * ies cauri visiem geimobjektiem no entities sláńa - 
		 * a) lai izsakitítu monétas
		 * b) lai noskaidrotu via ir kádi jauni geimobjekti, kas vél netiek skriptá uzskaitíti
		 */
        List<GameObject> entities = FindGameObjectsInLayer(8);
        for (int i = 0; i < entities.Count; i++)
        {

            switch (entities[i].name)
            {

                //saskaitísim śos:
                case "Coin":
                    CoinsInLevel++;
                    break;
                case "CoinX2":
                    Coins2InLevel++;
                    break;
                case "DiamondEgg":
                    DiamondEggs++;
                    break;

                case "Boost_magnet_crate":
                    BoostMagnetCrates++;
                    break;
                case "Boost_invincibility_crate":
                    BoostInvincibilityCrates++;
                    break;
                case "Boost_ice_crate":
                    BoostIceCrates++;
                    break;
                case "Boost_fuel_crate":
                    BoostFuelCrates++;
                    break;
                case "CoinCrate":
                    CoinCrates++;
                    break;

                case "CheckpointZone":
                    CheckpointsInLevel++;
                    break;

                //ignorésim śos:
                case "FinishZone":
                    break;
                case "StartZone":
                    break;
                case "StuntZone":
                    break;
                case "DeathZone":
                    break;
                case "WindZone":
                    break;
                case "SoundZone":
                    break;
                case "SpikeZone":
                    break;
                case "BirdZone":
                    break;
                case "Puddle":
                    break;
                case "PileOfLeaves":
                    break;
                case "entity_trigger"://mocim piederośa detaĺa
                    break;
                case "magnet_trigger"://arí mocim piederośa detaĺ
                    break;
                case "Boost_magnet"://uzreiz aktivizéjamie bústińi		
                    break;
                case "Boost_invincibility":
                    break;
                case "Boost_ice":
                    break;
                case "Boost_fuel":
                    break;
                case "UnzoomBoundsExtender":
                    break;

                default:
                    if (Debug.isDebugBuild)
                    {
                        Debug.LogWarning("Lieto jaunu \"Entities\" tipu: \"" + entities[i].name + "\", pirms tam nepastāstot par to savam draugam, LevelInfo skriptam ?");
                    }
                    break;


            }

        }


    }


    public JSONClass Save()
    {

        GatherInfo(); //pirms seivośanas ievákt info par límeni

        var J = new JSONClass();

        J["TimePar"].AsFloat = TimePar;
        J["CoinPar"].AsInt = CoinPar;

        J["TimePar2"].AsFloat = TimePar2;
        J["CoinPar2"].AsInt = CoinPar2;

        J["TimePar3"].AsFloat = TimePar3;
        J["CoinPar3"].AsInt = CoinPar3;

        J["CoinsInLevel"].AsInt = CoinsInLevel;
        J["Coins2InLevel"].AsInt = Coins2InLevel;

        J["CheckpointsInLevel"].AsInt = CheckpointsInLevel;

        J["SuggBoostIce"].AsBool = SuggBoostIce;
        J["SuggBoostMagnet"].AsBool = SuggBoostMagnet;
        J["SuggBoostInvincibility"].AsBool = SuggBoostInvincibility;
        J["SuggBoostFuel"].AsBool = SuggBoostFuel;
        J["AllowedRestartCount"].AsInt = AllowedRestartCount;

        //save star count needed to unlock level
        if (levelData != null && levelData.levelDataEntries != null)
        {
            if (StarsToUnlock > 0)
            {
                bool contains = false;
                foreach (var item in levelData.levelDataEntries)
                {
                    if (item.name == LevelName)
                    {
                        item.starsToUnlock = StarsToUnlock;
                        contains = true;
                    }
                }

                if (!contains)
                {
                    LevelDataEntry lde = new LevelDataEntry();
                    lde.name = LevelName;
                    lde.starsToUnlock = StarsToUnlock;
                    levelData.levelDataEntries.Add(lde);
                }
            }
            else
            {//remove if set to zero
                List<LevelDataEntry> removeList = new List<LevelDataEntry>();
                foreach (var item in levelData.levelDataEntries)
                {
                    if (item.name == LevelName)
                    {
                        removeList.Add(item);
                    }
                }

                foreach (var item in removeList)
                {
                    levelData.levelDataEntries.Remove(item);
                }
            }

            levelData.levelDataEntries.Sort(LevelDataEntry.Compare);
        }

        return J;
    }

    public void Load(JSONNode N)
    {
        TimePar = N["TimePar"].AsFloat;
        CoinPar = N["CoinPar"].AsInt;
        TimePar2 = N["TimePar2"].AsFloat;
        CoinPar2 = N["CoinPar2"].AsInt;
        TimePar3 = N["TimePar3"].AsFloat;
        CoinPar3 = N["CoinPar3"].AsInt;

        CoinsInLevel = N["CoinsInLevel"].AsInt;
        Coins2InLevel = N["Coins2InLevel"].AsInt;

        CheckpointsInLevel = N["CheckpointsInLevel"].AsInt;

        SuggBoostIce = N["SuggBoostIce"].AsBool;
        SuggBoostMagnet = N["SuggBoostMagnet"].AsBool;
        SuggBoostInvincibility = N["SuggBoostInvincibility"].AsBool;
        SuggBoostFuel = N["SuggBoostFuel"].AsBool;
        AllowedRestartCount = (N["AllowedRestartCount"].Value != "" ? N["AllowedRestartCount"].AsInt : -1);

        // Try to load from LoadAddressable_Vasundhara first, fallback to Resources
        if (LoadAddressable_Vasundhara.Instance != null)
        {
            levelData = LoadAddressable_Vasundhara.Instance.GetScriptableObjLevelData_Resources("LevelData");
        }
        
        // Fallback to Resources if LoadAddressable_Vasundhara failed or returned null
        if (levelData == null)
        {
            levelData = Resources.Load<LevelDataContainer>("Data/LevelData");
        }
        
        // If still null, create a default LevelDataContainer to prevent errors
        if (levelData == null)
        {
            Debug.LogWarning("LevelData not found in LoadAddressable_Vasundhara or Resources. Creating default LevelDataContainer.");
            levelData = ScriptableObject.CreateInstance<LevelDataContainer>();
            levelData.levelDataEntries = new System.Collections.Generic.List<LevelDataEntry>();
        }
        
        Debug.Log("<color=green>LevelData Loaded Successfully: </color>" + (levelData != null ? levelData.name : "null"));

        if (levelData != null && levelData.levelDataEntries != null)
        {
            foreach (var item in levelData.levelDataEntries)
            {
                if (item.name == LevelName)
                {
                    StarsToUnlock = item.starsToUnlock;
                }
            }
        }

        Match match = Regex.Match(LevelName.ToLower(), "(bonus|long)");
        type = (match.Success ? match.Value : "regular");

#if UNITY_EDITOR
        GatherInfo();
#endif

    }


    //atrodi visus scénas geimobjektus śajá slání (sláńi tikai cipariski)
    private List<GameObject> FindGameObjectsInLayer(int layer)
    {

        GameObject[] GOs = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < GOs.Length; i++)
        {
            if (GOs[i].layer == layer)
            {
                list.Add(GOs[i]);
            }
        }

        return list;
    }
}

}
