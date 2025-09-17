namespace vasundharabikeracing {
// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// // using Soomla.Store;

// //namespace Soomla.Store.Example{

// /// <summary>
// /// This class defines our game's economy, which includes virtual goods, virtual currencies
// /// and currency packs, virtual categories
// /// </summary>
// public class StoreAssets : IStoreAssets{

// 	/// <summary>
// 	/// see parent.
// 	/// </summary>
// 	public int GetVersion() {
// 		return 0;
// 	}

// 	/// <summary>
// 	/// see parent.
// 	/// </summary>
// 	public VirtualCurrency[] GetCurrencies() {
// 		return new VirtualCurrency[]{COINS};
// 	}

// 	/// <summary>
// 	/// see parent.
// 	/// </summary>
// 	public VirtualGood[] GetGoods() {
//         return new VirtualGood[] {NO_ADS_LTVG/*MUFFINCAKE_GOOD*/};
// 	}

// 	/// <summary>
// 	/// see parent.
// 	/// </summary>
// 	public VirtualCurrencyPack[] GetCurrencyPacks() {
//         return new VirtualCurrencyPack[] {COINS_1_PACK,COINS_2_PACK,COINS_3_PACK,COINS_4_PACK,COINS_5_PACK,OFFER_1,OFFER_2,OFFER_3,OFFER_4,OFFER_5,OFFER_6,OFFER_7,OFFER_8};
// 	}

// 	/// <summary>
// 	/// see parent.
// 	/// </summary>
// 	public VirtualCategory[] GetCategories() {
// 		return new VirtualCategory[]{GENERAL_CATEGORY};
// 	}


// 	/** Static Final Members **/



// 	public const string COINS_ITEM_ID      = "coins";


// 	public const string COINS_1_PACK_PRODUCT_ID      = "com.fungenerationlab.bikeup.pack01";
// 	public const string COINS_2_PACK_PRODUCT_ID      = "com.fungenerationlab.bikeup.pack02";
// 	public const string COINS_3_PACK_PRODUCT_ID      = "com.fungenerationlab.bikeup.pack03";
// 	public const string COINS_4_PACK_PRODUCT_ID      = "com.fungenerationlab.bikeup.pack04";
// 	public const string COINS_5_PACK_PRODUCT_ID      = "com.fungenerationlab.bikeup.pack05";
// 	public const string OFFER_1_PRODUCT_ID     		 = "com.fungenerationlab.bikeup.offer01";
// 	public const string OFFER_2_PRODUCT_ID     	 	 = "com.fungenerationlab.bikeup.offer02";
//     public const string OFFER_3_PRODUCT_ID           = "com.fungenerationlab.bikeup.offer03";
//     public const string OFFER_4_PRODUCT_ID           = "com.fungenerationlab.bikeup.offer04";
//     public const string OFFER_5_PRODUCT_ID           = "com.fungenerationlab.bikeup.offer05";
//     public const string OFFER_6_PRODUCT_ID           = "com.fungenerationlab.bikeup.offer06";
//     public const string OFFER_7_PRODUCT_ID           = "com.fungenerationlab.bikeup.offer07";
//     public const string OFFER_8_PRODUCT_ID           = "com.fungenerationlab.bikeup.offer08";
//     public const string PRODUCT_1_PRODUCT_ID         = "com.fungenerationlab.bikeup.prod01";

//     public const string PRODUCT_1_ITEM_ID      	 = "prod01";



// //	public const string NO_ADS_LIFETIME_PRODUCT_ID = "no_ads";


// 	/** Virtual Currencies **/

// 	public static VirtualCurrency COINS = new VirtualCurrency(
// 		"Coins",										// name
// 		"",												// description
// 		COINS_ITEM_ID							// item id
// 		);


// 	/** Virtual Currency Packs **/

// 	public static VirtualCurrencyPack COINS_1_PACK = new VirtualCurrencyPack(
// 		"25,000",                                   // name
// 		"",                      					 // description
// 		"coins_1",                                   // item id
// 		25000,												// number of currencies in the pack
// 		COINS_ITEM_ID,                        // the currency associated with this pack
// 		new PurchaseWithMarket(COINS_1_PACK_PRODUCT_ID, 0.01) //0.99
// 		);

// 	public static VirtualCurrencyPack COINS_2_PACK = new VirtualCurrencyPack(
// 		"50,000",                                   // name
// 		"",                      					 // description
// 		"coins_2",                                   // item id
// 		50000,												// number of currencies in the pack
// 		COINS_ITEM_ID,                        // the currency associated with this pack
// 		new PurchaseWithMarket(COINS_2_PACK_PRODUCT_ID, 0.02)//1.99
// 		);

// 	public static VirtualCurrencyPack COINS_3_PACK = new VirtualCurrencyPack(
// 		"100,000",                                   // name
// 		"",                       					// description
// 		"coins_3",                                   // item id
// 		100000,												// number of currencies in the pack
// 		COINS_ITEM_ID,                        // the currency associated with this pack
// 		new PurchaseWithMarket(COINS_3_PACK_PRODUCT_ID, 0.03)//6.99
// 		);

// 	public static VirtualCurrencyPack COINS_4_PACK = new VirtualCurrencyPack(
// 		"250,000",                                   // name
// 		"",                       					// description
// 		"coins_4",                                   // item id
// 		250000,												// number of currencies in the pack
// 		COINS_ITEM_ID,                        // the currency associated with this pack
// 		new PurchaseWithMarket(COINS_4_PACK_PRODUCT_ID, 0.04)//9.99
// 		);

// 	public static VirtualCurrencyPack COINS_5_PACK = new VirtualCurrencyPack(
// 		"500,000",                                   // name
// 		"",                      					 // description
// 		"coins_5",                                   // item id
// 		500000,												// number of currencies in the pack
// 		COINS_ITEM_ID,                        // the currency associated with this pack
// 		new PurchaseWithMarket(COINS_5_PACK_PRODUCT_ID, 0.05)//19.99
// 		);


// 	//PopupPromoBehaviour.cs un StoreManager.cs failos ir hárdkodéts, kurś no śiem offeriem dod klát visus stilus un káda ir feiká atlaide
// 	public static VirtualCurrencyPack OFFER_1 = new VirtualCurrencyPack(
// 		"50,000",                                   // name
// 		"",                       					// description
// 		"offer_1",                                   // item id
// 		50000,												// number of currencies in the pack
// 		COINS_ITEM_ID,                        // the currency associated with this pack
// 		new PurchaseWithMarket(OFFER_1_PRODUCT_ID, 0.06)
// 		);
// 	public static VirtualCurrencyPack OFFER_2 = new VirtualCurrencyPack(
// 		"500,000",                                   // name
// 		"",                       					// description
// 		"offer_2",                                   // item id
// 		500000,												// number of currencies in the pack
// 		COINS_ITEM_ID,                        // the currency associated with this pack
// 		new PurchaseWithMarket(OFFER_2_PRODUCT_ID, 0.07)
// 		);
//     public static VirtualCurrencyPack OFFER_3 = new VirtualCurrencyPack(
//         "500,000",                                   // name
//         "",                                         // description
//         "offer_3",                                   // item id
//         500000,                                             // number of currencies in the pack
//         COINS_ITEM_ID,                        // the currency associated with this pack
//         new PurchaseWithMarket(OFFER_3_PRODUCT_ID, 0.08)
//         );
//     public static VirtualCurrencyPack OFFER_4 = new VirtualCurrencyPack(
//         "fastest and 3 styles power LT 180",                                   // name
//         "",                                         // description
//         "offer_4",                                   // item id
//         0,                                             // number of currencies in the pack
//         COINS_ITEM_ID,                        // the currency associated with this pack
//         new PurchaseWithMarket(OFFER_4_PRODUCT_ID, 0.09)
//         );
//     public static VirtualCurrencyPack OFFER_5 = new VirtualCurrencyPack(
//         "fastest and 3 styles power GE 180",                                   // name
//         "",                                         // description
//         "offer_5",                                   // item id
//         0,                                             // number of currencies in the pack
//         COINS_ITEM_ID,                        // the currency associated with this pack
//         new PurchaseWithMarket(OFFER_5_PRODUCT_ID, 0.10)
//         );
//     public static VirtualCurrencyPack OFFER_6 = new VirtualCurrencyPack(
//         "expert upgrade to 7 power LT 180",                                   // name
//         "",                                         // description
//         "offer_6",                                   // item id
//         0,                                             // number of currencies in the pack
//         COINS_ITEM_ID,                        // the currency associated with this pack
//         new PurchaseWithMarket(OFFER_6_PRODUCT_ID, 0.11)
//         );
//     public static VirtualCurrencyPack OFFER_7 = new VirtualCurrencyPack(
//         "expert upgrade to 7 power GE 180",                                   // name
//         "",                                         // description
//         "offer_7",                                   // item id
//         0,                                             // number of currencies in the pack
//         COINS_ITEM_ID,                        // the currency associated with this pack
//         new PurchaseWithMarket(OFFER_7_PRODUCT_ID, 0.12)
//         );
//     public static VirtualCurrencyPack OFFER_8 = new VirtualCurrencyPack(
//         "no ads",                                   // name
//         "",                                         // description
//         "offer_8",                                   // item id
//         0,                                             // number of currencies in the pack
//         COINS_ITEM_ID,                        // the currency associated with this pack
//         new PurchaseWithMarket(OFFER_8_PRODUCT_ID, 0.13)
//         );


// 	/** Virtual Goods **
// 	unlock_all_tracks, visticamák, vajadzés ká "LifetimeVG"

// 	public static VirtualGood UNLOCK_ALL_GOOD = new SingleUseVG(
// 		"Fruit Cake",                                       		// name
// 		"Customers buy a double portion on each purchase of this cake", // description
// 		"fruit_cake",                                       		// item id
// 		new PurchaseWithVirtualItem(MUFFIN_CURRENCY_ITEM_ID, 225)); // the way this virtual good is purchased
// 	*/
//     // NOTE: Create non-consumable items using LifeTimeVG with PurchaseType of PurchaseWithMarket.
//     public static VirtualGood NO_ADS_LTVG = new LifetimeVG(
//         "No Ads",                             // Name
//         "No More Ads!",                       // Description
//         PRODUCT_1_ITEM_ID,                          // Item ID
//         new PurchaseWithMarket(               // Purchase type (with real money $)
//            PRODUCT_1_PRODUCT_ID,                      // Product ID
//            0.99                                   // Price (in real money $)
//         )
//         );


// 	/** Virtual Categories **   -- man nav vajadzigs */
// 	// The muffin rush theme doesn't support categories, so we just put everything under a general category.
// 	public static VirtualCategory GENERAL_CATEGORY = new VirtualCategory(
// 	"General", new List<string>(new string[] { "whatevs" })
// 		);
// 	///*/
// }

// //}

}
