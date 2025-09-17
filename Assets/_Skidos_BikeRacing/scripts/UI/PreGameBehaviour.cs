namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PreGameBehaviour : MonoBehaviour
{

    string levelName = "_";

    GameObject tutorialPanel;
    GameObject uiPanel;
    GameObject zoomButton;
    GameObject stopButtonPanel;

    BoostPanelBehaviour boostPanelBehaviour;
    //    SlideInBehaviour boostPanelSlideInBehaviour;

    //    SlideInBehaviour holdPanelTweenBehaviour;

    SlideInBehaviour stopButtonTweenBehaviour;

    public static GameObject stopButtonGo; //prefabs - bilde, ko ieládé tikai, ja járáda; iznícina ieládéto prefabu, ja nav járáda

    public static GameObject TutorialGo;

    GameObject startButton;

    GameObject onScreenControlPanel;

    GameObject brakesPanel;

    GameObject onScreenControlPanelTutorial;

    //ieládéts prefabs - animéts tutoriális - ieládé, ja vajag rádít; izdzéś, kad nokillo límeni - levelManager.KillLevel()

    void Awake()
    {
        tutorialPanel = transform.Find("TutorialPanel").gameObject;
        uiPanel = transform.Find("UIPanel").gameObject;
        zoomButton = uiPanel.transform.Find("ZoomButton").gameObject;
        startButton = uiPanel.transform.Find("StartButton").gameObject;
        stopButtonPanel = uiPanel.transform.Find("StartButton/StopButtonPanel").gameObject;
        stopButtonPanel.SetActive(false);
        tutorialPanel.SetActive(false);
        startButton.SetActive(false);

        boostPanelBehaviour = uiPanel.transform.Find("BoostPanel").GetComponent<BoostPanelBehaviour>();
        //        boostPanelSlideInBehaviour = uiPanel.transform.FindChild("BoostPanel").GetComponent<SlideInBehaviour> ();

        //        holdPanelTweenBehaviour = uiPanel.transform.FindChild("StartButton/HoldPanel").GetComponent<SlideInBehaviour>();
        stopButtonTweenBehaviour = uiPanel.transform.Find("StartButton/StopButtonPanel").GetComponent<SlideInBehaviour>();

        onScreenControlPanel = uiPanel.transform.Find("OnScreenControlPanel").gameObject;
        onScreenControlPanelTutorial = onScreenControlPanel.transform.Find("Tutorial").gameObject;
        brakesPanel = onScreenControlPanel.transform.Find("BrakesPanel").gameObject;
        onScreenControlPanel.SetActive(false);
        onScreenControlPanelTutorial.SetActive(false);
    }

    void OnEnable()
    {
        levelName = "_"; //this will force Update() to do its magic 
    }

    void OnDisable()
    {

    }

    void Update()
    {

        //executed once for every new level:
        if (BikeGameManager.initialized && LevelManager.CurrentLevelName != levelName)
        {
            levelName = LevelManager.CurrentLevelName;

            boostPanelBehaviour.Actualize();
            //            boostPanelSlideInBehaviour.Play();
            //            holdPanelTweenBehaviour.Play();

            if (BikeDataManager.SettingsAccelerometer)
            {
                startButton.SetActive(true);
                onScreenControlPanel.SetActive(false);
                onScreenControlPanelTutorial.SetActive(false);
            }
            else
            {
                startButton.SetActive(false);
                onScreenControlPanel.SetActive(true);

                if (levelName == "a___001" || levelName == "a___002")
                {
                    onScreenControlPanelTutorial.SetActive(true);
                }
                else
                {
                    onScreenControlPanelTutorial.SetActive(false);
                }
            }

            if ((levelName == "a___001" || levelName == "a___002") && BikeDataManager.SettingsAccelerometer)
            {
                ShowTutorial();
            }
            else
            {
                HideTutorial();
            }

            //print("PreGameBehaviour:levelName="  + levelName);
            if (levelName == "a___003" || levelName == "a___004" || levelName == "a___012" || levelName == "a___016")
            {
                //print("ShowStopButton++");
                if (BikeDataManager.SettingsAccelerometer)
                {
                    ShowStopButton();
                }
                else
                {
                    brakesPanel.SetActive(true);
                }
            }
            else
            {
                //print("HideStopButton++");
                if (BikeDataManager.SettingsAccelerometer)
                {
                    HideStopButton();
                }
                else
                {
                    brakesPanel.SetActive(false);
                }
            }

            if (levelName.ToLower().Contains("long"))
            { //disable UNZOOM button for long bonus levels
                zoomButton.SetActive(false);
            }
            else
            {
                zoomButton.SetActive(true);
            }
        }

    }


    void ShowStopButton()
    {
        //print("ShowStopButton::start");
        if (stopButtonGo == null)
        {
            //print("ShowStopButton::create");
            GameObject stopPrefab = LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("StopButtonImage") as GameObject;
            Debug.Log("<color=yellow>Prefab Loaded Name = </color>" + stopPrefab);
            //GameObject stopPrefab = Resources.Load("Prefabs/UI/StopButtonImage") as GameObject;
            stopButtonGo = Instantiate(stopPrefab) as GameObject;
            stopButtonGo.transform.SetParent(stopButtonPanel.transform);
            stopButtonGo.transform.localPosition = new Vector3(0, 0, 1);
            stopButtonGo.transform.localScale = new Vector3(1, 1, -50);
        }
        stopButtonPanel.SetActive(true);

        if (stopButtonTweenBehaviour != null)
            stopButtonTweenBehaviour.Play();
    }

    void HideStopButton()
    {
        //print("HideStopButton::start");
        if (stopButtonGo != null)
        {
            //print("HideStopButton::destroy");
            Destroy(stopButtonGo);
        }
        stopButtonPanel.SetActive(false);
    }


    void ShowTutorial()
    {
            try
            {
                CreateTutorial();
                tutorialPanel.SetActive(true);
                uiPanel.SetActive(false);
            }
            catch (System.Exception ex)
            {
                Debug.Log("Exception in ShowTutorial");
            }
        
    }



    void CreateTutorial()
    {
            try
            {
                if (TutorialGo == false)
                {
                    GameObject tutPrefab = LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("tutorial") as GameObject;
                    Debug.Log("<color=yellow>Prefab Loaded Name = </color>" + tutPrefab);
                    //GameObject tutPrefab = Resources.Load("Prefabs/UI/tutorial") as GameObject;
                    TutorialGo = Instantiate(tutPrefab) as GameObject;

                    TutorialGo.transform.parent = tutorialPanel.transform;

                    TutorialGo.transform.localScale = new Vector3(49, 49, 1);
                    TutorialGo.transform.localPosition = new Vector3(1, 1, -50);

                    TutorialGo.name = "tutorial";

                }
            }
            catch (System.Exception ex)
            {

            }
      
    }




    //for use in UI
    public void HideTutorial()
    {
        tutorialPanel.SetActive(false);
        uiPanel.SetActive(true);
    }

}

}
