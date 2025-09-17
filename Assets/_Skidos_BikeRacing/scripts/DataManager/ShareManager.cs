namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
// #if !UNITY_IOS
// using com.playGenesis.VkUnityPlugin;
// #endif
using SimpleJSON;
// using Facebook.Unity;
using System.Collections.Generic;

//just Like and Share, boys, just Like and Share
public class ShareManager : MonoBehaviour
{

    public static void ShareToFacebook()
    {

        // if(!FB.IsInitialized){
        // 	FB.Init(FBInitCallback, null,null);
        // } else {
        // 	FBInitCallback();		
        // }
    }


    private static void FBInitCallback()
    {
        // if(!FB.IsLoggedIn){
        //     FB.LogInWithReadPermissions(new List<string>() { "" }, FBLoginCallback);
        // } else {
        // 	Share();
        // }
    }

    // private static void FBLoginCallback(IResult result){

    // 	if (result.Error != null){
    // 		if(Debug.isDebugBuild){Debug.Log("++Error Response:\n" + result.Error);}
    // 		//neko nedaram - lai pats aizver popupu
    // 	} else if (!FB.IsLoggedIn)		{
    // 		if(Debug.isDebugBuild){Debug.Log("++Login cancelled by Player");}
    // 		//arí neko nedaram, lai pats aizver popupu
    // 	} else {
    // 		if(Debug.isDebugBuild){Debug.Log("++Login was successful!");}
    // 		Share();
    // 	}

    // }

    private static void Share()
    {
        // FB.FeedShare(
        //     string.Empty,
        //     link: new System.Uri("http://www.bikeupgame.com/" + GetFBLinkPostfix()),
        // 	linkName: Lang.Get("Share:FB:Line1"), //virsraksts / pirmá rinda 
        // 	linkCaption: Lang.Get("Share:FB:Line2"), // otraa rinda teksta
        // 	linkDescription: Lang.Get("Share:FB:Line3"), //treśá rinda teksta
        // 	//picture: "",
        // 	callback: FBDone
        // 	);
    }

    // private static void FBDone(IResult response) {
    // 	//{"post_id":"885079611520951_1027297597299151"}  -- veiksmígs FB posts  (daźádi varianti uz daźádám platformám un daźádiem postéśanas veidiem :\
    // 	if( 
    // 	   (response.RawResult.Contains("id") || response.RawResult.Contains("post_id") || response.RawResult.Contains("posted") )
    // 	   && !response.RawResult.Contains("cancelled") ) { 
    // 		DataManager.Levels[GameManager.SelectedLevelName].Shared = true;
    // 		UIManager.ToggleScreen(GameScreenType.PopupUnlockBonusLevels); //tad aizver lodzińu ..
    // 		GameManager.ExecuteCommand(GameCommand.PlaySpecificLevel); // .. un piestarté izvéléto límeni
    //         IncrementShareCount();
    // 		TelemetryManager.EventSharedFB("ok");
    // 	} else {
    // 		TelemetryManager.EventSharedFB("notok");
    // 	}
    // 	if(Debug.isDebugBuild){print("ShareManager::FBDone:" + response.RawResult);}
    // }



    // 	public static void ShareToTwitter(){
    //         string textToDisplay = Lang.Get("Share:TWITTER:"+GameManager.SelectedLevelName) + " http://www.bikeupgame.com/in/t/";

    //         // TwitterManager.Send(textToDisplay, TwitterDone);

    // //		float startTime;
    // //		startTime = Time.timeSinceLevelLoad;
    // //
    // //		Application.OpenURL("twitter://post?message=" + textToDisplay);
    // //		
    // //		if(Time.timeSinceLevelLoad - startTime <= 1f) {
    // //			//fail, Open browser (@note the escaped text)
    // //			Application.OpenURL("http://twitter.com/intent/tweet?text=" + WWW.EscapeURL(textToDisplay));
    // //		}

    // 	}

    // private static void TwitterDone(bool success) {
    //     if(success) { 
    //         DataManager.Levels[GameManager.SelectedLevelName].Shared = true;
    //         UIManager.ToggleScreen(GameScreenType.PopupUnlockBonusLevels);
    //         GameManager.ExecuteCommand(GameCommand.PlaySpecificLevel); //péc fokusa atgúśanas starté nepiecieśamo límeni (ParamS1)
    // 		TelemetryManager.EventSharedTW("ok");
    //     } else {
    // 		TelemetryManager.EventSharedTW("notok");
    // 		if(Debug.isDebugBuild){Debug.Log("Failed to send a tweet...");}
    //     }
    // }

    #region twitter_free_coins
    //     public static void ShareToTwitterForFreeCoins(){
    //         string textToDisplay = Lang.Get("Share:TWITTER") + " http://www.bikeupgame.com/in/t/";

    // #if UNITY_EDITOR
    //         print("Twitter only works on device: " + textToDisplay);
    //         AwardFreeCoins();
    // #else
    //         //TwitterManager.Send(textToDisplay, TwitterForFreeCoinsDone);
    // #endif
    //     }

    // private static void TwitterForFreeCoinsDone(bool success) {
    //     if(success) { 
    //         AwardFreeCoins();
    //         TelemetryManager.EventSharedTW("coins-ok");
    //     } else {
    //         TelemetryManager.EventSharedTW("coins-notok");
    //         if(Debug.isDebugBuild){Debug.Log("Failed to send a tweet...");}
    //     }
    // }

    public static int ShareCoinReward = 7500;
    public static bool ShareMPReward = false;

    //    static void AwardFreeCoins() {
    //        UIManager.ToggleScreen(GameScreenType.PopupFreeCoins);
    //        DataManager.Coins += ShareCoinReward;
    //		TelemetryManager.EventShareReward(ShareCoinReward);
    //
    //        if(ShareMPReward) {
    //            DataManager.MultiplayerFreeCoinsReceived = true;
    //        } else {
    //            DataManager.FreeCoinsReceived = true;
    //        }
    //    }
    #endregion

    #region facebook_free_coins
    //     public static void ShareToFacebookForFreeCoins(){
    // #if UNITY_EDITOR
    //         print("Facebook only works on device.");
    //         AwardFreeCoins();
    // #else
    //         if(!FB.IsInitialized){
    //             FB.Init(FBInitCallbackForFreeCoins, null,null);
    //         } else {
    //             FBInitCallbackForFreeCoins();       
    //         }
    // #endif
    //     }


    // private static void FBInitCallbackForFreeCoins(){
    //     if(!FB.IsLoggedIn){
    // 		FB.LogInWithReadPermissions(new List<string>() { "" }, FBLoginCallbackForFreeCoins);
    //     } else {
    //         ShareForFreeCoins();
    //     }
    // }

    // private static void FBLoginCallbackForFreeCoins(IResult result){

    //     if (result.Error != null){
    //         if(Debug.isDebugBuild){Debug.Log("++Error Response:\n" + result.Error);}
    //         //neko nedaram - lai pats aizver popupu
    //     } else if (!FB.IsLoggedIn)      {
    //         if(Debug.isDebugBuild){Debug.Log("++Login cancelled by Player");}
    //         //arí neko nedaram, lai pats aizver popupu
    //     } else {
    //         if(Debug.isDebugBuild){Debug.Log("++Login was successful!");}
    //         ShareForFreeCoins();
    //     }

    // }

    // private static void ShareForFreeCoins(){
    //     FB.FeedShare(
    //         string.Empty,
    //         link: new System.Uri("http://www.bikeupgame.com/" + GetFBLinkPostfix()),
    //         linkName: Lang.Get("Share:FB:Line1"), //virsraksts / pirmá rinda 
    //         linkCaption: Lang.Get("Share:FB:Line2"), // otraa rinda teksta
    //         linkDescription: Lang.Get("Share:FB:Line3"), //treśá rinda teksta
    //         //picture: "",
    //         callback: FBDoneForFreeCoins
    //         );
    // }

    // private static void FBDoneForFreeCoins(IResult response) {

    //     bool posted = (response.RawResult.Contains("id") || response.RawResult.Contains("post_id") || response.RawResult.Contains("posted") );
    //     bool cancelled = response.RawResult.Contains("cancelled");

    //     if(posted && !cancelled) { 
    //         AwardFreeCoins();
    //         IncrementShareCount();
    // 		TelemetryManager.EventSharedFB("coins-ok");
    //     } else {
    // 		TelemetryManager.EventSharedFB("coins-notok");
    //     }
    //     if(Debug.isDebugBuild){print("ShareManager::FBDone:" + response.RawResult);}
    // }
    #endregion

    public static int FacebookShareCount = 0;

    private static void IncrementShareCount()
    {
        FacebookShareCount++;

        if (FacebookShareCount > 2)
        {
            FacebookShareCount = 0;
        }

        //it would probably be a good idea to save here
        BikeDataManager.Flush(); //save results
    }

    // private static string GetFBLinkPostfix() {
    //     string postfix;

    //     switch (FacebookShareCount)
    //     {
    //         case 1:
    //             postfix = "fba/";
    //             break;
    //         case 2:
    //             postfix = "fbb/";
    //             break;
    //         default:
    //             postfix = "";
    //             break;
    //     }

    //     return postfix;
    // }




    // public static void ShareToVKForFreeCoins(){
    // 	#if !UNITY_IOS
    // 	SetupAndPostToVK(delegate(){//success callback:
    // 		AwardFreeCoins();
    // 		DataManager.Flush();
    // 		TelemetryManager.EventSharedVK("coins-ok");
    // 	}, delegate(){//failure callback:
    // 		TelemetryManager.EventSharedVK("coins-notok");
    // 	});
    // 	#endif
    // }

    // public static void ShareToVK(){
    // 	#if !UNITY_IOS
    // 	SetupAndPostToVK(delegate(){//success callback:
    // 		DataManager.Levels[GameManager.SelectedLevelName].Shared = true;
    // 		UIManager.ToggleScreen(GameScreenType.PopupUnlockBonusLevels);
    // 		GameManager.ExecuteCommand(GameCommand.PlaySpecificLevel);
    // 		TelemetryManager.EventSharedVK("ok");
    // 	}, delegate(){//failure callback:
    // 		TelemetryManager.EventSharedVK("notok");
    // 	});
    // 	#endif
    // }

    // #if !UNITY_IOS
    // static VkApi vkapi;
    // //static Downloader d;
    // static VkSettings sets;

    // public static void SetupAndPostToVK(System.Action SuccessCallback, System.Action FailureCallback){

    // 	SetupVK();


    // 	// ja nav ielogojies, tad uzsitu logina kolbeku un sútu ielogoties
    // 	if(vkapi.TokenValidFor() < 120) {		
    // 		vkapi.LoggedIn += () => { PostToVK(SuccessCallback,FailureCallback); };
    // 		vkapi.Login();		
    // 	} else {
    // 		PostToVK(SuccessCallback,FailureCallback);
    // 	}

    // }

    // public static void SetupVK(){

    // 	if(GameObject.FindObjectOfType<Downloader>() == null){
    // 		//Debug.LogWarning("nav VkApi objekta, instanceeshu");

    // 		GameObject prefab = Resources.Load("Prefabs/VkApiForMobile/VkApi") as GameObject;
    // 		GameObject apiObj = Instantiate(prefab) as GameObject;
    // 		apiObj.name = "VkApi";

    // 		//śie 3 skripti pieder VkApi prefabam, kas tiko ir instancéts 
    // 		vkapi=GameObject.FindObjectOfType<VkApi>();
    // 		//d = GameObject.FindObjectOfType<Downloader>();
    // 		sets = GameObject.FindObjectOfType<VkSettings>();


    // 	} else {
    // 		//print("ir VkApi objekts");
    // 	}

    // }


    // private static void PostToVK(System.Action SuccessCallback, System.Action FailureCallback){
    // 	//Debug.Log("VK::PostToVK");
    // 	string msg = Lang.Get("Share:FB:Line1") + "\n" + Lang.Get("Share:FB:Line2") + "\n" + Lang.Get("Share:FB:Line3");
    // 	var r="wall.post?message="+msg + "&attachments=photo320521979_379259857,http://www.bikeupgame.com/";
    // 	vkapi.Call(r, (VkResponseRaw arg1, object[] arg2)=>{
    // 		//print("wall.post atbilde:" + arg1.text);

    // 		JSONNode N = JSON.Parse(arg1.text);
    // 		if(N["Error"] != null){
    // 			//print("Error:"+ N["Error"]["Code"] + " " + N["Error"]["error_msg"]);
    // 			FailureCallback();
    // 		} else if(N["response"] != null) {
    // 			//print("Response: post_id="+ N["response"]["post_id"]);
    // 			SuccessCallback();

    // 		} else {
    // 			//print("dunno");
    // 			FailureCallback();
    // 		}



    // 	});

    // }
    // #endif


}

}
