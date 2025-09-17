namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
// using UnityEngine.Advertisements;
// using Heyzap;

public class AdManager : MonoBehaviour
{

    static int stage = 0;
    static int network = 1;
    static float countDown = 0;
    static MonoBehaviour mb;

    public static bool ShowRemoveAdsOffer = false;
    public static bool JustFinishedRace = false; // lai nerádítu vairák ká 1 reklému péc 1 izbrauktas trases (SP un MP)  | to uzsit LevelManager.KillLevel();

    static bool presageAdFound = false;
    static bool presageAdNotFound = false;
    static bool firstTime = true;

    static public void Init()
    {

        Debug.Log("Start admanager.");
        mb = GameObject.Find("Main Camera").GetComponent<MonoBehaviour>();
        mb.StartCoroutine(CountDownAndShowInterstitialAds());

        //SkidosSetupController.Instance.ShowSkidos();
        //SkidosRewardSystem.RewardEvent += new SkidosRewardSystem.RewardHandler(CollectReward);
        //UnityAds - mediétas arí caur HeyZap
        // if(Advertisement.isSupported)
        // {
        // 	#if UNITY_ANDROID
        //     Advertisement.Initialize("46309");//A: 46309
        // 	#elif UNITY_IOS
        //     Advertisement.Initialize("46310");//iOS: 46310
        // 	#endif
        // }
        //Debug.Log("AAAA)Advertisement.isSupported: " + Advertisement.isSupported);
        //Debug.Log("AAAA)Advertisement.isInitialized: " + Advertisement.isInitialized);


        // #if UNITY_ANDROID && !UNITY_EDITOR
        // 		PresageManager.Init(PresageAdFound, PresageAdNotFound);
        // #endif


    }

    private static void CollectReward(string rewardID, float rewardAmount)
    {
        if (rewardID == "CoinAnswer")
        {
            Debug.Log("pievieno naudu tagad + " + rewardAmount);
            BikeDataManager.Coins += Mathf.CeilToInt(rewardAmount);
        }
        else if (rewardID.Contains("CoinsAnswer"))
        {
            Debug.Log("rewardid - - " + rewardID);
            switch (rewardID)
            {
                case "CoinsAnswer5":
                    rewardAmount = 500;
                    break;
                case "CoinsAnswer10":
                    rewardAmount = 1500;
                    break;
                case "CoinsAnswer15":
                    rewardAmount = 3000;
                    break;
            }

            Debug.Log("pievieno naudu tagad caur shop + " + rewardAmount);
            BikeDataManager.Coins += Mathf.CeilToInt(rewardAmount);
        }
        else if (rewardID.Contains("compulsory"))
        {
            //            LevelLineupManager.data.AnsweredLevels();
            //            LevelLineupManager.data.UnlockLevels();
            if (LevelManager.CurrentLevelName != "" && BikeDataManager.Levels.ContainsKey(LevelManager.CurrentLevelName))
                BikeDataManager.Levels[LevelManager.CurrentLevelName].Tried = true;
            UIManager.SwitchScreen(GameScreenType.Levels);

        }
    }

    static void PresageAdFound() // probably should be in presage manager and not here
    {
        presageAdFound = true;
    }

    static void PresageAdNotFound()
    {
        presageAdNotFound = true;
    }

    // public static IEnumerator CheckOnPresage()
    // {
    // 	while (!presageAdNotFound && !presageAdFound) //if either ad found or not found, exit this loop
    // 	{
    // 		yield return null;
    // 	}

    // 	if (presageAdNotFound)
    // 		ShowPreloadedIntersitial();

    // 	if (presageAdFound)
    // 		TelemetryManager.EventAdShown("presage");

    // 	presageAdNotFound = false;
    // 	presageAdFound = false;
    // }

    static public void ResetTimer()
    {
        //print("AdManager::ResetTimer");
        stage = 0;
        countDown = 0;
    }

    //    static bool unityAdsInitialized = false;

    static IEnumerator CountDownAndShowInterstitialAds()
    {

        /*
		yield return new WaitForSeconds(2);
		HeyzapAds.showMediationTestSuite();
		yield break;//*/

        //Debug.Log("BBBB)Advertisement.isInitialized: " + Advertisement.isInitialized);
        //HeyzapAds.showMediationTestSuite();


        //print("AdManager: saaakam");

        float interval = 2;

        while (true)
        {
            yield return new WaitForSeconds(interval);
            countDown += interval;


            //Debug.Log("BBBB)Advertisement.isInitialized: " + Advertisement.isInitialized);
            //HeyzapAds.showMediationTestSuite();


            //            if(!unityAdsInitialized && Advertisement.isInitialized){
            //                Debug.Log("Advertisement.isInitialized: " + Advertisement.isInitialized);
            //                unityAdsInitialized = Advertisement.isInitialized;
            //            }

            // if(DataManager.HasBoughtAnything){ //ir kaut ko nopircis veikalá - nerádam automátiskás reklámas ..
            // 	#if !UNITY_ANDROID
            // 	//print("AdManager: skip - bought anything");
            // 	yield break; //..izńemot Androídam - tie lai pérk un turpina skatít reklámas >:)
            // 	#endif

            // }

            if (PopupShopBehaviour.IsOpen)
            { //ja ir atvérts popupśops, tad neráda reklámu un reseto taimeri
                countDown = 0;
                //print("AdManager: skip - PopupShop open");
                continue;
            }

            // if (!DataManager.Levels["a___001"].Tried)
            // { // ja nav braukta pirmaa trase, tad neráda reklámas
            //   //print("AdManager: skip - no first level");
            //     continue;
            // }

            // if (DataManager.Levels["a___006"].Tried && !DataManager.Levels["a___007"].Tried)
            // { // ja ir 6., bet vél nav 7. 
            //   //print("AdManager: skip:  6++ && 7--");
            //     continue;
            // }

            if (!JustFinishedRace)
            {
                //print("nav tikko izbraukta trase, nebús rekláma");
                continue;
            }


            // if((stage == 0 && countDown >= 30) // pirmá rekláma
            //    ||(stage == 1 && countDown >= 50) // otrá rekláma
            //    ||(stage == 2 && countDown >= 55) 
            //    ||(stage == 3 && countDown >= 70) 
            //    ||(stage == 4 && countDown >= 55) 
            //    ||(stage == 5 && countDown >= 55) 
            //    ||(stage >= 6 && countDown >= 55 && countDown % 2 != 0)// katra nákamá nepára skaita rekláma
            //    ||(stage >= 6 && countDown >= 55 && countDown % 2 == 0)// katra nákamá pára skaita rekláma
            //    ){

            if (true)
            {




                //print("tikliidz varam - jaaraada reklaama " + stage);
                if ((UIManager.currentScreenType == GameScreenType.Levels || UIManager.currentScreenType == GameScreenType.MultiplayerMenu) && !SpinManager.spinInProgress)
                {
                    //print("bujakasha - reklaama " + stage);
                    JustFinishedRace = false; //jáizbrauc vél viena trase, lai redzétu nákamo reklámu
                    stage++;
                    countDown = 0;

                    // #if UNITY_ANDROID
                    // PresageManager.ShowInterstitial(); // if it can't find an ad it'll call ShowPreloadedAd
                    // mb.StartCoroutine(CheckOnPresage());
                    // #else
                    // ShowPreloadedIntersitial();
                    // #endif
                    // Debug.Log("Sajaa vietaa radiis to jauno lietu----");
                    // SkidosRewardSystem.INSTANCE.StartWithSingleQuestions("CoinAnswer", 500, 2, 0, false, false);
                    //                    SkidosRewardSystem.INSTANCE.StartCompulsoryQuestions("CoinAnswer",1,500);

                }
            }

            //print("ar we there yet " + countDown + "  stage=" + stage + "   network=" + network );	


        }
    }

    // public static void ShowPreloadedIntersitial()
    // {
    // 	if(HeyZapManager.IsIntersitialReady()){
    // 		//print("HeyZap Intersitial IS Ready");
    // 		HeyZapManager.ShowInterstitial();
    // 		TelemetryManager.EventAdShown("interstitial-heyzap");
    // 		ShowRemoveAdsOffer = true;
    // 	} else {
    // 		//print("HeyZap Intersitial NOT Ready");
    // 		TelemetryManager.EventAdNotShown("interstitial-heyzap-not");

    // 		//nav heizaptíkla reklámas - fallbeks uz ne-mediétu AdMobu
    // 		if(AdMobManager.IsIntersitialReady()){
    // 			//print("AdMob Intersitial IS Ready");
    // 			AdMobManager.ShowInterstitial();
    // 			ShowRemoveAdsOffer = true;
    // 		} else {
    // 			AdMobManager.LoadInterstitial();
    // 			//print("AdMob Intersitial NOT Ready");
    // 		}

    // 	}
    // }


    public static void ShowAdTestScreen()
    {
        //print("ShowAdTestScreen On");
        // HeyzapAds.ShowMediationTestSuite();
    }

}
}
