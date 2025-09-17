namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Data_MainProject;

public class PlayerMultiplayerLevelLineupManager : MonoBehaviour
{

    public static OrderedList_BikeRace<int, PlayerMultiplayerLevelRecord> DefinePlayerMultiplayerLevels()
    {
        OrderedList_BikeRace<int, PlayerMultiplayerLevelRecord> PlayerMultiplayerLevels = new OrderedList_BikeRace<int, PlayerMultiplayerLevelRecord>();

        //skaitam no 0, rádot skaitam no 1
        PlayerMultiplayerLevels[0] = new PlayerMultiplayerLevelRecord("Level1", 0, 250, 5000);
        PlayerMultiplayerLevels[1] = new PlayerMultiplayerLevelRecord("Level2", 300, 400, 7500);
        PlayerMultiplayerLevels[2] = new PlayerMultiplayerLevelRecord("Level3", 600, 500, 10000);
        PlayerMultiplayerLevels[3] = new PlayerMultiplayerLevelRecord("Level4", 900, 600, 15000);
        PlayerMultiplayerLevels[4] = new PlayerMultiplayerLevelRecord("Level5", 1200, 750, 20000, true);
        PlayerMultiplayerLevels[5] = new PlayerMultiplayerLevelRecord("Level6", 1500, 1000, 25000, true);
        PlayerMultiplayerLevels[6] = new PlayerMultiplayerLevelRecord("Level7", 1800, 1500, 35000, true);
        PlayerMultiplayerLevels[7] = new PlayerMultiplayerLevelRecord("Level8", 2100, 1750, 50000, true, true);
        PlayerMultiplayerLevels[8] = new PlayerMultiplayerLevelRecord("Level9", 2400, 2000, 75000, true, true);
        PlayerMultiplayerLevels[9] = new PlayerMultiplayerLevelRecord("Level10", 3000, 2500, 100000, true, true);//5009 will never get awarded, since there's no level 11

        return PlayerMultiplayerLevels;
    }
}



/**
 * spélétája límenis, netiek seivots JSONá
 */
public class PlayerMultiplayerLevelRecord
{

    public PlayerMultiplayerLevelRecord(string title, int cups, int coinsPerWin, int coinsForLevelUp, bool hideEasy = false, bool hideMedium = false)
    {
        Title = title;//Lang.Get("PlayerMultiplayerLevel:Title:" + title); //iztulkos péc izveidośanas
        Cups = cups;
        CoinsPerWin = coinsPerWin;
        CoinsForLevelUp = coinsForLevelUp;
        hideEasy = HideEasy;
        HideMedium = hideMedium;
    }

    public string Title; //límeńa nosaukums
    public int Cups; //cik kausu jásavác, lai iegútu  
    public int CoinsPerWin; //cik naudu dos par jebkuru MP uzvaru
    public int CoinsForLevelUp; //reward for leveling up (for completing this level, NOT for reaching this level)
    public bool HideEasy; //vai śajá límení esośam spélétájam nedod vieglos lígas pretiniekus
    public bool HideMedium;//vai śajá límení esośam spélétájam nedod vidéjos lígas pretiniekus

}

}
