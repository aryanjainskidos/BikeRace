namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * límení vác Coins un citas lietas
 */
public class PickupManager : MonoBehaviour
{


    public static int CoinsCollected = 0;
    public static Dictionary<string, List<GameObject>> coinForHudPool = new Dictionary<string, List<GameObject>>();// coin pool:  coin_type => list of pooled objects

    //    public static List<string> cratesCollected = new List<string>(); //TODO List<string> -> Dictionary<string, int>
    public static Dictionary<string, List<int>> cratesCollected = new Dictionary<string, List<int>>(){
        {"magnet", new List<int>(){}},
        {"invincibility", new List<int>(){}},
        {"ice", new List<int>(){}},
        {"fuel", new List<int>(){}},
        {"coin", new List<int>(){}},
        {"coinOnce", new List<int>(){}}
    }; //TODO List<string> -> Dictionary<string, int>
       //	public static List<int> coinCratesCollected = new List<int>();

    public static int CoinX2Value = 100;

    static GameObject coinForHud;

    public static void PickUp(GameObject pickup)
    {

        switch (pickup.name)
        {
            case "Coin":
                CoinsCollected++;
                SoundManager.Play("PickupCoin");
                break;

            case "CoinX2":
                CoinsCollected += CoinX2Value;
                SoundManager.Play("PickupCoin2x");
                break;

            case "DiamondEgg": //a.k.a. --  zelta kjivere, pieskaitam uzreiz datu menedźerí
                AchievementManager.AchievementProgress("diamond_eggs", 1);
                AchievementManager.AchievementProgress("diamond_eggs__2", 1);
                AchievementManager.AchievementProgress("diamond_eggs__3", 1);
                BikeDataManager.Levels[LevelManager.CurrentLevelName].DiamondEggCollected = true;
                SoundManager.Play("PickupGoldenHelmet");
                break;


            case "Boost_magnet_crate":
                //                cratesCollected.Add("magnet");
                cratesCollected["magnet"].Add(pickup.GetComponent<CrateBehaviour>().count);
                SoundManager.Play("PickupBox");
                break;
            case "Boost_invincibility_crate":
                //                cratesCollected.Add("invincibility");
                cratesCollected["invincibility"].Add(pickup.GetComponent<CrateBehaviour>().count);
                SoundManager.Play("PickupBox");
                break;
            case "Boost_ice_crate":
                //                cratesCollected.Add("ice");
                cratesCollected["ice"].Add(pickup.GetComponent<CrateBehaviour>().count);
                SoundManager.Play("PickupBox");
                break;
            case "Boost_fuel_crate":
                //                cratesCollected.Add("fuel");
                cratesCollected["fuel"].Add(pickup.GetComponent<CrateBehaviour>().count);
                SoundManager.Play("PickupBox");
                break;
            case "CoinCrate":
                //print("coin crate collected");
                cratesCollected["coin"].Add(pickup.GetComponent<CrateBehaviour>().count);

                //            coinCratesCollected.Add(pickup.GetComponent<CrateBehaviour>().count);//TODO remove
                SoundManager.Play("PickupBox");
                break;
            case "CoinCrateOnce":
                //print("coin crate collected");
                cratesCollected["coinOnce"].Add(pickup.GetComponent<CrateBehaviour>().count);

                SoundManager.Play("PickupBox");
                break;

            //uzreiz aktivizéjamie bústińi
            case "Boost_magnet":
                if (!BikeDataManager.Boosts["magnet"].Active)
                {
                    BoostManager.ActivateBoost("magnet");
                    SoundManager.Play("BoostOn");
                }
                break;
            case "Boost_invincibility":
                if (!BikeDataManager.Boosts["invincibility"].Active)
                {
                    BoostManager.ActivateBoost("invincibility");
                    SoundManager.Play("BoostOn");
                }
                break;
            case "Boost_ice":
                if (!BikeDataManager.Boosts["ice"].Active)
                {
                    BoostManager.ActivateBoost("ice");
                    SoundManager.Play("BoostOn");
                }
                break;
            case "Boost_fuel":
                if (!BikeDataManager.Boosts["fuel"].Active)
                {
                    BoostManager.ActivateBoost("fuel");
                    SoundManager.Play("BoostOn");
                }
                break;

            default:
                break;
        }

        //        foreach (var item in cratesCollected) {
        //            print(item.Key + " " + item.Value);
        //        }

        PickupBehaviour pickupBehaviour = pickup.GetComponent<PickupBehaviour>();
        //        if (pickupBehaviour != null) {
        //            pickupBehaviour.MarkAsCollected();
        //        }

        if (pickupBehaviour != null)
        {
            pickupBehaviour.MarkAsCollected();
        }
        else
        {
            //can´t disable the coin game object, becuse unity is silly and won´t find a deactivated game object, 
            //other option is to store a reference array of inactive coins
            SpriteRenderer[] sprites = pickup.transform.parent.GetComponentsInChildren<SpriteRenderer>();
            for (int j = 0; j < sprites.Length; j++)
            {
                sprites[j].enabled = false;
            }

            BikeGameManager.ShowChildren(pickup.transform, false);
        }

        pickup.GetComponent<Collider2D>().enabled = false;

        //		ThrowHUDCoin (pickup);//TODO might need this later if it's decidet that magnet should attract coins again, DON'T remove
    }

    public static int CollectedCrateCount()
    {
        int count = 0;

        foreach (var item in cratesCollected)
        {
            count += item.Value.Count;
        }

        return count;
    }

    public static void CashCratesIn()
    {
        foreach (var item in cratesCollected)
        {
            string crateName = item.Key;
            if (crateName != "coin" && crateName != "coinOnce")
            {
                foreach (var count in item.Value)
                {
                    BikeDataManager.Boosts[crateName].Number += count;//++;
                    BikeDataManager.Boosts[crateName].Discovered = true;

                    //TODO this will break if there are multiple crates of same type in a level, but we were promised that such a thing will not happen
                    switch (crateName)
                    {
                        case "magnet":
                            BikeDataManager.Levels[LevelManager.CurrentLevelName].BoostMagnetCrateCollected = true;
                            break;
                        case "invincibility":
                            BikeDataManager.Levels[LevelManager.CurrentLevelName].BoostInvincibilityCrateCollected = true;
                            break;
                        case "ice":
                            BikeDataManager.Levels[LevelManager.CurrentLevelName].BoostIceCrateCollected = true;
                            break;
                        case "fuel":
                            BikeDataManager.Levels[LevelManager.CurrentLevelName].BoostFuelCrateCollected = true;
                            break;
                        default:
                            Debug.LogWarning("unknown crate name");
                            break;
                    }
                }
            }
        }
    }

    public static int CoinCrateAmount()
    {
        int amount = 0;
        if (cratesCollected["coin"] != null)
        {
            foreach (var item in cratesCollected["coin"])
            {
                amount += item;
            }
        }

        if (cratesCollected["coinOnce"] != null)
        {

            BikeDataManager.Levels[LevelManager.CurrentLevelName].CoinCrateCollected = true;

            foreach (var item in cratesCollected["coinOnce"])
            {
                amount += item;
            }
        }

        return amount;
    }

    //TODO might need this later if it's decidet that magnet should attract coins again, DON'T remove
    //	private static void ThrowHUDCoin(GameObject coin) {
    //		string coinType = coin.name;
    //
    //		if(!coinForHudPool.ContainsKey(coinType)) {
    //			coinForHudPool[coinType] = new List<GameObject>(); //new coin type, must initialize pool
    //		}
    //		
    //		coinForHud = null;
    //		for (int i = 0; i < coinForHudPool[coinType].Count; i++) {
    //			
    //			if (coinForHudPool[coinType][i].GetComponent<ThrowCoinAtHUD>().completed == true) {
    //				coinForHud = coinForHudPool[coinType][i];
    //				break;
    //			}
    //		}
    //		
    //		//if no free coins in pool create one
    //		if (coinForHud == null) {
    //			coinForHud = (GameObject)Instantiate(Resources.Load("Prefabs/UI/CoinForHUD", typeof(GameObject)));
    //			coinForHud.name = "CoinForHUD";
    //			ThrowCoinAtHUD throwCoinAtHUD = coinForHud.GetComponent<ThrowCoinAtHUD>();
    //			throwCoinAtHUD.toPosition = Vector3.up * 0.8f;
    //			throwCoinAtHUD.toScale = Vector3.one * 0.5f;
    //			coinForHud.GetComponent<SpriteRenderer>().sprite = coin.transform.parent.FindChild("Visual").GetComponent<SpriteRenderer>().sprite;
    //
    //			Color coinForHudColor = coinForHud.GetComponent<SpriteRenderer>().color;
    //			throwCoinAtHUD.toColor = new Color(coinForHudColor.r, coinForHudColor.g, coinForHudColor.b, 0);
    //			throwCoinAtHUD.fromColor = coinForHudColor;
    //			
    //			coinForHudPool[coinType].Add(coinForHud);
    //		}
    //		
    //		coinForHud.GetComponent<ThrowCoinAtHUD> ().completed = false;
    //		coinForHud.transform.position = coin.transform.position;
    //		coinForHud.transform.localScale = coin.transform.localScale;
    //		coinForHud.GetComponent<SpriteRenderer>().color = coinForHud.GetComponent<ThrowCoinAtHUD> ().fromColor;
    //		coinForHud.transform.parent = Camera.main.transform;
    //		
    //	}

    public static void ResetPickups()
    {
        CoinsCollected = 0;

        GameObject[] Pickup = GameObject.FindGameObjectsWithTag("Pickup");
        for (int i = 0; i < Pickup.Length; i++)
        {
            bool turnOn = true;

            // turn off one-time-only pickups if already collected in this level
            if (Pickup[i].name == "DiamondEgg")
            {
                if (BikeDataManager.Levels[LevelManager.CurrentLevelName].DiamondEggCollected)
                {
                    turnOn = false;
                }
            }
            if (Pickup[i].name == "Boost_magnet_crate")
            {
                if (BikeDataManager.Levels[LevelManager.CurrentLevelName].BoostMagnetCrateCollected)
                {
                    turnOn = false;
                }
            }
            if (Pickup[i].name == "Boost_invincibility_crate")
            {
                if (BikeDataManager.Levels[LevelManager.CurrentLevelName].BoostInvincibilityCrateCollected)
                {
                    turnOn = false;
                }
            }
            if (Pickup[i].name == "Boost_ice_crate")
            {
                if (BikeDataManager.Levels[LevelManager.CurrentLevelName].BoostIceCrateCollected)
                {
                    turnOn = false;
                }
            }
            if (Pickup[i].name == "Boost_fuel_crate")
            {
                if (BikeDataManager.Levels[LevelManager.CurrentLevelName].BoostFuelCrateCollected)
                {
                    turnOn = false;
                }
            }
            if (Pickup[i].name == "CoinCrateOnce")
            {
                if (BikeDataManager.Levels[LevelManager.CurrentLevelName].CoinCrateCollected)
                {
                    turnOn = false;
                }
            }

            BikeGameManager.ShowChildren(Pickup[i].transform, turnOn);

            SpriteRenderer[] sprites = Pickup[i].transform.parent.GetComponentsInChildren<SpriteRenderer>();
            for (int j = 0; j < sprites.Length; j++)
            {
                sprites[j].enabled = turnOn;
            }

            PickupBehaviour pickupBehaviour = Pickup[i].GetComponent<PickupBehaviour>();
            if (pickupBehaviour != null)
            {
                pickupBehaviour.Reset();
            }

            Pickup[i].GetComponent<Collider2D>().enabled = turnOn;
        }

        DeleteCoinsForHUD();


        //        cratesCollected.Clear();
        foreach (var item in cratesCollected)
        {
            item.Value.Clear();
        }

        //        coinCratesCollected.Clear();//TODO remove


    }



    public static void ResetBoostPickups()
    {

        GameObject[] Pickup = GameObject.FindGameObjectsWithTag("Pickup");
        for (int i = 0; i < Pickup.Length; i++)
        {

            if (Pickup[i].name == "Boost_magnet" ||
                Pickup[i].name == "Boost_invincibility" ||
                Pickup[i].name == "Boost_ice" ||
                Pickup[i].name == "Boost_fuel")
            {

                BikeGameManager.ShowChildren(Pickup[i].transform, true);

                SpriteRenderer[] sprites = Pickup[i].transform.parent.GetComponentsInChildren<SpriteRenderer>();
                for (int j = 0; j < sprites.Length; j++)
                {
                    sprites[j].enabled = true;
                }

                Pickup[i].GetComponent<Collider2D>().enabled = true;

                PickupBehaviour pickupBehaviour = Pickup[i].GetComponent<PickupBehaviour>();
                if (pickupBehaviour != null)
                {
                    pickupBehaviour.Reset();
                }

            }
        }

    }

    //doesnt delete, just moves them out of screen
    public static void DeleteCoinsForHUD()
    {


        Vector3 goAway = new Vector3(9999, 9999, 9999);
        foreach (KeyValuePair<string, List<GameObject>> pool in coinForHudPool)
        {
            //print("coinForHudPool num  "  + pool.Key + "  = " + pool.Value.Count);
            for (int i = 0; i < pool.Value.Count; i++)
            {
                pool.Value[i].transform.position = goAway;
                pool.Value[i].GetComponent<ThrowCoinAtHUD>().Reset();
            }
        }

    }

}

}
