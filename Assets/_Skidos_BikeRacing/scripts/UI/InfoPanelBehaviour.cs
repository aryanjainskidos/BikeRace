namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InfoPanelBehaviour : MonoBehaviour
{

    public int upgradeID = -1;
    public UIClickIndexDelegate.IndexDelegate upgradeDelegate;

    UpgradeEntryBehaviour upgradeEntityBehaviour;

    public List<Text> tableLevelTexts;
    public List<Text> tableValueTexts;

    public List<Text> graphValueTexts;

    Text tableValueNameText;
    Text tableValueUnitsText;

    Text graphValueNameText;
    Text graphValueUnitsText;

    UILineRenderer lineRenderer;
    RectTransform lineRendererRectTransform;

    RectTransform pointerRectTransform;

    Text coinText;
    Text percentageText;

    GameObject upgradeButton;
    GameObject upgradePointer;
    //Text upgradeText;

    bool initialized = false;
    // Use this for initialization
    void Awake()
    {
        upgradeEntityBehaviour = transform.Find("UpgradeEntity").GetComponent<UpgradeEntryBehaviour>();

        tableValueTexts = new List<Text>();
        tableLevelTexts = new List<Text>();
        graphValueTexts = new List<Text>();

        tableValueNameText = transform.Find("Table/ValueNameText").GetComponent<Text>();
        tableValueUnitsText = transform.Find("Table/ValueUnitsText").GetComponent<Text>();

        graphValueNameText = transform.Find("GraphPanel/ValueNameText").GetComponent<Text>();
        graphValueUnitsText = transform.Find("GraphPanel/ValueUnitsText").GetComponent<Text>();

        Text tmpText;
        for (int i = 1; i <= 10; i++)
        {
            tmpText = transform.Find("Table/ValueNum" + i + "Text").GetComponent<Text>();
            if (tmpText != null)
            {
                tableValueTexts.Add(tmpText);
            }
            tmpText = transform.Find("Table/LevelNum" + i + "Text").GetComponent<Text>();
            if (tmpText != null)
            {
                tableLevelTexts.Add(tmpText);
            }
        }

        for (int i = 0; i <= 10; i++)
        {
            tmpText = transform.Find("GraphPanel/ValueNum" + i + "Text").GetComponent<Text>();
            if (tmpText != null)
            {
                graphValueTexts.Add(tmpText);
            }
        }

        lineRenderer = transform.Find("GraphPanel/GraphLine").GetComponent<UILineRenderer>();
        lineRendererRectTransform = transform.Find("GraphPanel/GraphLine").GetComponent<RectTransform>();

        pointerRectTransform = transform.Find("GraphPanel/PointerPanel/PointerImage").GetComponent<RectTransform>();

        upgradeButton = transform.Find("UpgradeEntity/UpgradeButton").gameObject;
        upgradePointer = transform.Find("UpgradeEntity/UpgradeButton/Pointer").gameObject;
        upgradeButton.GetComponent<Button>().onClick.AddListener(OnUpgradeClick);
        //upgradeText = transform.FindChild("UpgradeEntity/NameText").GetComponent<Text>();
        coinText = transform.Find("UpgradeEntity/UpgradeButton/CoinText").GetComponent<Text>();
        percentageText = transform.Find("UpgradeEntity/UpgradeButton/PercentageText").GetComponent<Text>();

        initialized = true;
    }

    void OnEnable()
    {
        Actualize();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnUpgradeClick()
    {
        if (upgradePointer.activeSelf)
        {
            upgradePointer.SetActive(false);
        }
    }

    public void Actualize()
    {
        //print("InfoPanelBehaviour::Actualize");

        if (upgradeID >= 0 && BikeDataManager.Upgrades.ContainsKey(upgradeID))
        {
            UpgradeRecord ur = BikeDataManager.Upgrades[upgradeID];
            float delta = (ur.FakeValues.Length == 0) ?
                ur.Values[ur.Values.Length - 1] - ur.Values[0] :
                    ur.FakeValues[ur.FakeValues.Length - 1] - ur.FakeValues[0];

            int upgradeLevel;
            string recordName;

            if (ur.Availability == UpgradeAvailabilityType.MultiplayerOnly)
            {

                recordName = BikeDataManager.MultiplayerPlayerBikeRecordName;

                if (upgradeID == (int)UpgradeType.AdditionalRestarts)
                {
                    tableValueNameText.text = Lang.Get("Garage:Infopanel:Count");
                    graphValueNameText.text = Lang.Get("Garage:Infopanel:Count");
                }
                else
                {
                    tableValueNameText.text = Lang.Get("Garage:Infopanel:Percent");
                    graphValueNameText.text = Lang.Get("Garage:Infopanel:Percent");
                }
                tableValueUnitsText.text = "";
                graphValueUnitsText.text = "";
            }
            else
            {

                recordName = BikeDataManager.SingleplayerPlayerBikeRecordName;

                tableValueNameText.text = Lang.Get("Garage:Infopanel:Time");
                graphValueNameText.text = Lang.Get("Garage:Infopanel:Time");
                tableValueUnitsText.text = Lang.Get("Garage:Infopanel:(min)");
                graphValueUnitsText.text = Lang.Get("Garage:Infopanel:(min)");
            }

            upgradeEntityBehaviour.recordName = recordName;

            //            upgradeLevel = DataManager.Bikes[recordName].Upgrades[upgradeID];//TODO use perm here
            upgradeLevel = BikeDataManager.Bikes[recordName].UpgradesPerm[upgradeID];//TODO use perm here

            if (upgradeLevel < ur.Prices.Length)
            {

                if (!upgradeButton.activeSelf)
                    upgradeButton.SetActive(true);

                ShowUpgradePointer();

                //                print("ur.FakeValues == null " + (ur.FakeValues == null) +" "+ (ur.FakeValues != null ? ur.FakeValues.Length.ToString() : ""));

                coinText.text = ur.Prices[upgradeLevel].ToString();
                float valueCurrent = (ur.FakeValues.Length == 0) ?
                    ur.Values[upgradeLevel] : ur.FakeValues[upgradeLevel];
                float valueNext = (ur.FakeValues.Length == 0) ?
                    ur.Values[upgradeLevel + 1] : ur.FakeValues[upgradeLevel + 1];

                if (ur.Availability == UpgradeAvailabilityType.MultiplayerOnly)
                {
                    percentageText.text = "+" + ((1 - valueCurrent / valueNext) * 100).ToString("F0") + "%";
                }
                else
                {
                    percentageText.text = "+" + ((1 - valueNext / valueCurrent) * 100).ToString("F0") + "%";
                }

            }
            else
            {
                upgradeButton.SetActive(false);
            }

            for (int i = 0; i < ur.Values.Length; i++)
            {
                //                if (ur.Availability == UpgradeAvailabilityType.MultiplayerOnly) {

                float normLevel = (i / 10.0f);
                float normValue = (ur.FakeValues.Length == 0) ?
                    (ur.Values[i] - ur.Values[0]) / delta :
                        (ur.FakeValues[i] - ur.FakeValues[0]) / delta;

                if (i > 0)
                {
                    if (ur.Availability == UpgradeAvailabilityType.SingleplayerOnly ||
                        upgradeID == (int)UpgradeType.AdditionalRestarts)
                    {
                        tableValueTexts[i - 1].text = (ur.FakeValues.Length == 0) ?
                            ur.Values[i].ToString("F0") :
                                ur.FakeValues[i].ToString("F0");
                    }
                    else
                    {
                        //                        print("ur.FakeValues == null " + (ur.FakeValues == null) +" "+ (ur.FakeValues != null ? ur.FakeValues.Length.ToString() : ""));
                        tableValueTexts[i - 1].text = (ur.FakeValues.Length == 0) ?
                            (ur.Values[i] / ur.Values[0] * 100).ToString("F0") + "%" :
                                (ur.FakeValues[i] / ur.FakeValues[0] * 100).ToString("F0") + "%";
                    }

                    if (upgradeLevel == i)
                    {
                        tableValueTexts[i - 1].color = new Color32(0, 224, 2, 255);
                        tableLevelTexts[i - 1].color = new Color32(0, 224, 2, 255);
                    }
                    else
                    {
                        tableValueTexts[i - 1].color = new Color32(127, 127, 127, 255);
                        tableLevelTexts[i - 1].color = new Color32(127, 127, 127, 255);
                    }
                }


                if (ur.Availability == UpgradeAvailabilityType.SingleplayerOnly ||
                    upgradeID == (int)UpgradeType.AdditionalRestarts)
                {
                    graphValueTexts[i].text = (ur.FakeValues.Length == 0) ?
                        ur.Values[i].ToString("F0") :
                            ur.FakeValues[i].ToString("F0");
                }
                else
                {
                    //                    print("ur.FakeValues == null " + (ur.FakeValues == null) +" "+ (ur.FakeValues != null ? ur.FakeValues.Length.ToString() : ""));
                    graphValueTexts[i].text = (ur.FakeValues.Length == 0) ?
                        (ur.Values[i] / ur.Values[0] * 100).ToString("F0") + "%" :
                            (ur.FakeValues[i] / ur.FakeValues[0] * 100).ToString("F0") + "%";
                }

                if (upgradeLevel == i)
                {
                    pointerRectTransform.anchoredPosition = new Vector2(
                        normLevel * lineRendererRectTransform.rect.width,
                        normValue * lineRendererRectTransform.rect.height);

                }

                // lineRenderer.Points[i].x = normLevel;
                // lineRenderer.Points[i].y = normValue;

                //                } //TODO: SP Upgrades


            }
        }
    }

    void ShowUpgradePointer()
    {
        if (!BikeDataManager.FirstProduce && BikeDataManager.FirstUpgrade)
        {
            upgradePointer.SetActive(true);
            BikeDataManager.FirstUpgrade = false;
        }
        else
        {
            upgradePointer.SetActive(false);
        }
    }

    public int UpgradeID
    {
        get
        {
            return upgradeID;
        }
        set
        {
            upgradeID = value;

            if (upgradeEntityBehaviour != null)
            {
                upgradeEntityBehaviour.UpgradeID = upgradeID;
            }
            else print("InfoPanelBehaviour::upgradeEntityBehaviour == null");
        }
    }

    public UIClickIndexDelegate.IndexDelegate UpgradeDelegate
    {
        get
        {
            return upgradeDelegate;
        }
        set
        {
            upgradeDelegate = value;

            if (!initialized)
            {
                Awake();
            }

            if (upgradeEntityBehaviour != null)
            {
                upgradeEntityBehaviour.UpgradeDelegate = upgradeDelegate;
            }
            else print("InfoPanelBehaviour::upgradeEntityBehaviour == null");
        }
    }
}

}
