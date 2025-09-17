namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

public class GarageBehaviour : MonoBehaviour
{

    bool initialized = false;
    public bool singleplayerMode = true;

    string bikeRecordName;

    BikePanelBehaviour bikePanelBehvaiour;
    UpgradePanelBehaviour upgradePanelBehvaiour;
    CustomizePanelBehaviour customizePanelBehvaiour;
    // Transform saleOff;

    Toggle customizeToggle;
    Toggle upgradeToggle;

    GameObject switchPanel;

    GameObject backgroundSP;
    GameObject backgroundMP;

    RankPanelBehaviour rankPanelBehvaiour;

    StyleBoostPanelBehaviour styleBoostPanelBehvaiour;

    GarageToggleBehaviour upgradeToggleBehaviour;

    // Text saleButtonText;

    void Awake()
    {

        backgroundSP = transform.Find("Background").gameObject;
        backgroundMP = transform.Find("BackgroundMP").gameObject;
        backgroundMP.SetActive(false);

        rankPanelBehvaiour = transform.Find("RankPanel").GetComponent<RankPanelBehaviour>();
        rankPanelBehvaiour.gameObject.SetActive(false);

        //        
        styleBoostPanelBehvaiour = transform.Find("StyleBoostPanel").GetComponent<StyleBoostPanelBehaviour>();
        //        styleBoostPanelBehvaiour.gameObject.SetActive(false);

        bikePanelBehvaiour = transform.Find("BikePanel").GetComponent<BikePanelBehaviour>();
        upgradePanelBehvaiour = transform.Find("UpgradePanel").GetComponent<UpgradePanelBehaviour>();
        customizePanelBehvaiour = transform.Find("CustomizePanel").GetComponent<CustomizePanelBehaviour>();

        switchPanel = transform.Find("SwitchPanel").gameObject;
        customizeToggle = transform.Find("SwitchPanel/CustomizeToggle").GetComponent<Toggle>();
        upgradeToggle = transform.Find("SwitchPanel/UpgradeToggle").GetComponent<Toggle>();
        upgradeToggleBehaviour = upgradeToggle.GetComponent<GarageToggleBehaviour>();

        // saleOff = transform.Find("SaleOffButton"); //button - "SALE 70% OFF !!!!"
        // saleButtonText = saleOff.Find("Text").GetComponent<Text>();
        // saleButtonText.text = Lang.Get("UI:Garage:Sale");

        // if(saleOff != null) {
        // 	saleOff.gameObject.SetActive(false);
        // }

    }
    void OnEnable()
    {
        if (Startup.Initialized && initialized)
        {
            CancelColorConfiguration();

            // if(saleOff != null) {
            // 	saleOff.gameObject.SetActive(PopupPromoBehaviour.ArePromosAvailable()); //show button if promos are available š		
            // }

            //if this is the second visit
            if (BikeDataManager.GarageShowCount == 4)
            {
                PopupPromoBehaviour.SchedulePromo(PromoSubPopups.Sale50, true);
                PopupPromoBehaviour.ShowPromoIfScheduled();
                BikeDataManager.GarageShowCount++;
            }
            else if (BikeDataManager.GarageShowCount < 5)
            {
                BikeDataManager.GarageShowCount++;
            }

            BikeDataManager.ShowGarageButtonNotification = false;
            NewsListManager.EmptyCategory(NewsListItemType.boost);

            if (!upgradePanelBehvaiour.infoPanel.activeSelf && !switchPanel.activeSelf)
            {
                switchPanel.SetActive(true);
            }
        }
    }


    void Start()
    {
        if (Startup.Initialized && !initialized)
        {
            Init();
            OnEnable();
        }
    }

    public void Init()
    {
        upgradePanelBehvaiour.gameObject.SetActive(true);
        customizePanelBehvaiour.gameObject.SetActive(true);

        upgradePanelBehvaiour.Init();
        customizePanelBehvaiour.Init();

        //        upgradePanelBehvaiour.singleplayerToggle.onValueChanged.AddListener(OnModeChange); //toggles are linked in the editor, uncommenting this executes onModeChange twice
        customizePanelBehvaiour.singleplayerToggle.onValueChanged.AddListener(OnModeChange);

        customizeToggle.isOn = true;

        customizeToggle.onValueChanged.AddListener(OnTabChange);

        customizePanelBehvaiour.supb.clearButton.onClick.AddListener(() => CancelColorConfiguration());
        customizePanelBehvaiour.supb.clearUnlockButton.onClick.AddListener(() => CancelColorConfiguration());
        customizePanelBehvaiour.supb.confirmButton.onClick.AddListener(() => SaveColorConfiguration());
        customizePanelBehvaiour.mupb.clearButton.onClick.AddListener(() => CancelColorConfiguration());
        customizePanelBehvaiour.mupb.clearUnlockButton.onClick.AddListener(() => CancelColorConfiguration());
        customizePanelBehvaiour.mupb.confirmButton.onClick.AddListener(() => SaveColorConfiguration());

        foreach (var ppb in customizePanelBehvaiour.supb.map)
        {
            ppb.Value.panelDelegate += bikePanelBehvaiour.ColorPartGroup;
        }

        foreach (var ppb in customizePanelBehvaiour.mupb.map)
        {
            ppb.Value.panelDelegate += bikePanelBehvaiour.ColorPartGroup;
        }

        customizePanelBehvaiour.supb.stylePanelBehaviour.panelDelegate += bikePanelBehvaiour.SetRider;
        customizePanelBehvaiour.mupb.stylePanelBehaviour.panelDelegate += bikePanelBehvaiour.SetRider;
        customizePanelBehvaiour.supb.stylePanelBehaviour.panelDelegate += styleBoostPanelBehvaiour.SetRider;

        upgradePanelBehvaiour.upgradeClickedDelegate = bikePanelBehvaiour.UpgradeBike;
        upgradePanelBehvaiour.upgradeClickedDelegate += rankPanelBehvaiour.UpdateRating;
        upgradePanelBehvaiour.upgradeClickedDelegate += OnBikeUpgrade;

        upgradePanelBehvaiour.upgradeSelectedDelegate = OnBikeUpgradeSelected;
        upgradePanelBehvaiour.upgradeCloseDelegate = OnBikeUpgradeClose;

        bikeRecordName = BikeDataManager.SingleplayerPlayerBikeRecordName;
        bikePanelBehvaiour.bikeRecordName = bikeRecordName;
        bikePanelBehvaiour.Init();

        upgradeToggle.isOn = false;

        initialized = true;
    }


    void Update()
    {
        BoostManager.FarmingUpdate();

        if (!customizeToggle.isOn && customizePanelBehvaiour.gameObject.activeSelf)
        {
            customizePanelBehvaiour.gameObject.SetActive(false);
        }

        if (upgradePanelBehvaiour.infoPanel.activeSelf && switchPanel.activeSelf)
        {
            switchPanel.SetActive(false);
        }

        if (!upgradePanelBehvaiour.infoPanel.activeSelf && !switchPanel.activeSelf)
        {
            switchPanel.SetActive(true);
        }
    }

    void OnTabChange(bool value)
    {
        //        print("OnTabChange " + value + (value ? " customize" : " upgrade"));
        bool sp = false;
        //true - customize is visible
        if (value)
        {
            sp = customizePanelBehvaiour.singleplayerToggle.isOn ? true : false;
        }
        else
        {
            sp = upgradePanelBehvaiour.singleplayerToggle.isOn ? true : false;
        }

        OnModeChange(sp);
    }

    void OnModeChange(bool value)
    {
        //        print("OnModeChange" + value + (value ? " Singleplayer" : " Multiplayer"));

        singleplayerMode = value;

        if (singleplayerMode)
        {
            bikeRecordName = BikeDataManager.SingleplayerPlayerBikeRecordName;
        }
        else
        {
            bikeRecordName = BikeDataManager.MultiplayerPlayerBikeRecordName;
        }

        if (singleplayerMode)
        {
            customizePanelBehvaiour.supb.Load();
            styleBoostPanelBehvaiour.SetRider(BikeDataManager.Bikes[bikeRecordName].StyleID);
        }
        else
        {
            customizePanelBehvaiour.mupb.Load();
        }

        backgroundSP.SetActive(singleplayerMode);
        backgroundMP.SetActive(!singleplayerMode);
        rankPanelBehvaiour.gameObject.SetActive(!singleplayerMode);
        styleBoostPanelBehvaiour.gameObject.SetActive(singleplayerMode);

        upgradeToggleBehaviour.SetLabels(singleplayerMode ? Lang.Get("UI:Garage:Boosts") : Lang.Get("UI:Garage:Upgrades"));

        // saleButtonText.text = 
        // (!singleplayerMode && PopupPromoBehaviour.IsPromoAvailable(PromoSubPopups.SaleUpgrades) ? Lang.Get("UI:Garage:SaleMP") : Lang.Get("UI:Garage:Sale"));
        PopupPromoBehaviour.centerOn = singleplayerMode ? PromoSubPopups.Sale50 : PromoSubPopups.None;

        bikePanelBehvaiour.SwapBikes(bikeRecordName);
        NewsListManager.EmptyCategory(NewsListItemType.boost);
    }

    void SaveColorConfiguration()
    {

        if (singleplayerMode)
        {
            customizePanelBehvaiour.supb.Save();
        }
        else
        {
            customizePanelBehvaiour.mupb.Save();
        }

        AchievementManager.AchievementProgress("garage_color", 1);

        BikeGameManager.reloadBikeAtRestart = true;



        //zinjos serverim par nokraastoto baiku
        string coloring = "";

        int styleID = BikeDataManager.Bikes[BikeDataManager.SingleplayerPlayerBikeRecordName].StyleID;
        string styleName = BikeDataManager.Styles[styleID].Name;
        //print("styleID:" + styleID + " = " + styleName);

        foreach (var s in BikeDataManager.Bikes[BikeDataManager.SingleplayerPlayerBikeRecordName].StyleGroupPresetIDs[styleID])
        {
            int presetID = s.Value;
            string groupName = s.Key;

            string presetName = BikeDataManager.Presets[presetID].Name;

            //print(groupName + " -> " + presetName);
            coloring += groupName + ":" + presetName + ",";
        }

        TelemetryManager.EventStyleChanging(styleName, coloring);

    }

    void CancelColorConfiguration()
    {

        //print ("cancel configuration");

        if (bikeRecordName != null)
        {

            if (singleplayerMode)
            {
                customizePanelBehvaiour.supb.Load();
                styleBoostPanelBehvaiour.SetRider(BikeDataManager.Bikes[bikeRecordName].StyleID);
            }
            else
            {
                customizePanelBehvaiour.mupb.Load();
            }

            bikePanelBehvaiour.SwapBikes(bikeRecordName);
        }

    }

    void OnDisable()
    {
        PopupPromoBehaviour.centerOn = PromoSubPopups.Sale50;
    }

    void OnBikeUpgrade(int index)
    {
        //        throw new NotImplementedException();
        //get executed before (PromoSubPopups.SaleUpgrades) is set to IsPromoAvailable that's why also checking the power rating
        // if (saleButtonText.text == Lang.Get("UI:Garage:Sale")) {
        //     saleButtonText.text = 
        //         (!singleplayerMode && (PopupPromoBehaviour.IsPromoAvailable(PromoSubPopups.SaleUpgrades) || MultiplayerManager.PowerRating >= 60) ? Lang.Get("UI:Garage:SaleMP") : Lang.Get("UI:Garage:Sale"));
        // }
    }

    void OnBikeUpgradeSelected()
    {
        // if (!singleplayerMode && saleOff.gameObject.activeSelf) {
        //     saleOff.gameObject.SetActive(false);
        // }
    }

    void OnBikeUpgradeClose()
    {
        // if (!saleOff.gameObject.activeSelf) {
        //     saleOff.gameObject.SetActive(true);
        // }
    }
}

}
