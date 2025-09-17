namespace vasundharabikeracing {
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * statisks menedźeris, kas ieslégs/izslégs bústińus
 */
public class BoostManager : MonoBehaviour
{

    private static GameObject bikeMagnetTrigger;


    //tikai ieslédz bústu, nenodarbojás ar ekonomiku
    public static void ActivateBoost(string boostName, bool showEffect = true)
    {

        if (!BikeDataManager.Boosts.ContainsKey(boostName))
        {
            Debug.LogError("Neveiksmiigs meegjinaajums iesleegt neeksisteejoshu buustu: \"" + boostName + "\"");
            return;
        }
        BikeDataManager.Boosts[boostName].Discovered = true;

        if (BikeDataManager.Boosts[boostName].Active)
        {
            Debug.LogError("Buustinsh \"" + boostName + "\" ir jau ieslégts, dubult" + boostName + " netiek suportéts!");
            return;
        }

        switch (boostName)
        {
            case "magnet": //magnéts tikai palielina objektu, ar ko braucéjs vác lietas, śḱietami palielinás savákśanas rádiuss
                bikeMagnetTrigger = BikeGameManager.player.transform.parent.Find("magnet_trigger").gameObject; //piekeśos trigera objektu, lai átrák varétu noresetot
                if (bikeMagnetTrigger == null)
                {
                    Debug.LogError("Nevar atrast baika \"magnet_trigger\" ");
                    return;
                }
                else
                {
                    bikeMagnetTrigger.GetComponent<CircleCollider2D>().radius = bikeMagnetTrigger.GetComponent<BikeMagnetTrigger>().defaultRadius * 5; // palielinu
                }
                break;


            case "invincibility":
                BikeGameManager.playerState.invincible = true; //ignore crash in game manager
                BikeGameManager.player.layer = 9; //9 = "NEKAS" //baika bodiju un baikeri ieliek citá fizikas slání
                BikeGameManager.player.transform.Find("PlayerCore").Find("Core").gameObject.layer = 9;
                BikeGameManager.player.transform.Find("Player_parts").Find("Core").gameObject.layer = 9;
                BikeGameManager.player.transform.Find("Player_parts").Find("Helmet").gameObject.layer = 9;
                break;


            case "ice":
                //nothing special, bike will freeze water by himself if this boost is active
                break;


            case "fuel":
                //@todo -- turn on boost
                //print("fuel on");
                BikeGameManager.playerControl.accelerationBoostCoef = 1.1f;
                break;



            default:
                Debug.LogError("Pieprasítais bústs \"" + boostName + "\" nav implementéts");
                return;
        }

        if (showEffect)
        {
            EnableBoostEffect(boostName);
        }

        BikeDataManager.Boosts[boostName].Active = true;
        //		DataManager.Boosts[boostName].Selected = true;
        //Debug.Log("Boostinsh: \""+boostName+"\" ieslégts ");

    }


    public static void DeactivateBoost(string boostName)
    {

        switch (boostName)
        {
            case "magnet": //magnéts tikai palielina objektu, ar ko braucéjs vác lietas, śḱietami palielinás savákśanas rádiuss
                //magnet
                if (bikeMagnetTrigger != null)
                {
                    bikeMagnetTrigger.GetComponent<BikeMagnetTrigger>().Reset();
                }
                BikeDataManager.Boosts["magnet"].Active = false;
                break;

            case "invincibility":
                //invicibility
                BikeGameManager.playerState.invincible = false;
                BikeGameManager.player.layer = 0; //0 = "DEFAULT" 
                BikeGameManager.player.transform.Find("PlayerCore").Find("Core").gameObject.layer = 0;
                BikeGameManager.player.transform.Find("Player_parts").Find("Core").gameObject.layer = 0;
                BikeGameManager.player.transform.Find("Player_parts").Find("Helmet").gameObject.layer = 0;
                BikeDataManager.Boosts["invincibility"].Active = false;
                break;

            case "ice":
                //ice
                BikeDataManager.Boosts["ice"].Active = false;
                break;

            case "fuel":
                //@todo -- remove fuel effect
                BikeGameManager.playerControl.accelerationBoostCoef = 1;
                BikeDataManager.Boosts["fuel"].Active = false;
                break;

            default:
                Debug.LogError("Pieprasítais bústs \"" + boostName + "\" nav implementéts");
                return;
        }

        DisableBoostEffect(boostName);

        BikeDataManager.Boosts[boostName].Active = false;
        //Debug.Log("Boostinsh: \""+boostName+"\" ieslégts ");

    }

    public static void ResetBoosts()
    {

        if (BikeGameManager.player == null)
        {
            //Debug.LogError("Nav atrasts baiks!");.
            return;
        }

        //magnet
        if (bikeMagnetTrigger != null)
        {
            bikeMagnetTrigger.GetComponent<BikeMagnetTrigger>().Reset();
        }
        BikeDataManager.Boosts["magnet"].Active = false;
        BikeDataManager.Boosts["magnet"].Selected = false;

        //invicibility
        if (BikeGameManager.playerState != null)
        {
            BikeGameManager.playerState.invincible = false;
        }
        
        if (BikeGameManager.player != null)
        {
            BikeGameManager.player.layer = 0; //0 = "DEFAULT" 
            
            Transform playerCore = BikeGameManager.player.transform.Find("PlayerCore");
            if (playerCore != null)
            {
                Transform core = playerCore.Find("Core");
                if (core != null)
                {
                    core.gameObject.layer = 0;
                }
            }
            
            Transform playerParts = BikeGameManager.player.transform.Find("Player_parts");
            if (playerParts != null)
            {
                Transform core = playerParts.Find("Core");
                if (core != null)
                {
                    core.gameObject.layer = 0;
                }
                
                Transform helmet = playerParts.Find("Helmet");
                if (helmet != null)
                {
                    helmet.gameObject.layer = 0;
                }
            }
        }
        BikeDataManager.Boosts["invincibility"].Active = false;
        BikeDataManager.Boosts["invincibility"].Selected = false;

        //ice
        BikeDataManager.Boosts["ice"].Active = false;
        BikeDataManager.Boosts["ice"].Selected = false;

        //@todo -- remove fuel effect
        if (BikeGameManager.playerControl != null)
        {
            BikeGameManager.playerControl.accelerationBoostCoef = 1;
        }
        else
        {
            Debug.LogWarning("BoostManager: playerControl is null, cannot reset accelerationBoostCoef");
        }
        BikeDataManager.Boosts["fuel"].Active = false;
        BikeDataManager.Boosts["fuel"].Selected = false;

        //aizvádu visus bústu efektu objektus
        GameObject[] fxs = GameObject.FindGameObjectsWithTag("BoostEffect");
        for (int i = 0; i < fxs.Length; i++)
        {
            Destroy(fxs[i]);
        }


    }


    /**
	 * bústińiem iedos efektu - prefabu, kas saucás "Prefabs/Effects/Boost_buustinjaVaards.prefab"   (prefabam vajag tagu "BoostEffect")
	 * ja ir jau ieslégts, neko nedariis
	 */
    public static void EnableBoostEffect(string boostName)
    {

        GameObject[] fxs = GameObject.FindGameObjectsWithTag("BoostEffect");
        for (int i = 0; i < fxs.Length; i++)
        {
            if (fxs[i].name == boostName)
            {
                Destroy(fxs[i]);
                //print("Buustinja " + boostName + " efekts jau ir ieslégts");
                //				return; 
            }
        }

        Debug.Log("<color=yellow>Prefab is loading from = </color>" + boostName);
        GameObject prefab = LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("Boost_" + boostName + "Obj") as GameObject;
        Debug.Log("<color=yellow>Prefab Name = </color>" + prefab);
        //GameObject prefab = Resources.Load("Prefabs/Effects/Boost_"+boostName) as GameObject;
        if (prefab != null)
        {
            GameObject fx = Instantiate(prefab) as GameObject;
            fx.transform.parent = BikeGameManager.magnetTrigger.transform; //palieku zem baika 
            fx.transform.localPosition = new Vector3(0.15f, 0.4f, 0.1f); //nocentré uz baika pozícijas (+ nedaudz nobída pa Z)
            fx.transform.localRotation = Quaternion.identity; //always position the effect relative to bike, not bikes position when activated
            fx.name = boostName;
        }
        else
        {
            //print("Buustinjam " + boostName + " nav efekta prefaba");
        }

    }

    public static void DisableBoostEffect(string boostName)
    {
        GameObject[] fxs = GameObject.FindGameObjectsWithTag("BoostEffect");
        for (int i = 0; i < fxs.Length; i++)
        {
            if (fxs[i].name == boostName)
            {
                Destroy(fxs[i]);
            }
        }
    }

    /**
	 * Tiek izsaukt vairákákás vietás - Upadate() funkcijás 
	 * @note -- prátígi bútu śo taisít ká ko-rútínu, kas vienmér darbojas, bet slinkums pártaisít
	 * 
	 * @return - ir vai nav palielinájies bústinju skaits
	 */
    static int haxSkipFrames = 0;
    public static bool FarmingUpdate()
    {

        if (haxSkipFrames++ < 5)
        { //skipo daźus kadrus
            return false;
        }
        haxSkipFrames = 0;

        bool changed = false;
        BoostRecord boostRecord;

        for (int i = 0; i < BikeDataManager.Boosts.Count; i++)
        {
            boostRecord = BikeDataManager.Boosts[BoostLineupManager.BoostNames[i]];
            if (boostRecord.FarmingTimestamp != DateTime.MinValue)
            {
                TimeSpan diff = DateTime.Now.Subtract(boostRecord.FarmingTimestamp);
                if (TimeSpan.Compare(diff, boostRecord.FarmingDuration) >= 0)
                {
                    boostRecord.Number += boostRecord.NumberPerFarming;
                    boostRecord.FarmingTimestamp = DateTime.MinValue;
                    changed = true;
                    BikeDataManager.ShowGarageButtonNotification = true;

                    AchievementManager.AchievementProgress("boost_earned", 1);

                    if (UIManager.currentScreenType != GameScreenType.Garage)
                    { //ja ir garáźas ekráná - tátad redz savus bústinjńus - tad nelikt zinju
                        NewsListManager.Push(Lang.Get("News:BoostReady"), NewsListItemType.boost, GameScreenType.Garage, "Upgrade", "Career");
                    }

                    TelemetryManager.EventBoostEnd(i, true); // ID,  waited/paid = true/false

                }
            }
        }
        return changed;
    }

    public static void UpdateTimeSpans()
    {

        int upgradeLevel;
        System.TimeSpan timeSpan;
        foreach (var upgradeKeyValuePair in BikeDataManager.Upgrades)
        {
            UpgradeRecord upgrade = upgradeKeyValuePair.Value;
            //TODO should be executed on every level up
            if (upgrade.Availability == UpgradeAvailabilityType.SingleplayerOnly)
            {

                upgradeLevel = BikeDataManager.Bikes[BikeDataManager.SingleplayerPlayerBikeRecordName].Upgrades[upgradeKeyValuePair.Key];// getting the total upgrades, but we don't really have any temp upgrades for boosts
                timeSpan = new System.TimeSpan(0, (int)upgrade.Values[upgradeLevel], 0);

                switch ((UpgradeType)upgradeKeyValuePair.Key)
                {
                    case UpgradeType.Ice:
                        BikeDataManager.Boosts["ice"].FarmingDuration = timeSpan;
                        break;
                    case UpgradeType.Magnet:
                        BikeDataManager.Boosts["magnet"].FarmingDuration = timeSpan;
                        break;
                    case UpgradeType.Immortality:
                        BikeDataManager.Boosts["invincibility"].FarmingDuration = timeSpan;
                        break;
                    case UpgradeType.Fuel:
                        BikeDataManager.Boosts["fuel"].FarmingDuration = timeSpan;
                        break;
                    default:
                        break;
                }
            }
        }

    }


}

}
