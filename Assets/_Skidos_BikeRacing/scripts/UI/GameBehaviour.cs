namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameBehaviour : MonoBehaviour
{

    float waitAfterFinish = 2;
    float secondsSinceFinish = 0;
    int coins = -1;
    Text coinText;
    GameObject skipButton;
    UIButtonSwitchScreen switchScreenComponent;

    RectTransform coinDisplayPanel;
    RectTransform timeDisplayPanel;
    Vector2 coinAnchoredPosition;

    Transform canvasDynamicTr;
    Transform coinDisplayTr;
    Transform timeDisplayTr;



    void Awake()
    {

        canvasDynamicTr = GameObject.Find("CanvasDynamic").transform;
        canvasDynamicTr.GetComponent<Canvas>().enabled = true;
        canvasDynamicTr.GetComponent<CanvasScaler>().enabled = true;
        foreach (Transform c in canvasDynamicTr)
        {
            if (c.name == "LevelCoinPanel_SP" || c.name == "TimeDisplay_SP")
            {//iznícina visus CanvasDynamic atrastos objektus (tikai kurus tur ievieto śís skripts) 
                Destroy(c.gameObject);
            }
        }


        //@todo -- padarít statisku un veidot tikai, ja nav 
        //@tas pats -- MultiplayerGameBehaviour
        coinDisplayTr = transform.Find("LevelCoinPanel");
        timeDisplayTr = transform.Find("TimeDisplay");

        coinText = coinDisplayTr.Find("CoinText").GetComponent<Text>();
        coinText.text = "0";

        coinDisplayTr.name += "_SP";
        timeDisplayTr.name += "_SP";
        coinDisplayTr.SetParent(canvasDynamicTr, false);//move coin display and time display to another canvas (for HUGE performance boost)
        timeDisplayTr.SetParent(canvasDynamicTr, false);
        coinDisplayTr.gameObject.SetActive(false); //will activate/deactivate on enable/disable
        timeDisplayTr.gameObject.SetActive(false);


        coinDisplayPanel = coinDisplayTr.GetComponent<RectTransform>();
        timeDisplayPanel = timeDisplayTr.GetComponent<RectTransform>();
        coinAnchoredPosition = coinDisplayPanel.anchoredPosition;


        skipButton = transform.Find("SkipButton").gameObject;
        skipButton.SetActive(false);
        switchScreenComponent = skipButton.GetComponent<UIButtonSwitchScreen>();


    }

    int frame = 0;

    void Update()
    {

        if (frame < 3)
            frame++;
        else
        {
            if (BikeGameManager.initialized && LevelManager.loadedLevel && BikeGameManager.gamePaused) //something is slowing the game down for the first 2 frames, so put the unpausing off by 2 frames
            {
                BikeGameManager.ExecuteCommand(GameCommand.PauseOff);
            }

            if (BikeGameManager.playerState != null)
            {

                //DONE move this to bike state data? or somwhere else
                if (BikeGameManager.playerState.finished)
                {
                    //DONE wait for a sec before switching
                    if (secondsSinceFinish >= waitAfterFinish)
                    {
                        Reset();

                        //if long level switch to PostGameLong
                        if (BikeGameManager.longLevel)
                        {//GameManager.levelInfo.LevelName.ToLower().Contains("long")
                            UIManager.SwitchScreen(GameScreenType.PostGameLong);
                        }
                        else
                        {
                            UIManager.SwitchScreen(GameScreenType.PostGame);
                        }
                    }
                    else
                    {
                        if (!skipButton.activeSelf)
                        {
                            skipButton.SetActive(true);

                            //if long level set the button target to PostGameLong
                            if (BikeGameManager.longLevel)
                            {//GameManager.levelInfo.LevelName.ToLower().Contains("long")
                                switchScreenComponent.screen = GameScreenType.PostGameLong;
                            }
                            else
                            {
                                switchScreenComponent.screen = GameScreenType.PostGame;
                            }
                        }
                        secondsSinceFinish += Time.deltaTime;
                    }

                }

                if (BikeGameManager.playerState.dead && !BikeGameManager.playerState.finished)
                {//if reset was called will skip this				
                    UIManager.SwitchScreen(GameScreenType.Crash); //can´t call this right away(in BikeJustFinished)
                }

                if (BikeGameManager.lastCommand == GameCommand.Reset)
                {
                    UIManager.SwitchScreen(GameScreenType.PreGame);
                }

                if (!BikeGameManager.playerState.dead)
                {
                    BikeGameManager.CheckBikeAgainstBounds();
                }

                if (BikeGameManager.longLevel)
                {
                    if (BikeGameManager.TimeElapsed > 360)
                        BikeGameManager.ExecuteCommand(GameCommand.KillBike);
                }
                else
                {
                    if (BikeGameManager.TimeElapsed > 60)
                        BikeGameManager.ExecuteCommand(GameCommand.KillBike);
                }
            }


            if (PickupManager.CoinsCollected != coins)
            {
                coins = PickupManager.CoinsCollected;
                coinText.text = coins.ToString("F0");
            }
        }
    }


    void OnEnable()
    {
        if (Startup.Initialized)
        {

            BikeGameManager.StartGame();
            TelemetryManager.EventLevelstart(LevelManager.CurrentLevelName);

            //ALWAYS hide timer UI and move coin display to timer position for neurodivergent accessibility
            if (timeDisplayPanel.gameObject.activeSelf)
            {
                timeDisplayPanel.gameObject.SetActive(false);
            }
            coinDisplayPanel.anchoredPosition = timeDisplayPanel.anchoredPosition;

            if (!coinDisplayPanel.gameObject.activeSelf)
            {
                coinDisplayPanel.gameObject.SetActive(true);
            }

        }

    }

    void OnDisable()
    {
        if (!Startup.Initialized)
        {
            return;
        }

        if (coinDisplayTr != null)
            coinDisplayTr.gameObject.SetActive(false);

        if (timeDisplayTr != null)
            timeDisplayTr.gameObject.SetActive(false);

        Reset();
    }


    void Reset()
    {
        frame = 0;
        secondsSinceFinish = 0;
        skipButton.SetActive(false);
    }


}

}
