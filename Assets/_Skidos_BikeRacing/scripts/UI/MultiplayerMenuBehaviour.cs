namespace vasundharabikeracing {
 using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;

 public class MultiplayerMenuBehaviour : MonoBehaviour {

//     Toggle friendsToggle;

//     GameObject friendsPanel;

//     GameObject friendNotificationPanel;
//     GameObject leagueNotificationPanel;

//     Text friendNotificationText;

// 	bool friendNotificationOn;

// 	Text PowerPointsText;

//     GameObject pointerUpgrade;
// //    GameObject pointerInvite;
//     GameObject pointerFriends;

// 	GameObject blackoutPanel;

//    // Button inviteButton;
// 	bool firstFrame = false;

//     GameObject saleOffButton;
//     // RemoveAdsButtonBehaviour removeAdsButton;

//     ButtonScrollViewBehaviour buttonScrollView;

// 	void Awake () {

//         pointerUpgrade = transform.Find("LeaguePanel/PointerUpgrade").gameObject;
// //		pointerInvite = transform.FindChild("PointerInvite").gameObject;
// 		pointerFriends = transform.Find("PointerFriends").gameObject;

// 		friendsToggle = transform.Find("FriendsToggle").GetComponent<Toggle>();
// 		friendsToggle.onValueChanged.AddListener(OnFriendsValueChanged);
// 		friendsPanel = transform.Find("FriendsPanel").gameObject;

// 		friendNotificationPanel = transform.Find("FriendsNotificationsPanel").gameObject;
// 		leagueNotificationPanel = transform.Find("LeagueNotificationsPanel").gameObject;

// 		friendNotificationText = friendNotificationPanel.transform.Find("Text").GetComponent<Text>();

// 		friendNotificationPanel.SetActive(false);
// 		leagueNotificationPanel.SetActive(false); //currently off

// 		friendNotificationOn = false;

// 		PowerPointsText = transform.Find("UpgradeButton/RankText").GetComponent<Text>(); //poga, kas ved uz garáźu, satur uzrakstu ar Power punktiem

// //		inviteButton = transform.FindChild("InviteButton").GetComponent<Button>(); //poga, kas ved uz garáźu, satur uzrakstu ar Power punktiem
// //		inviteButton.onClick.AddListener(OnInviteClick);

// 		//panelis, kas paslépj visu ekrána saturu - to izlédz atverot śo ekránu; ieslédz, ja to paprasa MP menedźeris
// 		blackoutPanel = transform.Find("BlackoutPanel").gameObject;
// 		blackoutPanel.transform.SetSiblingIndex(9999);
// 		blackoutPanel.SetActive(false);

//         saleOffButton = transform.Find("SaleOffButton").gameObject;
//         // removeAdsButton = transform.Find("RemoveAdsButton").GetComponent<RemoveAdsButtonBehaviour>();

//         buttonScrollView = transform.Find("ButtonScrollView").GetComponent<ButtonScrollViewBehaviour>();
// 	}

//     void OnEnable(){        

// 		if(Startup.Initialized) {
// 			//visas inicializéśas darbíbas párnestas uz 1. kadru UPDATE metodé
// 			firstFrame = true;
//             AdManager.ShowRemoveAdsOffer = false;// fixes the button appearing for a couple of frames and then hiding again if it was present in levels
//         }

//         if (!friendsToggle.isOn && friendsPanel.activeSelf) {
//             friendsPanel.SetActive(false);
//         }


// 		if(!PlayerPrefs.HasKey("HadIntroGame")){
// 			blackoutPanel.SetActive(true);
// 		} else {
// 			blackoutPanel.SetActive(false);
// 		}

// 	}

// 	void OnDisable(){
// 		firstFrame = false;
// 	}


// 	void Update () {

//         if (!friendsToggle.isOn && friendsPanel.activeSelf) {
//             friendsPanel.SetActive(false);
//         }

// 		if(NewsListManager.ActiveRides != NewsListManager.LastShownActiveRides){
// 			if(NewsListManager.ActiveRides > 0){
// 				friendNotificationPanel.SetActive(true);
// 				friendNotificationOn = true;
// 				friendNotificationText.text = NewsListManager.ActiveRides.ToString();
// 			} else {
// 				friendNotificationPanel.SetActive(false);
// 				friendNotificationOn = false;
// 			}
// 		} else if(friendNotificationOn) {
// 			friendNotificationPanel.SetActive(false);
// 			friendNotificationOn = false;
// 		}

// //         if (removeAdsButton.active && saleOffButton.activeSelf) {
// //             saleOffButton.SetActive(false);
// //         } else if (!removeAdsButton.active && !saleOffButton.activeSelf){
// // //            saleOffButton.SetActive(true);
// //         }

// 		if(firstFrame){
// 			firstFrame = false;

// 			//print("MultiplayerMenuBehaviour::Enabled" );
// 			MultiplayerManager.Startup();
// 			PowerPointsText.text = MultiplayerManager.PermanentPowerRating.ToString();

// 			if (DataManager.ShowMultiplayerButtonNotification) {
// 				DataManager.ShowMultiplayerButtonNotification = false;
// 			}

// 			if (MultiplayerManager.NumGames >= 2 && MultiplayerManager.PowerRating == 0)
// 			{
// 				pointerUpgrade.SetActive(true);
// 			} else {
// 				if (pointerUpgrade.activeSelf) {
// 					pointerUpgrade.SetActive(false);
// 				}
//             }

// //            if (MultiplayerManager.LoggedIn && PlayerPrefs.GetInt("HasInvited",0) == 0 && friendsToggle.isOn && DataManager.FirstInvite)
// //            {
// //                pointerInvite.SetActive(true);
// //            } else {
// //                if (pointerInvite.activeSelf) {
// //                    pointerInvite.SetActive(false);
// //                }
// //            }

// //            print("MultiplayerManager.NumGames " + MultiplayerManager.NumGames);
//             if (!MultiplayerManager.HasFB && 
//                 (   
//                  (MultiplayerManager.NumGames >= 3 && DataManager.MultiplayerFriendsPopupShowCount == 0) ||
//                  (MultiplayerManager.NumGames >= 10 && DataManager.MultiplayerFriendsPopupShowCount == 1) ||
//                  (MultiplayerManager.NumGames >= 20 && DataManager.MultiplayerFriendsPopupShowCount == 2)
//                 )
//                ) {
//                 //TODO show popup
//                 DataManager.MultiplayerFriendsPopupShowCount++;
//                 UIManager.ToggleScreen(GameScreenType.PopupMultiplayerFriends);
//             }

//             if (MultiplayerManager.NumGames >= 2 && DataManager.FirstFriends && MultiplayerManager.PowerRating != 0) //upgraded bike but hasn't invited friends
//             {
//                 pointerFriends.SetActive(true);
//             } else {
//                 if (pointerFriends.activeSelf) {
//                     pointerFriends.SetActive(false);
//                 }
//             }

//             if (MultiplayerManager.NumGames == 2) //played a game but hasn't upgraded bike 
//             {
//                 if (DataManager.UpgradeSupersalePromoShowCount == 0 && //hasn't been shown yet
//                     MultiplayerManager.PowerRating == 0 && //player has a power rating of 0
//                     !PopupPromoBehaviour.IsPromoAvailable(PromoSubPopups.SaleUpgrades) //is not available yet
//                     ) {//is not upgraded to the max
//                     PopupPromoBehaviour.SchedulePromo(PromoSubPopups.SaleUpgrades, true);
//                     PopupPromoBehaviour.SchedulePromo(PromoSubPopups.SaleExpert, true, false);
//                     PopupPromoBehaviour.ShowPromoIfScheduled();
//                 }

//             }

//             if (MultiplayerManager.recalculateMPLevel) {
//                 MultiplayerManager.RecalculateLevel();
//             }

//             if (MultiplayerManager.Cups > 500 && !DataManager.ShowedMultiplayerFreeCoins) { 
//                 DataManager.ShowedMultiplayerFreeCoins = true;
//                 ShareManager.ShareCoinReward = 15000;
//                 ShareManager.ShareMPReward = true;

//                 UIManager.ToggleScreen(GameScreenType.PopupFreeCoins);
//             }
// 		}

//     }

// //    void OnInviteClick()
// //    {
// //        DataManager.FirstInvite = false;
// //
// ////        if (pointerInvite.activeSelf) {
// ////            pointerInvite.SetActive(false);
// ////        }
// //    }

//     void OnFriendsValueChanged(bool arg0)
//     {
//         if (Startup.Initialized && arg0) {
//             if(DataManager.FirstFriends) {
//                 DataManager.FirstFriends = false;

//                 if(pointerFriends.activeSelf) {
//                     pointerFriends.SetActive(false);
//                 }
//             }
//         }

//         if (Startup.Initialized)
//         {
//             //TODO let the ButtonScrollView know that it needs to hide/show the extra buttons
//             buttonScrollView.ShowExtraButtons(!arg0);
//         }
//     }


 }

}
