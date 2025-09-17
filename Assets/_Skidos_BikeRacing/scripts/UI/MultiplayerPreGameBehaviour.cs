namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MultiplayerPreGameBehaviour : MonoBehaviour
{

    bool updated = false;

    SlideInBehaviour infoPanelTweenBehaviour;

    GameObject startButton;
    GameObject onScreenControlPanel;

    void Awake()
    {
        infoPanelTweenBehaviour = transform.Find("UIPanel/InfoPanel").GetComponent<SlideInBehaviour>();

        startButton = transform.Find("UIPanel/StartButton").gameObject;
        onScreenControlPanel = transform.Find("UIPanel/OnScreenControlPanel").gameObject;
        startButton.SetActive(false);
        onScreenControlPanel.SetActive(false);
    }

    void OnDisable()
    {
        updated = false;
    }

    void Update()
    {

        if (!updated && BikeGameManager.initialized && LevelManager.loadedLevel)
        {
            infoPanelTweenBehaviour.Play();
            updated = true;

            if (BikeDataManager.SettingsAccelerometer)
            {
                startButton.SetActive(true);
                onScreenControlPanel.SetActive(false);
            }
            else
            {
                startButton.SetActive(false);
                onScreenControlPanel.SetActive(true);
            }
        }
    }

    /*  skripts nav vajadzígs -- izdzést
	GameObject uiPanel;
	GameObject zoomButton;
	GameObject stopButtonPanel;
	//string levelName = "_";


	
	void Awake () {        

		uiPanel = transform.FindChild("UIPanel").gameObject;
        zoomButton = uiPanel.transform.FindChild("ZoomButton").gameObject;

	}

	void OnEnable(){
		//levelName = "_"; //this will force Update() to do its magic 
	}

	void OnDisable(){

	}
	
	void Update() {
		
		//executed once for every new level:
		if(GameManager.initialized &&  LevelManager.CurrentLevelName != levelName) {
			levelName = LevelManager.CurrentLevelName;
		}
		
	}*/

}

}
