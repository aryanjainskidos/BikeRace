namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CustomizePanelBehaviour : MonoBehaviour
{

    //    GameObject listPanel;
    //    GameObject infoPanel;
    //
    //    InfoPanelBehaviour infoPanelBehaviour;

    public PlayerCustomizePanelBehaviour mupb;
    public PlayerCustomizePanelBehaviour supb;


    public Toggle singleplayerToggle;

    GameObject toggleBackground;
    GameObject careerToggle;
    GameObject competitiveToggle;

    void Awake()
    {
        //        print("CustomizePanelBehaviour::Awake");

        mupb = transform.Find("MultiplayerPanel").GetComponent<PlayerCustomizePanelBehaviour>();
        mupb.bikeRecordName = BikeDataManager.MultiplayerPlayerBikeRecordName;
        supb = transform.Find("SingleplayerPanel").GetComponent<PlayerCustomizePanelBehaviour>();
        supb.bikeRecordName = BikeDataManager.SingleplayerPlayerBikeRecordName;
        //        closeButton.onClick.AddListener(()=>OnInfoClose());

        singleplayerToggle = transform.Find("CareerToggle").GetComponent<Toggle>();
        //        singleplayerToggle.onValueChanged.AddListener((bool value) => OnCareerValueChange());

        toggleBackground = transform.Find("ToggleBackground").gameObject;
        careerToggle = transform.Find("CareerToggle").gameObject;
        competitiveToggle = transform.Find("CompetitiveToggle").gameObject;

        toggleBackground.SetActive(false);
        //        careerToggle.SetActive(false);
        //        competitiveToggle.SetActive(false);
        careerToggle.GetComponent<Toggle>().interactable = false;
        competitiveToggle.GetComponent<Toggle>().interactable = false;

        Utils.EnableChildrenWithGraphics(careerToggle, false);
        Utils.EnableChildrenWithGraphics(competitiveToggle, false);
    }

    //    void OnCareerValueChange(bool value) {
    //
    //    }

    public void Init()
    {
        //        print("CustomizePanelBehaviour::Init");

        mupb.gameObject.SetActive(true);
        supb.gameObject.SetActive(true);

        mupb.Init();
        supb.Init();

        //        supb.gameObject.SetActive(false);
        transform.Find("CareerToggle").GetComponent<Toggle>().isOn = true;
        transform.Find("CompetitiveToggle").GetComponent<Toggle>().isOn = false;
        mupb.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        //        if (!singleplayerToggle.isOn && supb.gameObject.activeSelf)
        //        {
        //            supb.gameObject.SetActive(false);
        //        }

        //        if (DataManager.MultiplayerUnlocked) {
        //            toggleBackground.SetActive(true);
        //            careerToggle.SetActive(true);
        //            competitiveToggle.SetActive(true);
        //        } else {
        //            toggleBackground.SetActive(false);
        //            careerToggle.SetActive(false);
        //            competitiveToggle.SetActive(false);
        //        }
    }

    //    void OnUpgradeSelected(int upgradeID) {
    //        infoPanelBehaviour.UpgradeID = upgradeID;
    //        TogglePanels();
    //    }
    //
    //    void OnInfoClose() {
    //        TogglePanels();
    //    }
    //
    //    void TogglePanels() {
    //        bool listPanelActive = listPanel.activeSelf;
    //
    //        listPanel.SetActive(!listPanelActive);
    //        infoPanel.SetActive(listPanelActive);
    //    }

    //    void OnUpgradeClick(int index)
    //    {
    //        print("" + index);
    //        if (index >= 0 && index < DataManager.Upgrades.Count)
    //        {
    //            print("try to upgrade " + DataManager.Upgrades[index].Name);
    //            string bikeRecordName = 
    //                DataManager.Upgrades[index].Availability == UpgradeAvailabilityType.SingleplayerOnly ? 
    //                    DataManager.SingleplayerPlayerBikeRecordName : 
    //                    DataManager.MultiplayerPlayerBikeRecordName;
    //            
    //            int upgradeLevel = DataManager.Bikes[bikeRecordName].Upgrades[index];
    //            
    //            if (upgradeLevel < 10 && PurchaseManager.CoinPurchase(DataManager.Upgrades[index].Prices[upgradeLevel])) {
    //                print("upgrade " + DataManager.Upgrades[index].Name);
    //                
    //                DataManager.Bikes[bikeRecordName].Upgrades[index]++;
    //                BoostManager.UpdateTimeSpans();
    //
    //              TelemetryManager.EventUpgrade(index, DataManager.Bikes[bikeRecordName].Upgrades[index]);
    //              AchievementManager.AchievementProgress("garage_upgrade", 1); 
    //
    //            }
    //            
    //        }
    //        
    //        Actualize();
    //    }

    //    void Actualize() {
    //        infoPanelBehaviour.Actualize();
    //    }

}

}
