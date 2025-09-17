namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RewardRecord
{

    public int Coin; //coin count
    public Dictionary<string, int> Boosts; //boost name, boost count
    public Dictionary<int, int> Upgrades; //upgrade ID, level count
    public List<int> Presets; //preset ID's(Indices in Preset list)
                              //TODO    public List<int> Styles; //preset ID's(Indices in Preset list)
    public int ItemCount;//coins arent counted as an item

    public RewardRecord(int coin = 0, Dictionary<string, int> boosts = null, Dictionary<int, int> upgrades = null, List<int> presets = null)
    {
        Coin = coin;
        Boosts = boosts;
        Upgrades = upgrades;
        Presets = presets;

        if (boosts != null)
            ItemCount += boosts.Count;

        if (upgrades != null)
            ItemCount += upgrades.Count;

        if (presets != null)
            ItemCount += presets.Count;
    }

}


}
