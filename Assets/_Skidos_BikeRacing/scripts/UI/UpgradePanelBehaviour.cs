namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class UpgradePanelBehaviour : MonoBehaviour
{

    GameObject listPanel;
    public GameObject infoPanel;

    InfoPanelBehaviour infoPanelBehaviour;

    MultiplayerUpgradesPanelBehaviour mupb;
    SingleplayerUpgradesPanelBehaviour supb;

    public Toggle singleplayerToggle;

    GameObject toggleBackground;
    GameObject careerToggle;
    GameObject competitiveToggle;

    public UIClickIndexDelegate.IndexDelegate upgradeClickedDelegate;
    public Action upgradeSelectedDelegate;
    public Action upgradeCloseDelegate;

    GameObject buyFastestButton;

    void Awake()
    {
        //        print("UpgradePanelBehaviour::Awake");

        listPanel = transform.Find("ListPanel").gameObject;
        infoPanel = transform.Find("InfoPanel").gameObject;

        listPanel.SetActive(true); //in case they are disabled in editor
        infoPanel.SetActive(true); //need both visible

        infoPanelBehaviour = infoPanel.GetComponent<InfoPanelBehaviour>();

        mupb = listPanel.transform.Find("MultiplayerPanel").GetComponent<MultiplayerUpgradesPanelBehaviour>();
        supb = listPanel.transform.Find("SingleplayerPanel").GetComponent<SingleplayerUpgradesPanelBehaviour>();

        mupb.gameObject.SetActive(true);
        supb.gameObject.SetActive(true);

        Button closeButton = infoPanel.transform.Find("CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(() => OnInfoClose());

        singleplayerToggle = listPanel.transform.Find("CareerToggle").GetComponent<Toggle>();

        toggleBackground = listPanel.transform.Find("ToggleBackground").gameObject;
        careerToggle = listPanel.transform.Find("CareerToggle").gameObject;
        competitiveToggle = listPanel.transform.Find("CompetitiveToggle").gameObject;

        toggleBackground.SetActive(false);
        //        careerToggle.SetActive(false);
        //        competitiveToggle.SetActive(false);
        careerToggle.GetComponent<Toggle>().interactable = false;
        competitiveToggle.GetComponent<Toggle>().interactable = false;

        Utils.EnableChildrenWithGraphics(careerToggle, false);
        Utils.EnableChildrenWithGraphics(competitiveToggle, false);

        buyFastestButton = transform.Find("BuyFastestButton").gameObject;
    }

    public void Init()
    {
        //        print("UpgradePanelBehaviour::Init");

        mupb.gameObject.SetActive(true);
        supb.gameObject.SetActive(true);

        mupb.UpgradeDelegate = OnUpgradeSelected;
        supb.UpgradeDelegate = OnUpgradeSelected;

        infoPanelBehaviour.UpgradeDelegate = OnUpgradeClick;

        listPanel.transform.Find("CareerToggle").GetComponent<Toggle>().isOn = true;
        listPanel.transform.Find("CompetitiveToggle").GetComponent<Toggle>().isOn = false;

        mupb.gameObject.SetActive(false);

        buyFastestButton.SetActive(false);

        BoostManager.UpdateTimeSpans();
    }

    void OnEnable()
    {
        listPanel.SetActive(true);
        infoPanel.SetActive(false);
        buyFastestButton.SetActive(false);

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

    void OnUpgradeSelected(int upgradeID)
    {
        infoPanelBehaviour.UpgradeID = upgradeID;
        TogglePanels();

        if (upgradeSelectedDelegate != null)
        {
            upgradeSelectedDelegate();
        }
    }

    void OnInfoClose()
    {
        TogglePanels();

        if (upgradeCloseDelegate != null)
        {
            upgradeCloseDelegate();
        }
    }

    void TogglePanels()
    {
        bool listPanelActive = listPanel.activeSelf;

        listPanel.SetActive(!listPanelActive);
        infoPanel.SetActive(listPanelActive);

        if (listPanelActive && listPanel.transform.Find("CompetitiveToggle").GetComponent<Toggle>().isOn)
        {
            buyFastestButton.SetActive(true);
        }
        else
        {
            buyFastestButton.SetActive(false);
        }
    }

    void OnDisable()
    {
        bool listPanelActive = false;

        listPanel.SetActive(!listPanelActive);
        infoPanel.SetActive(listPanelActive);

        if (listPanelActive && listPanel.transform.Find("CompetitiveToggle").GetComponent<Toggle>().isOn)
        {
            buyFastestButton.SetActive(true);
        }
        else
        {
            buyFastestButton.SetActive(false);
        }
    }

    void OnUpgradeClick(int index)
    {
        //print("" + index);
        if (index >= 0 && index < BikeDataManager.Upgrades.Count)
        {
            //print("try to upgrade " + DataManager.Upgrades[index].Name);
            string bikeRecordName =
                BikeDataManager.Upgrades[index].Availability == UpgradeAvailabilityType.SingleplayerOnly ?
                    BikeDataManager.SingleplayerPlayerBikeRecordName :
                    BikeDataManager.MultiplayerPlayerBikeRecordName;

            //            int upgradeLevel = DataManager.Bikes[bikeRecordName].Upgrades[index]; //TODO UpgradesPerm
            int upgradeLevel = BikeDataManager.Bikes[bikeRecordName].UpgradesPerm[index]; //TODO UpgradesPerm

            if (upgradeLevel < 10 && PurchaseManager.CoinPurchase(BikeDataManager.Upgrades[index].Prices[upgradeLevel]))
            {
                //print("upgrade " + DataManager.Upgrades[index].Name);

                //                DataManager.Bikes[bikeRecordName].Upgrades[index]++;//TODO UpgradesPerm++
                BikeDataManager.Bikes[bikeRecordName].UpgradesIncrement(index, 1);//TODO UpgradesPerm++
                BoostManager.UpdateTimeSpans();

                //                TelemetryManager.EventUpgrade(index, DataManager.Bikes[bikeRecordName].Upgrades[index]); //TODO might want to send the perm upgrade value here
                TelemetryManager.EventUpgrade(index, BikeDataManager.Bikes[bikeRecordName].UpgradesPerm[index]); //TODO might want to send the perm upgrade value here
                AchievementManager.AchievementProgress("garage_upgrade", 1);

                MultiplayerManager.RecalculateMyPowerRating();

                if (bikeRecordName == BikeDataManager.MultiplayerPlayerBikeRecordName)
                {
                    //TODO redraw bike if multiplayer upgrade bought
                    if (upgradeClickedDelegate != null)
                    {
                        upgradeClickedDelegate(index);
                    }
                    //show promo
                    print("upgrade " + BikeDataManager.Upgrades[index].Name);
                    if ((BikeDataManager.UpgradeSupersalePromoShowCount == 0 && MultiplayerManager.PowerRating >= 80) ||
                        (BikeDataManager.UpgradeSupersalePromoShowCount == 1 && MultiplayerManager.PowerRating >= 150) ||
                        //                        (DataManager.UpgradeSupersalePromoShowCount == 2 && MultiplayerManager.PowerRating >= 180) || 
                        (BikeDataManager.UpgradeSupersalePromoShowCount == 2 && MultiplayerManager.PowerRating >= 240)
                        )
                    {
                        BikeDataManager.UpgradeSupersalePromoShowCount++;

                        if (BikeDataManager.UpgradeSupersalePromoShowCount == 1)
                        { // first time showing 

                            //                            PopupPromoBehaviour.SchedulePromo(PromoSubPopups.SaleUpgrades, true);
                            //                            PopupPromoBehaviour.SchedulePromo(PromoSubPopups.SaleExpert, true, false);

                        }
                        else
                        {// if not first time then show only if it is available, ignore if it's been bought

                            if (PopupPromoBehaviour.IsPromoAvailable(PromoSubPopups.SaleUpgrades))
                            {
                                //                                PopupPromoBehaviour.SchedulePromo(PromoSubPopups.SaleUpgrades, true);
                            }

                            if (PopupPromoBehaviour.IsPromoAvailable(PromoSubPopups.SaleExpert))
                            {
                                //                                PopupPromoBehaviour.SchedulePromo(PromoSubPopups.SaleExpert, true, false);
                            }

                        }

                        //                        PopupPromoBehaviour.ShowPromoIfScheduled();
                        print("show promo");
                    }
                }
            }

        }

        Actualize();
    }

    void Actualize()
    {
        infoPanelBehaviour.Actualize();
    }

}

}
