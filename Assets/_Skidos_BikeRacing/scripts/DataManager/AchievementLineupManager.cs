namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using Data_MainProject;


/**
 * tikai definé achívmentus
 * pagaidám HelperMenedźeris - galvená datu struktúra "Achievements" ir lietojama caur DataManager 
 */
public class AchievementLineupManager : MonoBehaviour
{


    public static OrderedList_BikeRace<string, AchievementRecord> DefineAchievements()
    {
        OrderedList_BikeRace<string, AchievementRecord> Achievements = new OrderedList_BikeRace<string, AchievementRecord>();


        /**
		 * liste ar visiem achiivmentiem:
		 * 
		 * 		kods - péc śí meklés spélé un DB (vélams íss un obligáti unikáls)
		 * 		=>
		 * 		cilvécígs nosaukums - śo rádís spélétájiem 		 
		 *		cik gabli śís lietas jásavác
		 *		cik daudz naudas dosim par izpiliśanu 
		 *		cik XP punktus dosim par izpildíśanu
		 *
		 */

        Achievements["jumps"] = new AchievementRecord("Jump |param| times", 15, 100, 5);
        Achievements["jumps__2"] = new AchievementRecord("Jump |param| times", 150, 150, 10);
        Achievements["jumps__3"] = new AchievementRecord("Jump |param| times", 250, 250, 20);

        Achievements["airtime"] = new AchievementRecord("Fly for 1 minute", 60, 100, 5); //mérvieníbas ir sekundes
        Achievements["airtime__2"] = new AchievementRecord("Fly for |param| minutes", 60 * 5, 150, 10);
        Achievements["airtime__3"] = new AchievementRecord("Fly for |param| minutes", 60 * 15, 250, 15);

        Achievements["distance"] = new AchievementRecord("Drive |param| meters", 500, 100, 5); //mérvieníbas ir metri (ístie metri nevis unity vieníbas (1U = 75cm))
        Achievements["distance__2"] = new AchievementRecord("Drive |param| meters", 2500, 150, 10);
        Achievements["distance__3"] = new AchievementRecord("Drive |param| meters", 5000, 250, 15);

        Achievements["wheelie_back"] = new AchievementRecord("Drive |param| meters on back wheel", 50, 100, 5);
        Achievements["wheelie_back__2"] = new AchievementRecord("Drive |param| meters on back wheel", 250, 150, 10);
        Achievements["wheelie_back__3"] = new AchievementRecord("Drive |param| meters on back wheel", 1000, 250, 15);

        Achievements["wheelie_front"] = new AchievementRecord("Drive |param| meters on front wheel", 50, 100, 5);
        Achievements["wheelie_front__2"] = new AchievementRecord("Drive |param| meters on front wheel", 150, 150, 10);
        Achievements["wheelie_front__3"] = new AchievementRecord("Drive |param| meters on front wheel", 250, 250, 15);

        Achievements["backflips"] = new AchievementRecord("Do a backflip", 1, 100, 5);
        Achievements["backflips__2"] = new AchievementRecord("Do |param| backflips", 10, 150, 10);
        Achievements["backflips__3"] = new AchievementRecord("Do |param| backflips", 35, 250, 15);

        Achievements["frontflips"] = new AchievementRecord("Do a frontflip", 1, 100, 5);
        Achievements["frontflips__2"] = new AchievementRecord("Do |param| frontflips", 5, 150, 10);
        Achievements["frontflips__3"] = new AchievementRecord("Do |param| frontflips", 20, 250, 15);

        Achievements["diamond_eggs"] = new AchievementRecord("Collect |param| golden helmets", 3, 100, 10);
        Achievements["diamond_eggs__2"] = new AchievementRecord("Collect |param| golden helmets", 7, 250, 15);
        Achievements["diamond_eggs__3"] = new AchievementRecord("Collect |param| golden helmets", 15, 500, 25);

        Achievements["crash"] = new AchievementRecord("Crash |param| times", 15, 100, 10);
        Achievements["crash__2"] = new AchievementRecord("Crash |param| times", 25, 150, 15);
        Achievements["crash__3"] = new AchievementRecord("Crash |param| times", 50, 250, 25);

        Achievements["drown"] = new AchievementRecord("Drown 1 time", 1, 100, 5);
        Achievements["drown__2"] = new AchievementRecord("Drown |param| times", 5, 150, 10);
        Achievements["drown__3"] = new AchievementRecord("Drown |param| times", 15, 250, 15);

        Achievements["finish"] = new AchievementRecord("Finish |param| times", 30, 100, 10); //vnk pabeidz sacíksti (kaut vai vienu paśu 10 reizes)
        Achievements["finish__2"] = new AchievementRecord("Finish |param| times", 50, 150, 15);
        Achievements["finish__3"] = new AchievementRecord("Finish |param| times", 70, 250, 25);

        Achievements["finish_unq"] = new AchievementRecord("Finish |param| tracks", 10, 100, 5); //izbrauc 10 daźádas trases
        Achievements["finish_unq__2"] = new AchievementRecord("Finished |param| tracks", 25, 150, 10);
        Achievements["finish_unq__3"] = new AchievementRecord("Finished |param| tracks", 85, 500, 25);

        Achievements["finish_long"] = new AchievementRecord("Complete 1 challenge", 1, 100, 5); // izbrauca garo trasi, jebkuru
        Achievements["finish_long__2"] = new AchievementRecord("Complete all challenges", 3, 500, 25); // izbrauca 4 garás trases (pagaidám mums tikai 3 ir)

        Achievements["finish_bonus"] = new AchievementRecord("Finish 1 bonus level", 1, 100, 5); // izbrauca bonustrasi, jebkuru
        Achievements["finish_bonus__2"] = new AchievementRecord("Finish all bonus levels", 5, 500, 25); // izbrauca 5 bonustrases (pagaidám mums tikai 5 ir)

        Achievements["money"] = new AchievementRecord("Earn |param| coins", 5000, 100, 10);//trasé savácamá nauda
        Achievements["money__2"] = new AchievementRecord("Earn |param| coins", 10000, 150, 15);
        Achievements["money__3"] = new AchievementRecord("Earn |param| coins", 20000, 500, 25);

        Achievements["stars"] = new AchievementRecord("Get |param| stars", 50, 100, 10);
        Achievements["stars__2"] = new AchievementRecord("Get |param| stars", 125, 150, 15);
        Achievements["stars__3"] = new AchievementRecord("Get all stars", 255, 500, 25);

        Achievements["wheelie_level"] = new AchievementRecord("Complete any level on back wheel", 1, 100, 5); //mérvieníba ir trases, lai paliek 1 gab, jo netiek chekota unikalitáte - nobrauc jebkuru un ir achívments
        Achievements["wheelie_level__2"] = new AchievementRecord("Complete LVL 1 on back wheel", 1, 150, 10); //1. trase (defineets BikeAchievementGatherer  skriptá, sagúglé péc śí achi nosaukuma)
        Achievements["wheelie_level__3"] = new AchievementRecord("Complete LVL 63 on back wheel", 1, 250, 15);//63. trase

        Achievements["garage_color"] = new AchievementRecord("Change bike color", 1, 100, 5);

        Achievements["garage_upgrade"] = new AchievementRecord("Upgrade bike", 1, 100, 5);

        Achievements["boost_tried"] = new AchievementRecord("Use boost once", 1, 100, 5); //ieslédz bústińu pirmsspéles ekránaa
        Achievements["boost_earned"] = new AchievementRecord("Produce boost", 1, 100, 5);// bústu fermińá izaudzé un novác pirmo

        //	Achievements["mp_game"] = 			new AchievementRecord("Race |param| times in league", 5, 100, 5, true); //sák mp spéli (draugu spéli vai líga spéli)
        //	Achievements["mp_game__2"] = 		new AchievementRecord("Race |param| times in league", 25, 150, 10, true); 
        //	Achievements["mp_game__3"] = 		new AchievementRecord("Race |param| times in league", 100, 500, 25, true);

        return Achievements;
    }

    public static int CountUnclaimedAchievements(OrderedList_BikeRace<string, AchievementRecord> Achievements)
    {
        //unlocked level count = successfully finished level count + 1

        var unclaimedAchievementCount = 0;
        foreach (var achievementKVP in Achievements)
        {
            if (achievementKVP.Value.Progress >= achievementKVP.Value.Target && !achievementKVP.Value.Claimed)
            {
                unclaimedAchievementCount++;
            }
        }

        return unclaimedAchievementCount;
    }

}

/**
 * JSONá saglabájama datu struktúra
 */
public class AchievementRecord
{

    public AchievementRecord(string name, float target, int rewardCoins = 100, int rewardPoints = 10, bool mp = false, int progress = 0)
    {
        Name = Lang.Get("Achievement:Name:" + name).Replace("|param|", target.ToString()); //iztulkos péc izveidośanas
        Target = target;
        ProgressDisplay = progress;
        RewardCoins = rewardCoins;
        RewardPoints = rewardPoints;
        MP = mp;
    }

    public string Name; //for display purposes
    public int ProgressDisplay; //0,1,2 -- cik bieźi rádít progreszińojumus | nerádam - fícha atcelta
    public bool MP; // vai śis achívments tiks vákts arí MP brauciena laiká (default == NÉ)-

    public int RewardCoins;
    public int RewardPoints;

    public bool Done = false; //vai ir nopelníts          
    public bool Claimed = false; //vai ir nopelníts		
    public float Progress;

    public float Percentage
    { //tikai paneĺu kártośanai Achi ekránaá
        get
        {
            if (Claimed)
            {
                return 2000; //kleimotie paśá apakśá (netiks rádíti)
            }
            if (Progress > 0)
            {
                float percentage = (Progress / Target) * 100;
                if (percentage > 100)
                {
                    percentage = 100;
                }
                return 100 - percentage;//iesáktie achi kártosies secígi péc %, izpildítákie augśá

            }
            else
            {
                return 1000; //neiesáktie achi kártosies apakśá, oriǵinálajá sakártojumá
            }
        }
        set
        {
            // #pass
        }
    }

    //cik daudz ir padaríts							| tikai śis parametrs tiek seivots JSONá
    public float Target; // cik kopá ir jápadara, lai skaitítos nopelnít


}


}
