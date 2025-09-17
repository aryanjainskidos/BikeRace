namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MultiplayerGameBehaviour : MonoBehaviour
{

    float waitAfterFinish = 2;
    float secondsSinceFinish = 0;
    //int coins = -1;
    Text coinText;
    GameObject skipButton;
    UIButtonSwitchScreen skipButtonSwitchScreenComponent;

    RectTransform timeDisplayPanel;

    Transform canvasDynamicTr;
    Transform timeDisplayTr;


    void Awake()
    {

        canvasDynamicTr = GameObject.Find("CanvasDynamic").transform;
        canvasDynamicTr.GetComponent<Canvas>().enabled = true;
        canvasDynamicTr.GetComponent<CanvasScaler>().enabled = true;
        foreach (Transform c in canvasDynamicTr)
        {
            if (c.name == "TimeDisplay_MP")
            {//iznícina visus CanvasDynamic atrastos objektus (tikai kurus tur ievieto śís skripts) 
                Destroy(c.gameObject);
            }
        }

        timeDisplayTr = transform.Find("TimeDisplay");

        timeDisplayTr.name += "_MP";
        timeDisplayTr.SetParent(canvasDynamicTr, false);
        timeDisplayTr.gameObject.SetActive(false);


        timeDisplayPanel = timeDisplayTr.GetComponent<RectTransform>();


        skipButton = transform.Find("SkipButton").gameObject;
        skipButton.SetActive(false);
        skipButtonSwitchScreenComponent = skipButton.GetComponent<UIButtonSwitchScreen>();

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

                if (BikeGameManager.playerState.finished)
                {
                    if (secondsSinceFinish >= waitAfterFinish)
                    {
                        Reset();
                        SwitchToCorrectMPPostGameScreen();
                    }
                    else
                    {
                        if (!skipButton.activeSelf)
                        {
                            skipButton.SetActive(true);
                            skipButtonSwitchScreenComponent.screen = GetCorrectFinishScreen();
                        }
                        secondsSinceFinish += Time.deltaTime;
                    }
                }

                if (BikeGameManager.playerState.dead && !BikeGameManager.playerState.finished)
                {//if reset was called will skip this				
                    UIManager.SwitchScreen(GameScreenType.MultiplayerCrash); //can´t call this right away(in BikeJustFinished)
                }

                if (BikeGameManager.lastCommand == GameCommand.Reset)
                {
                    UIManager.SwitchScreen(GameScreenType.MultiplayerPreGame);
                }

                if (!BikeGameManager.playerState.dead)
                {
                    BikeGameManager.CheckBikeAgainstBounds();
                }

                if (BikeGameManager.TimeElapsed > 60)
                    BikeGameManager.ExecuteCommand(GameCommand.KillBike);

            }
        }

    }


    void OnEnable()
    {
        if (Startup.Initialized)
        {
            TelemetryManager.EventLevelstart(LevelManager.CurrentLevelName);

            if (!timeDisplayPanel.gameObject.activeSelf)
            {
                timeDisplayPanel.gameObject.SetActive(true);
            }
        }
    }

    void OnDisable()
    {
        if (!Startup.Initialized)
        {
            return;
        }

        if (timeDisplayTr != null)
        {
            timeDisplayTr.gameObject.SetActive(false);
        }

        Reset();
    }


    void Reset()
    {
        frame = 0;
        secondsSinceFinish = 0;
        skipButton.SetActive(false);
    }


    private GameScreenType GetCorrectFinishScreen()
    {
        if (MultiplayerManager.CurrentOpponent.MPType == MPTypes.first)
        {
            return GameScreenType.MultiplayerPostGameFriend;
        }
        else if (MultiplayerManager.CurrentOpponent.MPType == MPTypes.revanche)
        {
            return GameScreenType.MultiplayerPostGameRevanche;
        }
        else if (MultiplayerManager.CurrentOpponent.MPType == MPTypes.league)
        {
            return GameScreenType.MultiplayerPostGameLeague;
        }
        else if (MultiplayerManager.CurrentOpponent.MPType == MPTypes.replay)
        {
            return GameScreenType.MultiplayerPostGameReplay;
        }
        else
        {
            Debug.LogError("Wrong MP type");
            return GameScreenType.MultiplayerPostGameFriend;
        }
    }


    private void SwitchToCorrectMPPostGameScreen()
    {
        UIManager.SwitchScreen(GetCorrectFinishScreen());
    }

}

}
