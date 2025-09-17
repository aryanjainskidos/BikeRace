namespace vasundharabikeracing {
// using UnityEngine;
// using System.Collections;
// //#if !UNITY_IOS
// using GoogleMobileAds.Api;


// public class AdMobManager : MonoBehaviour {

// 	static InterstitialAd interstitial;

// 	#if UNITY_ANDROID
// 	static string adUnitId = "ca-app-pub-9469960157099009/5957441577";
// 	#elif UNITY_IPHONE
// 	static string adUnitId = "ca-app-pub-9469960157099009/6040422771";
// 	#else
// 	static string adUnitId = "unexpected_platform";
// 	#endif


// 	public static void Init(){
// 		LoadInterstitial(); //lai ieládé 1. interstiśáli uzreiz
// 	}

// 	public static bool IsIntersitialReady(){
// 		return interstitial.IsLoaded();
// 	}

// 	public static void ShowInterstitial(){
// 		if(interstitial.IsLoaded()){
// 			interstitial.Show();//paráda.. 	
// 			LoadInterstitial(); //.. un ládé nákamo
// 			print("AdMob: Showing Interstitial");
// 			TelemetryManager.EventAdShown("interstitial-admob");
// 		} else {
// 			print("AdMob: Not Showing Interstitial - not yet ready");
// 			TelemetryManager.EventAdNotShown("interstitial-admob");
// 		}

// 	}

// 	public static void LoadInterstitial()
// 	{
// 		interstitial = new InterstitialAd(adUnitId);		
// 		AdRequest request = new AdRequest.Builder()
// 			//.AddTestDevice(AdRequest.TestDeviceSimulator)
// 			//.AddTestDevice("2077ef9a63d2b398840261c8221a0c9b")
// 			.Build();

// 		interstitial.LoadAd(request);
// 	}

// }

// /*
// //# else 
// //iOSam ir dummy klase
// public class AdMobManager : MonoBehaviour { 
// 	public static void Init(){}
// 	public static bool IsIntersitialReady(){return false;}
// 	public static void ShowInterstitial(){}
// }
// //# endif
// */
}
