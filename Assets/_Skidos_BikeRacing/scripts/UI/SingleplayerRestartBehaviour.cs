namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SingleplayerRestartBehaviour : MonoBehaviour
{

    Text restartsLabel;
    Transform restartButton;
    bool updated = false;

    UIButtonGameCommand gameCommandComponent;
    UIButtonSwitchScreen switchScreenComponent;


    void Awake()
    {
        restartsLabel = transform.Find("RestartsText").GetComponent<Text>();
        restartButton = transform.Find("RestartButton");

        if (restartButton != null)
        {
            gameCommandComponent = restartButton.GetComponent<UIButtonGameCommand>();
            switchScreenComponent = restartButton.GetComponent<UIButtonSwitchScreen>();
        }

    }

    void OnDisable()
    {
        updated = false;
    }

    void OnEnable()
    {
        if (Startup.Initialized)
        {
            updated = false;
            Update();
            updated = false;
        }
    }

    void Update()
    {
        if (!updated && BikeGameManager.initialized)
        {
            if (BikeGameManager.singlePlayerRestarts > -1)
            {

                if (!restartsLabel.gameObject.activeSelf)
                {
                    restartsLabel.gameObject.SetActive(true);
                }
                restartsLabel.text = "x" + BikeGameManager.singlePlayerRestarts;

            }
            else
            {
                if (restartsLabel.gameObject.activeSelf)
                {
                    restartsLabel.gameObject.SetActive(false);
                }
            }

            //TODO if there is a restart button and restart count is 0, rewire the button to go to finish screen
            if (restartButton != null)
            {
                if (BikeGameManager.singlePlayerRestarts == 0)
                {

                    switchScreenComponent.screen = GameScreenType.PostGameLong;// postgamelong
                                                                               //                    gameCommandComponent.enabled = false;
                    gameCommandComponent.command = GameCommand.KillBike;

                }
                else
                {

                    if (switchScreenComponent.screen != GameScreenType.PreGame)
                    {
                        switchScreenComponent.screen = GameScreenType.PreGame;
                        //                        gameCommandComponent.enabled = true;
                        gameCommandComponent.command = GameCommand.Reset;
                    }

                }
            }

            updated = true;
        }
    }

}
}
