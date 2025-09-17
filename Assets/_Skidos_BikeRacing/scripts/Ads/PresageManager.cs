namespace vasundharabikeracing {
// #pragma warning disable 0618
// using UnityEngine;
// using System.Collections;
// using System;

// #if UNITY_ANDROID
// public class PresageManager
// {

//     static bool hadSfx = false;
//     static bool hadMusic = false;

//     static bool adOpened = false;
//     static bool adFound = false;
// 	static bool adNotFound = false;

// 	static MonoBehaviour mb;

//     static WHandlerImpl handlerImpl = new WHandlerImpl();

//     // public static event Action InterstitialNotFound;

//     static public void Init(Action AdFoundCallback, Action AdNotFoundCallback)
//     {
//         Debug.Log("PresageManager::Init");

//         // Initializing WPresage
//         WPresage.Initialize();

//         handlerImpl.AdFound += HandleInterstitialFound;
//         handlerImpl.AdFound += AdFoundCallback;

//         handlerImpl.AdNotFound += HandleInterstitialNotFound;
//         handlerImpl.AdNotFound += AdNotFoundCallback;//HandleInterstitialNotFound;
//         handlerImpl.AdDisplayed += HandleInterstitialOpened;
//         handlerImpl.AdClosed += HandleInterstitialClosed;

//     }

//     private static void HandleInterstitialFound()
//     {
//         Debug.Log("PresageManager::HandleInterstitialFound");
//         WPresage.ShowInterstitial(handlerImpl);
//         adFound = true;
//     }

//     #region supersonic listeners
//     static void HandleInterstitialNotFound()
//     {
//         Debug.Log("PresageManager::HandleInterstitialNotFound");
//         adNotFound = true;
//     }


//     static void HandleInterstitialClosed()
//     {
//         Debug.Log("PresageManager::InterstitialAdClosedEvent");

//         //ielieku atpakaĺ vai bija muz/efekti, audio sáksies tiklídz bús nepiecieśamíba péc tá
// 		DataManager.SettingsSfx = hadSfx;
// 		DataManager.SettingsMusic = hadMusic;

//         // The ad being shown has finished using audio.
//         // You can resume any background music.
//         adOpened = false;
//     }

//     /*
// 	* Invoked when the Interstitial Ad Unit has opened
// 	*/
//     static void HandleInterstitialOpened()
//     {
//         Debug.Log("PresageManager::InterstitialAdOpenedEvent");
//         //pieraksta vai bija muz/efekti
//         hadSfx = DataManager.SettingsSfx;
// 		hadMusic = DataManager.SettingsMusic;

//         //izslédz visu
// 		DataManager.SettingsSfx = false;
// 		DataManager.SettingsMusic = false;


//         // The ad being shown will use audio. Mute any background music

//         adOpened = true;
//     }
//     #endregion


//     public static void ShowInterstitial()
//     {
//         Debug.Log("PresageManager::ShowInterstitial");
//         adFound = false;
// 	    adNotFound = false;

// //		mb.StartCoroutine( CheckOnOpen() );
// //        WPresage.AdToServe("interstitial", handlerImpl);
//         WPresage.AdToServe(handlerImpl);
//     }

// 	public static IEnumerator CheckOnOpen(){
// 		while(!adOpened && !adNotFound) //if either ad found or not found, exit this loop
// 		{
// 			yield return null;
// 		}

// 		if(adOpened)
//         {
// 			SoundManager.StopAmbience();
//             SoundManager.StopSfx();
// 		}
// 	}

// }

// /**
//  * Implementation of the Handler used for AdToServe
//  */
// public class WHandlerImpl : WPresage.IADHandler {

//     public event Action AdNotFound;
//     public event Action AdFound;
//     public event Action AdClosed;
//     public event Action<int> AdError;
//     public event Action AdDisplayed;

//     public void OnAdNotFound() {
//         Debug.Log ("onAdNotFound");

//         if(AdNotFound != null)
//             AdNotFound();
//     }

//     public void OnAdFound() {
//         Debug.Log ("onAdFound");

//         if(AdFound != null)
//             AdFound();
//     }

//     public void OnAdClosed() {
//         Debug.Log ("onAdClosed");
//         if(AdClosed != null)
//             AdClosed();
//     }

//     public void OnAdError(int code) {
//         Debug.Log ("onAdError");
//         if(AdError != null)
//             AdError(code);
//     }

//     public void OnAdDisplayed() {
//         Debug.Log ("onAdDisplayed");
//         if(AdDisplayed != null)
//             AdDisplayed();
//     }
// }
// #endif
}
