namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

//for ingame purchases only  (not IAP/IAB)
public class PurchaseManager : MonoBehaviour
{


    public static bool CoinPurchase(int amount)
    {

        bool successful = false;

        if (amount <= BikeDataManager.Coins)
        {
            BikeDataManager.Coins -= amount;
            successful = true;
        }
        else
        {
            // UIManager.ToggleScreen(GameScreenType.PopupShop);
        }

        return successful;
    }


}

}
