namespace vasundharabikeracing {
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Data_MainProject;
using SimpleJSON;

/**
 * 
 * pagaidám HelperMenedźeris - galvená datu struktúra "Bikes" ir lietojama caur DataManager 
 */
public class BikeLineupManager : MonoBehaviour
{


    static public OrderedList_BikeRace<string, BikeRecord> DefineBikes()
    {
        OrderedList_BikeRace<string, BikeRecord> Bikes = new OrderedList_BikeRace<string, BikeRecord>();


        /**
		 * prefaba nosaukums (ja baiks ir speciáls un lietotájs, to nevar selektét, tad nosaukums var nebút prefaba nosaukums) 
		 * => 
		 * nosaukums spélé
		 * vai ir paredzéts pirkśanai/braukśanai  (false - spec. baiki, ko lietotájs nevar selektét)
		 */
        //        print("UpgradeType.Fuel" + UpgradeType.Fuel);
        //        int en = Convert.ToInt32(UpgradeType.Fuel);
        //        print("en" + en);

        int[] singleplayerUpgrades = {
            (int)UpgradeType.Fuel,
            (int)UpgradeType.Ice,
            (int)UpgradeType.Immortality,
            (int)UpgradeType.Magnet
        };
        int[] multiplayerUpgrades = {
            (int)UpgradeType.Acceleration,
            (int)UpgradeType.AccelerationStart,
            (int)UpgradeType.AdditionalRestarts,
            (int)UpgradeType.BreakSpeed,
            (int)UpgradeType.MaxSpeed
        };

        Bikes["Regular"] = new BikeRecord(singleplayerUpgrades);
        Bikes["MPRegular"] = new BikeRecord(multiplayerUpgrades, "RegularMP");

        Bikes["SPGhost"] = new BikeRecord(singleplayerUpgrades);
        Bikes["MPGhost1"] = new BikeRecord(multiplayerUpgrades, "RegularMP");
        Bikes["MPGhost2"] = new BikeRecord(multiplayerUpgrades, "RegularMP");


        return Bikes;
    }

}

/**
 * JSONá saglabájama datu struktúra
 */
public class BikeRecord
{

    public BikeRecord(int[] upgradeTypes, string prefabName = "Regular")
    {

        PrefabName = prefabName; //TODO will have to use a different prefab for mp

        StyleID = 0;

        //        GroupPresetIDs= new Dictionary<string,int>();
        //        GroupPresetIDs.Add("helmet", 0);
        //        GroupPresetIDs.Add("body", 0);
        //        GroupPresetIDs.Add("bike", 0);
        //        GroupPresetIDs.Add("rims", 0);

        Upgrades = new Dictionary<int, int>();
        UpgradesPerm = new Dictionary<int, int>();
        UpgradesTemp = new Dictionary<int, int>();
        foreach (var item in upgradeTypes)
        {
            Upgrades.Add(item, 0);
            UpgradesTemp.Add(item, 0);
            UpgradesPerm.Add(item, 0);
        }

        StyleGroupPresetIDs = new List<Dictionary<string, int>>();

        Dictionary<string, int> tmpGroupPresetIDs;
        foreach (var styleRecord in BikeDataManager.Styles) //for each style
        {
            tmpGroupPresetIDs = new Dictionary<string, int>(); //add a container to store selected presets

            int groupIndex = 0;
            int presetIndex = 0;
            foreach (var presets in styleRecord.Value.GroupPresetIDs)
            { //set default presets to each group

                presetIndex = (styleRecord.Value.SelectedPresetIndices == null) ? 0 : styleRecord.Value.SelectedPresetIndices[groupIndex++]; //get either the preset index for the group or use the default 0
                tmpGroupPresetIDs.Add(presets.Key, presets.Value[presetIndex]);

            }

            StyleGroupPresetIDs.Add(tmpGroupPresetIDs);
        }
    }

    public List<Dictionary<string, int>> StyleGroupPresetIDs;

    //    SelectedGroupPresetIDs= new Dictionary<string,int>();
    //    SelectedGroupPresetIDs.Add("helmet", defaultPresets[0]);
    //    SelectedGroupPresetIDs.Add("body", defaultPresets[0]);
    //    SelectedGroupPresetIDs.Add("bike", defaultPresets[0]);
    //    SelectedGroupPresetIDs.Add("rims", rimPresets[0]);

    /**
	 * śis tiks aizpildíts krásojot moci
	 * detaĺas_gameobject_nosaukums => krása
	 */
    //    public Dictionary<string,int> GroupPresetIDs; //holds palette id for bike group
    public Dictionary<string, int> GroupPresetIDs
    {
        get
        {
            return StyleGroupPresetIDs[StyleID];
        }
    }

    //holds palette id for bike group

    public Dictionary<int, int> Upgrades; // = UpgradesPerm + UpgradesTemp //TODO make read only 
    public Dictionary<int, int> UpgradesTemp; //use for writing
    public Dictionary<int, int> UpgradesPerm; //use for writing

    public void UpgradesSet(int key, int value)
    {
        UpgradesPerm[key] = value;

        if (BikeDataManager.PowerBoostEnabled)
            BikeDataManager.RecalulateTemporaryPowerBoost();

        //TODO recalculate upgrades
        RecalculateUpgradeValue(key);
    }

    public void UpgradesIncrement(int key, int value)
    {
        UpgradesPerm[key] += value;

        if (BikeDataManager.PowerBoostEnabled)
            BikeDataManager.RecalulateTemporaryPowerBoost();

        //TODO recalculate upgrades
        RecalculateUpgradeValue(key);
    }

    public void UpgradesTempSet(int key, int value)
    {
        UpgradesTemp[key] = value;

        //TODO recalculate upgrades
        RecalculateUpgradeValue(key);
    }

    public void UpgradesTempIncrement(int key, int value)
    {
        UpgradesTemp[key] += value;

        //TODO recalculate upgrades
        RecalculateUpgradeValue(key);
    }

    void RecalculateUpgradeValue(int key)
    {
        Upgrades[key] = Mathf.Clamp(UpgradesPerm[key] + UpgradesTemp[key], 0, 10);
    }

    public string PrefabName;

    public int StyleID;

    public JSONClass InputJSONClass; //player input comes from inputDevice, ghost input comes from file


    public JSONClass MakeJSONClass()
    {

        JSONClass J = new JSONClass();

        J["StyleID"].AsInt = StyleID;

        foreach (KeyValuePair<string, int> gp in GroupPresetIDs)
        {
            J["GroupPresetIDs"][gp.Key].AsInt = gp.Value;
        }

        //        foreach(KeyValuePair<int,int> u in Upgrades){
        foreach (KeyValuePair<int, int> u in UpgradesPerm)
        {
            J["Upgrades"][u.Key.ToString()].AsInt = u.Value;
        }

        J["Input"] = InputJSONClass;

        //TODO rider

        return J;
    }

    public void LoadJSONNode(JSONNode jsonNode)
    {

        try
        {
            if (jsonNode != null)
            {

                StyleID = jsonNode["StyleID"].AsInt;

                foreach (KeyValuePair<string, JSONNode> groupPreset in (JSONClass)jsonNode["GroupPresetIDs"])
                {
                    GroupPresetIDs[groupPreset.Key] = groupPreset.Value.AsInt;
                }

                foreach (KeyValuePair<string, JSONNode> upgrade in (JSONClass)jsonNode["Upgrades"])
                {
                    //                    Upgrades[Int32.Parse(upgrade.Key)] = upgrade.Value.AsInt; //TODO UpgradesPerm
                    UpgradesSet(Int32.Parse(upgrade.Key, System.Globalization.CultureInfo.InvariantCulture), upgrade.Value.AsInt);
                }

                InputJSONClass = (JSONClass)jsonNode["Input"];

                //TODO rider
            }
        }
        catch
        {
            //daźreiz defektíva ghost failińa déĺ nesanák nolasít JSONu, śáds raids tiek ignoréts

            //(btw, śí nav MonoBehaviour klase, tápéc nevar PRINT ):
        }

    }

    public string Serialize()
    {

        return MakeJSONClass().ToString();
    }

}

}
