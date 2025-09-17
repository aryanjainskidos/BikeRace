namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class MultiplayerPreGameReplayBehaviour : MonoBehaviour
{

    [SerializeField]
    float waitAfterFinish = 1;
    [SerializeField]
    float secondsSinceFinish = 0;

    SlideInBehaviour infoPanelTweenBehaviour;

    void Awake()
    {
        infoPanelTweenBehaviour = transform.Find("UIPanel/InfoPanel").GetComponent<SlideInBehaviour>();
    }

    void OnEnable()
    {
        secondsSinceFinish = 0;
    }
    // Update is called once per frame
    void Update()
    {

        if (BikeGameManager.initialized && LevelManager.loadedLevel)
        {
            if (secondsSinceFinish == 0)
            {
                Camera.main.GetComponent<BikeCamera>().Reset();
                infoPanelTweenBehaviour.Play();
            }

            if (secondsSinceFinish >= waitAfterFinish)
            {
                UIManager.SwitchScreen(GameScreenType.MultiplayerGameReplay);
            }
            else
            {
                secondsSinceFinish += Time.unscaledDeltaTime;
            }
        }
    }
}

}
