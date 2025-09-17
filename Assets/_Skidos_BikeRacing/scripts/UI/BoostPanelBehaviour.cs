namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BoostPanelBehaviour : MonoBehaviour
{

    public List<GameObject> boostButtonGOs;
    List<BoostToggleBehaviour> boostToggleBehaviours;
    GameObject title;
    GameObject content;
    GameObject NoBoostLogo;
    GameObject BoostText;
    GameObject BoostBackground;

    Transform recomendedText;

    bool initialized = false;

    void Awake()
    {
        content = transform.Find("ScrollView/Content").gameObject;
        recomendedText = transform.Find("ScrollView/Content/Text");

        boostButtonGOs = new List<GameObject>();
        boostButtonGOs.Add(transform.Find("ScrollView/Content/FreezeBoostToggle").gameObject);
        boostButtonGOs.Add(transform.Find("ScrollView/Content/MagnetBoostToggle").gameObject);
        boostButtonGOs.Add(transform.Find("ScrollView/Content/ShieldBoostToggle").gameObject);
        boostButtonGOs.Add(transform.Find("ScrollView/Content/FuelBoostToggle").gameObject);

        Transform noBoostLogo = transform.parent.Find("NoBoostLogo");
        if (noBoostLogo != null)
        {
            NoBoostLogo = noBoostLogo.gameObject;
        }
        Transform boostText = transform.parent.Find("BoostText");
        if (noBoostLogo != null)
        {
            BoostText = boostText.gameObject;
        }
        Transform boostBackground = transform.parent.Find("BoostBackground");
        if (boostBackground != null)
        {
            BoostBackground = boostBackground.gameObject;
        }

        Init();

    }

    public void Actualize()
    {

        if (initialized && BikeGameManager.initialized)
        {
            //Debug.Log("BoostPanelBehaviour::Actualize Pass");
            bool suggested;
            bool showPanel = false;
            GameObject boostToggle;
            string key;

            bool toggledNoBoostsAlready = false;

            bool atLeastOneIsSelected = false;

            //Debug.Log("Started going through the boostToggle");
            foreach (BoostToggleBehaviour boostToggleBehaviour in boostToggleBehaviours)
            {
                if (boostToggleBehaviour != null && Startup.Initialized)
                {

                    key = boostToggleBehaviour.key;
                    boostToggle = boostToggleBehaviour.gameObject;
                    //Debug.Log("boostToggleBehaviour.key " + key);

                    if ((transform.parent.parent != null && transform.parent.parent.name == "PreGame") || transform.parent.name == "PreGamePause")
                    {
                        if (BikeDataManager.Boosts[key].Number == 0 && BikeDataManager.Boosts[key].Selected)
                        { //if it was selected, then force deselecting it
                            //Debug.Log("deselect");
                            boostToggleBehaviour.toggle.isOn = false;
                            BikeDataManager.Boosts[key].Selected = false;
                            BoostManager.DisableBoostEffect(key);

                            //TODO show popup
                            if (!toggledNoBoostsAlready && UIManager.lastScreenType != GameScreenType.Levels)
                            { //don't show if already shown or just came from levels
                                //Debug.Log("BUY SOME MORE!");
                                UIManager.ToggleScreen(GameScreenType.PopupNoBoosts);
                                toggledNoBoostsAlready = true;
                            }
                        }
                    }

                    boostToggleBehaviour.toggle.isOn = BikeDataManager.Boosts[key].Selected;
                    boostToggleBehaviour.SetCount(BikeDataManager.Boosts[key].Number);

                    suggested = false;
                    if (BikeGameManager.levelInfo != null)
                    {
                        switch (key)
                        {
                            case "fuel":
                                suggested = BikeGameManager.levelInfo.SuggBoostFuel;
                                break;
                            case "ice":
                                suggested = BikeGameManager.levelInfo.SuggBoostIce;
                                break;
                            case "invincibility":
                                suggested = BikeGameManager.levelInfo.SuggBoostInvincibility;
                                break;
                            case "magnet":
                                suggested = BikeGameManager.levelInfo.SuggBoostMagnet;
                                break;
                            default:
                                suggested = false;
                                break;
                        }
                    }

                    List<string> styleBoosts = BikeGameManager.styleBoosts;//DataManager.Styles[DataManager.Bikes[DataManager.SingleplayerPlayerBikeRecordName].StyleID].Boosts;
                    if (styleBoosts != null && styleBoosts.Contains(key))
                    {
                        suggested = false;
                    }

                    if (BikeDataManager.Boosts[key].Discovered)
                    {
                        boostToggle.SetActive(suggested); //if discovered and suggested show, if discovered but not suggested hide

                        if (suggested)
                        {
                            showPanel = true;
                        }

                    }
                    else
                    {
                        boostToggle.SetActive(false); //if not discovered, hide
                    }

                    if (BikeDataManager.Boosts[key].Selected)
                    {
                        atLeastOneIsSelected = true;
                    }
                }
            }
            //Debug.Log("Finished going through the boostToggle");

            if (NoBoostLogo != null)
            {
                NoBoostLogo.SetActive(!showPanel);
            }
            if (BoostText != null)
            {
                BoostText.SetActive(showPanel);
            }
            if (BoostBackground != null)
            {
                BoostBackground.SetActive(showPanel);
            }
            if (content != null)
            {
                content.SetActive(showPanel);
            }
            if (recomendedText != null)
            {
                recomendedText.gameObject.SetActive(!atLeastOneIsSelected);
            }

            //Debug.Log("Finished activating various objects");

        }
    }


    public void Init()
    {
        boostToggleBehaviours = new List<BoostToggleBehaviour>();

        foreach (var item in boostButtonGOs)
        {
            BoostToggleBehaviour btb = item.GetComponent<BoostToggleBehaviour>();
            if (btb.cid == null)
            {
                btb.Awake();
            }
            btb.key = BoostLineupManager.BoostNames[btb.cid.index];
            btb.cid.indexDelegate = OnButtonClick;
            boostToggleBehaviours.Add(btb);
        }

        initialized = true;
    }

    void OnEnable()
    {
        Actualize();
    }

    void OnButtonClick(int index)
    {

        string key = boostToggleBehaviours[index].key;

        if (BikeDataManager.Boosts[key].Number == 0)
        {

            if (boostToggleBehaviours[index].toggle.isOn == true)
            { //don't show popup if deselecting boost
                UIManager.ToggleScreen(GameScreenType.PopupNoBoosts);
            }

            boostToggleBehaviours[index].toggle.isOn = false;
            BikeDataManager.Boosts[key].Selected = false;
            BoostManager.DisableBoostEffect(key);

        }
        else
        {
            if (boostToggleBehaviours[index].toggle.isOn)
            {
                BikeDataManager.Boosts[key].Selected = true;
                BoostManager.EnableBoostEffect(key);
            }
            else
            {
                BikeDataManager.Boosts[key].Selected = false;
                BoostManager.DisableBoostEffect(key);
            }
        }

        //enable disable recommended text
        if (recomendedText != null)
        {
            bool atLeastOneIsSelected = false;

            foreach (BoostToggleBehaviour boostToggleBehaviour in boostToggleBehaviours)
            {
                if (boostToggleBehaviour != null && Startup.Initialized)
                {
                    if (BikeDataManager.Boosts[boostToggleBehaviour.key].Selected)
                    {
                        atLeastOneIsSelected = true;
                        //                        print(boostToggleBehaviour.key + " " + DataManager.Boosts[boostToggleBehaviour.key].Selected);
                    }
                }
            }

            recomendedText.gameObject.SetActive(!atLeastOneIsSelected);
        }

    }

    public List<string> on = new List<string>(new string[] { "", "", "", "" }); //previously known as "active" - it made Unity compiler a very unhappy camper
    public List<string> selected = new List<string>(new string[] { "", "", "", "" });

    void Update()
    {
        if (BoostManager.FarmingUpdate() || true)
        {
            foreach (GameObject bb in boostButtonGOs)
            {
                if (bb != null)
                {
                    bb.transform.Find("Text").GetComponent<Text>().text = "x" + BikeDataManager.Boosts[bb.GetComponent<BoostToggleBehaviour>().key].Number.ToString();
                }
            }
        }

        int i = 0;
        foreach (var item in BikeDataManager.Boosts)
        {
            on[i] = item.Key + " " + item.Value.Active;
            selected[i] = item.Key + " " + item.Value.Selected;
        }
    }


}

}
