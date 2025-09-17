namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/**
 * Statisks menedźeris,
 * kas sútís spéles lietośanas statistiku serverim
 */
public class TelemetryManager : MonoBehaviour
{

    /*
		@todo -- MP: sútít papildus karodzińu, ka notikums ir MP
		@todo -- aizvákt gandríz visus eventus, tiklídz beidzam testéśanu
	*/


    public const string ServerUrlTelemetry = "http://stats.bikeupgame.com";

    public static Dictionary<string, int> Levelstarts = new Dictionary<string, int>(); // te uzglabás info par to cik reizes katra trase ir restartéta (arí startéta) daźreiz tiks nosútíts uz serveri un iztíríts

    static MonoBehaviour mb;//haxxx: man vajag MonoBehaviour, lai varétu ko-rutínas piestarté statiská klasé

    static public void Init()
    {
        mb = GameObject.Find("Main Camera").GetComponent<MonoBehaviour>();
    }


    /**
	 * zińo serverim, ka ieslégta appa
	 * @note -- vismaz vienu telemetrijas eventu ir jáatstáj (ja visus izslédz árá ká nevajadzígus) - lai push notifikáciju tokenus padotu serverim
	 */
    public static void EventStartup()
    {
        WWWForm form = new WWWForm();
        InformServer("startup", form);
    }


    public static void EventPowerBoostEnabled()
    {
        WWWForm form = new WWWForm();
        InformServer("powerboost", form);
    }

    public static void EventBikeForStarsStarted()
    {
        WWWForm form = new WWWForm();
        InformServer("b4stars_st", form);
    }

    public static void EventBikeForStarsCompleted()
    {
        WWWForm form = new WWWForm();
        InformServer("b4stars_en", form);
    }

    /**
	 * sákta [arí restartéta spéle]
	 * śie mazsvarígie eventi tiks savákti Dictá un nosútíti visi reizé, kopá ar kádu svarígáku eventu
	 */
    public static void EventLevelstart(string levelName)
    {
        if (!Levelstarts.ContainsKey(levelName))
        {
            Levelstarts[levelName] = 1;
        }
        else
        {
            Levelstarts[levelName]++;
        }
    }


    /**
	 * zińo serverim par izbrauktu trasi
	 */
    public static void EventFinished(string trackName, float time, int coins, int stars)
    {

        //#if UNITY_IOS
        // uz iOS par límeństartiem zińo tikai waitlistétajiem límeńiem
        string trackNameLower = trackName.ToLower();
        if (trackNameLower != "a___010" &&
           trackNameLower != "a___020" &&
           trackNameLower != "a___030" &&
           trackNameLower != "a___035" &&
           trackNameLower != "a___040" &&
           trackNameLower != "a___045" &&
           trackNameLower != "a___050" &&
           trackNameLower != "a___055" &&
           trackNameLower != "a___060" &&
           trackNameLower != "a___065" &&
           trackNameLower != "a___070" &&
           trackNameLower != "a___075" &&
           trackNameLower != "a___080" &&
           !trackNameLower.Contains("bonuss") &&
           !trackNameLower.Contains("long")
           )
        {
            //print("skipping levelfinish telemetry for level " + trackName);
            // return; 
        }
        //#endif

        WWWForm form = new WWWForm();

        form.AddField("TrackName", trackName);
        form.AddField("FinishTime", time.ToString());
        form.AddField("FinishCoins", coins.ToString());

        if (trackName.ToLower().Contains("long"))
        { //LONG límeńiem zvaigźnju vietá súta cik chekpointi izbraukti
            form.AddField("FinishStars", BikeGameManager.checkpointsReached);
        }
        else
        {
            form.AddField("FinishStars", stars.ToString());
        }

        string boosts = "boosts:" +
            BikeDataManager.Boosts["ice"].Number + "," +
                BikeDataManager.Boosts["magnet"].Number + "," +
                BikeDataManager.Boosts["invincibility"].Number + "," +
                BikeDataManager.Boosts["fuel"].Number;
        form.AddField("Info", boosts);

        InformServer("finished", form);
    }



    /**
	 * zińo serverim, ka noporkts apgreids #id
	 */
    public static void EventUpgrade(int id, int level)
    {
        //nesútam
        //WWWForm form = new WWWForm();
        //form.AddField("UpgradeID", id);
        //form.AddField("UpgradeLevel", level);
        //InformServer("upgrading", form);
    }

    /**
	 * zińo serverim, ka nopirkts/izaudzéts bústs #id, waited = vai gaidíja (false - negaidíja, nopirka)
	 */
    public static void EventBoostEnd(int id, bool waited)
    {

#if UNITY_IOS
        return; //nezińo par śo eventu uz iOSa
#else
		WWWForm form = new WWWForm();
		form.AddField("BoostID", id);
		form.AddField("Waited", waited ? "1" : "0");
		
		string boosts = "boosts:" + 
			DataManager.Boosts["ice"].Number + "," +
				DataManager.Boosts["magnet"].Number + "," +
				DataManager.Boosts["invincibility"].Number + "," +
				DataManager.Boosts["fuel"].Number;
		form.AddField("Info", boosts);		
		InformServer("boost_end", form);		
#endif

    }

    /**
	 * zińo serverim, ka saak audzeet bústu #id
	 */
    public static void EventBoostStart(int id)
    {
#if UNITY_IOS
        return; //nezińo par śo eventu uz iOSa
#else

		WWWForm form = new WWWForm();
		form.AddField("BoostID", id);
		string boosts = "boosts:" + 
			DataManager.Boosts["ice"].Number + "," +
				DataManager.Boosts["magnet"].Number + "," +
				DataManager.Boosts["invincibility"].Number + "," +
				DataManager.Boosts["fuel"].Number;
		form.AddField("Info", boosts);
		InformServer("boost_start", form);
#endif
    }

    /**
	 * źińo serverim, ka ielúgti num cilvéki un sańemti bonusi
	 */
    public static void EventInviting(int num)
    {
        WWWForm form = new WWWForm();
        form.AddField("NumInvited", num);
        InformServer("inviting", form);
    }

    /**
	 * kleimots achívments
	 */
    public static void EventAchievementClaiming(string id)
    {
        //nesútam
        //WWWForm form = new WWWForm();
        //form.AddField("Info", id);
        //InformServer("claiming", form);
    }

    /**
	 * źińo serverim, ka saglabáta mocha párkrásośana / nomainíśana
	 */
    public static void EventStyleChanging(string styleName, string coloring)
    {
        //	nesutam
        //	WWWForm form = new WWWForm();
        //	form.AddField("StyleName", styleName);
        //	form.AddField("Info", coloring);
        //	InformServer("style", form);
    }


    /**
	 * nopirkts IAP
	 */
    public static void EventPurchasing(string id)
    {
        //	nesutam
        //	WWWForm form = new WWWForm();
        //	form.AddField("Info", id);
        //	InformServer("purchasing", form);
    }

    /**
	 * tikko unlokotas visas trases
	 */
    public static void EventTracksUnlocking()
    {
        WWWForm form = new WWWForm();
        InformServer("unlocking", form);
    }

    /**
	 * pazińo serverim, ka parádíta rekláma 
	 * "interstitial", "rewarded" or smtn
	 */
    public static void EventAdShown(string type)
    {
        WWWForm form = new WWWForm();
        form.AddField("Info", type);
        InformServer("adshown", form);
    }

    /**
	 * pazińo serverim, ka bija járáda rekláma, bet tá nebija piegádáta
	 * "interstitial", "rewarded" or smtn
	 */
    public static void EventAdNotShown(string type)
    {
        WWWForm form = new WWWForm();
        form.AddField("Info", type);
        InformServer("adnotshown", form);
    }

    public static void EventSharedFB(string outcome)
    {
        WWWForm form = new WWWForm();
        form.AddField("Info", outcome);
        InformServer("sharefb", form);
    }

    public static void EventSharedTW(string outcome)
    {
        WWWForm form = new WWWForm();
        form.AddField("Info", outcome);
        InformServer("sharetw", form);
    }

    public static void EventSharedVK(string outcome)
    {
        WWWForm form = new WWWForm();
        form.AddField("Info", outcome);
        InformServer("sharevk", form);
    }

    public static void EventWOF(string prize)
    {
        //	nesutam
        //	WWWForm form = new WWWForm();
        //	form.AddField("Info", prize);
        //	InformServer("wof", form);
    }

    public static void EventShareReward(int ammount)
    {
        WWWForm form = new WWWForm();
        form.AddField("Info", ammount);
        InformServer("sharerwrd", form);
    }
    public static void EventRestoredGamedata(int levels)
    {
        WWWForm form = new WWWForm();
        form.AddField("Info", levels);
        InformServer("restoredgd", form);
    }

    private static void InformServer(string eventType, WWWForm form)
    {

        try
        {
            double now = (System.DateTime.Now - new System.DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds; //taimstamps ar decimáldaĺám
            double lastSysInfoSentAt = System.Convert.ToDouble(PlayerPrefs.GetString("sysInfoSent", "0"));// PlayerPrefos stringá pierakstu taimstampu, kad pédéjoreiz sútíta info



            // ++++ ja sen nav sútíta sistéminfo (vai nav vispár) ++++
            if ((now - lastSysInfoSentAt) > 86000)
            { //laiks kopś pédéjás reizes ir lieláks par n dienám (vai arí nekad nav sútíts)

                string token = PlayerPrefs.GetString("PushNotificationToken", "");
                if (token == null || token.Length == 0)
                { //ja iesákot spéli, nav tokena, tad to noskaidros un nosútíts serverim péc neilga mirkĺa
                  //				print("TT nebija tokena");
                    mb.StartCoroutine(GetAndSendNotificationToken());
                }
                else
                {
                    //				print("TT bija tokens="+token);
                }

                form.AddField("token", token);
                form.AddField("SysInfo", "1");
                form.AddField("SysInfo_deviceModel", SystemInfo.deviceModel.ToString());
                form.AddField("SysInfo_graphicsDeviceName", SystemInfo.graphicsDeviceName.ToString());
                form.AddField("SysInfo_graphicsDeviceVendor", SystemInfo.graphicsDeviceVendor.ToString());
                form.AddField("SysInfo_graphicsMemorySize", SystemInfo.graphicsMemorySize.ToString());
                form.AddField("SysInfo_graphicsPixelFillrate", SystemInfo.graphicsPixelFillrate.ToString());
                form.AddField("SysInfo_operatingSystem", SystemInfo.operatingSystem.ToString());
                form.AddField("SysInfo_processorCount", SystemInfo.processorCount.ToString());
                form.AddField("SysInfo_processorType", SystemInfo.processorType.ToString());
                form.AddField("SysInfo_systemMemorySize", SystemInfo.systemMemorySize.ToString());
                form.AddField("SysInfo_systemLanguage", Application.systemLanguage.ToString());
                form.AddField("SysInfo_preferedLanguage", Lang.PreferedLanguage);


                System.TimeZone zone = System.TimeZone.CurrentTimeZone;
                System.TimeSpan offset = zone.GetUtcOffset(System.DateTime.Now);
                form.AddField("SysInfo_tzOffsetMinutes", (offset.Hours * 60 + offset.Minutes).ToString());





                PlayerPrefs.SetString("sysInfoSent", now.ToString());
                //print("sysInfo!Sent, sending");
            }
            else
            {
                //print("sysInfoSent, skipping");
            }


            // ++++ obligátie lauki visiem eventiem ++++

            form.AddField("UID", SystemInfo.deviceUniqueIdentifier);
            form.AddField("FBID", MultiplayerManager.FBID);
            form.AddField("EventType", eventType); //DB ir 1 simbolu garuma ierobeźojums
            form.AddField("Version", GameVersionManager.V);
            form.AddField("NumCoins", BikeDataManager.Coins.ToString());
            form.AddField("NumStars", BikeDataManager.Stars.ToString());
            //@todo -- sakráto bústińu skaitus


            // ++++ [re-]startu info (if any) ++++
            foreach (KeyValuePair<string, int> s in Levelstarts)
            {

                //#if UNITY_IOS
                // uz iOS par límeństartiem zińo tikai waitlistétajiem límeńiem
                if (s.Key != "a___010" &&
                   s.Key != "a___020" &&
                   s.Key != "a___030" &&
                   s.Key != "a___035" &&
                   s.Key != "a___040" &&
                   s.Key != "a___045" &&
                   s.Key != "a___050" &&
                   s.Key != "a___055" &&
                   s.Key != "a___060" &&
                   s.Key != "a___065" &&
                   s.Key != "a___070" &&
                   s.Key != "a___075" &&
                   s.Key != "a___080" &&
                   !s.Key.ToLower().Contains("bonuss") &&
                   !s.Key.ToLower().Contains("long"))
                {
                    //print("skipping Levelstarts telemetry for level " + s.Key);
                    // continue; 
                }
                //#endif

                form.AddField("Levelstarts[" + s.Key + "]", s.Value);
            }
            Levelstarts = new Dictionary<string, int>(); //reset


            mb.StartCoroutine(SendInfo(form));
        }
        catch (System.Exception ex)
        {
            Debug.Log("Exception in InformServer: " + ex);
        }


    }




    private static IEnumerator SendInfo(WWWForm form)
    {

        MultiplayerManager.SignRequest(ref form); //paraksta pieprasíjumu

        WWW www = new WWW(ServerUrlTelemetry + "/skidos/skidos.php", form);
        yield return www;
        if (Debug.isDebugBuild) { print("got:" + www.text); }


    }

    private static IEnumerator GetAndSendNotificationToken()
    {

        yield return new WaitForSeconds(0.5f);//pagaidís pusssekundi, lai bútu drośs, ka serveris ir jau sańémis telemetrijas ierakstu (citádi nebús kam piekabinát klát tokenu)
        GameObject ECPNManagerGo = GameObject.Find("ECPNManager");
        if (ECPNManagerGo == null)
        {
            //			Debug.LogError("TT nav atrasts ECPNManager objekts!");
        }
        else
        {
            //			Debug.LogError("TT ir atrasts ECPNManager objekts, prasiis tokenu");
            //			ECPNManagerGo.GetComponent<ECPNManager>().RequestDeviceToken();
        }
    }
}




}
