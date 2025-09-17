using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vasundharabikeracing;

public class Vasundhara_BikeRacingShop : MonoBehaviour, ShopInterfaceBikeRacing
{
    public static Vasundhara_BikeRacingShop instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

    }

    void AfterSuccesBuy(int amount)
    {
        switch (amount)
        {
            case 5:
                BikeDataManager.Coins += 500;
                Debug.Log("Collect 500 Coins");
                break;

            case 10:
                BikeDataManager.Coins += 1500;
                Debug.Log("Collect 1500 Coins");
                break;

            case 15:
                BikeDataManager.Coins += 3000;
                Debug.Log("Collect 3000 Coins");
                break;
        }
        BikeDataManager.Flush();
    }
    void FailedBug(GameObject obj)
    {

    }

    public void BuyOnClick(int obj)
    {
        if (obj != null)
        {
            AfterSuccesBuy(obj);
            return;
        }
    }
}