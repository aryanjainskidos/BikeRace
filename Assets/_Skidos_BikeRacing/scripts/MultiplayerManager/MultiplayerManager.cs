namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System;
// using Facebook.Unity;
using System.Linq;

public class MultiplayerManager : MonoBehaviour
{

    public const string LAST_INVITE_TIMESTAMP_KEY = "LastInviteTimestamp";

    public static int DataID = 0; // sańemot jaunus datus no Servera, nomaina śo skaitli - lai visi lodzińi zin, kad apdeitot savu info
    public static int DataID_friends = 0;
    private static int currentlyDownloading = 0; //cik pieprasíjumi paślaik tiek lejupieládéti (pikçám un raidiem kopá)
    private static int maxCurrentlyDownloading = 5; //cik max vienlaicígi pieprasíjumi ar bút

    public const string ServerUrlMP = "http://mp.bikeupgame.com";

    public static bool? AppVersionOk = null; //uz servera noskaidros, vai appas versija ir visjaunáká (neĺaus spélét, ja nebús)
    public static bool LoggedIn = false; //ielogojies FB un MP serverí tikai 1x spéles dzíves laiká
    public static string PlayerName = ""; //uzzin no MP logina vai FB konekcijas
    public static string PlayerPicture = "";

    static int _playerTeamID = 0;
    public static int PlayerTeamID
    {
        set { _playerTeamID = value; PlayerPrefs.SetInt("PlayerTeamID", value); }
        get { return _playerTeamID; }
    }
    public static int[] TeamPopularity = { 1, 2, 3, 4 }; //kádá secíbá ir jásakárto komandas, serveris to izlémis izkaitot katras komandas dalíbniekus - tikai komandas izvélei
    public static int[] TeamCups = { 0, 0, 0, 0, 0 }; //katras komandas punkti aktívajá sezoná

    public static System.DateTime SeasonEndDate; //cikos beigsies sezona péc lokálá (iespéjams kĺúdainá laika) - párréḱina ikreiz sańemot jaunu TTL
    private static int _seasonTTL; //priváts mainígais, kas satur vértíbu, lai publiskais nedabútu bezgalígo ciklu ar saviem seteriem/geteriem
    public static int SeasonTTL
    { //cikos sekundes lídz sezonas beigám - sańem no servera - péc ielogośanás un braucienu submita	
        get
        {
            return _seasonTTL;
        }
        set
        {
            _seasonTTL = value;
            MultiplayerManager.SeasonEndDate = System.DateTime.Now;
            MultiplayerManager.SeasonEndDate = MultiplayerManager.SeasonEndDate.AddSeconds(_seasonTTL).ToLocalTime();
        }
    }
    public static int SeasonID = 0; //kura sezona paślaik rit
    public static JSONNode SeasonPrizes; // saraksts ar śís sezonas balvám.  |  tikai informatíva nozíme

    public static int PlayerLeagueContributionCups = 0; //spélétája paveiktais śajá sezoná
    public static int PlayerLeagueContributionGames = 0;

    public static int PrevSeasonID = 0;
    public static JSONNode PrevSeasonPrizes; // saraksts ar śís iepriekśéjás sezonas balvám.  |  visas śís dávanas tiks dávinátas, tiklídz te ielikta info (ja spélétája komanda bús uzvaréjusi)
    public static int PlayerPrevLeagueContributionCups = 0;
    public static int PlayerPrevLeagueContributionGames = 0;
    public static int[] PrevTeamCups = { 0, 0, 0, 0, 0 }; //katras komandas punkti iepriekśéjá sezoná
    public static int PrevSeasonWinnerID = 0; //kura komanda uzvaréja iepr. sezoná
    public static bool SendLeagueRidesToo = false; //vai sútít uz serveri arí raidińus lígas spélém (pie ielogośanás śo vértíbu uzzin no servera)

    //sekojośie mainígie tiek saglabáti PlayerPrefos:
    public static string MPID = "";//spélétája unikáls identifikators; ja nav tad izveido jaunu no UID, kad bús FB, tad śis tiks ignoréts
    public static bool HasFB = false; //spélés anoními vai ir jau sakonektéts ar FB
    public static string FBID = "0";
    public static int NumSP = 0; //cik ir izbrauktas SP trases (unikálas)
    public static int NumWins = 0; // cik reizes ir uzvaréjis (tikai lokáli skaita)
    public static int NumGames = 0; // cik reizes ir submitojis braucienus (tikai lokáli skaita)


    public static int CoinsPerWin
    {
        get
        {
            return BikeDataManager.PlayerMultiplayerLevels[MPLevel - 1].CoinsPerWin;
        }
    }

    public static int CoinsForLevellingUp
    {
        get
        {
            int reward = 0;
            if (MPLevel - 1 > BikeDataManager.MaxMultiplayerLevelReached)
            {
                reward = BikeDataManager.PlayerMultiplayerLevels[MPLevel - 2].CoinsForLevelUp; //-2 because we need the value from the last level and additional -1 because some have trouble counting from 0
            }
            return reward;
        }
    }

    private static System.DateTime lastTimeLevelUp = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
    public static int MPLevel = 1; //sákotnéjá vértíba glabájás playerprefos, bet tiklídz ir konekcija ar serveri un iegúti aktuálákie kausińi - párskaitís límeni | cilvécígs cipars, nevis no 0 skaitíts !

    private static int _cups;
    static bool leveledDownWithinXMinutesOfLevelingUp = false;

    static bool[] levelupSeen = new bool[] { false, false, false, false, false, false, false, false, false, false };

    public static bool recalculateMPLevel = false;

    public static int Cups
    { //sákotnéjá vértíba glabájás playerprefos, bet tiklídz ir konekcija ar serveri - tá párrakstís ar jaunáko vértíbu
        get
        {
            return _cups;
        }
        set
        {
            _cups = value;

            recalculateMPLevel = true;

            PlayerPrefs.SetInt("Cups", Cups);
        }
    }

    public static void RecalculateLevel()
    {

        int newMPLevel = 0;
        foreach (var mpLevel in BikeDataManager.PlayerMultiplayerLevels)
        { //atjaunotjot kausińu skaitu párrékjina MP límeni
            if (Cups >= mpLevel.Value.Cups)
            {
                newMPLevel = mpLevel.Key + 1;
            }
        }

        if (newMPLevel != MPLevel)
        { //ir mainíjies límenis
            if (newMPLevel > MPLevel)
            { // ir palielinájies
                MPLevel = newMPLevel;
                if (UIManager.currentScreenType != GameScreenType.Menu && UIManager.currentScreenType != GameScreenType.Garage)
                { //ja vien nav pirmais ekráns vai garáźa (kur śis popups izskatítos neglíts_

                    if (!levelupSeen[MPLevel - 1] && MPLevel > 1)
                    {
                        UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLevelUp, true);
                        levelupSeen[MPLevel - 1] = true;
                    }

                }
            }
            else
            {
                MPLevel = newMPLevel; //mainíjies, bet ne palielinájies
            }

        }
        PlayerPrefs.SetInt("MPLevel", MPLevel);
        PlayerPrefs.Save();

        //        print("!!! DataManager.MaxMultiplayerLevelReached " + DataManager.MaxMultiplayerLevelReached);
        if (MPLevel - 1 > BikeDataManager.MaxMultiplayerLevelReached)
        {

            //            print("!!! MPLevel - 1 > DataManager.MaxMultiplayerLevelReached");
            BikeDataManager.Coins += CoinsForLevellingUp;
            BikeDataManager.MaxMultiplayerLevelReached = MPLevel - 1; //don't call this before CoinsForLevellingUp
        }


        recalculateMPLevel = false;
    }


    public static int PowerRating = 0; //MP upgreidi ~ powerRating
    public static int PermanentPowerRating = 0; //MP upgreidi ~ powerRating - only permanent ones

    public static string[] FBFriendsPlaying = new string[0]; //FB draudzińu IDi, iegúst no /mp/get_friends
    public static bool FBFriendsDownloaded = false; // true, ja kaut reizi ir lejupieládéti draugi priekś draugu spélém (tos lietos arí FB invaitiem - lai nelúgtu jau esośos)

    public static int[] InvBonusAmmount = { 0, 20000, 30000, 45000, 55000, 100000 }; // cik maksásim par katru cilvéku (nullto ignoré)
    public static int InvBonusNum = 5; //tikai par pirmajiem N

    //śís listes tiek izveidotas par jaunu, katru reizi lejupieládéjot un attélojot attiecígo sarakstu
    public static List<MPOpponent> OpponentsStarted; //saraksts ar pretiniekiem,  ar kuriem ir iesákta spéle
                                                     //	public static List<MPOpponent> OpponentsNotStarted;  //saraksts ar pretiniekiem,  ar kuriem vél nav iesákta spéle
    public static MPOpponent CurrentOpponent; //tagadéjais pretienieks - tiek nomainíts, nospieźot PLAY pogu
    public static MPOpponent[] LeagueCandidateOpponents = { null, null, null }; //3 gabali lígas kandidáti - nejauśi piemeklétie pretinieki (peslépti, bet tádi paśas paśi panelíśi ká draugu sarakstá)

    private static MonoBehaviour mb;//haxxx: man vajag MonoBehaviour, lai varétu ko-rutínas piestarté statiská klasé 

    private static System.DateTime lastTimeGotFriends = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
    private static System.DateTime lastTimeGotLeagueCandidates = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);


    /**
	 * piestarté MP lietas, kurám ir jánotiek árpus MP ekrána
	 */
    public static void Init()
    {


        mb = GameObject.Find("Main Camera").GetComponent<MonoBehaviour>();
        if (Debug.isDebugBuild) { print("MP::Init"); }

        RecalculateMyPowerRating();

        //JÁBRAUC mp intro game vienmér, sákot spéli :D
        /*
		PlayerPrefs.DeleteKey("HadIntroGame");
		PlayerPrefs.DeleteKey("HasFB");
		PlayerPrefs.DeleteKey("MPID");
		PlayerPrefs.DeleteKey("PlayerTeamID");
		//*/

        //ielúgumu bonusu testéśanai:
        //PlayerPrefs.DeleteKey("HasInvitedEveryone");
        //PlayerPrefs.DeleteKey("HasInvited");

        HasFB = PlayerPrefs.GetInt("HasFB", 0) == 1 ? true : false;
        int MPNewsCheckPeriod = 60 * 30; //ja nav ielogojies, tad cheko zinjas reti
        if (HasFB)
        {
            FBID = PlayerPrefs.GetString("FBID", "0"); //péc FB logina tiek atjaunots FBID, bet śeit njemu piekeśoto, lai lietotu pirms FB logina - ielúgumu bonusu noskaidrośana
            MultiplayerManager.GetInviteBonus();
            MPNewsCheckPeriod = 60; // ja ir ielogojies, tad cheko zinjas reizi minúté

        }

        UpdateMPID();

        mb.StartCoroutine(CheckMPNews(MPNewsCheckPeriod));

        NumWins = PlayerPrefs.GetInt("NumWins", 0);
        NumGames = PlayerPrefs.GetInt("NumGames", 0);
        MPLevel = PlayerPrefs.GetInt("MPLevel", 0); //pirms kausinjiem (lai nedodu lieku LVLUP)
        Cups = PlayerPrefs.GetInt("Cups", 0);//lai zin cik ir kausińi, pirms izveidota konekcija ar serveri
        PlayerTeamID = PlayerPrefs.GetInt("PlayerTeamID", 0);//lai zin kurá komandá pirms konekcijas

    }

    /**
	 * pa ístam palaiź MP 
	 * izsauc ikreiz atverot galveno MP logu 
	 */
    public static void Startup()
    {

#if !UNITY_EDITOR
		if(AppVersionOk == null) {
			mb.StartCoroutine(checkAppVersion());
		} else if(AppVersionOk == true){
			UIManager.ToggleScreen(GameScreenType.PopupDownloadLatestVersion,false);
		} else {
			UIManager.ToggleScreen(GameScreenType.PopupDownloadLatestVersion,true);
		}
#endif

        NumSP = BikeDataManager.GetNumSP();

        UpdateMPID();


        if (!LoggedIn)
        {

            UIManager.SetScreenTimeout(GameScreenType.PopupMultiplayerLoading, -1);// w/o timeouting
            UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, true); //loading overlay

            if (HasFB)
            {
                if (Debug.isDebugBuild) { print("autoFB-konekts"); }
                FBConnect(); //ielogosies FB un tad ielogosies MP serverí
            }
            else
            {
                if (Debug.isDebugBuild) { print("!logged in && no FB"); }
                MPConnect();
            }
        }
        else
        {
            if (Debug.isDebugBuild) { print("already logged in"); }
            UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, false); //jáaizvác, ja nu ir palicis no iepr. reizes (piem.: aizverot logu FB ielogośanas laiká)
            if (HasFB)
            {
                if (Debug.isDebugBuild) { print("already logged in + FB"); }
            }
            else
            {
                if (Debug.isDebugBuild) { print("already logged in but no FB"); }
            }
        }

        //śajá mirklí vél nav beidzies logins, bet komandas ID mums ir piekeśots, var rasties probléma, ja cilvéks izvélas sev komandu un tad pabeidzas logins un cilvéks sańem veco komandas numuru
        if (PlayerTeamID == 0)
        { //ja nav izvéléta komanda, tad neturpina un párslédz uz komandu izvéles logu
            UIManager.SwitchScreen(GameScreenType.MultiplayerLeagueSelection);
            return;
        }

        if (!PlayerPrefs.HasKey("HadIntroGame"))
        { //nák pirmo reizi - ir jáieslédz ípaśá pirmá spéle		
            PlayerPrefs.SetInt("HadIntroGame", 1);
            SetupAndStartIntroGame();
        }

    }

    //izsauc pirmajá
    public static void StartupFirstFrame()
    {

    }


    //ielúgt FB draugus uz appu - paŕáda FB libas radítu popupu
    public static void ButtonFBInvite()
    {

        // if(!HasFB || (HasFB && !FB.IsInitialized) ){//nav FB vai arí ir neinicializéts FB, tad ir jáved logoties
        // 	UIManager.SetScreenTimeout(GameScreenType.PopupMultiplayerLoading, -1);
        // 	UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading,true); 
        // 	//print("veelaak atveers invaitu popupu");
        // 	FBConnect();
        // 	mb.StartCoroutine(OpenInvitePopupASAP()); //tiklídz bús ielogojies - śaus vaĺá invaitu popupu
        // 	return;
        // }

        // ButtonFBInvite();

        // if(!FBFriendsDownloaded) { //nav draugi ne reizi lejupieládéti (nav draugu saraksts ticis atvérts)
        // 	UIManager.SetScreenTimeout(GameScreenType.PopupMultiplayerLoading, -1);
        // 	UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading,true);

        // 	//veiks fikso pieprasíjumu serverim - iegút druagu sarakstu, bet no tá ńems tikai IDus
        // 	WWWForm data = new WWWForm();
        // 	mb.StartCoroutine(MPRequest("get_friends", data, delegate(WWW www){
        // 		if(Debug.isDebugBuild){print("quick get_friends++ " + www.text);}
        // 		FBFriendsDownloaded = true;
        // 		try {
        // 			JSONNode N = JSON.Parse(www.text);
        // 			FBFriendsPlaying = new string[N["friends"].AsArray.Count];								
        // 			for(int i = 0; i < N["friends"].AsArray.Count; i++) {
        // 				FBFriendsPlaying[i] = N["friends"][i]["id"];//saváks visu draudzińu IDus				
        // 			}
        // 			//izsauc par jaunu - tagad bús draugu IDi
        // 			ButtonFBInvite();
        // 		} catch(System.Exception e) {
        // 			if(Debug.isDebugBuild){print("g) Wrong JSON:" + e.Message); }
        // 			MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
        // 			//lai ráda erroru (pat ja ir loading)
        // 			UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading,true); 
        // 			UIManager.SetScreenTimeout(GameScreenType.PopupMultiplayerLoading, 0);
        // 		}
        // 	}, delegate(WWW www){
        // 		if (www.error.ToString().Contains("Internal Server Error"))
        //             MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
        //         else if (www.error.ToString().Contains("404 Not Found"))
        //             MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
        //         else
        //             MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailNoInternet");
        // 		UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading,false);
        // 		if(Debug.isDebugBuild){print("quick get_friends-- " + www.error);}
        // 	}));


        // 	return;
        // }


        UIManager.SetScreenTimeout(GameScreenType.PopupMultiplayerLoading, -1);
        UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, true);


        // 		FB.AppRequest(
        // 			Lang.Get("FBInviteTitle"), //FriendSelectorMessage -- śo redz ielúguma sańémejs
        // 			null,
        // 			null, //FriendSelectorFiltersArr, -- kas tas ir ?
        // 			FBFriendsPlaying, //excludeIds 
        // 			100, //maxRecipients,
        // 			"{}", //FriendSelectorData,
        // 			Lang.Get("FBFriendSelectorTitle"),//FriendSelectorTitle
        // 			delegate(IAppRequestResult result) {

        // 				WWWForm data = new WWWForm();

        // 				print("result.Text" + result.RawResult);
        // 				try {

        //                     var responseObject = result.ResultDictionary;

        // 					IEnumerable<string> invites = result.To;
        //                     int invitesCount = invites.Count();

        // 					if (invitesCount > 0)
        //                     {

        //                         PlayerPrefs.SetInt("HasInvited", 1);
        //                         PlayerPrefs.SetString(LAST_INVITE_TIMESTAMP_KEY, DateTime.Now.ToString());
        //                         PlayerPrefs.Save();
        //                         // multiplayerData.UpdateFBAndInviteRaceStep();

        // 						if (invitesCount > 4)
        //                         {
        //                             //                            print("Give Boost");
        //                             //DONE give the kid a medal or sth
        //                             DataManager.GiveTemporaryPowerBoost();
        //                             DataManager.SetupTemporaryBoostExpirationCheck();

        //                             UIManager.ToggleScreen(GameScreenType.PopupInviteFriendsForPower, false); //close popup if it's open

        //                             TelemetryManager.EventPowerBoostEnabled();
        //                         }

        //                         // GiveInviteBonus();
        //                         for (int i = 0; i < invitesCount; i++)
        //                         {
        //                             // Debug.Log("MPManager:: Invite ["+i+"] =  " + invites.ElementAt(i));
        //                             data.AddField("IDs[" + i + "]", invites.ElementAt(i));
        //                         }

        // 						mb.StartCoroutine(MPRequest("post_invites", data, delegate (WWW www)
        //                         {
        //                             if (Debug.isDebugBuild) { print("post_invites++\n" + www.text); }
        //                             MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
        //                         }, delegate (WWW www)
        //                         {
        //                             if (Debug.isDebugBuild) { print("post_invites--"); }
        //                             if (www.error.ToString().Contains("Internal Server Error"))
        //                                 MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
        //                             else if (www.error.ToString().Contains("404 Not Found"))
        //                                 MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
        //                             else
        //                                 MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailNoInternet");
        //                         }));


        //                     }


        //                     // JSONNode N = JSON.Parse(result.RawResult);		




        // //                     if(N["to"].AsArray.Count > 0){
        // // 						PlayerPrefs.SetInt("HasInvited",1);
        // //                         PlayerPrefs.SetString(LAST_INVITE_TIMESTAMP_KEY, DateTime.Now.ToString());//DONE write down a timestamp of last invite session
        // //                         PlayerPrefs.Save();

        // //                         //DONE if invited 5+ friends give power bonus
        // //                         if (N["to"].AsArray.Count > 4) {
        // // //                            print("Give Boost");
        // //                             //DONE give the kid a medal or sth
        // //                             DataManager.GiveTemporaryPowerBoost();
        // //                             DataManager.SetupTemporaryBoostExpirationCheck();

        // //                             UIManager.ToggleScreen(GameScreenType.PopupInviteFriendsForPower, false); //close popup if it's open

        // //                             TelemetryManager.EventPowerBoostEnabled();
        // //                         }

        // // 						for(int i = 0; i < N["to"].AsArray.Count; i++) {					
        // // 							data.AddField("IDs["+i+"]", N["to"][i]);
        // // 						}
        // // 						print("data after post invite " + data.ToString());

        // // 						mb.StartCoroutine(MPRequest("post_invites", data, delegate(WWW www){
        // // 						if(Debug.isDebugBuild){print("post_invites++\n"+ www.text);}
        // // 							MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
        // // 						},delegate(WWW www){
        // // 							if(Debug.isDebugBuild){print("post_invites--");}
        // // 							if (www.error.ToString().Contains("Internal Server Error"))
        // //                                 MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
        // //                             else if (www.error.ToString().Contains("404 Not Found"))
        // //                                 MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
        // //                             else
        // //                                 MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailNoInternet");
        // // 						}));
        // // 					}

        // 					//ielúgumi nosútíti, jáaizver overlejs:
        // 					UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading,false);				

        // 				} catch(System.Exception e) {
        // 					if(Debug.isDebugBuild){print("h) Wrong JSON:" + e.Message); }

        // 					//lai ráda erroru (pat ja ir loading)
        // 					UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading,true); 
        // 					UIManager.SetScreenTimeout(GameScreenType.PopupMultiplayerLoading, 0);
        // 				}
        // 			}

        // );
    }

    /**
	 * ielogosies músu MP serverí (max 1 reizi sesijá - izńemot, ja lokáli tiks izskaitĺots, ka sezona beigusies, tad vélreiz logosies)
	 * ja spélétájam ir pievienots FB, tad tur ielogośanás ir jau notikusi
	 */
    public static void MPConnect()
    {

        WWWForm data = new WWWForm();

        //súta pieprasíjumu uz músu MP serveri
        mb.StartCoroutine(MPRequest("login", data, delegate (WWW www)
        {
            try
            {

                if (Debug.isDebugBuild) { print("login++ " + www.text); }
                JSONNode N = JSON.Parse(www.text);
                PlayerName = N["name"];
                PlayerPicture = N["picture"];
                PlayerTeamID = N["team_id"].AsInt;
                Cups = N["cups"].AsInt;
                SeasonTTL = N["season_ttl"].AsInt;
                SeasonID = N["season_id"].AsInt;
                SendLeagueRidesToo = N["send_league_rides_too"].AsBool;

                string sPop = N["team_pop"];
                string[] pop = sPop.Split(','); //no servera nák CSV ar 4 komandu IDiem, piem.: "1,3,2,4" - kádá secíbá ir jáattélo komandas 
                for (int i = 0; i < 4; i++)
                {
                    TeamPopularity[i] = int.Parse(pop[i], System.Globalization.CultureInfo.InvariantCulture);
                }

                string sCups = N["team_cups"];
                string[] cups = sCups.Split(','); //CSV ar kausińiem 5 komandám (arí nulltá ar nulle :P )
                for (int i = 0; i <= 4; i++)
                {
                    TeamCups[i] = int.Parse(cups[i], System.Globalization.CultureInfo.InvariantCulture);
                }

                SeasonPrizes = null;
                if (N["prize_info"].ToString().Length > 10)
                {
                    SeasonPrizes = N["prize_info"];
                }

                PlayerLeagueContributionCups = N["league_contribution_cups"].AsInt;
                PlayerLeagueContributionGames = N["league_contribution_games"].AsInt;

                //info par iepriekśéjo sezonu - tikai, ja tá tikko nomainíjusies
                PrevSeasonID = N["prev_league_season_participated"].AsInt;
                if (PrevSeasonID > 0)
                {
                    PlayerPrevLeagueContributionCups = N["prev_league_contribution_cups"].AsInt;
                    PlayerPrevLeagueContributionGames = N["prev_league_contribution_games"].AsInt;
                    PrevSeasonPrizes = null;
                    if (N["prev_prize_info"].ToString().Length > 10)
                    {
                        PrevSeasonPrizes = N["prev_prize_info"];
                    }

                    int maxPoints = -5;
                    string spCups = N["prev_team_cups"];
                    string[] pCups = spCups.Split(','); //CSV ar kausińiem 5 komandám (arí nulltá ar nulle :P )
                    for (int i = 0; i <= 4; i++)
                    {
                        PrevTeamCups[i] = int.Parse(pCups[i], System.Globalization.CultureInfo.InvariantCulture);

                        if (PrevTeamCups[i] > maxPoints)
                        {
                            maxPoints = PrevTeamCups[i];
                            PrevSeasonWinnerID = i; //lokáli noskaidros uzvarétáju - kurai komanda ir visvairák punktu | neizśḱirts mekad nebús - serveris par to parúpesies, heh, heh
                        }
                    }

                    //print("juuhuu, sezona cauri, uzvareetaajs:"+PrevSeasonWinnerID+"!\nDaavanas:" + PrevSeasonPrizes.ToString() );

                    if (PlayerTeamID == PrevSeasonWinnerID)
                    {
                        //print("Pavei - tu esi uzvareejis");

                        UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLeagueWon);
                        MultiplayerLeagueWonBehaviour popupScript = GameObject.Find("PopupMultiplayerLeagueWon").GetComponent<MultiplayerLeagueWonBehaviour>();
                        popupScript.SetTeam(PrevSeasonWinnerID);

                        if (MultiplayerManager.PrevSeasonPrizes["coins"].AsInt > 0)
                        {
                            BikeDataManager.Coins += MultiplayerManager.PrevSeasonPrizes["coins"].AsInt; //uzdávinu monétas
                            popupScript.SetCoins(MultiplayerManager.PrevSeasonPrizes["coins"].AsInt);
                        }

                        if (MultiplayerManager.PrevSeasonPrizes["cups"].AsInt > 0)
                        {
                            //kausińi jau tika pieśḱirti uz servera, bet tie te parádísies tikai nákamajá reizé, kad tiks ievákti dati, kas var notikt minúti vélák, tápéc drośíbas péc pieskaitu lokáli (apdeitojot no servera, śis skaitlis tiek párrakstíts nevis inkrementés)
                            MultiplayerManager.Cups += MultiplayerManager.PrevSeasonPrizes["cups"].AsInt;
                            popupScript.SetCups(MultiplayerManager.PrevSeasonPrizes["cups"].AsInt);
                        }

                        //@note -- tikai nauda un kausińi - páréjás balvas netiek dávinátas (JSONá ir info par tám, bet tás visas ir 0,0,0,0... jo uz servera tá rakstíts)


                    }
                    else
                    {
                        //print("Your team is bad and you should feel bad !");

                        UIManager.ToggleScreen(GameScreenType.PopupMultiplayerNewSeason);
                        PopupMultiplayerNewSeasonBehaviour popupScript = GameObject.Find("PopupMultiplayerNewSeason").GetComponent<PopupMultiplayerNewSeasonBehaviour>();

                        if (MultiplayerManager.SeasonPrizes["coins"].AsInt > 0)
                        {
                            popupScript.SetCoins(MultiplayerManager.SeasonPrizes["coins"].AsInt);
                        }
                        if (MultiplayerManager.SeasonPrizes["cups"].AsInt > 0)
                        {
                            popupScript.SetCups(MultiplayerManager.SeasonPrizes["cups"].AsInt);
                        }


                        string newsText = "";

                        if (PrevSeasonWinnerID == 1)
                        {
                            newsText = Lang.Get("News:MP:Team Red has won");
                        }
                        else if (PrevSeasonWinnerID == 2)
                        {
                            newsText = Lang.Get("News:MP:Team Green has won");
                        }
                        else if (PrevSeasonWinnerID == 3)
                        {
                            newsText = Lang.Get("News:MP:Team Blue has won");
                        }
                        else if (PrevSeasonWinnerID == 4)
                        {
                            newsText = Lang.Get("News:MP:Team Purple has won");
                        }
                        NewsListManager.Push(newsText, NewsListItemType.prize, GameScreenType.MultiplayerMenu, "League"); // info, ka cita komanda uzvaréja, iemet zińás, ká vissvarígáko (tips=prize)

                    }

                }

                LoggedIn = true;
                //@oldshit mainGreetingLabel.text = "Hello, " +PlayerName;

                //@oldshit connectFBGO.SetActive(!HasFB); //FB poga
                UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, false); //loading overlay

                if (HasFB)
                {//ir FB, tad ir draudzińi, jádabú to spéles
                 //	MPGetFriendGames(); -- né, tás ieládés atverot draugu tabu

                    //@oldshit friendTopButtonGO.SetActive(true); //ieslédzu pogas, kas pienákás tikai FB spélétájiem
                    //@oldshit friendNewGameButtonGO.SetActive(true);
                }

                /*
				 * dabú lígas statistiku:
				 * [√] info par balvám 
				 * [√] info par manu pienesumu śajá sezoná
				 * [√] info par komandu sniegumu śajá sezoná
				 * 
				 * [√] info par pedéjo sezonu, ja tá tikko beigusies  - śi bús neatkaríga lapa - pati ieládés savus datus
				 */


                DataID++; //sanjemti jauni dati - MP logi sevi atjaunos nákamajá Updeitá

            }
            catch (System.Exception e)
            {
                if (Debug.isDebugBuild) { print("b) Wrong JSON:" + e.Message); }
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
                //lai ráda erroru (pat ja ir loading)
                UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, true);
                UIManager.SetScreenTimeout(GameScreenType.PopupMultiplayerLoading, 0);
            }

        }, delegate (WWW www)
        {
            if (Debug.isDebugBuild) { print("failure delegate++ " + www.text); }
            if (www.error.ToString().Contains("Internal Server Error"))
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
            else if (www.error.ToString().Contains("404 Not Found"))
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
            else
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailNoInternet");
            //popups ir valjá, ar bezgalígu taimaitu, tápéc nomainu tikai taimaitu, lai uzreiz ráda erroru
            UIManager.SetScreenTimeout(GameScreenType.PopupMultiplayerLoading, 0);

            //print("@todo -- B) errormsg ar luugumu meeginaat veelreiz - jáaizver MP lapa");

        }));

        PlayerPrefs.Save();//drośíbas péc pieseivos uz diska (normáli tas notiktu aizverot appu (quit))

    }

    //dabú visus draudzińus un vińu iesáktás spéles if any
    public static void MPGetFriends()
    {


        PlayerPrefs.SetInt("numMPNews", 0); //noreseto jaunumu skaitu 
                                            //@todo -- jáizvác visas MP zińas no zinju listes


        //ja vél nav pagájuśas X sekundes - neprasa serverim atkártoti
        System.TimeSpan diff = System.DateTime.Now.ToUniversalTime() - lastTimeGotFriends.ToUniversalTime();
        if (diff.TotalSeconds < 10)
        {
            if (Debug.isDebugBuild) { print("Not reloading friends (only " + (diff.TotalSeconds) + " sec passed)"); }
            return;
        }
        lastTimeGotFriends = System.DateTime.Now;

        WWWForm data = new WWWForm();


        mb.StartCoroutine(MPRequest("get_friends", data, delegate (WWW www)
        {

            if (Debug.isDebugBuild) { print("get_friends++ " + www.text); }

            try
            {
                JSONNode N = JSON.Parse(www.text);
                Cups = N["cups"].AsInt; //blakus spéĺu listei ir arí svarígákie, apdeitojamie spélétája parametri);

                DataID_friends++;
                FBFriendsDownloaded = true;
                FBFriendsPlaying = new string[N["friends"].AsArray.Count];
                OpponentsStarted = new List<MPOpponent>();

                for (int i = 0; i < N["friends"].AsArray.Count; i++)
                {
                    FBFriendsPlaying[i] = N["friends"][i]["id"];//saváks visu draudzińu IDus

                    MPOpponent opponent = new MPOpponent();
                    OpponentsStarted.Add(opponent);

                    opponent.FBID = N["friends"][i]["id"];
                    opponent.Name = N["friends"][i]["name"];
                    opponent.Picture = N["friends"][i]["picture"];
                    opponent.TeamID = N["friends"][i]["team_id"].AsInt;
                    opponent.NextTrack = N["friends"][i]["next_track"];
                    opponent.CupsNextTrackWin = N["friends"][i]["next_track_cups_win"].AsInt;
                    opponent.CupsNextTrackLose = N["friends"][i]["next_track_cups_lose"].AsInt;
                    opponent.Balance = N["friends"][i]["cups_balance"].AsInt;
                    opponent.Cups = N["friends"][i]["cups"].AsInt;
                    opponent.CupsAfterRevanche = N["friends"][i]["cups"].AsInt;
                    opponent.LastPlayedAt = N["friends"][i]["last_played_at"].AsInt;
                    opponent.LastPokedAt = N["friends"][i]["last_poked_at"].AsInt;
                    opponent.Waiting = N["friends"][i]["waiting"].AsInt == 1; // true/false
                    opponent.MPType = MPTypes.first; //péc nokluséjuma Spélétájs sáks braukt pret pretinieku [..]
                    opponent.PowerRating = MultiplayerManager.CalculatePowerRating(N["friends"][i]["player_upgrades"].AsIntArray); //globálais pretinieka powerreitings

                    if (N["friends"][i]["challenge"].ToString().Length > 0)
                    {
                        opponent.Time = N["friends"][i]["challenge"]["time"].AsFloat;
                        opponent.Track = N["friends"][i]["challenge"]["track"];
                        opponent.Upgrades = N["friends"][i]["challenge"]["upgrades"].AsIntArray; //CSV
                        opponent.Ride = N["friends"][i]["challenge"]["ride"];
                        opponent.Message = N["friends"][i]["challenge"]["message"];
                        opponent.CupsTrackWin = N["friends"][i]["challenge"]["track_cups_win"].AsInt;
                        opponent.CupsTrackLose = N["friends"][i]["challenge"]["track_cups_lose"].AsInt;
                        opponent.ReplayMyRide = N["friends"][i]["challenge"]["replay_my_ride"];
                        opponent.ReplayOppRide = N["friends"][i]["challenge"]["replay_opp_ride"];
                        opponent.ReplayMyTime = N["friends"][i]["challenge"]["replay_my_time"].AsFloat;
                        opponent.ReplayOppTime = N["friends"][i]["challenge"]["replay_opp_time"].AsFloat;
                        opponent.ReplayTrack = N["friends"][i]["challenge"]["replay_track"];
                        opponent.ReplayMyUpgrades = N["friends"][i]["challenge"]["replay_my_upgrades"].AsIntArray;
                        opponent.ReplayOppUpgrades = N["friends"][i]["challenge"]["replay_opp_upgrades"].AsIntArray;
                        opponent.PowerRating = MultiplayerManager.CalculatePowerRating(N["friends"][i]["challenge"]["upgrades"].AsIntArray); // ja ir jábrauc chelendźś, tad rádít pretinieka powerreitingu, káds tas bija śiní chelendźá

                        if (opponent.ReplayMyRide != null && opponent.ReplayMyRide.Length > 0)
                        {
                            opponent.MPType = MPTypes.replay; //[..] bet, ja ir gan chelendźśs, gan replejs, tad jásák ar repleju [..]
                        }
                        else
                        {
                            opponent.MPType = MPTypes.revanche; //[..] ja ir tikai chelendźśs, tad sáks ar revanśu
                        }
                    }
                }

            }
            catch (System.Exception e)
            {
                if (Debug.isDebugBuild) { print("c) Wrong JSON:" + e.Message); }
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
                //lai ráda erroru (pat ja ir loading)
                UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, true);
                UIManager.SetScreenTimeout(GameScreenType.PopupMultiplayerLoading, 0);
            }

        }, delegate (WWW www)
        {
            if (Debug.isDebugBuild) { print("get_friends-- " + www.error); }
            if (www.error.ToString().Contains("Internal Server Error"))
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
            else if (www.error.ToString().Contains("404 Not Found"))
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
            else
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailNoInternet");
            //print("@todo -- A) errormsg ar luugumu meeginaat veelreiz");			
        }));

    }

    /**
	 * uzprasís serverim vai man nepienákás naudas balva par ielúgtajiem cilvékiem
	 * veiks FB loginu 
	 */
    public static void GetInviteBonus()
    {

        if (PlayerPrefs.GetInt("HasInvitedEveryone", 0) == 0)
        { //tikai, ja nav vél visus ielúdzis (ir vél kas bonuss nopelnáms)

            if (PlayerPrefs.GetInt("HasInvited", 0) == 1)
            { //tikai, ja cilvéks ir kádu ielúdzis (uz śís ieríces)

                WWWForm data = new WWWForm();

                mb.StartCoroutine(MPRequest("get_invite_bonus", data, delegate (WWW www)
                {
                    if (Debug.isDebugBuild) { print("get_invite_bonus++\n" + www.text); }

                    try
                    {
                        JSONNode N = JSON.Parse(www.text);
                        int numBonusReceived = N["bonus_received"].AsInt;//cik gabali ir jau pieśḱirti
                        int numBonusNew = N["bonus_new"].AsInt;  //cik gabali tagad jápieśḱir

                        if (numBonusNew == 0)
                        {
                            return;
                        }

                        int sumReceived = 0;
                        int sumNew = 0;
                        int totalInvited = numBonusReceived + numBonusNew;
                        for (int i = 1; i <= Mathf.Min(totalInvited, InvBonusNum); i++)
                        {
                            if (i <= numBonusReceived)
                            {
                                sumReceived += InvBonusAmmount[i];
                            }
                            else
                            {
                                sumNew += InvBonusAmmount[i];
                            }
                        }

                        NewsListManager.Push(Lang.Get("News:Invite bonus |param| coins ").Replace("|param|", sumNew.ToString()), NewsListItemType.prize);


                        if (totalInvited >= InvBonusNum)
                        {
                            PlayerPrefs.SetInt("HasInvitedEveryone", 1); //pieraksta, ka sanjémis visus invaitbonusus - var turpmák neprasít serverim, tápat naudu nedabús
                        }

                        BikeDataManager.Coins += sumNew;
                        BikeDataManager.Flush();

                        TelemetryManager.EventInviting(numBonusNew);

                    }
                    catch (System.Exception e)
                    {
                        if (Debug.isDebugBuild) { print("d) Wrong JSON:" + e.Message); }
                        MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
                        //lai ráda erroru (pat ja ir loading)
                        UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, true);
                        UIManager.SetScreenTimeout(GameScreenType.PopupMultiplayerLoading, 0);
                    }


                }, delegate (WWW www)
                {
                    if (Debug.isDebugBuild) { print("get_invite_bonus--"); }
                    if (www.error.ToString().Contains("Internal Server Error"))
                        MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
                    else if (www.error.ToString().Contains("404 Not Found"))
                        MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
                    else
                        MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailNoInternet");
                }));


            }// else { print("speeleetaajs nav nevienu ieluudzis, skipojam bonusu chekoshanu"); }

        }// else { print("speeleetaajs ir jau visus ieluudzis, skipojam bonusu chekoshanu"); }
    }


    // private static void GenericFBRequestCallback(IResult result){

    // 	if (!String.IsNullOrEmpty (result.Error))
    // 	{
    // 		print("Error Response:\n" + result.Error);
    // 	}
    // 	else if (!String.IsNullOrEmpty (result.RawResult))
    // 	{
    // 		print( "Success Response:\n" + result.RawResult);
    // 	}
    // 	// else if (result.Texture != null)
    // 	// {
    // 	// 	print("Success Response: texture\n");
    // 	// }
    // 	else
    // 	{
    // 		print("Empty Response\n");
    // 	}
    // }

    /**
	 * ielogojas feisbuká: 
	 *  	prasot lietotájam ielogoties
	 * 		vai zinot vińu un pa kluso ielogojot
	 * ja veiksmigi, tad ielogojas músu MP serverí, padodot FB infu
	 */
    public static void FBConnect()
    {

        // if(!FB.IsInitialized){
        // 	//Debug.Log("A1");
        // 	FB.Init(FBInitCallback, null,null);
        // } else {
        // 	//Debug.Log("A2");
        // 	FBInitCallback();		
        // }

    }

    private static void FBInitCallback()
    {
        // if(Debug.isDebugBuild){Debug.Log("++FB inicializeets");}
        // if(!FB.IsLoggedIn){
        // 	FB.LogInWithReadPermissions(new List<string>() { "email" , "user_friends" } , FBLoginCallback);
        // } else {
        // 	//ir viss zináms par klińǵeri
        // 	if(Debug.isDebugBuild){Debug.Log("++Is allright, is in");}
        // 	HasFB = true;
        // 	FBID = AccessToken.CurrentAccessToken.UserId;
        // 	PlayerPrefs.SetInt("HasFB", 1);
        // 	PlayerPrefs.SetString("FBID", FBID);
        // 	PlayerPrefs.Save();
        // 	MPConnect();
        // }




        //Debug.Log("++FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
    }

    // private static void FBLoginCallback(IResult result){

    // 	if (result.Error != null){
    // 		if(Debug.isDebugBuild){Debug.Log("++Error Response:\n" + result.Error);}
    // 	} else if (!FB.IsLoggedIn)		{
    // 		if(Debug.isDebugBuild){Debug.Log("++Login cancelled by Player");}
    // 	} else {
    // 		if(Debug.isDebugBuild){Debug.Log("++Login was successful!");}
    // 		HasFB = true;
    // 		FBID = AccessToken.CurrentAccessToken.UserId;
    // 		PlayerPrefs.SetInt("HasFB", 1);
    // 		PlayerPrefs.SetString("FBID", FBID);
    // 		PlayerPrefs.Save();
    // 		MPConnect();
    // 	}

    // }


    //pańems 3 lígas spéles - easy/medium/hard;  nelejupieládés to ride failińus
    public static void GetLeagueGameCandidates(System.Action SuccessCallback)
    {

        //ja vél nav pagájuśas X sekundes - neprasa serverim atkártoti
        System.TimeSpan diff = System.DateTime.Now.ToUniversalTime() - lastTimeGotLeagueCandidates.ToUniversalTime();
        if (diff.TotalSeconds < 2)
        {
            if (Debug.isDebugBuild) { print("Not reloading league candidates (only " + (diff.TotalSeconds) + " sec passed)"); }
            SuccessCallback();
            return;
        }
        lastTimeGotLeagueCandidates = System.DateTime.Now;



        WWWForm data = new WWWForm(); //nepadod nekádus papildus parametrus

        mb.StartCoroutine(MPRequest("get_league_game", data, delegate (WWW www)
        {

            if (Debug.isDebugBuild) { print("get_league_game++ " + www.text); }
            LeagueCandidateOpponents = new MPOpponent[] { null, null, null }; //noresetoju kandidátu sarakstu

            try
            {
                JSONNode N = JSON.Parse(www.text);

                if (N == null)
                {
                    if (Debug.isDebugBuild) { print("nebrauks, ejam tálák 1"); }
                    UIManager.SwitchScreen(GameScreenType.MultiplayerMenu);
                    UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, false);
                    return;
                }

                for (int i = 0; i < N["friends"].AsArray.Count; i++)
                { //bús 3 gabali - katram sava difficulty: 0,1,2

                    MPOpponent opponent = new MPOpponent();
                    int difficulty = N["friends"][i]["challenge"]["difficulty"].AsInt;

                    opponent.FBID = N["friends"][i]["id"];
                    opponent.Name = N["friends"][i]["name"];
                    opponent.Picture = N["friends"][i]["picture"];
                    opponent.TeamID = N["friends"][i]["team_id"].AsInt;
                    opponent.Cups = N["friends"][i]["cups"].AsInt;
                    opponent.LeagueChallengeId = N["friends"][i]["league_challenge_id"].AsInt;
                    opponent.Waiting = false;
                    opponent.MPType = MPTypes.league;
                    opponent.Time = N["friends"][i]["challenge"]["time"].AsFloat;
                    opponent.Track = N["friends"][i]["challenge"]["track"];
                    opponent.Ride = N["friends"][i]["challenge"]["ride"];
                    opponent.CupsTrackWin = N["friends"][i]["challenge"]["track_cups_win"].AsInt;
                    opponent.CupsTrackLose = N["friends"][i]["challenge"]["track_cups_lose"].AsInt;
                    opponent.PowerRating = MultiplayerManager.CalculatePowerRating(N["friends"][i]["challenge"]["upgrades"].AsIntArray);

                    Destroy(GameObject.Find("League.MP.Candidate." + difficulty)); //izdzéś iepriekśéjo, if any
                    LeagueCandidateOpponents[difficulty] = opponent; // ieliek sarakstá, identificéjot péc grutíbas, ká vienu no kandidátiem
                }

                SuccessCallback();

            }
            catch (System.Exception e)
            {
                if (Debug.isDebugBuild) { print("d) Wrong JSON:" + e.Message); }
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
                //lai ráda erroru (pat ja ir loading)
                UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, true);
                UIManager.SetScreenTimeout(GameScreenType.PopupMultiplayerLoading, 0);
            }

        }, delegate (WWW www)
        {
            if (Debug.isDebugBuild) { print("get_league_game-- " + www.error); }
            if (www.error.ToString().Contains("Internal Server Error"))
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
            else if (www.error.ToString().Contains("404 Not Found"))
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
            else
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailNoInternet");
            UIManager.SwitchScreen(GameScreenType.MultiplayerMenu);
            UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, false);
        }));

    }


    //lejupieládé, izveido pretinieku un piestarté intro spéli
    //@note -- tikai intro spélém
    public static void SetupAndStartIntroGame()
    {

        //print("introgame");
        MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailNoInternet");

        UIManager.SetScreenTimeout(GameScreenType.PopupMultiplayerLoading, 10);// w/ timeouting
        UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, true); //loading overlay

        WWWForm data = new WWWForm();

        data.AddField("IntroGame", 1);

        mb.StartCoroutine(MPRequest("get_league_game", data, delegate (WWW www)
        {

            if (Debug.isDebugBuild) { print("get_league_game++ " + www.text); }
            CurrentOpponent = null;

            try
            {
                JSONNode N = JSON.Parse(www.text);

                if (N == null)
                {
                    if (Debug.isDebugBuild) { print("nebrauks, ejam tálák 1"); }
                    UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, false);
                    return;
                }

                for (int i = 0; i < N["friends"].AsArray.Count; i++)
                {
                    MPOpponent opponent = new MPOpponent();
                    CurrentOpponent = opponent;
                    opponent.FBID = N["friends"][i]["id"];
                    opponent.Name = N["friends"][i]["name"];
                    opponent.Picture = N["friends"][i]["picture"];
                    opponent.TeamID = RandomNotMyTeam(); //[1-4] lai visi intro nebrauktu pret konkrétu komandu (bet pieskata, lai arí nav spélétája komandá)
                    opponent.Cups = N["friends"][i]["cups"].AsInt;
                    opponent.LeagueChallengeId = N["friends"][i]["league_challenge_id"].AsInt;
                    opponent.Waiting = false;
                    opponent.MPType = MPTypes.league;
                    opponent.Time = N["friends"][i]["challenge"]["time"].AsFloat;
                    opponent.Track = N["friends"][i]["challenge"]["track"];
                    opponent.Ride = N["friends"][i]["challenge"]["ride"];
                    opponent.CupsTrackWin = N["friends"][i]["challenge"]["track_cups_win"].AsInt;
                    opponent.CupsTrackLose = N["friends"][i]["challenge"]["track_cups_lose"].AsInt;


                    GameObject prefab = LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("FriendGameEntry") as GameObject;
                    Debug.Log("<color=yellow>Prefab Loaded Name = </color>" + prefab);
                    //GameObject prefab = Resources.Load("Prefabs/UI/FriendGameEntry") as GameObject;
                    GameObject entry = Instantiate(prefab) as GameObject;

                    entry.transform.SetParent(GameObject.Find("MultiplayerMenu").transform);
                    entry.transform.localScale = Vector3.one;
                    entry.GetComponent<RectTransform>().localPosition = new Vector2(9899, 9999);

                    FriendGameEntryBehaviour script = entry.GetComponent<FriendGameEntryBehaviour>();
                    script.AutoPlay = true; //sáks spéli tiklídz bús lejupieládéta visa info ...
                    script.AutoDownload = true; //... un lieku lejupieládét uzreiz
                    script.SetData(opponent); //iedod pretinieka ierakstu - poga pati sev saliks datus

                    UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, false); //aizveru loading ekránu - nupat tiks ieslégts level-loading ar savu loading ekránu
                }

                if (CurrentOpponent == null)
                { //nav atrasts pretinieks
                    print("nebrauks, ejam tálák 2");
                    UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, false);
                    return;
                }

            }
            catch (System.Exception e)
            {
                if (Debug.isDebugBuild) { print("e) Wrong JSON:" + e.Message); }
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");

                //lai ráda erroru (pat ja ir loading)
                UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, true);
                UIManager.SetScreenTimeout(GameScreenType.PopupMultiplayerLoading, 0);
            }

        }, delegate (WWW www)
        {
            if (Debug.isDebugBuild) { print("get_league_game-- " + www.error); }
            if (www.error.ToString().Contains("Internal Server Error"))
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
            else if (www.error.ToString().Contains("404 Not Found"))
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
            else
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailNoInternet");
            UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, false);
        }));

    }


    /**
	 * jáizsauc caur mb.StartCoroutine()
	 */
    public static IEnumerator MPRequest(string method, WWWForm data, System.Action<WWW> SuccessCallback, System.Action<WWW> FailureCallback)
    {

        //obligátie parametri:
        data.AddField("UID", SystemInfo.deviceUniqueIdentifier);
        data.AddField("MPID", MPID);
        data.AddField("FBID", FBID);
        // data.AddField("Token", HasFB ? AccessToken.CurrentAccessToken.TokenString : "");
        data.AddField("NumSP", NumSP);
        data.AddField("Version", GameVersionManager.V);



#if UNITY_EDITOR
        data.AddField("OSType", 1);//izlikśos, ka tas ir iOS
#elif UNITY_IOS
		data.AddField("OSType", 1);
#elif UNITY_ANDROID
		data.AddField("OSType", 0);
#else
		data.AddField("OSType", -1);
#endif

        SignRequest(ref data);

        string url = ServerUrlMP + "/mp/" + method;
        WWW www = null;
        // if(HasFB)
        // 			print("HasFB + " + HasFB + " AccessToken.CurrentAccessToken.ToString() = " + AccessToken.CurrentAccessToken.TokenString);
        // print("----Start method=" + method);
        // 		print(SystemInfo.deviceUniqueIdentifier);
        //         print(MPID);
        //         print(FBID);
        //         print(HasFB ? AccessToken.CurrentAccessToken.TokenString : "");
        //         print(NumSP);
        //         print(GameVersionManager.V);
        // 		print(url);
        // print("----End");

        int retries = 5;
        if (method == "post_a_game")
        {
            retries = 7; //parastiem pieprasíjumiem méǵinás divreiz, spéĺu postéśanai - piecreiz!
        }
        while (retries-- > 0)
        {
            www = new WWW(url, data);
            yield return www;

            if (www.error == null)
            {
                SuccessCallback(www);
                yield break;
            }
            else if (www.error.ToString().Contains("Internal Server Error"))
            { //ja ir 500 errors, tad méǵina péc bríźa
                SignRequest(ref data); //par jaunu jáparaksta (lai nomainítos rekvesta laiks) - jo pieprasíjumi ar vienádu laiku uz servera tiks dropoti
                yield return new WaitForSeconds(0.7f);
            }
            /*
			else if(www.error.ToString().Contains("Internal Server Error") ){ //ja ir 500 errors, tad neméǵina atkaŕtoti
				FailureCallback(www);
				yield break;
			}*/

            if (Debug.isDebugBuild)
            {
                print("retry: \"" + method + "\" " + www.error + " data: " + data.data.ToString());
            }
            yield return new WaitForSeconds(0.5f);
        }

        FailureCallback(www); //esam tik tálu tikuśi, bez success, tátad failure >:)


    }


    //rekvesta objektam pieśaus klát taimstampu un parakstís 
    public static void SignRequest(ref WWWForm data)
    {
        string salt = "sBXUewTmFbfLZCmreQxDdw4jLD7jNcxdzjBfezsjXvzCHpjq7bxeGmDNMt4HM7QJ";
        DateTime epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
        double timestamp = (System.DateTime.UtcNow - epochStart).TotalMilliseconds * 100; // *100, lai iegútu veselu skaitĺi (sákotnéji ir 0.00)

        data.AddField("Mark", timestamp.ToString());
        data.AddField("MarkIV", Sha1.Sha1Sum(timestamp.ToString() + SystemInfo.deviceUniqueIdentifier + FBID + salt)); //vienkárśíbas labad heśośu tikai taimstampu un UIDu, páréjo var mainít cik grib - bet pieprasíjums tiks dropots, ja nebús nomainíts taimstamps (to nomainot paraksts nedarbosies)

    }

    /**
	 * salídzina appas esośo versijas numuru ar servera teikto 
	 * 
	 * nomaina AppVersionOk
	 */
    public static IEnumerator checkAppVersion()
    {

#if UNITY_IOS || UNITY_EDITOR
        string url = ServerUrlMP + "/v_ios.txt";
#elif UNITY_ANDROID
		string url = ServerUrlMP + "/v_android.txt"; 
#else
		string url = "";
		Debug.LogWarning("Version check on nonsuppported platform");
		yield break;
#endif

        WWW www = new WWW(url);
        yield return www;

        if (www.error == null)
        {

            //print(url+ "  "  + www.text + "===" + GameVersionManager.V);
            if (www.text == GameVersionManager.VMP)
            {
                AppVersionOk = true;
                //Debug.Log("V=OK");
                UIManager.ToggleScreen(GameScreenType.PopupDownloadLatestVersion, false);
            }
            else
            {
                AppVersionOk = false;
                //Debug.Log("V!=OK");
                UIManager.ToggleScreen(GameScreenType.PopupDownloadLatestVersion, true);
            }

        }
        else
        {
            //hmm
            if (Debug.isDebugBuild) { Debug.LogError("nav versijfaila: " + www.error.ToString()); }
            AppVersionOk = null;

            UIManager.SwitchScreen(GameScreenType.Levels);
            // PopupGenericErrorBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailNoInternet");
            // UIManager.ToggleScreen(GameScreenType.PopupGenericError,true);

        }

    }





    /**
	 * nosúta pédéjo spéli ar tagadéjo pretinieku uz serveri
	 * 
	 * noseivo un augśupládé brauciena failu
	 * 
	 * 
	 */
    public static void MPPostGame(int leagueChallengeId, string track, string time, int cupsWin, int cupsLose, string message = "", System.Action<string> FinishCallback = null)
    {
        mb.StartCoroutine(_MPPostGame(leagueChallengeId, track, time, cupsWin, cupsLose, message, FinishCallback));

    }

    private static IEnumerator _MPPostGame(int leagueChallengeId, string track, string time, int cupsWin, int cupsLose, string message, System.Action<string> FinishCallback)
    {


        WWWForm data = new WWWForm();


        if ((leagueChallengeId == 0 || SendLeagueRidesToo) && HasFB)
        { //sútís raidińu (tikai ne-lígas braucieniem vai arí visiem, ja serveris tá saka) bet tikai, ja ir FB, citadi śie raidińi nav lietojami
            BikeGameManager.SaveGameMP();//noseivo failu un iegúst tá sazipotos baitus un nosaukumu | 
            data.AddBinaryData("RideFile", BikeDataManager.LastZippedContent);
            data.AddField("RideFileName", BikeGameManager.mpLastSaveGameName);
        }
        LevelManager.KillLevel(); //pártrauc límeńa renderéśanu 


        if (leagueChallengeId == 0)
        { //parametri, kas nav jásúta ligas gadíjumá
            data.AddField("Message", message);
        }
        data.AddField("LeagueChallengeId", leagueChallengeId);
        data.AddField("FriendID", CurrentOpponent.FBID);
        data.AddField("FriendTeamID", CurrentOpponent.TeamID);
        data.AddField("Time", time);
        data.AddField("Track", track);
        data.AddField("CupsWin", cupsWin); //cik kausińi ir bijuśi apsolíti uzvaras gadíjumá (solíja serveris, bet negribu uz servera vélreiz ŕéḱinát, iespéjams, ka ir mainíjies nebutu smuki neturét solíjumu)
        data.AddField("CupsLose", cupsLose);
        string b = BikeDataManager.MultiplayerPlayerBikeRecordName;
        data.AddField("Upgrades", BikeDataManager.Bikes[b].UpgradesPerm[4] + "," + BikeDataManager.Bikes[b].UpgradesPerm[5] + "," + BikeDataManager.Bikes[b].UpgradesPerm[6] + "," + BikeDataManager.Bikes[b].UpgradesPerm[7] + "," + BikeDataManager.Bikes[b].UpgradesPerm[8]); //upgradesPerm ir bez +100 powerreitinga, ja táds ieslégts
        data.AddField("PowerRating", PermanentPowerRating);
        data.AddField("DoNotUse", BikeDataManager.PowerBoostEnabled ? 1 : 0); //súta 1, ja braucéjs ir dabújis feiku bústu +100 powerreiting - serveris nedríkst śo raidińu leitot tálák



        mb.StartCoroutine(MPRequest("post_a_game", data, delegate (WWW www)
        {
            if (Debug.isDebugBuild) { print("post_a_game++ " + www.text); }

            try
            {
                JSONNode N = JSON.Parse(www.text);
                Cups = N["cups"].AsInt; //apdeitoju aktuáláko kausińu skaitu
                SeasonTTL = N["season_ttl"].AsInt;

                int _seasonID = N["season_id"].AsInt;
                if (_seasonID != SeasonID)
                {
                    print("Forced login - league season changed");
                    MPConnect(); //ja jaunais season_id atsskiras no iepr. - tad jáforsé LOGIN, lai sańemtu info par iepr. sezonu
                }
                SeasonID = _seasonID;


                //apdeitoju komandu statistiku
                string sCups = N["team_cups"];
                string[] cups = sCups.Split(','); //CSV ar kausińiem 5 komandám (arí nulltá ar nulle :P )
                for (int i = 0; i <= 4; i++)
                {
                    TeamCups[i] = int.Parse(cups[i], System.Globalization.CultureInfo.InvariantCulture);
                }

                PlayerLeagueContributionCups = N["league_contribution_cups"].AsInt;
                PlayerLeagueContributionGames = N["league_contribution_games"].AsInt;

                if (FinishCallback != null)
                {
                    FinishCallback("ok");
                }

                DataID++;
            }
            catch (System.Exception e)
            {
                if (Debug.isDebugBuild) { print("f) Wrong JSON:" + e.Message); }
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");

                //lai ráda erroru (pat ja ir loading)
                UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, true);
                UIManager.SetScreenTimeout(GameScreenType.PopupMultiplayerLoading, 0);
            }


        }, delegate (WWW www)
        {
            if (Debug.isDebugBuild) { print("post_a_game-- " + www.text); }
            if (www.error.ToString().Contains("Internal Server Error"))
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
            else if (www.error.ToString().Contains("404 Not Found"))
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
            else
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailNoInternet");
            if (FinishCallback != null)
            {
                FinishCallback("notok");
            }
        }));


        yield break;
    }


    /**
	 * padod UIButtonMultiplayerFriendCommand (pogu, kurai pieder MPOpponent skripts)
	 * 
	 * lejupieládés:
	 * a) pretinieka ride-failińu
	 * b) abus repleja ride-failińus
	 * 
	 * péc lejupieládes pabiksta pogu, lai tá padara sevi aktívu (ja VISI lejupieláźu darbińi tai ir beigti)
	 * 
	 */
    public static void GetRide(FriendGameEntryBehaviour entry, string fileName)
    {
        mb.StartCoroutine(_GetRide(entry, fileName));
    }

    private static IEnumerator _GetRide(FriendGameEntryBehaviour entry, string fileName)
    {

        string urlToFetch;

        //vai nav piekeśots
        bool cacheHit = false;


        if (fileName != null && fileName.Length > 0)
        { //lejupieládé tikai, ja ir norádíts raidfailińś
            entry.DownloadsInProgress++;


            int retries = 2;
            while (retries-- > 0)
            { // ja gadísies neveiksmíga konekcija, méǵinás vélreiz
                string filePath = Application.persistentDataPath + "/mp-cache/" + fileName + ".bytes";

                if (System.IO.File.Exists(filePath))
                {
                    //print( Name + "'s ride's found " + fileName);
                    cacheHit = true;
                    urlToFetch = "file://" + filePath; // "lejupieládés" no keśa
                }
                else
                {
                    //print( Name + "'s ride's not found " + fileName);
                    urlToFetch = MultiplayerManager.ServerUrlMP + "/rides/" + fileName + ".bytes"; // lejupieládés no MP servera
                }


                if (!cacheHit)
                {
                    //gaida, ja jau tiek vienlaicígi lejupieládéti vairák ká X faili
                    //śo vajag, jo daudzu draugu sarakstá vai líderbordá, vienlaicígi pieprasís daudz pikchu
                    if (currentlyDownloading >= maxCurrentlyDownloading)
                    {
                        yield return new WaitForSeconds(0.3f);
                    }
                }

                currentlyDownloading++;
                WWW www = new WWW(urlToFetch);
                yield return www;
                currentlyDownloading--;

                if (www.error == null)
                {
                    if (!cacheHit && www.bytes.Length > 5)
                    { //piekeśo raidińu
                        System.IO.File.WriteAllBytes(filePath, www.bytes);
                    }
                    break;
                }
                else
                {
                    if (Debug.isDebugBuild) { print("retry downloading ride: " + urlToFetch); }
                    continue;
                }

            }

            entry.DownloadsInProgress--;
        }


        entry.EnablPlayIfEverythingIsGood(); //pabiksta pogu

    }


    /*
	 * pańems pikchu un ride-failińus no servera (ja nebúst atrodami keśá)
	 * 
	 * url
	 * fbid -- kam pieder pikcha (0, ja nav fb IDa), lai var piekeśot ar unikálu nosaukumu
	 * pictureGO -- kur ievietot lejupieládéto spraitu
	 * 
	 * 
	 * 
	 * bilde, tochna, nemainás, kamér nenotiek jauns logins MP serverí (restartéta appa), péc logina tiek DB pieseivota aktualáká FB bilde, nezinu vai nemainoties bildei, var mainíties tás draudígá adrese
	 * artúra bilde 25. novembrí, mototrial appai
	 * https://fbcdn-profile-a.akamaihd.net/hprofile-ak-xfa1/v/t1.0-1/c33.139.497.497/s200x200/377247_10151037123711315_1351899130_n.jpg?oh=c155fa65ffa69f5e8a8acc9bdf512a7e&oe=54DAA80F&__gda__=1427484084_45f53583187c8cdd61487a41b5150b33
	 */
    public static void GetPicture(string url, string fbid, Image imageCompo)
    {
        mb.StartCoroutine(_GetPicture(url, fbid, imageCompo));
    }

    private static IEnumerator _GetPicture(string url, string fbid, Image imageCompo)
    {

        if (url == null || url.Length == 0)
        {
            //Debug.LogWarning("no pic ");
            yield break;
        }

        string urlToFetch;

        //vai nav piekeśots
        bool cacheHit = false;
        string picCodename = fbid + Sha1.Sha1Sum(url) + ".jpg"; //unikáls nosaukums garantéts™
        string picCodenamePath = Application.persistentDataPath + "/mp-cache/" + picCodename;

        if (System.IO.File.Exists(picCodenamePath))
        {
            //print( Name + "'s pic's found " + picCodename);
            cacheHit = true;
            urlToFetch = "file://" + picCodenamePath; // "lejupieládés" no keśa
        }
        else
        {
            //print( Name + "'s pic's not found " + picCodename);
            urlToFetch = url; // lejupieládés no feisbuka
        }


        if (!cacheHit)
        {
            //gaida, ja jau tiek vienlaicígi lejupieládéti vairák ká X faili
            //śo vajag, jo daudzu draugu sarakstá vai líderbordá, vienlaicígi pieprasís daudz pikchu
            if (currentlyDownloading >= maxCurrentlyDownloading)
            {
                yield return new WaitForSeconds(0.3f);
            }
        }

        currentlyDownloading++;
        WWW www = new WWW(urlToFetch);
        yield return www;

        currentlyDownloading--;

        if (www.error != null && www.error.Length > 0)
        {
            if (Debug.isDebugBuild) { print("unable to download; www.error=" + www.error); }
        }
        else
        {

            if (!cacheHit && www.bytes.Length > 5)
            { //piekeśo bildi && nekeśo tukśas
                System.IO.File.WriteAllBytes(picCodenamePath, www.bytes);
                //print( Name + "'s pic's cached" + picCodename);
            }



            if (imageCompo != null)
            {

                if (www.texture.height > 8 && www.texture.width > 8)
                { // vienígais veids, ká identificét nederígas bildes (lejupieládéta 404 lapa nevis jpg fails) ir péc izméra - nederígas bildes iz 8x8
                    imageCompo.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
                } //else  { print("didn't changed pic");  }
            } //else  { print("didn't changed pic - nowhere to put it");  }
        }
    }

    //nosúta serverim zińu, ka ir nomainíta komanda
    public static void MPPostTeamSelection(System.Action FinishCallback = null)
    {
        mb.StartCoroutine(_MPPostTeamSelection(FinishCallback));
    }
    private static IEnumerator _MPPostTeamSelection(System.Action FinishCallback)
    {
        WWWForm data = new WWWForm();

        data.AddField("TeamID", PlayerTeamID);

        mb.StartCoroutine(MPRequest("post_team_selection", data, delegate (WWW www)
        {
            if (Debug.isDebugBuild) { print("post_team_selection++ " + www.text); }
            MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");

            if (FinishCallback != null)
            {
                FinishCallback();
            }
        }, delegate (WWW www)
        {
            if (Debug.isDebugBuild) { print("post_team_selection-- " + www.text); }
            if (www.error.ToString().Contains("Internal Server Error"))
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
            else if (www.error.ToString().Contains("404 Not Found"))
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
            else
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailNoInternet");
            if (FinishCallback != null)
            {
                FinishCallback();
            }
        }));


        yield break;
    }

    /**
	 * izdzésís challange starp paśreizéjo appas spélétáju un izvéléto draugu
	 */
    public static void DeleteChallenge(string fbidToRemove)
    {
        WWWForm data = new WWWForm();

        data.AddField("FriendID", fbidToRemove);

        mb.StartCoroutine(MPRequest("remove_challenge", data, delegate (WWW www)
        {
            if (Debug.isDebugBuild) { print("remove_challenge++ " + www.text); }
            MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
        }, delegate (WWW www)
        {
            if (Debug.isDebugBuild) { print("remove_challenge-- " + www.text); }
            if (www.error.ToString().Contains("Internal Server Error"))
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
            else if (www.error.ToString().Contains("404 Not Found"))
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
            else
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailNoInternet");
        }));
    }


    public static void SendPoke(string receiverFBID)
    {
        WWWForm data = new WWWForm();

        data.AddField("ReceiverFBID", receiverFBID);

        mb.StartCoroutine(MPRequest("poke", data, delegate (WWW www)
        {
            if (Debug.isDebugBuild) { print("poke++ " + www.text); }
            MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
        }, delegate (WWW www)
        {
            if (Debug.isDebugBuild) { print("poke-- " + www.text); }
            if (www.error.ToString().Contains("Internal Server Error"))
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
            else if (www.error.ToString().Contains("404 Not Found"))
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
            else
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailNoInternet");
        }));
    }


    /**
	 * regulári skatísies vai nav jaunumu (pouki) 
	 * jaunumus liks jaunumu listé
	 * inkrementés jaunumu skaitu - ko radít pie MP pogám
	 * lígas sezonas terminjus un uzvaru/zaudéjumu liks pie zinjám
	 */
    private static IEnumerator CheckMPNews(int MPNewsCheckPeriod)
    {

        int lastMPNewsID = PlayerPrefs.GetInt("lastMPNewsID", 0); //poke ID datubázé. pédejais, kas ir lejupieládéts

        int numMPNews = PlayerPrefs.GetInt("numMPNews", 0); //cik liels skaitlis járáda pie MP pogas  @note -- ís netiek lietots, tá vietá ir "active_rides" 

        if (NumGames == 0)
        {
            yield break; //necheko jaunumus, ka spélétájs vél nav izbraucis nevienu MP braucienu
        }

        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (UIManager.currentScreenType != GameScreenType.MultiplayerGame &&
               UIManager.currentScreenType != GameScreenType.MultiplayerGameReplay &&
               UIManager.currentScreenType != GameScreenType.Game)
            {//neprasam jaunumu, ja paślaik notiek brauciens			   
             //print("nenotiek brauciens");

                // ká arí neprasam jaunumus, ja ir MP galvenais logs ar draugu tabu valjá
                if (UIManager.currentScreenType == GameScreenType.MultiplayerMenu && GameObject.Find("Canvas/MultiplayerMenu/FriendsPanel") != null && GameObject.Find("Canvas/MultiplayerMenu/FriendsPanel").activeSelf)
                {
                    //print("draugu tabs valjaa");
                }
                else
                {
                    //print("draugu tabs nav valjaa");

                    WWWForm data = new WWWForm();
                    data.AddField("LastID", lastMPNewsID);

                    mb.StartCoroutine(MPRequest("get_news", data, delegate (WWW www)
                    {
                        if (Debug.isDebugBuild) { print("get_news++ " + www.text); }

                        try
                        {

                            JSONNode N = JSON.Parse(www.text); //met exception, ja serveris neatgrieź korektu JSONu

                            FBFriendsPlaying = new string[N["pokes"].AsArray.Count];
                            for (int i = 0; i < N["pokes"].AsArray.Count; i++)
                            {
                                numMPNews++;
                                lastMPNewsID = N["pokes"][i]["id"].AsInt;
                                string msg = N["pokes"][i]["message"];
                                NewsListManager.Push(msg, NewsListItemType.mpFriends, GameScreenType.MultiplayerMenu, "Friends");
                            }
                            PlayerPrefs.SetInt("lastMPNewsID", lastMPNewsID);
                            PlayerPrefs.SetInt("numMPNews", numMPNews);


                            //atjauno sezonas info (ja nav bijis MP logins, tad vispár nav info par śo) 
                            SeasonTTL = N["season_ttl"].AsInt;
                            SeasonID = N["season_id"].AsInt;

                            //int lastSeasonWinner = N["prev_winner_team"].AsInt; //kura komanda uzvaréja 

                            NewsListManager.ActiveRides = N["active_rides"].AsInt; //cik braucieni gaida uz spélétáju
                            NewsListManager.LeagueGamesPlayed = N["league_contribution_games"].AsInt; //cik spéles spéléjis śajá sezoná

                            //1x sezoná jáveic 12h un 2h brídinájums, ká arí sezonas beigu pazinjojums ar uzvaru/zaudéjumu (tehniski: nák. sezonas sákumu)


                            int lastSeasonStartMentioned = PlayerPrefs.GetInt("lastSeasonStartMentioned", 0);
                            if (lastSeasonStartMentioned != SeasonID)
                            { //nav pazińots par śís sezonas sákśanos

                                if (lastSeasonStartMentioned != 0)
                                { //pirmá reize - nezinjosim par 0. sezonas beigám
                                    NewsListManager.Push(
                                        Lang.Get("News:MP:Season  # |param| is over").Replace("|param|", (SeasonID - 1).ToString())
                                        , NewsListItemType.mpLeague, GameScreenType.MultiplayerMenu, "League");
                                }
                                PlayerPrefs.SetInt("lastSeasonStartMentioned", SeasonID);
                            }

                            int lastSeason12hMentioned = PlayerPrefs.GetInt("lastSeason12hMentioned", 0); //
                            if (SeasonTTL < 12 * 3600 && SeasonTTL > 2 * 3600 && lastSeason12hMentioned != SeasonID)
                            { //ir mazák par 12h lídz beigám un śai sezonai nav vél bijusi 12h zińa
                                NewsListManager.Push(Lang.Get("News:MP:LeagueSeasonOverSoon"), NewsListItemType.mpLeague, GameScreenType.MultiplayerMenu, "League");
                                PlayerPrefs.SetInt("lastSeason12hMentioned", SeasonID);
                            }

                            int lastSeason2hMentioned = PlayerPrefs.GetInt("lastSeason2hMentioned", 0);
                            if (SeasonTTL < 2 * 3600 && lastSeason2hMentioned != SeasonID)
                            { //ir mazák par 2h lídz beigám un śai sezonai nav vél bijusi 2h zińa
                                NewsListManager.Push(Lang.Get("News:MP:LeagueSeasonOverVerySoon"), NewsListItemType.mpLeague, GameScreenType.MultiplayerMenu, "League");
                                PlayerPrefs.SetInt("lastSeason2hMentioned", SeasonID);
                            }

                        }
                        catch (System.Exception e)
                        {
                            if (Debug.isDebugBuild) { print("a) Wrong JSON:" + e.Message); }
                            MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
                        }



                    }, delegate (WWW www)
                    {
                        if (www.error.ToString().Contains("Internal Server Error"))
                            MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
                        else if (www.error.ToString().Contains("404 Not Found"))
                            MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
                        else
                            MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailNoInternet");
                        UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, false);
                        if (Debug.isDebugBuild) { print("get_news-- " + www.error); }
                    }));

                }
            }
            else
            {
                if (Debug.isDebugBuild) { print("Notiek brauciens"); }
            }


            yield return new WaitForSeconds(MPNewsCheckPeriod);
        }

    }


    /**
	 * idzésís failus no MP keśa diras, kas nav atvérti vismaz 7 dienas
	 * neizpildás bieźák ká 1x diená
	 */
    public static void CleanUpOldFiles()
    {

        int day = System.DateTime.Now.Day;
        //print("CleanUpOldFiles day=" + day);
        if (PlayerPrefs.GetInt("lastMPCleanupDay", 0) == day)
        {
            //print("Skip MP cleanup");
            return;
        }

        PlayerPrefs.SetInt("lastMPCleanupDay", day);



        string[] fileList = System.IO.Directory.GetFiles(Application.persistentDataPath + "/mp-cache/"/*, "*.jpg"*/);

        for (int i = 0; i < fileList.Length; i++)
        {

            System.IO.FileInfo fi = new System.IO.FileInfo(fileList[i]);
            System.DateTime t = fi.LastAccessTime;

            System.TimeSpan age = System.DateTime.Now - t;

            if (age.TotalDays >= 7)
            {
                //print(i + ") " + fileList[i] + " t = " + age + "  vecs!");
                System.IO.File.Delete(fileList[i]);
            }
            else
            {
                //print(i + ") " + fileList[i] + " t = " + age + "  paturam");
            }



        }
    }

    //izlogo árá no FB un arí no MP servera - lai nákamreiz nák iekśá bez FB
    public static void FBLogOut()
    {
        HasFB = false;
        PlayerPrefs.SetInt("HasFB", 0);

        FBID = "0";
        PlayerPrefs.SetString("FBID", "0");

        PlayerPrefs.DeleteKey("MPID"); //aizvác MP IDu, lai izdomá par jaunu nákamreiz 

        //lietas sesijá - já nu vińś grib túlít spélét
        PlayerTeamID = 0;
        NumWins = 0;
        NumGames = 0;
        MPLevel = 0;
        Cups = 0;
        LoggedIn = false;

        //piekeśotás lietas, kuras tápat tiks párrakstítas péc 1. logina
        PlayerPrefs.SetInt("NumWins", 0);
        PlayerPrefs.SetInt("NumGames", 0);
        PlayerPrefs.SetInt("MPLevel", 0);
        PlayerPrefs.SetInt("Cups", 0);
        PlayerPrefs.SetInt("PlayerTeamID", 0);
        PlayerPrefs.DeleteKey("HadIntroGame");


        int MPNumReset = PlayerPrefs.GetInt("MPNumReset", 0);
        MPNumReset++;
        PlayerPrefs.SetInt("MPNumReset", MPNumReset); //inkrementé resetośanas skaitu - lai śo IDu turpmák lietotu nákamajá MPIDá


        PlayerPrefs.Save();

        LoggedIn = false;
        FBFriendsDownloaded = false;

        if (Debug.isDebugBuild) { print("FB log out done"); }
    }

    public static void QuitGame()
    {
        Time.timeScale = 1f;

        switch (MultiplayerManager.CurrentOpponent.MPType)
        {
            case MPTypes.league:
                //MultiplayerPostGameLeague
                UIManager.SwitchScreen(GameScreenType.MultiplayerPostGameLeague);
                break;
            case MPTypes.replay:
                //MultiplayerPostGameReplay
                UIManager.SwitchScreen(GameScreenType.MultiplayerPostGameReplay);
                break;
            case MPTypes.revanche:
                //MultiplayerPostGameRevanche
                UIManager.SwitchScreen(GameScreenType.MultiplayerPostGameRevanche);
                break;
            case MPTypes.first:
                //MultiplayerPostGameFriend
                UIManager.SwitchScreen(GameScreenType.MultiplayerPostGameFriend);
                break;
        }
    }

    //párrékjina manu powerreitingu no paśreizéjiem MP upgreidiem
    public static void RecalculateMyPowerRating()
    {

        PowerRating = PowerRatingManager.Calculate(
            BikeDataManager.Bikes[BikeDataManager.MultiplayerPlayerBikeRecordName].Upgrades[(int)UpgradeType.Acceleration],
            BikeDataManager.Bikes[BikeDataManager.MultiplayerPlayerBikeRecordName].Upgrades[(int)UpgradeType.AccelerationStart],
            BikeDataManager.Bikes[BikeDataManager.MultiplayerPlayerBikeRecordName].Upgrades[(int)UpgradeType.MaxSpeed],
            BikeDataManager.Bikes[BikeDataManager.MultiplayerPlayerBikeRecordName].Upgrades[(int)UpgradeType.BreakSpeed]);


        PermanentPowerRating = PowerRatingManager.Calculate(
            BikeDataManager.Bikes[BikeDataManager.MultiplayerPlayerBikeRecordName].UpgradesPerm[(int)UpgradeType.Acceleration],
            BikeDataManager.Bikes[BikeDataManager.MultiplayerPlayerBikeRecordName].UpgradesPerm[(int)UpgradeType.AccelerationStart],
            BikeDataManager.Bikes[BikeDataManager.MultiplayerPlayerBikeRecordName].UpgradesPerm[(int)UpgradeType.MaxSpeed],
            BikeDataManager.Bikes[BikeDataManager.MultiplayerPlayerBikeRecordName].UpgradesPerm[(int)UpgradeType.BreakSpeed]);
    }

    //pretinieku powereitinga réḱináśana @vélams -- ka formulas sakrít ;)
    public static int CalculatePowerRating(int[] upgrades)
    {

        return PowerRatingManager.Calculate(
            upgrades[0],
            upgrades[1],
            upgrades[2],
            upgrades[3]);

    }

    public static void ManualResetTeam()
    {

        if (PlayerTeamID == 0)
        { /// nav komandas vispár - lai iet uz MP galveno lapu - tur ar vinju tiks galá
			UIManager.SwitchScreen(GameScreenType.MultiplayerMenu);
            return;
        }


        if (SeasonTTL > 60 * 60 * 6)
        { //ja vairák ká 6h lídz sezonas beigám, tad tikai ljauj mainít
            UIManager.SwitchScreen(GameScreenType.MultiplayerLeagueSelection);
        }
        else
        {
            PopupGenericErrorBehaviour.ErrorMessage = Lang.Get("MP:Error:UnableToChangeTeam:DeadlineTooClose");
            UIManager.ToggleScreen(GameScreenType.PopupGenericError);
        }

    }

    private static void UpdateMPID()
    {
        int MPNumReset = PlayerPrefs.GetInt("MPNumReset", 0);  //jkatrs MP resets inkrementé śo,  tas liks MPIDam nomainíties
        MPID = PlayerPrefs.GetString("MPID", "AN-" + MPNumReset + "-" + SystemInfo.deviceUniqueIdentifier); //jauns vai atrasts pierakstíts anonímais identifikátors
    }

    private static int RandomNotMyTeam()
    {
        int x;
        while (true)
        {
            x = UnityEngine.Random.Range(1, 5);// [1-4]
            if (x != PlayerTeamID)
            {
                return x; //atrada nejauśu numuru, kas nav spélétája komandas numurs
            }
        }
    }

    private static int asapCounter = 0;
    private static IEnumerator OpenInvitePopupASAP()
    {
        asapCounter++;

        if (asapCounter > 1)
        {
            //print("max 1 popupa atveereejs");
            yield break;
        }

        while (true)
        {

            yield return new WaitForSeconds(0.3f);

            if (!LoggedIn || !HasFB)
            {
                //print("veel nav ielogojjies");
                continue;
            }

            //print("nu ir jaaver valjaa invaitu popups!");
            ButtonFBInvite();
            asapCounter--;
            yield break;

        }
    }

    //zińo serverim, ka ir vinnéjis kausińus (serveris pieńem tikai predefinétus daudzumus un ne párák bieźi)
    public static void GiveCups(int number)
    {

        WWWForm data = new WWWForm();
        data.AddField("WonCups", number);

        mb.StartCoroutine(MPRequest("post_", data, delegate (WWW www)
        {
            MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
            //	print("GiveCups++");
        }, delegate (WWW www)
        {
            if (www.error.ToString().Contains("Internal Server Error"))
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
            else if (www.error.ToString().Contains("404 Not Found"))
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
            else
                MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailNoInternet");
            //	print("GiveCups--");
        }));

    }

}




}
