namespace vasundharabikeracing {
// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;

// public class SpinForVideoButtonBehaviour : MonoBehaviour {

//     public bool visible = true;

//     Button button;
//     Image image;

//     void Awake () {
//         button = GetComponent<Button>();
//         button.onClick.AddListener(OnClick);

//         image = GetComponent<Image>();
//     }

//     void OnClick()
//     {
//         AdManager.JustFinishedRace = false; //lai neráda reklámu, lídz nákamá límeńa izbraukśanai               
//         print("klik on RewardedVideoBehaviour");
//         // HeyZapManager.ShowRewarded(IncentiveType.Spin);
//         TelemetryManager.EventAdShown("rewarded_spin");
//         SetVisibility(false);
//     }

//     void SetVisibility(bool value) {
//         visible = value;
//         image.enabled = value;
//         button.enabled = value;

//         Utils.ShowChildrenGraphics(gameObject, value);
//     }

// 	// Use this for initialization
// 	void OnEnable () {

//         bool heyzapIsReady = true;
// // #if !UNITY_EDITOR
// //         heyzapIsReady = HeyZapManager.IsRewardedReady();
// // #endif
//         bool noSpinsTillSkip = ( SpinManager.GetSpinsTillSkip() == 0 );

//         if( heyzapIsReady && noSpinsTillSkip ){
//             SetVisibility(true);
//         } else {
//             SetVisibility(false);
//         }
// 	}

// 	// Update is called once per frame
// 	void Update () {

// 	}
// }

}
