namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameBoostPanelBehaviour : MonoBehaviour
{

    public List<GameObject> boostButtonGOs;
    List<GameBoostToggleBehaviour> boostToggleBehaviours;
    GameObject content;

    void Awake()
    {
        content = transform.Find("ScrollView/Content").gameObject;

        boostButtonGOs = new List<GameObject>();
        boostButtonGOs.Add(transform.Find("ScrollView/Content/FreezeBoostToggle").gameObject);
        boostButtonGOs.Add(transform.Find("ScrollView/Content/MagnetBoostToggle").gameObject);
        boostButtonGOs.Add(transform.Find("ScrollView/Content/ShieldBoostToggle").gameObject);
        boostButtonGOs.Add(transform.Find("ScrollView/Content/FuelBoostToggle").gameObject);

        Init();

    }

    public void Actualize()
    {
        bool suggested;
        bool showPanel = false;
        GameObject boostToggle;
        string key;

        foreach (GameBoostToggleBehaviour boostToggleBehaviour in boostToggleBehaviours)
        {
            if (boostToggleBehaviour != null && Startup.Initialized)
            {

                key = boostToggleBehaviour.key;
                boostToggle = boostToggleBehaviour.gameObject;

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

                List<string> styleBoosts = BikeGameManager.styleBoosts;// = DataManager.Styles[DataManager.Bikes[DataManager.SingleplayerPlayerBikeRecordName].StyleID].Boosts;
                if (styleBoosts != null && styleBoosts.Contains(key))
                {
                    suggested = false;
                }

                if (suggested)
                {
                    boostToggleBehaviour.FadeOut();
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
            }
        }

        if (content != null)
        {
            content.SetActive(showPanel);
        }


    }


    public void Init()
    {
        boostToggleBehaviours = new List<GameBoostToggleBehaviour>();

        foreach (var item in boostButtonGOs)
        {
            GameBoostToggleBehaviour btb = item.GetComponent<GameBoostToggleBehaviour>();
            if (!btb.initialized)
            {
                btb.Awake();
            }
            //            btb.key = BoostLineupManager.BoostNames[btb.cid.index];
            boostToggleBehaviours.Add(btb);
        }

    }

    void OnEnable()
    {
        Actualize();
    }

    //    void Update() {
    //        if(BoostManager.FarmingUpdate() || true) {
    //            foreach (GameObject bb in boostButtonGOs) {
    //                if (bb != null) {                    
    //					bb.transform.FindChild("Text").GetComponent<Text>().text = DataManager.Boosts[bb.GetComponent<BoostToggleBehaviour>().key].Number.ToString();                    
    //                }
    //            }
    //        }
    //    }


}
}
