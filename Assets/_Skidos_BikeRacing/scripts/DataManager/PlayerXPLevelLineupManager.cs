namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Data_MainProject;


/**
 * tikai definé spélétája límeńus
 * spélétája XP un límenis dzívo datu menedźerí
 * límeńi tiek mainíti AchievementManagerí - péc achívmenta izpildes
 */
public class PlayerXPLevelLineupManager : MonoBehaviour
{


    public static OrderedList_BikeRace<int, PlayerXPLevelRecord> DefinePlayerXPLevels()
    {
        OrderedList_BikeRace<int, PlayerXPLevelRecord> PlayerXPLevels = new OrderedList_BikeRace<int, PlayerXPLevelRecord>();

        //skaitam no 0, rádot skaitam no 1
        PlayerXPLevels[0] = new PlayerXPLevelRecord("Level1", 0); //aka Novice
        PlayerXPLevels[1] = new PlayerXPLevelRecord("Level2", 20);
        PlayerXPLevels[2] = new PlayerXPLevelRecord("Level3", 40);
        PlayerXPLevels[3] = new PlayerXPLevelRecord("Level4", 60);
        PlayerXPLevels[4] = new PlayerXPLevelRecord("Level5", 90);
        PlayerXPLevels[5] = new PlayerXPLevelRecord("Level6", 130);
        PlayerXPLevels[6] = new PlayerXPLevelRecord("Level7", 180);
        PlayerXPLevels[7] = new PlayerXPLevelRecord("Level8", 240);
        PlayerXPLevels[8] = new PlayerXPLevelRecord("Level9", 310);
        PlayerXPLevels[9] = new PlayerXPLevelRecord("Level10", 400);


        /* balvas, ko dávina konkrétá límenja sasniegśana
		 * @note -- netiek lietotas, dávinám tikai monétas
        PlayerXPLevels[0].Reward = new RewardRecord(
            100, 
            new Dictionary<string, int>(){
                {"magnet", 1}
            },
            new Dictionary<int, int>(){
                {(int)UpgradeType.Ice, 1}
            },
            new List<int>(){0,3}
        ); */

        PlayerXPLevels[0].Reward = new RewardRecord(1000);//reward for finishing the level
        PlayerXPLevels[1].Reward = new RewardRecord(1500);
        PlayerXPLevels[2].Reward = new RewardRecord(2000);
        PlayerXPLevels[3].Reward = new RewardRecord(2500);
        PlayerXPLevels[4].Reward = new RewardRecord(3000);
        PlayerXPLevels[5].Reward = new RewardRecord(3500);
        PlayerXPLevels[6].Reward = new RewardRecord(4000);
        PlayerXPLevels[7].Reward = new RewardRecord(5000);
        PlayerXPLevels[8].Reward = new RewardRecord(10000);
        PlayerXPLevels[9].Reward = new RewardRecord(10000);



        //izdruká achi sarakstu un kopéjo punktu skaitu - lai zinátu vai XP un límeńi ir adekváti:
        /*
		int sum = 0;
		foreach(KeyValuePair<string, AchievementRecord> achi in DataManager.Achievements){
			sum += achi.Value.RewardPoints;
		}
		print("Mums ir " + DataManager.Achievements.Count + " achiivmenti ar kopeejo punktu summu " + sum);
		//*/


        return PlayerXPLevels;
    }
}



/**
 * spélétája límenis, netiek seivots JSONá
 */
public class PlayerXPLevelRecord
{

    public PlayerXPLevelRecord(string title, int xp, RewardRecord reward = null)
    {
        Title = Lang.Get("PlayerXPLevel:Title:" + title); //iztulkos péc izveidośanas
        XP = xp;
        Reward = reward;
    }

    public string Title; //límeńa nosaukums
    public int XP; //cik XP jásavác, lai iegútu
    public RewardRecord Reward;

}
}
