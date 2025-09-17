namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class CrashBehaviour : MonoBehaviour
{

    UIButtonSwitchScreen switchScreenComponent;
    UIButtonGameCommand gameCommandComponent;


    void Awake()
    {
        switchScreenComponent = transform.Find("StartButton").GetComponent<UIButtonSwitchScreen>();
        gameCommandComponent = transform.Find("StartButton").GetComponent<UIButtonGameCommand>();
    }


    void Update()
    {

    }

    void OnEnable()
    {

        if (Startup.Initialized)
        {

            if (BikeGameManager.singlePlayerRestarts == 0)
            {
                switchScreenComponent.screen = GameScreenType.PostGameLong;// postgamelong
                gameCommandComponent.enabled = false;
            }
            else
            {
                if (switchScreenComponent.screen != GameScreenType.PreGame)
                {
                    switchScreenComponent.screen = GameScreenType.PreGame;
                    gameCommandComponent.enabled = true;
                }

            }

        }
    }
}

}
