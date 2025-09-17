namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UpgradeEntryBehaviour : MonoBehaviour
{

    public int upgradeID = -1;
    public string recordName = "";
    public UIClickIndexDelegate.IndexDelegate upgradeDelegate;

    int dispalyedLevel = -1;
    int dispalyedTempLevel = -1;
    List<GameObject> bars;
    List<Color> barColors;

    UIClickIndexDelegate clickIndexDelegate;

    Text nameText;
    Image iconImage;

    bool initialized = false;

    Color32 greenColor = new Color32(0, 255, 3, 255);

    void Awake()
    {

        if (recordName == "")
            recordName = BikeDataManager.MultiplayerPlayerBikeRecordName;

        bars = new List<GameObject>();
        barColors = new List<Color>();
        for (int i = 0; i < 10; i++)
        {
            bars.Add(transform.Find("ProgressPanel/Bar" + i).gameObject);
            barColors.Add(bars[i].GetComponent<Image>().color);
        }

        Transform upgradeButtonT = transform.Find("UpgradeButton");
        clickIndexDelegate = upgradeButtonT.GetComponent<UIClickIndexDelegate>();

        clickIndexDelegate.index = upgradeID;
        clickIndexDelegate.indexDelegate = upgradeDelegate;

        nameText = transform.Find("NameText").GetComponent<Text>();
        iconImage = transform.Find("IconImage").GetComponent<Image>();
        iconImage.preserveAspect = true;
        initialized = true;
    }

    void OnEnable()
    {
        Actualize();
    }

    void Update()
    {
        Actualize();
    }

    void Actualize()
    {

        //        if (DataManager.Bikes.ContainsKey(recordName) && upgradeID >= 0 && DataManager.Bikes[recordName].Upgrades.ContainsKey(upgradeID))
        if (BikeDataManager.Bikes.ContainsKey(recordName) && upgradeID >= 0 && BikeDataManager.Bikes[recordName].UpgradesPerm.ContainsKey(upgradeID))
        {
            nameText.text = BikeDataManager.Upgrades[upgradeID].Name;

            iconImage.sprite = BikeDataManager.Upgrades[upgradeID].Icon;
            iconImage.SetNativeSize();

            //            int upgradeLevel = DataManager.Bikes[recordName].Upgrades[upgradeID];
            int upgradeLevel = BikeDataManager.Bikes[recordName].UpgradesPerm[upgradeID];
            bool permanentUpdated = false;

            if (upgradeLevel != dispalyedLevel)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (bars.Count > i)
                    {

                        if (i < upgradeLevel && !bars[i].activeSelf)
                        {
                            bars[i].SetActive(true);
                            //                            bars[i].GetComponent<Image>().color = barColors[i];
                        }

                        if (i < upgradeLevel)
                        {
                            bars[i].GetComponent<Image>().color = barColors[i];
                        }

                        if (i >= upgradeLevel && bars[i].activeSelf)
                        {
                            bars[i].SetActive(false);
                        }

                    }
                }

                dispalyedLevel = upgradeLevel;
                permanentUpdated = true;
            }

            int tempUpgradeLevel = BikeDataManager.Bikes[recordName].UpgradesTemp[upgradeID];
            if (tempUpgradeLevel != dispalyedTempLevel || permanentUpdated)
            {
                for (int i = upgradeLevel; i < 10; i++)
                {
                    if (bars.Count > i)
                    {

                        if (i < upgradeLevel + tempUpgradeLevel && !bars[i].activeSelf)
                        {
                            bars[i].SetActive(true);
                            bars[i].GetComponent<Image>().color = greenColor;
                        }

                        if (i >= upgradeLevel + tempUpgradeLevel && bars[i].activeSelf)
                        {
                            bars[i].SetActive(false);
                        }

                    }
                }

                dispalyedTempLevel = tempUpgradeLevel;
            }
        }

        //TODO display temporary upgrades

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

            if (clickIndexDelegate != null)
            {
                clickIndexDelegate.index = upgradeID;
            }
            else
            {
                //print("clickIndexDelegate != null");
            }
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

            if (clickIndexDelegate != null)
                clickIndexDelegate.indexDelegate = upgradeDelegate;
            else print("clickIndexDelegate != null");
        }
    }
}

}
