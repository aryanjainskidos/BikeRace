namespace vasundharabikeracing {
 using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;
// // using Soomla.Store;

 public class RemoveAdsButtonBehaviour : MonoBehaviour {

//     Image image;
//     Button button;

//     GameObject innerText;

//     public bool active = false;

// 	void Awake () {
//         image = GetComponent<Image>();
//         button = GetComponent<Button>();

//         innerText = transform.Find("Text").gameObject;

//         SetActive(false);

//         button.onClick.AddListener(OnClick);
// 	}

//     void SetActive(bool value) {
// 		#if UNITY_ANDROID
// 		value = false; // no cita skripta daźreiz grib ieslégt, nedríkst
// 		#endif

//         image.enabled = value;
//         button.enabled = value;
//         innerText.SetActive(value);
//         active = value;
//     }
// //never show this offer on android because of hackers
// 	void OnEnable () {
// #if UNITY_IOS
//         if (!AdManager.ShowRemoveAdsOffer) {
//             SetActive(false);
//         } else {
//             SetActive(true);
//         }
// #endif
// 	}

//     void Update() {
// #if UNITY_IOS
//         if (!AdManager.ShowRemoveAdsOffer && image.enabled) {
//             SetActive(false);
//         }

//         if (AdManager.ShowRemoveAdsOffer && !image.enabled)
//         {
//             SetActive(true);
//         }
// #endif
//     }

//     void OnClick()
//     {
//         // StoreInventory.BuyItem(StoreAssets.PRODUCT_1_ITEM_ID);
//     }

//     void OnDisable() {
//         AdManager.ShowRemoveAdsOffer = false;
//     }
 }

}
