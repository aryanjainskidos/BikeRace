namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MultiplayerUpgradesPanelBehaviour : MonoBehaviour
{

    Dictionary<int, UpgradeEntryBehaviour> map;
    public UIClickIndexDelegate.IndexDelegate upgradeDelegate;

    public UIClickIndexDelegate.IndexDelegate UpgradeDelegate
    {
        get
        {
            return upgradeDelegate;
        }
        set
        {
            upgradeDelegate = value;
            if (map != null)
            {
                foreach (var item in map)
                {
                    item.Value.UpgradeDelegate = upgradeDelegate;
                }
            }
        }
    }

    List<GameObject> upgradeEntries;

    void Awake()
    {

        map = new Dictionary<int, UpgradeEntryBehaviour>();
        map[(int)UpgradeType.Acceleration] = transform.Find("MPUpgradeEntryA").GetComponent<UpgradeEntryBehaviour>();
        map[(int)UpgradeType.AccelerationStart] = transform.Find("MPUpgradeEntrySB").GetComponent<UpgradeEntryBehaviour>();
        map[(int)UpgradeType.AdditionalRestarts] = transform.Find("MPUpgradeEntryR").GetComponent<UpgradeEntryBehaviour>();
        map[(int)UpgradeType.BreakSpeed] = transform.Find("MPUpgradeEntryB").GetComponent<UpgradeEntryBehaviour>();
        map[(int)UpgradeType.MaxSpeed] = transform.Find("MPUpgradeEntryTS").GetComponent<UpgradeEntryBehaviour>();

        foreach (var item in map)
        {
            item.Value.UpgradeID = item.Key;
            item.Value.recordName = BikeDataManager.MultiplayerPlayerBikeRecordName;
            //            item.Value.UpgradeDelegate = upgradeDelegate;//probably not set yet, so no point;
        }
    }

    //    public void Init () {
    //        if(upgradeEntries != null) {
    //            foreach (var item in upgradeEntries)
    //            {
    //                Destroy(item);
    //            }
    //            upgradeEntries.Clear();
    //        }
    //
    //        BoostManager.UpdateTimeSpans();
    //        PopulateUpgradeTable();
    //    }

    //	void Update () {
    //	
    //	}

    //    void PopulateUpgradeTable()
    //    {
    //        GameObject prefab = Resources.Load("Prefabs/UI/NGUIUpgradeEntry") as GameObject;
    //
    //        GameObject tmpUpgradeEntry;
    //        UILabel tmpLabel;
    //
    //        foreach (KeyValuePair<int, UpgradeRecord> record in DataManager.Upgrades) {
    //
    //            if (record.Value.Availability == upgradeType) {
    //                
    //                tmpUpgradeEntry = NGUITools.AddChild(table, prefab);
    //                tmpLabel = tmpUpgradeEntry.transform.FindChild("NameLabel").GetComponent<UILabel>();
    //                tmpLabel.text = record.Value.Name + ":";
    //                tmpLabel = tmpUpgradeEntry.transform.FindChild("LevelLabel").GetComponent<UILabel>();
    //
    //                int upgradeLevel = 0;
    //
    //                if (upgradeType == UpgradeAvailabilityType.SingleplayerOnly || upgradeType == UpgradeAvailabilityType.MultiplayerOnly) {
    //                    
    //                    string bikeRecordName = 
    //                        upgradeType == UpgradeAvailabilityType.SingleplayerOnly ? 
    //                            DataManager.SingleplayerPlayerBikeRecordName : 
    //                            DataManager.MultiplayerPlayerBikeRecordName;
    //
    //                    upgradeLevel = DataManager.Bikes[bikeRecordName].Upgrades[record.Key];
    //                    tmpLabel.text = "LVL-" + upgradeLevel;
    //                }
    //
    //                tmpLabel = tmpUpgradeEntry.transform.FindChild("PriceLabel").GetComponent<UILabel>();
    //                tmpLabel.text =  "Price: " + (record.Value.Prices.Length > upgradeLevel ? ""+ record.Value.Prices[upgradeLevel] : "---");
    //
    //                //for purchasing upgrade levels
    //                UIClickIndexDelegate cid = tmpUpgradeEntry.transform.FindChild("UpgradeButton").GetComponent<UIClickIndexDelegate>();
    //                cid.index = record.Key;
    //                cid.indexDelegate = OnUpgradeClick;
    //
    //                //TODO development only
    //                cid = tmpUpgradeEntry.transform.FindChild("DowngradeButton").GetComponent<UIClickIndexDelegate>();
    //                cid.index = record.Key;
    //                cid.indexDelegate = OnDowngradeClick;
    //
    //                upgradeEntries.Add(tmpUpgradeEntry);
    //            }
    //
    //        }
    //        
    //        table.GetComponent<UITable> ().Reposition ();
    //        table.GetComponent<UITable> ().repositionNow = true;
    //    }

    //    void OnEnable() {
    //
    ////        Actualize();
    //
    //    }

    //    public void Actualize() {
    //        
    ////        if(upgradeEntries != null) {
    ////
    ////            UILabel tmpLabel;
    ////            UIClickIndexDelegate cid;
    ////
    ////            foreach (var tmpUpgradeEntry in upgradeEntries) {
    ////                if (tmpUpgradeEntry != null) {
    ////                    cid = tmpUpgradeEntry.transform.FindChild("UpgradeButton").GetComponent<UIClickIndexDelegate>();
    ////                    tmpLabel = tmpUpgradeEntry.transform.FindChild("LevelLabel").GetComponent<UILabel>();
    ////
    ////                    int upgradeLevel = 0;
    ////                    if (upgradeType == UpgradeAvailabilityType.SingleplayerOnly || upgradeType == UpgradeAvailabilityType.MultiplayerOnly) {
    ////                        
    ////                        string bikeRecordName = 
    ////                            upgradeType == UpgradeAvailabilityType.SingleplayerOnly ? 
    ////                                DataManager.SingleplayerPlayerBikeRecordName : 
    ////                                DataManager.MultiplayerPlayerBikeRecordName;
    ////
    ////                        upgradeLevel = DataManager.Bikes[bikeRecordName].Upgrades[cid.index];
    ////                        tmpLabel.text = "LVL-" + upgradeLevel;
    ////                    }
    ////                
    ////                    tmpLabel = tmpUpgradeEntry.transform.FindChild("PriceLabel").GetComponent<UILabel>();
    ////                    tmpLabel.text =  "Price: " + (DataManager.Upgrades[cid.index].Prices.Length > upgradeLevel ? ""+ DataManager.Upgrades[cid.index].Prices[upgradeLevel] : "---");
    ////
    ////                    UIButton upgradeButton = tmpUpgradeEntry.transform.FindChild("UpgradeButton").GetComponent<UIButton>();
    ////                    if (DataManager.Upgrades[cid.index].Prices.Length <= upgradeLevel) {
    ////                        //TODO disable update button
    ////                        if(upgradeButton.isEnabled)
    ////                            upgradeButton.isEnabled = false;
    ////                    } else {
    ////                        if(!upgradeButton.isEnabled)
    ////                            upgradeButton.isEnabled = true;
    ////                    }
    ////
    ////                }
    ////            }
    ////        }
    //    }

    //    void OnUpgradeClick(int index)
    //    {
    //        print("" + index);
    //        if (index >= 0 && index < DataManager.Upgrades.Count)
    //        {
    //            print("try to upgrade " + DataManager.Upgrades[index].Name);
    //            string bikeRecordName = 
    //                upgradeType == UpgradeAvailabilityType.SingleplayerOnly ? 
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
    //				TelemetryManager.EventUpgrade(index, DataManager.Bikes[bikeRecordName].Upgrades[index]);
    //				AchievementManager.AchievementProgress("garage_upgrade", 1); 
    //
    //            }
    //            
    //        }
    //        
    //        Actualize();
    //    }

    //    void OnDowngradeClick(int index)
    //    {
    //        if (index >= 0 && index < DataManager.Upgrades.Count)
    //        {
    //            print("try to downgrade " + DataManager.Upgrades[index].Name);
    //            string bikeRecordName = 
    //                upgradeType == UpgradeAvailabilityType.SingleplayerOnly ? 
    //                    DataManager.SingleplayerPlayerBikeRecordName : 
    //                    DataManager.MultiplayerPlayerBikeRecordName;
    //            
    //            int upgradeLevel = DataManager.Bikes[bikeRecordName].Upgrades[index];
    //
    //            if (upgradeLevel > 0) { //PurchaseManager.CoinPurchase(DataManager.Upgrades[index].Prices[upgradeLevel])) {
    //                print("downgrade " + DataManager.Upgrades[index].Name);
    //                DataManager.Bikes[bikeRecordName].Upgrades[index]--;
    //                BoostManager.UpdateTimeSpans();
    //            }
    //            
    //        }
    //        
    //        Actualize();
    //    }
}

}
