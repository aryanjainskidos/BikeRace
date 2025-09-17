namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Data_MainProject;

public enum BikeStyleType
{
    Main = 0,
    Beach = 1,
    Agent = 2,
    Tourist = 3,
    Hipster = 4,
    Racer = 5,
    Astro = 6,
    Robot = 7,
    Punk = 8,
    Gold = 9
}

/**
 * tikai definé achívmentus
 * pagaidám HelperMenedźeris - galvená datu struktúra "Palettes" ir lietojama caur DataManager 
 */
public class StyleLineupManager : MonoBehaviour
{


    public static OrderedList_BikeRace<int, StyleRecord> DefineStyles()
    {

        OrderedList_BikeRace<int, StyleRecord> Styles = new OrderedList_BikeRace<int, StyleRecord>();
        //if helmetPresets|bodyPresets|bikePresets = {100,101,102,103,104,105} and rimPresets = {0,1,2} then: selectedPresetIndices = {{0-5}, {0-5}, {0-5}, {0-2}}
        //selectedPresetIndices = {helmetPresetIndex, bodyPresetIndex, bikePresetIndex, rimPresetIndex}
        //        Styles[0] = new StyleRecord("Regular", "RegularRider", "RegularRagdoll", "RegularRider3D", new int[]{0,1,2,3,4,5}, new int[]{0,1,2}, new int[]{1,2,3,2}); 
        //@note -- indexiem ir jábút sekvenciáliem un jásákás no 0
        Styles[0] = new StyleRecord("Main", "MainRider", "MainRagdoll", "MainRider3D", "MainHelmet",
                                    0,
                                    new int[] { 100, 101, 102, 103, 104, 105 }, //helmet
                                    new int[] { 110, 111, 112, 113, 114, 115 }, //body
                                    new int[] { 120, 121, 122, 123, 124, 125 }, //bike  
                                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, //rims
                                    new int[] { 4, 0, 4, 2 });//
        Styles[1] = new StyleRecord("Beach", "BeachRider", "BeachRagdoll", "BeachRider3D", "BeachHelmet",
                                    5000,
                                    new int[] { 200, 201, 202, 203, 204, 205 }, //helmet
                                    new int[] { 210, 211, 212, 213, 214, 215 }, //body
                                    new int[] { 220, 221, 222, 223, 224, 225 }, //bike  
                                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, //rims
                                    new int[] { 0, 4, 4, 0 });//
        Styles[2] = new StyleRecord("Agent", "AgentRider", "AgentRagdoll", "AgentRider3D", "BeachHelmet",
                                    100000,
                                    new int[] { 300, 301, 302, 303, 304, 305 }, //helmet
                                    new int[] { 310, 311, 312, 313, 314, 315 }, //body
                                    new int[] { 320, 321, 322, 323, 324, 325 }, //bike  
                                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, //rims
                                    new int[] { 0, 0, 0, 7 });//
        Styles[3] = new StyleRecord("Tourist", "TouristRider", "TouristRagdoll", "TouristRider3D", "TouristHelmet",
                                    5000,
                                    new int[] { 400, 401, 402, 403, 404, 405 }, //helmet
                                    new int[] { 410, 411, 412, 413, 414, 415 }, //body
                                    new int[] { 420, 421, 422, 423, 424, 425 }, //bike  
                                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, //rims
                                    new int[] { 3, 4, 1, 6 });//
        Styles[4] = new StyleRecord("Hipster", "HipsterRider", "HipsterRagdoll", "HipsterRider3D", "HipsterHelmet",
                                    10000,
                                    new int[] { 500, 501, 502, 503, 504, 505 }, //helmet
                                    new int[] { 510, 511, 512, 513, 514, 515 }, //body
                                    new int[] { 520, 521, 522, 523, 524, 525 }, //bike  
                                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, //rims
                                    new int[] { 2, 2, 2, 2 });//
        Styles[5] = new StyleRecord("Racer", "RacerRider", "RacerRagdoll", "RacerRider3D", "RacerHelmet",
                                    15000,
                                    new int[] { 600, 601, 602, 603, 604, 605 }, //helmet
                                    new int[] { 610, 611, 612, 613, 614, 615 }, //body
                                    new int[] { 620, 621, 622, 623, 624, 625 }, //bike  
                                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, //rims
                                    new int[] { 1, 1, 1, 0 });//
        Styles[6] = new StyleRecord("Astro", "AstroRider", "AstroRagdoll", "AstroRider3D", "AstroHelmet",
                                    20000,
                                    new int[] { 700, 701, 702, 703, 704, 705 }, //helmet
                                    new int[] { 710, 711, 712, 713, 714, 715 }, //body
                                    new int[] { 720, 721, 722, 723, 724, 725 }, //bike  
                                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, //rims
                                    new int[] { 2, 2, 3, 2 });//
        Styles[7] = new StyleRecord("Robot", "RobotRider", "RobotRagdoll", "RobotRider3D", "RobotHelmet",
                                    125000,
                                    new int[] { 800, 801, 802, 803, 804, 805 }, //helmet
                                    new int[] { 810, 811, 812, 813, 814, 815 }, //body
                                    new int[] { 820, 821, 822, 823, 824, 825 }, //bike  
                                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, //rims
                                    new int[] { 0, 0, 0, 0 });//
        Styles[8] = new StyleRecord("Punk", "PankRider", "PankRagdoll", "PankRider3D", "PankHelmet",
                                    20000,
                                    new int[] { 900, 901, 902, 903, 904, 905 }, //helmet
                                    new int[] { 910, 911, 912, 913, 914, 915 }, //body
                                    new int[] { 920, 921, 922, 923, 924, 925 }, //bike  
                                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, //rims
                                    new int[] { 0, 0, 0, 0 });//
        Styles[9] = new StyleRecord("Gold", "GoldRider", "GoldRagdoll", "GoldRider3D", "GoldHelmet",
                                    500000,
                                    new int[] { 1000 }, //helmet
                                    new int[] { 1010 }, //body
                                    new int[] { 1020 }, //bike  
                                    new int[] { 7 }, //rims
                                    new int[] { 0, 0, 0, 0 });//

        Styles[0].Locked = false;


        //ikonas ielaadee uzreiz (jo tas atrodas viena spraitshiitaa, kursh jau ir ielaadeets)
        Styles[0].Icon = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/GUI-ico", "StyleIco_Main");
        Styles[1].Icon = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/GUI-ico", "StyleIco_Beach");
        Styles[2].Icon = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/GUI-ico", "StyleIco_Agent");
        Styles[3].Icon = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/GUI-ico", "StyleIco_Tourist");
        Styles[4].Icon = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/GUI-ico", "StyleIco_Hipster");
        Styles[5].Icon = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/GUI-ico", "StyleIco_Racer");
        Styles[6].Icon = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/GUI-ico", "StyleIco_Astro");
        Styles[7].Icon = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/GUI-ico", "StyleIco_Robot");
        Styles[8].Icon = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/GUI-ico", "StyleIco_Pank");
        Styles[9].Icon = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/GUI-ico", "StyleIco_Gold");

        //dekalu 3D baikam ..
        Styles[0].DecalTextureName = "DecalMain";
        Styles[1].DecalTextureName = "DecalBeach";
        Styles[2].DecalTextureName = "DecalAgent";
        Styles[3].DecalTextureName = "DecalTourist";
        Styles[4].DecalTextureName = "DecalHipster";
        Styles[5].DecalTextureName = "DecalRacer";
        Styles[6].DecalTextureName = "DecalAstro";
        Styles[7].DecalTextureName = "DecalRobot";
        Styles[8].DecalTextureName = "DecalPank";
        Styles[9].DecalTextureName = "DecalGold";

        Styles[1].TireDecalTextureName = "Decal_TiresBeach";
        Styles[5].TireDecalTextureName = "Decal_TiresRacer";

        ////dekalu 3D baikam ..
        //Styles[0].DecalTextureName = "visuals/3D_dude/Style_Main/Decal";
        //Styles[1].DecalTextureName = "visuals/3D_dude/Style_Beach/Decal";
        //Styles[2].DecalTextureName = "visuals/3D_dude/Style_Agent/Decal";
        //Styles[3].DecalTextureName = "visuals/3D_dude/Style_Tourist/Decal";
        //Styles[4].DecalTextureName = "visuals/3D_dude/Style_Hipster/Decal";
        //Styles[5].DecalTextureName = "visuals/3D_dude/Style_Racer/Decal";
        //Styles[6].DecalTextureName = "visuals/3D_dude/Style_Astro/Decal";
        //Styles[7].DecalTextureName = "visuals/3D_dude/Style_Robot/Decal";
        //Styles[8].DecalTextureName = "visuals/3D_dude/Style_Pank/Decal";
        //Styles[9].DecalTextureName = "visuals/3D_dude/Style_Gold/Decal";

        //Styles[1].TireDecalTextureName = "visuals/3D_dude/Style_Beach/Decal_Tires";
        //Styles[5].TireDecalTextureName = "visuals/3D_dude/Style_Racer/Decal_Tires";

        Styles[0].NumberDecalTextureName = "visuals/3D_dude/Style_Main/NumberDecal";
        Styles[1].NumberDecalTextureName = "visuals/3D_dude/Style_Main/NumberDecal";
        Styles[2].NumberDecalTextureName = "visuals/3D_dude/Style_Main/NumberDecal";
        Styles[3].NumberDecalTextureName = "visuals/3D_dude/Style_Main/NumberDecal";
        Styles[4].NumberDecalTextureName = "visuals/3D_dude/Style_Main/NumberDecal";
        Styles[5].NumberDecalTextureName = "visuals/3D_dude/Style_Main/NumberDecal";
        Styles[6].NumberDecalTextureName = "visuals/3D_dude/Style_Main/NumberDecal";
        Styles[7].NumberDecalTextureName = "visuals/3D_dude/Style_Main/NumberDecal";
        Styles[8].NumberDecalTextureName = "visuals/3D_dude/Style_Main/NumberDecal";
        Styles[9].NumberDecalTextureName = "visuals/3D_dude/Style_Main/NumberDecal";

        //.. un dekalu 2D baikam ielaadee tikai pirmo reizi to lietojot 
        Styles[0].DecalSpriteSheetName = "Main";
        Styles[1].DecalSpriteSheetName = "Beach";
        Styles[2].DecalSpriteSheetName = "Agent";
        Styles[3].DecalSpriteSheetName = "Tourist";
        Styles[4].DecalSpriteSheetName = "Hipster";
        Styles[5].DecalSpriteSheetName = "Racer";
        Styles[6].DecalSpriteSheetName = "Astro";
        Styles[7].DecalSpriteSheetName = "Robot";
        Styles[8].DecalSpriteSheetName = "Pank";
        Styles[9].DecalSpriteSheetName = "Gold";

        ////.. un dekalu 2D baikam ielaadee tikai pirmo reizi to lietojot 
        //Styles[0].DecalSpriteSheetName = "visuals/Sprites/Player/Main";
        //Styles[1].DecalSpriteSheetName = "visuals/Sprites/Player/Beach";
        //Styles[2].DecalSpriteSheetName = "visuals/Sprites/Player/Agent";
        //Styles[3].DecalSpriteSheetName = "visuals/Sprites/Player/Tourist";
        //Styles[4].DecalSpriteSheetName = "visuals/Sprites/Player/Hipster";
        //Styles[5].DecalSpriteSheetName = "visuals/Sprites/Player/Racer";
        //Styles[6].DecalSpriteSheetName = "visuals/Sprites/Player/Astro";
        //Styles[7].DecalSpriteSheetName = "visuals/Sprites/Player/Robot";
        //Styles[8].DecalSpriteSheetName = "visuals/Sprites/Player/Pank";
        //Styles[9].DecalSpriteSheetName = "visuals/Sprites/Player/Gold";

        Styles[0].DecalSpriteName = "Bike_decal_c2";
        Styles[0].NumberDecalSpriteName = "Bike_Decal_Number";

        Styles[1].DecalSpriteName = "Bike_decal_c2";
        Styles[1].TireDecalSpriteName = "Bike_Decal_Tire";
        Styles[1].TireDecalSpriteSheetName = "Beach";
        Styles[1].NumberDecalSpriteName = "Bike_Decal_Number";

        Styles[2].DecalSpriteName = "Bike_decal_c2";
        Styles[2].NumberDecalSpriteName = "Bike_Decal_Number";

        Styles[3].DecalSpriteName = "Bike_decal_c2";
        Styles[3].NumberDecalSpriteName = "Bike_Decal_Number";

        Styles[4].DecalSpriteName = "Bike_decal_c2";
        Styles[4].NumberDecalSpriteName = "Bike_Decal_Number";

        Styles[5].DecalSpriteName = "Bike_decal_c2";
        Styles[5].TireDecalSpriteName = "Bike_Decal_Tire";
        Styles[5].TireDecalSpriteSheetName = "Racer";
        Styles[5].NumberDecalSpriteName = "Bike_Decal_Number";

        Styles[6].DecalSpriteName = "Bike_decal_c2";
        Styles[6].NumberDecalSpriteName = "Bike_Decal_Number";

        Styles[7].DecalSpriteName = "Bike_decal_c2";
        Styles[7].NumberDecalSpriteName = "Bike_Decal_Number";

        Styles[8].DecalSpriteName = "Bike_decal_c2";
        Styles[8].NumberDecalSpriteName = "Bike_Decal_Number";

        Styles[9].DecalSpriteName = "Bike_decal";
        Styles[9].NumberDecalSpriteName = "Bike_Decal_Number";

        Styles[0].Order = 0;//Main
        Styles[1].Order = 2;//Beach
        Styles[2].Order = 7;//Agent
        Styles[3].Order = 1;//Tourist
        Styles[4].Order = 3;//Hipster
        Styles[5].Order = 4;//Racer
        Styles[6].Order = 6;//Astro
        Styles[7].Order = 8;//Robot
        Styles[8].Order = 5;//Punk
        Styles[9].Order = 9;//Gold

        //"ice","magnet","invincibility","fuel"
        Styles[2].Boosts = new List<string> { "fuel", "magnet" };//Agent
        Styles[7].Boosts = new List<string> { "fuel", "invincibility" };//Robot
        Styles[9].Boosts = new List<string> { "fuel", "invincibility", "magnet", "ice" };//Gold

        return Styles;
    }

}

/**
 * JSONá saglabájama datu struktúra
 */
public class StyleRecord
{

    public StyleRecord(string name, string prefabName, string ragdollPrefabName, string garagePrefabName, string levelsPrefabName, int price, int[] helmetPresets, int[] bodyPresets, int[] bikePresets, int[] rimPresets, int[] selectedPresetIndices = null)
    {
        Name = Lang.Get("Style:Name:" + name);
        PrefabName = prefabName;
        RagdollPrefabName = ragdollPrefabName;
        GaragePrefabName = garagePrefabName;
        LevelsPrefabName = levelsPrefabName;

        GroupPresetIDs = new Dictionary<string, int[]>();
        GroupPresetIDs.Add("helmet", helmetPresets);
        GroupPresetIDs.Add("body", bodyPresets);
        GroupPresetIDs.Add("bike", bikePresets);
        GroupPresetIDs.Add("rims", rimPresets);

        SelectedPresetIndices = selectedPresetIndices;

        Locked = true;
        Price = price;
    }

    public string Name; //for display purposes
    public string PrefabName;
    public string RagdollPrefabName;
    public string GaragePrefabName;
    public string LevelsPrefabName;

    public int Order;

    public bool Locked;
    public int Price;
    public Sprite Icon;

    public string DecalTextureName;
    private Texture _decalTexture;
    public Texture DecalTexture
    { //ielade textuuru tikai peec pieprasiijuma 
        get
        {
            if (_decalTexture == null)
            {
                //_decalTexture = Resources.Load<Texture2D>(DecalTextureName);
                _decalTexture = LoadAddressable_Vasundhara.Instance.GetTexture_Resources(DecalTextureName);
                Debug.Log("<color=yellow>Texture Loaded From = </color>" + _decalTexture);
            }
            return _decalTexture;
        }
    }

    public string TireDecalTextureName;
    private Texture _tireDecalTexture;
    public Texture TireDecalTexture
    { //ielade textuuru tikai peec pieprasiijuma 
        get
        {
            if (_tireDecalTexture == null)
            {
                _tireDecalTexture = LoadAddressable_Vasundhara.Instance.GetTexture_Resources(TireDecalTextureName);
                Debug.Log("<color=yellow>Texture Loaded From = </color>" + _tireDecalTexture);

                //_tireDecalTexture = Resources.Load<Texture2D>(TireDecalTextureName);
            }
            return _tireDecalTexture;
        }
    }

    public string NumberDecalTextureName;
    private Texture _numberDecalTexture;
    public Texture NumberDecalTexture
    { //ielade textuuru tikai peec pieprasiijuma 
        get
        {
            if (_numberDecalTexture == null)
            {
                _numberDecalTexture = LoadAddressable_Vasundhara.Instance.GetTexture_Resources(NumberDecalTextureName);
                Debug.Log("<color=yellow>Texture Loaded From = </color>" + _numberDecalTexture);

                //_numberDecalTexture = Resources.Load<Texture2D>(NumberDecalTextureName);
            }
            return _numberDecalTexture;
        }
    }

    public string DecalSpriteSheetName = "";
    public string DecalSpriteName = "";
    private Sprite _decalSprite;
    public Sprite DecalSprite
    {
        get
        {
            if (_decalSprite == null && DecalSpriteSheetName.Length > 0 && DecalSpriteName.Length > 0)
            {
                _decalSprite = LevelManager.GetSprite(DecalSpriteSheetName, DecalSpriteName);
            }
            return _decalSprite;
        }
    }

    public string TireDecalSpriteName = "";
    public string TireDecalSpriteSheetName = "";
    private Sprite _tireDecalSprite = null;
    public Sprite TireDecalSprite
    {
        get
        {
            if (_tireDecalSprite == null && TireDecalSpriteSheetName.Length > 0 && TireDecalSpriteName.Length > 0)
            {
                _tireDecalSprite = LevelManager.GetSprite(DecalSpriteSheetName, TireDecalSpriteName);
                if (_tireDecalSprite == null)
                {
                    Debug.LogWarning("Failed to load TireDecalSprite: " + TireDecalSpriteName + " from sheet: " + DecalSpriteSheetName + ". Using null sprite.");
                    // Don't set _tireDecalSprite to null again, just log the warning
                }
            }
            return _tireDecalSprite;
        }
    }

    public string NumberDecalSpriteName = "";
    private Sprite _numberDecalSprite = null;
    public Sprite NumberDecalSprite
    {
        get
        {
            if (_numberDecalSprite == null && DecalSpriteSheetName.Length > 0 && NumberDecalSpriteName.Length > 0)
            {
                _numberDecalSprite = LevelManager.GetSprite(DecalSpriteSheetName, NumberDecalSpriteName);
            }
            return _numberDecalSprite;
        }
    }

    public Dictionary<string, int[]> GroupPresetIDs;//for each group an array of whitelisted presets

    public int[] SelectedPresetIndices;

    public List<string> Boosts;

}


}
