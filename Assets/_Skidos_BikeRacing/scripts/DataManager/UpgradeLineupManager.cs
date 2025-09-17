namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Data_MainProject;


/**
 * tikai definé achívmentus
 * pagaidám HelperMenedźeris - galvená datu struktúra "Palettes" ir lietojama caur DataManager 
 */
public class UpgradeLineupManager : MonoBehaviour
{


    public static OrderedList_BikeRace<int, UpgradeRecord> DefineUpgrades()
    {
        OrderedList_BikeRace<int, UpgradeRecord> Upgrades = new OrderedList_BikeRace<int, UpgradeRecord>();

        Upgrades[(int)UpgradeType.Ice] = new UpgradeRecord("Freeze", null, UpgradeAvailabilityType.SingleplayerOnly);
        Upgrades[(int)UpgradeType.Magnet] = new UpgradeRecord("Magnet", null, UpgradeAvailabilityType.SingleplayerOnly);
        Upgrades[(int)UpgradeType.Immortality] = new UpgradeRecord("Guard", null, UpgradeAvailabilityType.SingleplayerOnly);
        Upgrades[(int)UpgradeType.Fuel] = new UpgradeRecord("Fuel", null, UpgradeAvailabilityType.SingleplayerOnly);

        Upgrades[(int)UpgradeType.Acceleration] = new UpgradeRecord("Acceleration", null, UpgradeAvailabilityType.MultiplayerOnly);
        Upgrades[(int)UpgradeType.AccelerationStart] = new UpgradeRecord("Start boost", null, UpgradeAvailabilityType.MultiplayerOnly);
        Upgrades[(int)UpgradeType.MaxSpeed] = new UpgradeRecord("Top Speed", null, UpgradeAvailabilityType.MultiplayerOnly);
        Upgrades[(int)UpgradeType.BreakSpeed] = new UpgradeRecord("Breaks", null, UpgradeAvailabilityType.MultiplayerOnly);
        Upgrades[(int)UpgradeType.AdditionalRestarts] = new UpgradeRecord("Restarts", null, UpgradeAvailabilityType.MultiplayerOnly);

        Upgrades[(int)UpgradeType.Ice].Icon = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/GUI-ico", "BoostIco_Freeze");
        Upgrades[(int)UpgradeType.Magnet].Icon = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/GUI-ico", "BoostIco_Magnet");
        Upgrades[(int)UpgradeType.Immortality].Icon = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/GUI-ico", "BoostIco_Armor");
        Upgrades[(int)UpgradeType.Fuel].Icon = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/GUI-ico", "BoostIco_Fuel");
        Upgrades[(int)UpgradeType.Acceleration].Icon = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/GUI-ico", "ico_Acceleration");
        Upgrades[(int)UpgradeType.AccelerationStart].Icon = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/GUI-ico", "ico_StartBoost");
        Upgrades[(int)UpgradeType.MaxSpeed].Icon = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/GUI-ico", "ico_TopSpeed");
        Upgrades[(int)UpgradeType.BreakSpeed].Icon = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/GUI-ico", "ico_Brakes");
        Upgrades[(int)UpgradeType.AdditionalRestarts].Icon = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/GUI-ico", "ico_Tries");

        UpgradeDataContainer upgradeData = LoadAddressable_Vasundhara.Instance.GetScriptableObjUpgradeData_Resources("UpgradeData");
        if (upgradeData == null)
        {
            Debug.LogError("<color=red>UpgradeData is null! Check if UpgradeData scriptable object is assigned in LoadAddressable_Vasundhara.</color>");
        }
        else
        {
            Debug.Log("<color=yellow>UpgradeData Loaded From = </color>" + upgradeData.name);
        }
        //UpgradeDataContainer upgradeData = Resources.Load<UpgradeDataContainer> ("Data/UpgradeData");        
        
        if (upgradeData != null && upgradeData.upgradeDataEntries != null)
        {
            foreach (var item in upgradeData.upgradeDataEntries)
        {

            int upgradeIndex = (int)item.upgradeType;

            Upgrades[upgradeIndex].Prices = item.prices;
            Upgrades[upgradeIndex].Values = item.values;
            Upgrades[upgradeIndex].FakeValues = item.fakeValues;
            }
        }
        else
        {
            Debug.LogWarning("UpgradeData or upgradeDataEntries is null, using default upgrade values");
        }

        //        Upgrades[(int)UpgradeType.MaxSpeed].Part3DPlaceholderName = "MpBike_Body";
        //        Upgrades[(int)UpgradeType.MaxSpeed].Part3DPrefabNamePerLevel = new [] {
        //            "MpBike_Body_s1","MpBike_Body_s1","MpBike_Body_s1","MpBike_Body_s1", //0-3
        //            "MpBike_Body_s2_p0","MpBike_Body_s2_p0","MpBike_Body_s2_p0",//4-6
        //            "MpBike_Body_s3_p0","MpBike_Body_s3_p0","MpBike_Body_s3_p0",//7-9
        //            "MpBike_Body_s4_p0"
        //        };

        return Upgrades;
    }

    /*
     * -Upgrade0
     * ---Placeholder1
     * -----Level0
     * -------Part0
     * -------.....
     * -------PartN
     * -----LevelX
     * -------Part0
     * -------.....
     * -------PartN
     * ---Placeholder2
     * -----Level0
     * -------Part0
     * -------.....
     * -------PartN
     * -----LevelX
     * -------Part0
     * -------.....
     * -------PartN
            */
    //                      upgr  placehoder         lvl  part
    public static Dictionary<int, Dictionary<string, List<List<string>>>> UpgradeLevelParts3D = new Dictionary<int, Dictionary<string, List<List<string>>>>(){
        {
            (int)UpgradeType.MaxSpeed,
            new Dictionary<string, List<List<string>>>() {
                {
                    "",
                    new List<List<string>>(){
                        new List<string>() {"MpBike_Body_s1_p0"},//0
                        new List<string>() {"MpBike_Body_s1_p0", "MpBike_Body_s1_p1"},//1
                        new List<string>() {"MpBike_Body_s1_p0", "MpBike_Body_s1_p1"},//2
                        new List<string>() {"MpBike_Body_s1_p0", "MpBike_Body_s1_p1", "MpBike_Body_s1_p2"},//3
                        new List<string>() {"MpBike_Body_s2_p0", "MpBike_Body_s2_p0_d1"},//4
                        new List<string>() {"MpBike_Body_s2_p0", "MpBike_Body_s2_p1", "MpBike_Body_s2_p0_d1"},//5
                        new List<string>() {"MpBike_Body_s2_p0", "MpBike_Body_s2_p1", "MpBike_Body_s2_p2", "MpBike_Body_s2_p0_d1"},//6
                        new List<string>() {"MpBike_Body_s3_p0", "MpBike_Body_s3_p0_d1"},//7
                        new List<string>() {"MpBike_Body_s3_p0", "MpBike_Body_s3_p1", "MpBike_Body_s3_p0_d1"},//8
                        new List<string>() {"MpBike_Body_s3_p0", "MpBike_Body_s3_p1", "MpBike_Body_s3_p2", "MpBike_Body_s3_p0_d1"},//9
                        new List<string>() {"MpBike_Body_s3_p0", "MpBike_Body_s3_p1", "MpBike_Body_s3_p2", "MpBike_Body_s3_p0_d2"},//10
                    }
                }
            }
        },
        {
            (int)UpgradeType.Acceleration,
            new Dictionary<string, List<List<string>>>(){
                {
                    "",
                    new List<List<string>>(){
                        new List<string>() {"MpBike_Exhaust_s1_p0"},//0
                        new List<string>() {"MpBike_Exhaust_s1_p0", "MpBike_Exhaust_s1_p1"},//1
                        new List<string>() {"MpBike_Exhaust_s1_p0", "MpBike_Exhaust_s1_p1"},//2
                        new List<string>() {"MpBike_Exhaust_s1_p0", "MpBike_Exhaust_s1_p1"},//3
                        new List<string>() {"MpBike_Exhaust_s2_p0"},//4
                        new List<string>() {"MpBike_Exhaust_s2_p0"},//5
                        new List<string>() {"MpBike_Exhaust_s2_p0", "MpBike_Exhaust_s2_p1"},//6
                        new List<string>() {"MpBike_Exhaust_s3_p0"},//7
                        new List<string>() {"MpBike_Exhaust_s3_p0", "MpBike_Exhaust_s3_p1"},//8
                        new List<string>() {"MpBike_Exhaust_s3_p0", "MpBike_Exhaust_s3_p1", "MpBike_Exhaust_s3_p2"},//9
                        new List<string>() {"MpBike_Exhaust_s3_p0", "MpBike_Exhaust_s3_p1", "MpBike_Exhaust_s3_p2", "MpBike_Exhaust_s3_p1_d1"},//10
                    }
                }
            }
        },
        {
            (int)UpgradeType.AccelerationStart,
            new Dictionary<string, List<List<string>>>(){
                {
                    "Wheel_front",
                    new List<List<string>>(){
                        new List<string>() {"MpBike_Tires_s1"},//0
                        new List<string>() {"MpBike_Tires_s1"},//1
                        new List<string>() {"MpBike_Tires_s1"},//2
                        new List<string>() {"MpBike_Tires_s1"},//3
                        new List<string>() {"MpBike_Tires_s2"},//4
                        new List<string>() {"MpBike_Tires_s2"},//5
                        new List<string>() {"MpBike_Tires_s2"},//6
                        new List<string>() {"MpBike_Tires_s3_f"},//7
                        new List<string>() {"MpBike_Tires_s3_f"},//8
                        new List<string>() {"MpBike_Tires_s3_f"},//9
                        new List<string>() {"MpBike_Tires_s4_f"},//10
                    }
                },
                {
                    "Wheel_back",
                    new List<List<string>>(){
                        new List<string>() {"MpBike_Tires_s1"},//0
                        new List<string>() {"MpBike_Tires_s1"},//1
                        new List<string>() {"MpBike_Tires_s1"},//2
                        new List<string>() {"MpBike_Tires_s1"},//3
                        new List<string>() {"MpBike_Tires_s2"},//4
                        new List<string>() {"MpBike_Tires_s2"},//5
                        new List<string>() {"MpBike_Tires_s2"},//6
                        new List<string>() {"MpBike_Tires_s3_r"},//7
                        new List<string>() {"MpBike_Tires_s3_r"},//8
                        new List<string>() {"MpBike_Tires_s3_r"},//9
                        new List<string>() {"MpBike_Tires_s4_r"},//10
                    }
                }
            }
        },
        {
            (int)UpgradeType.BreakSpeed,
            new Dictionary<string, List<List<string>>>(){
                {
                    "Wheel_front",
                    new List<List<string>>(){
                        new List<string>() {"MpBike_Spokes_s1"},//0
                        new List<string>() {"MpBike_Spokes_s1"},//1
                        new List<string>() {"MpBike_Spokes_s2"},//2
                        new List<string>() {"MpBike_Spokes_s2"},//3
                        new List<string>() {"MpBike_Spokes_s2"},//4
                        new List<string>() {"MpBike_Spokes_s2"},//5
                        new List<string>() {"MpBike_Spokes_s2"},//6
                        new List<string>() {"MpBike_Spokes_s3"},//7
                        new List<string>() {"MpBike_Spokes_s3"},//8
                        new List<string>() {"MpBike_Spokes_s3"},//9
                        new List<string>() {"MpBike_Spokes_s4"},//10
                    }
                },
                {
                    "Wheel_back",
                    new List<List<string>>(){
                        new List<string>() {"MpBike_Spokes_s1"},//0
                        new List<string>() {"MpBike_Spokes_s1"},//1
                        new List<string>() {"MpBike_Spokes_s2"},//2
                        new List<string>() {"MpBike_Spokes_s2"},//3
                        new List<string>() {"MpBike_Spokes_s2"},//4
                        new List<string>() {"MpBike_Spokes_s2"},//5
                        new List<string>() {"MpBike_Spokes_s2"},//6
                        new List<string>() {"MpBike_Spokes_s3"},//7
                        new List<string>() {"MpBike_Spokes_s3"},//8
                        new List<string>() {"MpBike_Spokes_s3"},//9
                        new List<string>() {"MpBike_Spokes_s4"},//10
                    }
                }
            }
        }
    };

    //                      upgr  placehoder         lvl  part
    public static Dictionary<int, Dictionary<string, List<List<string>>>> UpgradeLevelParts2D = new Dictionary<int, Dictionary<string, List<List<string>>>>(){
        {
            (int)UpgradeType.MaxSpeed,
            new Dictionary<string, List<List<string>>>() {
                {
                    "MpBike_parts",
                    new List<List<string>>(){
                        new List<string>() {"MpBike_Body_s1_p0"},//0
                        new List<string>() {"MpBike_Body_s1_p0", "MpBike_Body_s1_p1"},//1
                        new List<string>() {"MpBike_Body_s1_p0", "MpBike_Body_s1_p1"},//2
                        new List<string>() {"MpBike_Body_s1_p0", "MpBike_Body_s1_p1", "MpBike_Body_s1_p2"},//3
                        new List<string>() {"MpBike_Body_s2_p0", "MpBike_Body_s2_p0_d1"},//4
                        new List<string>() {"MpBike_Body_s2_p0", "MpBike_Body_s2_p1", "MpBike_Body_s2_p0_d1"},//5
                        new List<string>() {"MpBike_Body_s2_p0", "MpBike_Body_s2_p1", "MpBike_Body_s2_p2", "MpBike_Body_s2_p0_d1"},//6
                        new List<string>() {"MpBike_Body_s3_p0", "MpBike_Body_s3_p0_d1"},//7
                        new List<string>() {"MpBike_Body_s3_p0", "MpBike_Body_s3_p1", "MpBike_Body_s3_p0_d1"},//8
                        new List<string>() {"MpBike_Body_s3_p0", "MpBike_Body_s3_p1", "MpBike_Body_s3_p2", "MpBike_Body_s3_p0_d1"},//9
                        new List<string>() {"MpBike_Body_s3_p0", "MpBike_Body_s3_p1", "MpBike_Body_s3_p2", "MpBike_Body_s3_p0_d2"},//10
                    }
                }
            }
        },
        {
            (int)UpgradeType.Acceleration,
            new Dictionary<string, List<List<string>>>(){
                {
                    "MpBike_parts",
                    new List<List<string>>(){
                        new List<string>() {"MpBike_Exhaust_s1_p0"},//0
                        new List<string>() {"MpBike_Exhaust_s1_p0", "MpBike_Exhaust_s1_p1"},//1
                        new List<string>() {"MpBike_Exhaust_s1_p0", "MpBike_Exhaust_s1_p1"},//2
                        new List<string>() {"MpBike_Exhaust_s1_p0", "MpBike_Exhaust_s1_p1"},//3
                        new List<string>() {"MpBike_Exhaust_s2_p0"},//4
                        new List<string>() {"MpBike_Exhaust_s2_p0"},//5
                        new List<string>() {"MpBike_Exhaust_s2_p0", "MpBike_Exhaust_s2_p1"},//6
                        new List<string>() {"MpBike_Exhaust_s3_p0"},//7
                        new List<string>() {"MpBike_Exhaust_s3_p0", "MpBike_Exhaust_s3_p1"},//8
                        new List<string>() {"MpBike_Exhaust_s3_p0", "MpBike_Exhaust_s3_p1", "MpBike_Exhaust_s3_p2"},//9
                        new List<string>() {"MpBike_Exhaust_s3_p0", "MpBike_Exhaust_s3_p1", "MpBike_Exhaust_s3_p2", "MpBike_Exhaust_s3_p1_d1"},//10
                    }
                }
            }
        },
        {
            (int)UpgradeType.AccelerationStart,
            new Dictionary<string, List<List<string>>>(){
                {
                    "wheel_front/Tire_front",
                    new List<List<string>>(){
                        new List<string>() {"MpBike_Tires_s1"},//0
                        new List<string>() {"MpBike_Tires_s1"},//1
                        new List<string>() {"MpBike_Tires_s1"},//2
                        new List<string>() {"MpBike_Tires_s1"},//3
                        new List<string>() {"MpBike_Tires_s2"},//4
                        new List<string>() {"MpBike_Tires_s2"},//5
                        new List<string>() {"MpBike_Tires_s2"},//6
                        new List<string>() {"MpBike_Tires_s3_f"},//7
                        new List<string>() {"MpBike_Tires_s3_f"},//8
                        new List<string>() {"MpBike_Tires_s3_f"},//9
                        new List<string>() {"MpBike_Tires_s4_f"},//10
                    }
                },
                {
                    "wheel_back/Tire_back",
                    new List<List<string>>(){
                        new List<string>() {"MpBike_Tires_s1"},//0
                        new List<string>() {"MpBike_Tires_s1"},//1
                        new List<string>() {"MpBike_Tires_s1"},//2
                        new List<string>() {"MpBike_Tires_s1"},//3
                        new List<string>() {"MpBike_Tires_s2"},//4
                        new List<string>() {"MpBike_Tires_s2"},//5
                        new List<string>() {"MpBike_Tires_s2"},//6
                        new List<string>() {"MpBike_Tires_s3_r"},//7
                        new List<string>() {"MpBike_Tires_s3_r"},//8
                        new List<string>() {"MpBike_Tires_s3_r"},//9
                        new List<string>() {"MpBike_Tires_s4_r"},//10
                    }
                }
            }
        },
        {
            (int)UpgradeType.BreakSpeed,
            new Dictionary<string, List<List<string>>>(){
                {
                    "wheel_front",
                    new List<List<string>>(){
                        new List<string>() {"MpBike_Spokes_s1"},//0
                        new List<string>() {"MpBike_Spokes_s1"},//1
                        new List<string>() {"MpBike_Spokes_s2"},//2
                        new List<string>() {"MpBike_Spokes_s2"},//3
                        new List<string>() {"MpBike_Spokes_s2"},//4
                        new List<string>() {"MpBike_Spokes_s2"},//5
                        new List<string>() {"MpBike_Spokes_s2"},//6
                        new List<string>() {"MpBike_Spokes_s3"},//7
                        new List<string>() {"MpBike_Spokes_s3"},//8
                        new List<string>() {"MpBike_Spokes_s3"},//9
                        new List<string>() {"MpBike_Spokes_s4"},//10
                    }
                },
                {
                    "wheel_back",
                    new List<List<string>>(){
                        new List<string>() {"MpBike_Spokes_s1"},//0
                        new List<string>() {"MpBike_Spokes_s1"},//1
                        new List<string>() {"MpBike_Spokes_s2"},//2
                        new List<string>() {"MpBike_Spokes_s2"},//3
                        new List<string>() {"MpBike_Spokes_s2"},//4
                        new List<string>() {"MpBike_Spokes_s2"},//5
                        new List<string>() {"MpBike_Spokes_s2"},//6
                        new List<string>() {"MpBike_Spokes_s3"},//7
                        new List<string>() {"MpBike_Spokes_s3"},//8
                        new List<string>() {"MpBike_Spokes_s3"},//9
                        new List<string>() {"MpBike_Spokes_s4"},//10
                    }
                }
            }
        }
    };

    //    public static Dictionary<int, List<string>> UpgradePart3DPlaceholders = new Dictionary<int, List<string>>() {
    //        { (int)UpgradeType.MaxSpeed, new List<string>() {""} },//MpBike_Body
    //        { (int)UpgradeType.Acceleration, new List<string>() {""} },
    //        { (int)UpgradeType.AccelerationStart, new List<string>() {"Wheel_front", "Wheel_back"} },
    //    };

}

//for convenience 
//@note -- same numbers are being used in MP server - for prizes: free upgrades
public enum UpgradeType
{
    Ice = 0,
    Magnet = 1,
    Immortality = 2,
    Fuel = 3,
    Acceleration = 4,      //4  -  Essential MP Upgrade™
    AccelerationStart = 5, //5  -  Essential MP Upgrade™
    MaxSpeed = 6,          //6  -  Essential MP Upgrade™ 
    BreakSpeed = 7,        //7  -  Essential MP Upgrade™ //TODO typo spotted 
    AdditionalRestarts = 8,//8  -  Essential MP Upgrade™
}

public enum UpgradeAvailabilityType
{
    None,
    SingleplayerOnly,
    MultiplayerOnly,
    All,
}


public class UpgradeRecord
{

    public UpgradeRecord(string name, float[] values, UpgradeAvailabilityType availability = UpgradeAvailabilityType.None)
    {
        Name = Lang.Get("Upgrade:Name:" + name);
        Values = values;
        FakeValues = null;
        Availability = availability;
    }

    public string Name; //for display purposes
    public float[] Values;//are defined in UpgradeData.asset object
    public float[] FakeValues;//are defined in UpgradeData.asset object
    public int[] Prices; //alaso are defined in UpgradeData.asset object
    public UpgradeAvailabilityType Availability;
    public Sprite Icon;
    //    public string Part3DPlaceholderName;
    //    public string[] Part3DPrefabNamePerLevel;
}


}
