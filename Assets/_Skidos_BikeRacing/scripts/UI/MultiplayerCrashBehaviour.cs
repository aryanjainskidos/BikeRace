namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class MultiplayerCrashBehaviour : MonoBehaviour
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

            if (BikeGameManager.multiPlayerRestarts == 0)
            {

                switch (MultiplayerManager.CurrentOpponent.MPType)
                {
                    case MPTypes.league:
                        switchScreenComponent.screen = GameScreenType.MultiplayerPostGameLeague;
                        break;
                    case MPTypes.replay:
                        switchScreenComponent.screen = GameScreenType.MultiplayerPostGameReplay;
                        break;
                    case MPTypes.revanche:
                        switchScreenComponent.screen = GameScreenType.MultiplayerPostGameRevanche;
                        break;
                    case MPTypes.first:
                        switchScreenComponent.screen = GameScreenType.MultiplayerPostGameFriend;
                        break;
                }

                gameCommandComponent.enabled = false;
            }
            else
            {
                if (switchScreenComponent.screen != GameScreenType.MultiplayerPreGame)
                {
                    switchScreenComponent.screen = GameScreenType.MultiplayerPreGame;
                    gameCommandComponent.enabled = true;
                }
            }

        }
    }
}

}
