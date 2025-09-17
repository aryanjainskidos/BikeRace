namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MultiplayerPostGameBehaviour : MonoBehaviour
{

    static Color32 red = new Color32(249, 2, 2, 255);
    static Color32 green = new Color32(0, 247, 8, 255);
    static Color32 yellow = new Color32(255, 228, 0, 255);

    CoinDisplayBehaviour coinDisplayBehaviour;

    Text playerNameText;
    Image playerPicture;
    Image playerTeamIcon;
    Text playerCoinText;
    Text playerTimeText;
    Text playerCupText;

    Text opponentNameText;
    Image opponentPicture;
    Image opponentTeamIcon;
    Text opponentCoinText;
    Text opponentTimeText;
    Text opponentCupText;

    GameObject playerCupImage;
    GameObject playerCoinImage;

    GameObject opponentCupImage;
    GameObject opponentCoinImage;

    Text finishCalloutText;
    GameObject bigCupImage;

    int Outcome = 999; //-1:spélétájs zaudé, 0:neizśḱirts, 1:spélétájs uzvar

    bool updated = false;

    void Awake()
    {

        coinDisplayBehaviour = transform.Find("CoinPanel").GetComponent<CoinDisplayBehaviour>();


        Transform x;

        playerNameText = transform.Find("InfoPanelPlayer/NameText").GetComponent<Text>();
        playerPicture = transform.Find("InfoPanelPlayer/Picture").GetComponent<Image>();
        playerTeamIcon = transform.Find("InfoPanelPlayer/TeamIcon").GetComponent<Image>();

        if ((x = transform.Find("InfoPanelPlayer/CoinText")) != null)
        {
            playerCoinText = x.GetComponent<Text>();
        }
        if ((x = transform.Find("InfoPanelPlayer/TimeText")) != null)
        {
            playerTimeText = x.GetComponent<Text>();
        }
        if ((x = transform.Find("InfoPanelPlayer/CupText")) != null)
        {
            playerCupText = x.GetComponent<Text>();
        }


        opponentNameText = transform.Find("InfoPanelOpponent/NameText").GetComponent<Text>();
        opponentPicture = transform.Find("InfoPanelOpponent/Picture").GetComponent<Image>();
        opponentTeamIcon = transform.Find("InfoPanelOpponent/TeamIcon").GetComponent<Image>();

        if ((x = transform.Find("InfoPanelOpponent/CoinText")) != null)
        {
            opponentCoinText = x.GetComponent<Text>();
            opponentCoinText.gameObject.SetActive(false); //nevajag pretienieka naudu rádít
        }
        if ((x = transform.Find("InfoPanelOpponent/TimeText")) != null)
        {
            opponentTimeText = x.GetComponent<Text>();
        }
        if ((x = transform.Find("InfoPanelOpponent/CupText")) != null)
        {
            opponentCupText = x.GetComponent<Text>();
        }


        if ((x = transform.Find("FinishCallout/Text")) != null)
        {
            finishCalloutText = x.GetComponent<Text>();
        }

        if ((x = transform.Find("BigCupImage")) != null)
        {
            bigCupImage = x.gameObject;
        }



        if ((x = transform.Find("InfoPanelPlayer/CupImage")) != null)
        {
            playerCupImage = x.gameObject;
        }
        if ((x = transform.Find("InfoPanelPlayer/CoinImage")) != null)
        {
            playerCoinImage = x.gameObject;
        }
        if ((x = transform.Find("InfoPanelOpponent/CupImage")) != null)
        {
            opponentCupImage = x.gameObject;
        }
        if ((x = transform.Find("InfoPanelOpponent/CoinImage")) != null)
        {
            opponentCoinImage = x.gameObject;
            opponentCoinImage.SetActive(false); //nevajag pretienieka naudu rádít
        }


        transform.Find("LevelButton").GetComponent<UIButtonSimpleDelegate>().buttonDelegate = SendButton;


    }


    void OnDisable()
    {
        updated = false;
    }


    /**
	 * update sáks izpildíties péc tam, kad śis ekráns bús attélots - tátad 1sek péc finiśa
	 * 
	 */
    void Update()
    {

        if (!updated)
        {

            playerNameText.text = MultiplayerManager.PlayerName;
            MultiplayerManager.GetPicture(MultiplayerManager.PlayerPicture, MultiplayerManager.FBID, playerPicture);
            if (MultiplayerManager.PlayerTeamID > 0)
            {
                playerTeamIcon.sprite = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/MP/MultiplayerTeams", "TeamIco" + MultiplayerManager.PlayerTeamID);
                playerTeamIcon.gameObject.SetActive(true);
            }
            else
            {
                playerTeamIcon.gameObject.SetActive(false);
            }

            opponentNameText.text = MultiplayerManager.CurrentOpponent.Name;
            MultiplayerManager.GetPicture(MultiplayerManager.CurrentOpponent.Picture, MultiplayerManager.CurrentOpponent.FBID, opponentPicture);
            if (MultiplayerManager.CurrentOpponent.TeamID > 0)
            {
                opponentTeamIcon.sprite = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/MP/MultiplayerTeams", "TeamIco" + MultiplayerManager.CurrentOpponent.TeamID);
                opponentTeamIcon.gameObject.SetActive(true);
            }
            else
            {
                opponentTeamIcon.gameObject.SetActive(false);
            }

            if (finishCalloutText != null)
            {
                //finishCalloutText.text = "lol,n00b";
            }
            if (bigCupImage != null)
            {
                bigCupImage.SetActive(false); //ieslégs vélák, ja vajadzés
            }




            float playerTime = 13;
            float opponentTime = 13;

            if (MultiplayerManager.CurrentOpponent.MPType == MPTypes.replay)
            {
                playerTime = MultiplayerManager.CurrentOpponent.ReplayMyTime;
                opponentTime = MultiplayerManager.CurrentOpponent.ReplayOppTime;
            }
            else
            { //revanśs vai pirmá 											

                if (BikeGameManager.playerState != null)
                { //playerState var bút null, ja śí nav normála braukśana, bet ir vienkárśi atvérts logs
                    playerTime = BikeGameManager.TimeElapsed;
                    opponentTime = MultiplayerManager.CurrentOpponent.Time;

                    if (!BikeGameManager.playerState.finished)
                    {
                        playerTime = -1;
                    }
                }
            }



            if (MultiplayerManager.CurrentOpponent.MPType == MPTypes.first)
            {


            }
            else
            { //replejs un revanśs un líga

                finishCalloutText.color = yellow; //default callout text

                if (playerTime > 0)
                { //spélétájs finiśéja

                    if (opponentTime < 0)
                    { //pretinieks gan neizbrauca
                        finishCalloutText.text = Lang.Get("MP:FinishCallout:Victory");
                        //opponentOutcome = "Defeat";
                        Outcome = 1;
                    }
                    else
                    { //pretinieks izbrauca
                        if (opponentTime.ToString("F2") == playerTime.ToString("F2"))
                        { // laiks, noapaĺojot, ir vienáds
                            finishCalloutText.text = Lang.Get("MP:FinishCallout:Tie");
                            //opponentOutcome = "Tie";
                            Outcome = 0;
                        }
                        else
                        {
                            if (opponentTime < playerTime)
                            {//ja nav neizśḱirts, tad jáskatás kurś átráks
                                finishCalloutText.text = Lang.Get("MP:FinishCallout:Defeat");
                                finishCalloutText.color = red;
                                //opponentOutcome = "Victory";
                                Outcome = -1;
                            }
                            else
                            {
                                finishCalloutText.text = Lang.Get("MP:FinishCallout:Victory");
                                //opponentOutcome = "Defeat";
                                Outcome = 1;
                            }
                        }
                    }
                }
                else
                { //spélétájs nefiniśéja

                    if (opponentTime < 0)
                    { //pretinieks arí neizbrauca
                        finishCalloutText.text = Lang.Get("MP:FinishCallout:Tie");
                        //opponentOutcome = "Tie";
                        Outcome = 0;
                    }
                    else
                    { //pretinieks izbrauca
                        finishCalloutText.text = Lang.Get("MP:FinishCallout:Defeat");
                        finishCalloutText.color = red; // override default color with red
                                                       //opponentOutcome = "Victory";
                        Outcome = -1;
                    }
                }
            }


            playerTimeText.text = (playerTime > 0 ? playerTime.ToString("F2") : Lang.Get("MP:Time:DNF"));

            if (MultiplayerManager.CurrentOpponent.MPType == MPTypes.first)
            {
                opponentTimeText.text = Lang.Get("MP:Time:Pending");
            }
            else
            { //replejs vai revanśs
                opponentTimeText.text = (opponentTime != -1 ? opponentTime.ToString("F2") : Lang.Get("MP:Time:DNF"));
            }


            int visualMultiplier = 1;
            if (CentralizedOfferManager.IsDoubleCoinWeekendOn())
            {
                visualMultiplier = 2;
            }

            if (Outcome == 1)
            { //naudas pieskaitíśana ir tikai vizuála, tá pa ístam tiks pieskaitíta tikai submitojot spéli
                coinDisplayBehaviour.auto = false;
                coinDisplayBehaviour.coinsTo += (MultiplayerManager.CoinsPerWin * visualMultiplier);
            }


            if (Outcome == 1)
            { //spélétájs uzvar
                bigCupImage.SetActive(true);

                playerCoinImage.SetActive(true);
                playerCoinText.gameObject.SetActive(true);
                playerCoinText.text = "+" + (MultiplayerManager.CoinsPerWin * visualMultiplier);

                playerCupImage.SetActive(true);
                playerCupText.gameObject.SetActive(true);
                playerCupText.color = green;
                playerCupText.text = "+" + MultiplayerManager.CurrentOpponent.CupsTrackWin;

                opponentCupImage.SetActive(true);
                opponentCupText.gameObject.SetActive(true);
                opponentCupText.color = red;
                opponentCupText.text = "-" + MultiplayerManager.CurrentOpponent.CupsTrackWin;
                //@todo -- sarkans

            }
            else if (Outcome == 0)
            { //neizśḱirts
                playerCoinImage.SetActive(false);
                playerCoinText.gameObject.SetActive(false);

                playerCupImage.SetActive(false);
                playerCupText.gameObject.SetActive(false);

                opponentCupImage.SetActive(false);
                opponentCupText.gameObject.SetActive(false);

            }
            else if (Outcome == -1)
            { //spélétájs zaudé

                playerCoinImage.SetActive(false);
                playerCoinText.gameObject.SetActive(false);


                playerCupImage.SetActive(true);
                playerCupText.gameObject.SetActive(true);
                playerCupText.color = red;
                playerCupText.text = "-" + MultiplayerManager.CurrentOpponent.CupsTrackLose;

                opponentCupImage.SetActive(true);
                opponentCupText.gameObject.SetActive(true);
                opponentCupText.color = green;
                opponentCupText.text = "+" + MultiplayerManager.CurrentOpponent.CupsTrackLose;

            }



            /*
			//promo zińas uzvarétájiem - aicinás uz garáźu
			if(MultiplayerManager.NumWins >= 2 && PlayerPrefs.GetInt("promoNews2MP", 0) == 0){ //ir 2 MP uzvaras un nav vél bijis pazińojums par MP uzvarám
				NewsListManager.Push(Lang.Get("News:Promo:UpgradeBike"), NewsListItemType.promo, GameScreenType.Garage, "Upgrade", "Competitive" ); 
				PlayerPrefs.SetInt("promoNews2MP", 1);
			}
			
			if(MultiplayerManager.NumWins >= 8 && PlayerPrefs.GetInt("promoNews8MP", 0) == 0){
				NewsListManager.Push(Lang.Get("News:Promo:UpgradeBike"), NewsListItemType.promo, GameScreenType.Garage, "Upgrade", "Competitive"); 
				PlayerPrefs.SetInt("promoNews8MP", 1);
			}*/

            updated = true;
        }
    }



    /**
	 * párslégs uz nákamo ekránu (if any)
	 * nosútís info serverim (if any)
	 */
    public void SendButton()
    {
        //print("MP:SendButton");


        if (Outcome == 1)
        { // ja uzvaréja, tad tagad ir laiks pa ístam pieśḱirt naudu

            int coinMultiplier = 1;
            if (CentralizedOfferManager.IsDoubleCoinWeekendOn())
            {
                coinMultiplier = 2;
            }
            BikeDataManager.Coins += (MultiplayerManager.CoinsPerWin * coinMultiplier);

            //print("real money++");
            MultiplayerManager.NumWins++;
        }
        MultiplayerManager.NumGames++;

        PlayerPrefs.SetInt("NumWins", MultiplayerManager.NumWins);
        PlayerPrefs.SetInt("NumGames", MultiplayerManager.NumGames);


        switch (MultiplayerManager.CurrentOpponent.MPType)
        {
            case MPTypes.league:

                UIManager.SetScreenTimeout(GameScreenType.PopupMultiplayerLoading, 10);
                UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, true); //ieslédzu loadingScreen ar taimautu

                MultiplayerManager.MPPostGame(MultiplayerManager.CurrentOpponent.LeagueChallengeId,
                                              MultiplayerManager.CurrentOpponent.Track,
                                              BikeGameManager.playerState.finished ? BikeGameManager.TimeElapsed.ToString("F2") : "-1",
                                              MultiplayerManager.CurrentOpponent.CupsTrackWin,
                                              MultiplayerManager.CurrentOpponent.CupsTrackLose,
                                              "",
                                              delegate (string status)
                                              {
                                                  UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, false);
                                                  UIManager.SwitchScreen(GameScreenType.MultiplayerMenu); //kad ir nosútíts (vai arí péc vairákiem méǵinájumiem - nav) aizveru loadingScreen un vedu spélétáju uz sákumu
                                          });



                //print("MP tips liiga  - ejam maajaas ");
                break;
            case MPTypes.replay:

                MultiplayerManager.CurrentOpponent.MPType++;
                MultiplayerManager.CurrentOpponent.Play();
                //print("MP tips replejs  - vajadzétu bút revanśam ");
                break;
            case MPTypes.revanche:

                MultiplayerManager.MPPostGame(0,
                                              MultiplayerManager.CurrentOpponent.Track,
                                              BikeGameManager.playerState.finished ? BikeGameManager.TimeElapsed.ToString("F2") : "-1",
                                              MultiplayerManager.CurrentOpponent.CupsTrackWin,
                                              MultiplayerManager.CurrentOpponent.CupsTrackLose
                                              );



                /*
                 * @todo -- loading 
                 * + jásagaida atbildi 
                 * + tad jáaizver loading un jáveic nákamá darbíba   <--- śís abas lietas, tikai, ja logs nav manuáli aizvérts!
                 * 
                 * @ideja -- negaidam uz POSTu - ĺaujam braukt, lai postéjas bekgraundá
                 * 
                 */

                MultiplayerManager.CurrentOpponent.MPType++; //nákamais brauciena veids
                MultiplayerManager.CurrentOpponent.Play(); //aidá, nákamais brauciens

                //print("MP tips revanśs - bús pirmais ");

                break;
            case MPTypes.first:
                //print("MP tips pirmais  - vairs nebús nekas ");

                string pokeText = transform.Find("InputField/Text").GetComponent<Text>().text;

                UIManager.SetScreenTimeout(GameScreenType.PopupMultiplayerLoading, 10);
                UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, true); //ieslédzu loadingScreen ar taimautu


                MultiplayerManager.MPPostGame(0,
                                              MultiplayerManager.CurrentOpponent.NextTrack,
                                              BikeGameManager.playerState.finished ? BikeGameManager.TimeElapsed.ToString("F2") : "-1", //laiks ir strings ar 2 zímém aiz komata; -1, ja nefiniśéja
                                              MultiplayerManager.CurrentOpponent.CupsNextTrackWin,
                                              MultiplayerManager.CurrentOpponent.CupsNextTrackLose,
                                              pokeText,
                                              delegate (string status)
                                              {
                                                  UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, false);
                                                  UIManager.SwitchScreen(GameScreenType.MultiplayerMenu); //kad ir nosútíts (vai arí péc vairákiem méǵinájumiem - nav) aizveru loadingScreen un vedu spélétáju uz sákumu
                                          });




                break;

        }


    }

}

}
