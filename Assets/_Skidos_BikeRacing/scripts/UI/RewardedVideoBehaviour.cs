namespace vasundharabikeracing {
 using UnityEngine;
    // using UnityEngine.UI;
    // using System.Collections;
    // using System;

    public class RewardedVideoBehaviour : MonoBehaviour
    {
    }

// 	[HideInInspector]
// 	static public int Reward = 2500; //pieskaita HeyZapManager failá (not anymore, now it gets passed to ShowRewarded)



//     Button button;
//     Image image;
// //	ShareButtonBehaviour ShareButton; //poga, kas atrodas virs śís pogas
// 	RectTransform justAContainerRect; //konteiners, kurá atrodas visi śís pogas iekśéjie elementi - lai tos pastumtu augśéjás pogas vietá, ja vajag (visu pogu nevaru stumt - SlideInBehaviour déĺ)
// 	Transform justAContainer;

//     const string PREREQUISITE_LEVEL = "a___004"; //was 015 at some point

//     public bool visible = true;
// 	static bool? triedLevel15 = null;
// 	static DateTime lastVideoSeen = DateTime.Now - TimeSpan.FromHours(127);
// 	bool? uptown = null; //nodrośina, ka katrá Updeitá nepárpozicioné, ja jau atrodas pareizá pozícijá



// 	void Awake () {
//         image = GetComponent<Image>();
//         button = GetComponent<Button>();
//         button.onClick.AddListener(
//             ()=>{
// 				AdManager.JustFinishedRace = false; //lai neráda reklámu, lídz nákamá límeńa izbraukśanai				
// 				print("klik on RewardedVideoBehaviour");
// 				lastVideoSeen = DateTime.Now;
// 				// HeyZapManager.ShowRewarded(IncentiveType.Coin, Reward);
// 				TelemetryManager.EventAdShown("rewarded");
// 				SetVisibility(false);

//             }
//         );


// 		justAContainer = transform.Find("JustAContainer");
// 		justAContainerRect = justAContainer.GetComponent<RectTransform>();
// 		justAContainer.Find("Text").GetComponent<Text>().text = Reward.ToString();
// //		ShareButton = transform.parent.FindChild("ShareButton").GetComponent<ShareButtonBehaviour>();





// 	}


//     void SetVisibility(bool value) {
//         visible = value;
//         image.enabled = value;
// 		button.enabled = value;
// 		justAContainer.gameObject.SetActive(value);
//     }


//     void OnEnable() {

// 		SetVisibility(false); //ja bús video gatavs, tad rádís pogu


// 		if(Startup.Initialized){



//             if(triedLevel15 == false && DataManager.Levels[PREREQUISITE_LEVEL].Tried){ //tikko izbraucis 15. límeni
// 				//print("RewardedVideoBehaviour::just now tried level 015");
// 				lastVideoSeen = DateTime.Now - TimeSpan.FromSeconds(60); //jágaida 60 sek uz naḱamo video
// 			}
//             triedLevel15 = DataManager.Levels[PREREQUISITE_LEVEL].Tried;


// 			if(triedLevel15 != true){
// 				//print("RewardedVideoBehaviour::!a___015");
// 				return;
// 			}

// 			if(Time.realtimeSinceStartup < 60){ //ne mazák ká 1 min kopś startéśanas
// 				//print("RewardedVideoBehaviour::Time Since Startup=" + Time.realtimeSinceStartup);
// 				return;
// 			}

// 			if((DateTime.Now - lastVideoSeen).TotalSeconds < 180 ){ //ne mazák ká 2 min kopś pédéjá video (tas reizé ir 1 min kopś sasniegts 15. límenis, ja tas ir tikko sasniegts)
// 				//print("RewardedVideoBehaviour::Last Video just seen " + (DateTime.Now - lastVideoSeen).TotalSeconds );
// 				return;
// 			}


// 			// if(HeyZapManager.IsRewardedReady()){
// 			// 	//print("RewardedVideoBehaviour::video is ready");
// 			// } else {
// 			// 	//print("RewardedVideoBehaviour::no video, no fun");
// 			// 	#if !UNITY_EDITOR
// 			// 	return; //redaktorá piedos, ka nav iefečots (jo te nemaz nevar iefečot)
// 			// 	#endif
// 			// }


// 			SetVisibility(true);
// 		}

//     }

// //	void Update(){

// //		if(visible){
// 			//párpozicioné pogu - savá vietá vai augśéjás pogas vietá (ja tá nav redzama)  --nepietiek to darít OnEnable, jo augśéjás pogas redzamíba tad vél nav zináma
// //			if(ShareButton.Visible && uptown != false) {
// //				print("RewardedVideoBehaviour::Visible Share - standatd pos.");
// //				justAContainerRect.localPosition = new Vector2(0, 0);
// //				uptown = false;
// //			} else if(!ShareButton.Visible && uptown != true) {
// //				print("RewardedVideoBehaviour::Not Visible Share - uptown pos.");
// //				justAContainerRect.localPosition = new Vector2(0, 67f);
// //				uptown = true;
// //			}
// //		}

// //	}



// }

}
