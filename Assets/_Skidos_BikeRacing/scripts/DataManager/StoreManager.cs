namespace vasundharabikeracing {
// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using Soomla.Store;
// using Soomla;


// public class StoreManager : MonoBehaviour {

// 	private static string currentlyBuying = "";
//     public static bool initialized = false;

// 	public static void Init () {



// 		StoreEvents.OnSoomlaStoreInitialized += onSoomlaStoreInitialized;
// 		StoreEvents.OnCurrencyBalanceChanged += onCurrencyBalanceChanged;

// 		StoreEvents.OnMarketPurchaseStarted += onMarketPurchaseStarted;
// 		StoreEvents.OnMarketPurchase += onMarketPurchase;

// 		StoreEvents.OnUnexpectedErrorInStore += onUnexpectedErrorInStore;
// 		StoreEvents.OnMarketPurchaseCancelled += onMarketPurchaseCancelled;
// 		StoreEvents.OnBillingNotSupported += onBillingNotSupported;

//         StoreEvents.OnItemPurchased += onItemPurchased;
//         StoreEvents.OnRestoreTransactionsStarted += OnRestoreTransactionsStarted;
//         StoreEvents.OnRestoreTransactionsFinished += OnRestoreTransactionsFinished;

// 		SoomlaStore.Initialize(new StoreAssets());

//         initialized = true;
// 	}

// 	//callback
// 	public static void onSoomlaStoreInitialized() {

// 		// some usage examples for add/remove currency
// 		// some examples
// 		/*if (StoreInfo.Currencies.Count>0) {
// 			try {
// 				StoreInventory.GiveItem(StoreInfo.Currencies[0].ItemId,4000);
// 				SoomlaUtils.LogDebug("SOOMLA ExampleEventHandler", "Currency balance:" + StoreInventory.GetItemBalance(StoreInfo.Currencies[0].ItemId));
// 			} catch (VirtualItemNotFoundException ex){
// 				SoomlaUtils.LogError("SOOMLA ExampleEventHandler", ex.Message);
// 			}
// 		}*/
// 		if(Debug.isDebugBuild){
// 			print("soomla init OK");
// 		}



// 	}


// 	//callback - kad mainíta bilance (pat transakcija: "nulle mínus nulle" trigero śo kólbeku)
// 	//@note -- paljaujos, ka śis kólbeks ir tikai péc koiníśu ieǵadáśanás (nekádá citá veidá spéle neliek naudu SOOMLAs macinjá)
// 	public static void onCurrencyBalanceChanged(VirtualCurrency virtualCurrency, int balance, int amountAdded) {
// //		print("money change: curr:" +  virtualCurrency.ToString() + "  balance:" + balance + "  amountAdded:"+amountAdded);



// 		//koinpaku pirkumi, kas dod naudu
// 		if(balance > 0){//svarígi -- citádi bús bezgalíga rekursija
// 			StoreInventory.TakeItem(StoreAssets.COINS_ITEM_ID, balance);//izńem visu naudu no SOOMLAs macinja
// 			DataManager.CoinsWOAchievement += balance; //identisku daudzumu pieskaita spéles naudai

//             if(currentlyBuying == "offer_1"){ //nauda + visis stilinji

//                 for(int i = 0; i < DataManager.Styles.Count; i++){//atslédzu visus stilus
//                     DataManager.Styles[i].Locked = false;
//                 }

//                 PlayerPrefs.SetInt("promo" + PromoSubPopups.SaleStyles.ToString(), 0); //pierakstu, ka vairs nav járáda śis promo-popups                

//                 UIManager.SwitchScreen(GameScreenType.Garage);//ejam uz garáźu
//                 UIManager.SwitchScreenTab(GameScreenType.Garage, "Customize", "Career"); // atveru tabu - stilińi/SP

//             }

// 			FinalizePurchase();
//         }


//         // śis pirkums, lai arí ir coinpack, nedod naudu
//         if(currentlyBuying == "offer_4" || currentlyBuying == "offer_5"){ //upgrades max + 3 stilinji

//             for(int i = 0; i < DataManager.Styles.Count; i++){//atslédzu visus stilus
//                 if (DataManager.Styles[i].Name == "Robot" ||
//                     DataManager.Styles[i].Name == "Agent" ||
//                     DataManager.Styles[i].Name == "Astro") {                    
//                     DataManager.Styles[i].Locked = false;
//                 }
//             }

//             List<int> keys = new List<int> (DataManager.Bikes[DataManager.MultiplayerPlayerBikeRecordName].Upgrades.Keys);
//             foreach (var item in keys) {
// //                DataManager.Bikes[DataManager.MultiplayerPlayerBikeRecordName].Upgrades[item] = 10;//set max upgrade level
//                 DataManager.Bikes[DataManager.MultiplayerPlayerBikeRecordName].UpgradesSet(item, 10);//set max upgrade level
//             }
//             MultiplayerManager.RecalculateMyPowerRating();

//             PlayerPrefs.SetInt("promo" + PromoSubPopups.SaleUpgrades.ToString(), 0); //pierakstu, ka vairs nav járáda śis promo-popups
//             PlayerPrefs.SetInt("promo" + PromoSubPopups.SaleExpert.ToString(), 0); //don't show a lesser promotion after the superior promotion been shown

//             UIManager.SwitchScreen(GameScreenType.Garage);//ejam uz garáźu
//             UIManager.SwitchScreenTab(GameScreenType.Garage, "Upgrade", "Competitive"); // atveru tabu - upgrades/MP

// 			FinalizePurchase();
//         }

//         // śis pirkums, lai arí ir coinpack, nedod naudu
//         if(currentlyBuying == "offer_6" || currentlyBuying == "offer_7"){ //upgrades to 7

//             List<int> keys = new List<int> (DataManager.Bikes[DataManager.MultiplayerPlayerBikeRecordName].Upgrades.Keys);
//             foreach (var item in keys) {
// //                DataManager.Bikes[DataManager.MultiplayerPlayerBikeRecordName].Upgrades[item] = 7;//set max upgrade level
//                 DataManager.Bikes[DataManager.MultiplayerPlayerBikeRecordName].UpgradesSet(item, 7);//set max upgrade level
//             }
//             MultiplayerManager.RecalculateMyPowerRating();

//             PlayerPrefs.SetInt("promo" + PromoSubPopups.SaleExpert.ToString(), 0); //pierakstu, ka vairs nav járáda śis promo-popups

//             UIManager.SwitchScreen(GameScreenType.Garage);//ejam uz garáźu
//             UIManager.SwitchScreenTab(GameScreenType.Garage, "Upgrade", "Competitive"); // atveru tabu - upgrades/MP

// 			FinalizePurchase();
//         }

//         if(currentlyBuying == "offer_8"){ //no more ads

// //            print("got rid of ads");
//             //no need to do anything, any purchase disables ads
//             AdManager.ShowRemoveAdsOffer = false; //hide remove ads button

//             FinalizePurchase();
//         }
// 	}

// 	public static void onMarketPurchaseStarted(PurchasableVirtualItem pvi) {
// 		currentlyBuying = pvi.ItemId;
// 		if(Debug.isDebugBuild){
// 			print("onMarketPurchaseStarted: "  + pvi.ItemId);
// 		}
// 	}

// 	public static void onMarketPurchase(PurchasableVirtualItem pvi, string payload, Dictionary<string, string> extra) {
// 		if(Debug.isDebugBuild){
// 			print("onMarketPurchase: " + pvi.ItemId +  "  payload: " + payload);		
// 			foreach(KeyValuePair<string,string> kvp in extra){
// 				print(kvp.Key + " ==> " + kvp.Value);
// 			}
// 		}
// 	}

// 	public static void onUnexpectedErrorInStore(string message) {
// 		if(Debug.isDebugBuild){
// 			print("onUnexpectedErrorInStore: " + message);
// 		}
// 		UIManager.ToggleScreen(GameScreenType.PopupLoading, false);//neizdodas nopirkt, jáaizver loading overlejs
// 	}

// 	public static void onMarketPurchaseCancelled(PurchasableVirtualItem pvi) {
// 		if(Debug.isDebugBuild){
// 			print("onMarketPurchaseCancelled: " + pvi.Name);
// 		}
// 		UIManager.ToggleScreen(GameScreenType.PopupLoading, false);//neizdodas nopirkt, jáaizver loading overlejs
// 	}

// 	public static void onBillingNotSupported() {
// 		if(Debug.isDebugBuild){
// 			print("onBillingNotSupported");
// 		}
// 		UIManager.ToggleScreen(GameScreenType.PopupLoading, false);//neizdodas nopirkt, jáaizver loading overlejs
// 	}

//     static void onItemPurchased(PurchasableVirtualItem pvi, string payload)
//     {
//         if(pvi.ItemId == StoreAssets.PRODUCT_1_ITEM_ID)
//         {
//             AdManager.ShowRemoveAdsOffer = false; //hide remove ads button

//             FinalizePurchase();
//         }
//     }

//     static void OnRestoreTransactionsStarted()
//     {
//         if(Debug.isDebugBuild){
//             Debug.Log("OnRestoreTransactionsStarted");
//         }
//     }

//     static void OnRestoreTransactionsFinished(bool success)
//     {
//         if(Debug.isDebugBuild){
//             Debug.Log("OnRestoreTransactionsFinished");
//         }

//         if (success)
//         {
//             TransactionsAlreadyRestored = true;

//             if(StoreInventory.GetItemBalance(StoreAssets.PRODUCT_1_ITEM_ID) > 0)
//                 AdManager.ShowRemoveAdsOffer = false; //hide remove ads button
//                 DataManager.HasBoughtAnything = true;
//                 DataManager.Flush();
//         }

//         UIManager.ToggleScreen(GameScreenType.PopupLoading, false);
//     }

// 	private static void FinalizePurchase(){

// 		TelemetryManager.EventPurchasing(currentlyBuying);
// 		DataManager.HasBoughtAnything = true;
// 		DataManager.Flush();

// 		UIManager.ToggleScreen(GameScreenType.PopupShop, false); //aizveru popupu, ja bija valjá
// 		UIManager.ToggleScreen(GameScreenType.PopupPromo, false); //arí promo popupu 
// 		UIManager.ToggleScreen(GameScreenType.PopupLoading, false); //aizvácu loading overleju
// 	}

//     public static void RestorePurchases()
//     {
//         if(Debug.isDebugBuild){
//             Debug.Log("RestorePurchases");
//         }

//         //if transactions not restored
//         if(!TransactionsAlreadyRestored) {
//             UIManager.ToggleScreen(GameScreenType.PopupLoading, true);
//             SoomlaStore.RestoreTransactions();
//         }
//     }

//     public static bool TransactionsAlreadyRestored = false;
// }

}
