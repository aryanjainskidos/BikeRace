namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;

public class PopupLongLevelBehaviour : MonoBehaviour
{


    Text coinText;
    int initialAmmount;


    void Awake()
    {
        coinText = transform.Find("InfoPanel/CoinText").GetComponent<Text>();
        initialAmmount = int.Parse(coinText.text, System.Globalization.CultureInfo.InvariantCulture);
        //print("PopupLongLevelBehaviour::initialAmmount="+initialAmmount);
    }

    void OnEnable()
    {

        //maina monétińu skaitu - tas nekur netiek lietots, tikai attélots smukumam

        int coinMultiplier = 1;
        if (CentralizedOfferManager.IsDoubleCoinWeekendOn())
        {
            coinMultiplier = 2;
        }
        coinText.text = (initialAmmount * coinMultiplier).ToString();
    }


}

}
