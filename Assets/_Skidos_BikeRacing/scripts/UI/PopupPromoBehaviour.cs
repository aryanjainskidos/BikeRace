namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
// using Soomla.Store;
using UnityEngine.UI;

public enum PromoSubPopups
{
    None = -1,
    Styles = 0,
    SaleStyles = 1, //śie cipari sakrít ar item id Store assets failá
    Sale50 = 2,
    Sale70 = 3,
    SaleUpgrades = 4,
    SaleUpgrades2 = 5,//internal use, don't use outside of this class
    SaleExpert = 6,
    SaleExpert2 = 7,//internal use
}

/**
 * pieder PopupPromo ekránam
 * 	 	+++
 * satur statisko interfeisu ar kuru mainít kuri promo sub-popupi bús redzami PopupPromo popupá
 */
public class PopupPromoBehaviour : MonoBehaviour
{


    // Dictionary<string, VirtualCurrencyPack> CurrencyPacksById;
    Transform[] promoSubPopups;
    bool forceScroll = false;

    ScrollViewControlBehaviour scrollViewControlBehaviour;
    ScrollViewMoveToTopBehaviour scrollViewMoveToTopBehaviour;

    static bool promoScheduled = false;
    static bool[] promoSubPopupsOn = { false, false, false, false, false, false, false, false }; //tik cik ir sub-popupi


    //------------------static------------------

    /**
	 * ierindo 1 promo sub-popupu, kurś tiks atvérts tiklídz lietotájs atvérs trases chúsku
	 * pieraksta PlayerPrefos, ka śis tips ir jau parádíts
	 */
    public static void SchedulePromo(PromoSubPopups type, bool force = false, bool overwrite = true)
    {
        //
        //		string playerPrefKey = "promo" + type.ToString();
        //
        //		if(PlayerPrefs.GetInt(playerPrefKey, 0) == 0 || force) { //ieskedjulés, ja jau nav rádíts
        //            if(overwrite) {
        //    			for(int i = 0; i < promoSubPopupsOn.Length; i++){
        //    				promoSubPopupsOn[i] = false;
        //    			}
        //            }
        //            if ((type == PromoSubPopups.SaleUpgrades || type == PromoSubPopups.SaleExpert) && MultiplayerManager.PowerRating >= 180) {
        //                promoSubPopupsOn[(int)(type == PromoSubPopups.SaleUpgrades ? PromoSubPopups.SaleUpgrades2 : PromoSubPopups.SaleExpert2)] = true; 
        //            } else {
        //			    promoSubPopupsOn[(int)type] = true; //ieslédz tikai izvéléto
        //            }
        //			promoScheduled = true; //ir járáda
        //
        //			PlayerPrefs.SetInt(playerPrefKey, 1); //1-time-only
        //		}
    }

    /**
	 * śo izsauc kad spéle ir spéjíga rádít promo, ja nekas nav ierindots - neko nedarís
	 */
    public static void ShowPromoIfScheduled()
    {
        // if(promoScheduled){
        // 	promoScheduled = false;
        // 	UIManager.ToggleScreen(GameScreenType.PopupPromo);
        // }
    }

    //vai spélétájam ir rádíti promo popupi - vai varl jĺaut vińam manuáli vért vaĺá promo ekránu (ja vinjam nav radíti, tad promo ekráns bús tukśs)
    public static bool ArePromosAvailable()
    {

        return (PlayerPrefs.GetInt("promo" + PromoSubPopups.SaleStyles.ToString(), 0) == 1) ||
                (PlayerPrefs.GetInt("promo" + PromoSubPopups.Sale50.ToString(), 0) == 1) ||
                (PlayerPrefs.GetInt("promo" + PromoSubPopups.Sale70.ToString(), 0) == 1) ||
                (PlayerPrefs.GetInt("promo" + PromoSubPopups.SaleUpgrades.ToString(), 0) == 1) ||
                (PlayerPrefs.GetInt("promo" + PromoSubPopups.SaleExpert.ToString(), 0) == 1);
        //true, ja ir káds no śiem promo bijis rádíts
    }

    public static bool IsPromoAvailable(PromoSubPopups subpopupType)
    {

        return (PlayerPrefs.GetInt("promo" + subpopupType.ToString(), 0) == 1);
        //true, ja ir káds no śiem promo bijis rádíts
    }

    /**
	 * śo izsauc poga garáźá - paráda promopopupu logu pirms tam ieslédzot tos popupus, ko spélétájam ir járedz (tie, kas jau atrádíti)
	 */
    public static void ShowInGarage()
    {

        if (!ArePromosAvailable())
        { //nevérs vaĺá promo ekránu - jo tur nav deríga subpopupa
            return;
        }

        for (int i = 0; i < promoSubPopupsOn.Length; i++)
        {
            promoSubPopupsOn[i] = false; //izslédz visus
        }

        //ieslédz stili+koinpaka, ja spélétájs ir jau redzéjis śo promo
        if (PlayerPrefs.GetInt("promo" + PromoSubPopups.SaleStyles.ToString(), 0) == 1)
        {
            promoSubPopupsOn[(int)PromoSubPopups.SaleStyles] = true;
        }

        //ieslédz koinpaka-70%, ja spélétájs ir redzéjis śo promo
        if (PlayerPrefs.GetInt("promo" + PromoSubPopups.Sale70.ToString(), 0) == 1)
        {
            promoSubPopupsOn[(int)PromoSubPopups.Sale70] = true;
        }
        else
        {
            //ja nav redzéjis 70%, tad skatás vai ir redzéjis 50%
            if (PlayerPrefs.GetInt("promo" + PromoSubPopups.Sale50.ToString(), 0) == 1)
            {
                promoSubPopupsOn[(int)PromoSubPopups.Sale50] = true;
            }

        }

        if (PlayerPrefs.GetInt("promo" + PromoSubPopups.SaleUpgrades.ToString(), 0) == 1)
        {
            if (MultiplayerManager.PowerRating < 180)
            {
                promoSubPopupsOn[(int)PromoSubPopups.SaleUpgrades] = true;
            }
            else
            {
                promoSubPopupsOn[(int)PromoSubPopups.SaleUpgrades2] = true;
            }
        }

        if (PlayerPrefs.GetInt("promo" + PromoSubPopups.SaleExpert.ToString(), 0) == 1)
        {
            if (MultiplayerManager.PowerRating < 180)
            {
                promoSubPopupsOn[(int)PromoSubPopups.SaleExpert] = true;
            }
            else
            {
                promoSubPopupsOn[(int)PromoSubPopups.SaleExpert2] = true;
            }
        }

        UIManager.ToggleScreen(GameScreenType.PopupPromo);

    }


    public static PromoSubPopups centerOn = PromoSubPopups.Sale50;

    //------------end---static------------------




    void Awake()
    {

        scrollViewMoveToTopBehaviour = transform.Find("ScrollView").GetComponent<ScrollViewMoveToTopBehaviour>();
        scrollViewControlBehaviour = transform.Find("ScrollView").GetComponent<ScrollViewControlBehaviour>();

        Transform[] _promoSubPopups = {
            transform.Find("ScrollView/Content/PopupPromoStyles"),				  //no offer
			transform.Find("ScrollView/Content/PopupPromoSaleStyles"), 	     //all_styles+coins 50% off  -  offer_1
			transform.Find("ScrollView/Content/PopupPromoSale50"),  			//coins 50% off  -  offer_2
            transform.Find("ScrollView/Content/PopupPromoSale70"),            //coins 70% off  -  offer_3
            transform.Find("ScrollView/Content/PopupPromoSaleUpgrades"),       //coins 70% off  -  offer_4
            transform.Find("ScrollView/Content/PopupPromoSaleUpgrades2"),      //coins 70% off  -  offer_5
            transform.Find("ScrollView/Content/PopupPromoSaleExpert"),         //coins 70% off  -  offer_6
            transform.Find("ScrollView/Content/PopupPromoSaleExpert2"), 	    //coins 70% off  -  offer_7
		};
        promoSubPopups = _promoSubPopups;

    }

    void OnEnable()
    {

        if (!Startup.Initialized)
        {
            return;
        }
        forceScroll = true;



        // #if UNITY_ANDROID && !UNITY_EDITOR
        // SoomlaStore.StartIabServiceInBg();
        // #endif	


        // /**
        //  * "CurrencyPacks[1]" nenozímé "currencyPack ID=1"  -- sańemot info no servera secíba tiek salauzta
        //  * tagad ir "coins_1" => "currencyPack ID=1"
        //  */
        // if(CurrencyPacksById == null){
        // 	CurrencyPacksById = new Dictionary<string, VirtualCurrencyPack>();
        // 	for(int i = 0; i < StoreInfo.CurrencyPacks.Count; i++){
        // 		CurrencyPacksById.Add(StoreInfo.CurrencyPacks[i].ItemId, StoreInfo.CurrencyPacks[i]);
        // 	}
        // }


        /**
		 * i = 1	50% atlaide + dávina visus stilus (StoreManager.cs failá - kur tiek pieskaitítas nopirktás monetas) 
		 * i = 2	50% atlaide
		 * i = 3	70% atlaide
		 */
        float[] fakeOff = { 00, 0.5f, 0.5f, 0.67f, 0.67f, 0.67f, 0.25f, 0.5f };

        int activeCount = 0;

        // for(int i = 0; i < promoSubPopups.Length; i++){
        // 	if(promoSubPopups[i] != null){

        // 		if(promoSubPopupsOn[i]){ //slépj izslégtos sub-popupus
        // 			promoSubPopups[i].gameObject.SetActive(true);
        //             activeCount++;
        // 		} else {
        // 			promoSubPopups[i].gameObject.SetActive(false);
        // 			continue;
        // 		}

        // 		Transform buyButton = promoSubPopups[i].Find("BuyButton");
        // 		if(buyButton != null) { //ne visiem subpopupiem ir BUY pogas

        // 			string itemID = "offer_" + i;
        // 			VirtualCurrencyPack currencyPack = CurrencyPacksById[itemID];

        //             if(promoSubPopups[i].Find("InfoPanel/TextNumCoins") != null){
        // 			    promoSubPopups[i].Find("InfoPanel/TextNumCoins").GetComponent<Text>().text = currencyPack.Name.ToString();
        // 			}
        // 			string price = ((PurchaseWithMarket)currencyPack.PurchaseType).MarketItem.MarketPriceAndCurrency;
        // 			promoSubPopups[i].Find("InfoPanel/TextPrice").GetComponent<Text>().text = price;

        // 			int priceCents = (int)((PurchaseWithMarket)currencyPack.PurchaseType).MarketItem.MarketPriceMicros;
        // 			priceCents /= 10000; // no mikro uz centiem
        // 			int fakePrice = (int) (priceCents / (1 - fakeOff[i]) ); // fake price ir Cik maksátu, ja nebútu feiká atlaide 50%
        // 			int lastdigit = (fakePrice % 10);
        // 			fakePrice += 9 - lastdigit;// nofeiko pédéjo ciparu uz 9
        // 			float fakePriceBig = fakePrice / 100.0f; // centi => eur

        // 			//string currency = ((PurchaseWithMarket)currencyPack.PurchaseType).MarketItem.MarketCurrencyCode; //nerádam, jo currency ir EUR nevis €, nevaru dabút pareizo simbolu

        // 			promoSubPopups[i].Find("InfoPanel/TextPriceOld").GetComponent<Text>().text = Lang.Get("Shop:PreviousPrice") + " " + fakePriceBig.ToString("F2");

        // 			UIClickKeyDelegate del = buyButton.GetComponent<UIClickKeyDelegate>();
        // 			del.key = itemID;

        // 			#if UNITY_EDITOR
        // 			del.keyDelegate = OnClickCoinPack; //redaktorá ĺauj pirkt (jo pirkumi tápat feiki)
        // 			#else
        // 			if(price == null || price.Length == 0){// uz ierícém párbaudís vai ir iegúta infa no veikalservera
        // 				del.keyDelegate = OnClickShowError; 
        // 			} else {
        // 				del.keyDelegate = OnClickCoinPack;
        // 			}
        // 			#endif


        // 		}
        // 	}
        // }

        if (activeCount > 1)
        {
            if (centerOn == PromoSubPopups.None)
            {
                scrollViewMoveToTopBehaviour.forceRecalculate = true;
                //                Debug.LogWarning("ScrollViewMoveToTopBehaviour");
            }
        }



    }

    void OnDisable()
    {
        if (!Startup.Initialized)
        {
            return;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
		// SoomlaStore.StopIabServiceInBg();
#endif
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

    void Update()
    {

        //paskrollé visu uz augśu [feikojot scroll eventu] - citádi péc vairáku scrollView elementu izslégśanas nekas nebútu redzams
        if (forceScroll)
        {
            forceScroll = false;

            if (centerOn != PromoSubPopups.None)
            {
                scrollViewControlBehaviour.CenterOnItem(promoSubPopups[(int)centerOn].GetComponent<RectTransform>());//TODO fugly, refactor
                                                                                                                     //                Debug.LogWarning("ScrollViewControlBehaviour " + promoSubPopups[(int)centerOn].name);
            }
            else
            {
                var pointer = new PointerEventData(EventSystem.current);
                ExecuteEvents.Execute(transform.Find("ScrollView").gameObject, pointer, ExecuteEvents.scrollHandler);
            }
        }

    }

}

}
