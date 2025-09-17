namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MultiplayerCustomizePanelBehaviour : MonoBehaviour
{

    //    Dictionary<int, UpgradeEntryBehaviour> map;
    //    public UIClickIndexDelegate.IndexDelegate upgradeDelegate;
    //
    //    public UIClickIndexDelegate.IndexDelegate UpgradeDelegate
    //    {
    //        get
    //        {
    //            return upgradeDelegate;
    //        }
    //        set
    //        {
    //            upgradeDelegate = value;
    //            if (map != null) {
    //                foreach (var item in map) {
    //                    item.Value.UpgradeDelegate = upgradeDelegate;
    //                }
    //            }
    //        }
    //    }
    //
    //    List<GameObject> upgradeEntries;

    void Awake()
    {

        //        map = new Dictionary<int, UpgradeEntryBehaviour>();
        //        map[(int)UpgradeType.Acceleration] = transform.FindChild("MPUpgradeEntryA").GetComponent<UpgradeEntryBehaviour>();
        //        map[(int)UpgradeType.AccelerationStart] = transform.FindChild("MPUpgradeEntrySB").GetComponent<UpgradeEntryBehaviour>();
        //        map[(int)UpgradeType.AdditionalRestarts] = transform.FindChild("MPUpgradeEntryR").GetComponent<UpgradeEntryBehaviour>();
        //        map[(int)UpgradeType.BreakSpeed] = transform.FindChild("MPUpgradeEntryB").GetComponent<UpgradeEntryBehaviour>();
        //        map[(int)UpgradeType.MaxSpeed] = transform.FindChild("MPUpgradeEntryTS").GetComponent<UpgradeEntryBehaviour>();
        //
        //        foreach (var item in map) {
        //            item.Value.UpgradeID = item.Key;
        //            item.Value.recordName = DataManager.MultiplayerPlayerBikeRecordName;
        ////            item.Value.UpgradeDelegate = upgradeDelegate;//probably not set yet, so no point;
        //        }
    }

    public void Init()
    {
        //        throw new System.NotImplementedException();
    }
}
}
