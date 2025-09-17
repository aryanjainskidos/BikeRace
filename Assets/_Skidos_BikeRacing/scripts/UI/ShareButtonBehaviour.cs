namespace vasundharabikeracing {
 using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;

 public class ShareButtonBehaviour : MonoBehaviour {

//     public int Coins = 7500;
// 	public bool ThisTotallyIsMultiplayerBelieveMe = false; // śis skripts pieder vismaz 3 pogám, no kurám 2 ir pieejamas péc MP sasnieguma, un viena pieejama vienmér; śis bool nosaka vai poga vienmér bús ieslégta vai skatísies nepiecieśamíbu bút ieslégtai
// 	public bool visible = true;

//     Image image;
//     Button button;



// 	void Awake () {
//         button = GetComponent<Button>();
//         button.onClick.AddListener(
//             ()=>{ 
//                 ShareManager.ShareCoinReward = Coins;
// 				ShareManager.ShareMPReward = ThisTotallyIsMultiplayerBelieveMe;
//             }
//         );

//         image = GetComponent<Image>();
// 	}

//     void SetVisibility(bool value) {
//         visible = value;
//         image.enabled = value;
//         button.enabled = value;

//         Utils.ShowChildrenGraphics(gameObject, value);
//     }

//     void OnEnable() {

// 		if (ThisTotallyIsMultiplayerBelieveMe)
//         {
//             if (DataManager.ShowedMultiplayerFreeCoins && !DataManager.MultiplayerFreeCoinsReceived) {
//                 SetVisibility(true);
//             } else {
//                 SetVisibility(false);
//             }
//         }
//     }

//     void Update() {
// 		if (ThisTotallyIsMultiplayerBelieveMe)
//         {
//             if (DataManager.ShowedMultiplayerFreeCoins && DataManager.MultiplayerFreeCoinsReceived && visible) {
//                 SetVisibility(false);
//             } 

//             if (DataManager.ShowedMultiplayerFreeCoins && !DataManager.MultiplayerFreeCoinsReceived && !visible) {
//                 SetVisibility(true);
//             } 
//         }
//     }

 }

}
