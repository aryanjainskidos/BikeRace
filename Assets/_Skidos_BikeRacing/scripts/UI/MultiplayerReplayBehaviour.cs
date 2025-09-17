namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class MultiplayerReplayBehaviour : MonoBehaviour
{
    //@todo -- pieśḱirt objektam
    //*
    bool updated = false;

    bool allAIFinishedOrCrashed;

    float waitAfterFinish = 1;
    float secondsSinceFinish = 0;

    SlideOutBehaviour replayHideTweenBehaviour;

    SlideOutBehaviour infoPanelTweenBehaviour;

    SlideOutBehaviour timeDisplayTweenBehaviour;

    void Awake()
    {
        infoPanelTweenBehaviour = transform.Find("InfoPanel").GetComponent<SlideOutBehaviour>();
        replayHideTweenBehaviour = transform.Find("ReplayText").GetComponent<SlideOutBehaviour>();
        timeDisplayTweenBehaviour = transform.Find("TimeDisplay").GetComponent<SlideOutBehaviour>();
    }

    void OnDisable()
    {
        updated = false;
        allAIFinishedOrCrashed = false;
    }

    void Update()
    {
        if (!updated && BikeGameManager.initialized && LevelManager.loadedLevel)
        {

            BikeGameManager.timer.timerLast = BikeGameManager.timer.timerStart = 0;
            BikeGameManager.timer.TimerStart();

            BikeGameManager.ExecuteCommand(GameCommand.PauseOff);
            infoPanelTweenBehaviour.Play();
            replayHideTweenBehaviour.Play();
            timeDisplayTweenBehaviour.Play();
            updated = true;
        }

        if (BikeGameManager.aiStates != null && !allAIFinishedOrCrashed && BikeGameManager.aiStates.Count > 0)
        {
            allAIFinishedOrCrashed = true;
            foreach (var aiState in BikeGameManager.aiStates)
            {
                if (!(aiState.finished || aiState.dead))
                { //if at least one is not finished and not dead
                    allAIFinishedOrCrashed = false;
                }
            }
        } //else if(GameManager.aiStates == null || GameManager.aiStates.Count <= 0)
          //  Debug.Log("GameManager.aiStates == null || GameManager.aiStates.Count <= 0");

        if (allAIFinishedOrCrashed)
        {
            if (secondsSinceFinish >= waitAfterFinish)
            {
                Reset();
                UIManager.SwitchScreen(GameScreenType.MultiplayerPostGameReplay);
            }
            else
            {
                secondsSinceFinish += Time.deltaTime;
            }
        }
    }

    void Reset()
    {
        secondsSinceFinish = 0;
    }


}

}
