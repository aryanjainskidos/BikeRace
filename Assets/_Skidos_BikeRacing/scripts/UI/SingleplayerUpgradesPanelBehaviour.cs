namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SingleplayerUpgradesPanelBehaviour : MonoBehaviour
{

    Dictionary<int, Transform> map;
    Dictionary<int, UpgradeEntryBehaviour> upgradeMap;
    Dictionary<int, FarmEntryBehaviour> farmMap;

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
            if (upgradeMap != null)
            {
                foreach (var item in upgradeMap)
                {
                    item.Value.UpgradeDelegate = upgradeDelegate;
                }
            }
        }
    }

    List<GameObject> upgradeEntries;

    void Awake()
    {
        //print("SingleplayerUpgradesPanelBehaviour::Awake");

        map = new Dictionary<int, Transform>();
        map[(int)UpgradeType.Magnet] = transform.Find("SPUpgradeEntryM");
        map[(int)UpgradeType.Immortality] = transform.Find("SPUpgradeEntryG");
        map[(int)UpgradeType.Ice] = transform.Find("SPUpgradeEntryI");
        //        map[(int)UpgradeType.Fuel] = transform.FindChild("SPUpgradeEntryF");

        //        upgradeMap[(int)UpgradeType.Magnet] = transform.FindChild("SPUpgradeEntryM").GetComponent<UpgradeEntryBehaviour>();
        //        upgradeMap[(int)UpgradeType.Immortality] = transform.FindChild("SPUpgradeEntryG").GetComponent<UpgradeEntryBehaviour>();
        //        upgradeMap[(int)UpgradeType.Ice] = transform.FindChild("SPUpgradeEntryI").GetComponent<UpgradeEntryBehaviour>();
        //        upgradeMap[(int)UpgradeType.Fuel] = transform.FindChild("SPUpgradeEntryF").GetComponent<UpgradeEntryBehaviour>();

        //        foreach (var item in upgradeMap) {
        //            item.Value.UpgradeID = item.Key;
        //            item.Value.recordName = DataManager.SingleplayerPlayerBikeRecordName;
        //        }

        FarmEntryBehaviour tmpFEB;
        UpgradeEntryBehaviour tmpUEB;

        farmMap = new Dictionary<int, FarmEntryBehaviour>();
        upgradeMap = new Dictionary<int, UpgradeEntryBehaviour>();

        foreach (var item in map)
        {
            item.Value.gameObject.SetActive(true);
            tmpFEB = item.Value.GetComponent<FarmEntryBehaviour>();
            tmpFEB.UpgradeID = item.Key;
            farmMap[item.Key] = tmpFEB;

            tmpUEB = item.Value.GetComponent<UpgradeEntryBehaviour>();
            tmpUEB.UpgradeID = item.Key;
            tmpUEB.recordName = BikeDataManager.SingleplayerPlayerBikeRecordName;
            upgradeMap[item.Key] = tmpUEB;
        }
    }

    //    public void Actualize() {
    //
    //    }

}

}
