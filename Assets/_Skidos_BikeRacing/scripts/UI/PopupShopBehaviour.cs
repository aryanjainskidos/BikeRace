namespace vasundharabikeracing {
using UnityEngine;

public class PopupShopBehaviour : MonoBehaviour
{

    public static bool IsOpen;

    void OnEnable()
    {

        if (!Startup.Initialized)
        {
            return;
        }
        IsOpen = true;

        //popupshop coinpack buttons - coinpacks 1-5
        Transform panel = transform.Find("Panel");
        if (panel != null)
        {
            int i = 0;
            // foreach (Transform tButton in panel){ 
            // 	if(tButton.name == "Button"){

            // 		string itemID = "coins_"+(i+1);
            // 		VirtualCurrencyPack currencyPack = CurrencyPacksById[itemID]; // coins_1 .. coins_5 ir definéti StoreAssets failá

            // 		//print("popupshop button++ " + "  i="+ i + "  Coins:"+ currencyPack.Name.ToString());
            // 		//print("popupshop button++ " + "  i="+ i + "  Price+Curr:"+ ((PurchaseWithMarket)currencyPack.PurchaseType).MarketItem.MarketPriceAndCurrency);

            // 		tButton.Find("CoinText").GetComponent<Text>().text = currencyPack.Name.ToString();
            // 		string price = ((PurchaseWithMarket)currencyPack.PurchaseType).MarketItem.MarketPriceAndCurrency;
            // 		tButton.Find("PriceText").GetComponent<Text>().text = price;

            // 		UIClickKeyDelegate del = tButton.GetComponent<UIClickKeyDelegate>();
            // 		del.key = itemID;

            // 		#if UNITY_EDITOR
            // 		del.keyDelegate = OnClickCoinPack; //redaktorá ĺauj pirkt (jo pirkumi tápat feiki)
            // 		#else
            // 		if(price == null || price.Length == 0){// uz ierícém párbaudís vai ir iegúta infa no veikalservera
            // 			del.keyDelegate = OnClickShowError; 
            // 		} else {
            // 			del.keyDelegate = OnClickCoinPack;
            // 		}
            // 		#endif

            //         Transform badge = tButton.Find("PercentBadge");
            //         if (badge != null) {
            //             var badgeTween = badge.GetComponent<BounceAndRepeatBehaviour>();
            //             if (DataManager.SettingsHD) {
            //                 badgeTween.enabled = true;
            //             } else {
            //                 badgeTween.enabled = false;
            //             }
            //         }


            // 		i++;
            // 	}
            // }
        }

    }

    public void OnClickCoinPack(string key)
    {
        UIManager.ToggleScreen(GameScreenType.PopupLoading, true); //paráda overleju - lai nespaida neko vairák, to aizváks, kad dabús atbildi no servera
                                                                   // StoreInventory.BuyItem(CurrencyPacksById[key].ItemId);
    }

    //nav pirkśana, járáda error
    public void OnClickShowError(string key)
    {
        PopupGenericErrorBehaviour.ErrorMessage = "Error connecting to store";
        UIManager.ToggleScreen(GameScreenType.PopupGenericError);
    }

}
}
