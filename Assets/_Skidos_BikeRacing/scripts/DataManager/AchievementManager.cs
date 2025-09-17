namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

//faux-static klase, kas sańem info par achívmentu progresu
public class AchievementManager : MonoBehaviour
{

    /*
	 * kurá kadrá pédéjoreiz rádíts achi progresa popups
	 * nodrośinás, ka 1 kadrá tikai pirmais progres apopups tiks rádíts 
	 * (piem.: ja ir savác 10 monétas un savác 100 monétas achívmenti, 
	 * tad abi nerádís progresa popupus, tikai pirmais neizpildítais)
	 */
    static int LastAchievementProgressShownInFrame = 0;


    /**
	 * inkrementé padotá achívmenta progresu
	 */
    public static void AchievementProgress(string name, float plusProgress)
    {

        AchievementRecord achievement;
        if (BikeDataManager.Achievements.TryGetValue(name, out achievement))
        {

            if (achievement.Done)
            { //pabeigtus achívmentus vairs neaiztiek
                return;
            }

            if (UIManager.currentScreenType == GameScreenType.MultiplayerGame ||
               UIManager.currentScreenType == GameScreenType.MultiplayerGameReplay ||
               UIManager.currentScreenType == GameScreenType.MultiplayerCrash ||
               UIManager.currentScreenType == GameScreenType.MultiplayerPostGameFriend ||
               UIManager.currentScreenType == GameScreenType.MultiplayerPostGameLeague ||
               UIManager.currentScreenType == GameScreenType.MultiplayerPostGameRevanche ||
               UIManager.currentScreenType == GameScreenType.MultiplayerPostGameReplay ||
               UIManager.currentScreenType == GameScreenType.MultiplayerPause ||
               UIManager.currentScreenType == GameScreenType.MultiplayerPreGame ||
               UIManager.currentScreenType == GameScreenType.MultiplayerPreGamePause
               )
            { //notiek MP brauciens
                if (!achievement.MP)
                { // un achívments nav multipleijera drośs
                    return;
                }
            }

            float progressPrecentageBefore = achievement.Progress / achievement.Target;
            achievement.Progress += plusProgress;

            //print("achi progress: " + achievement.Progress   + "  lielais progress: " + DataManager.Achievements[name].Progress);
            if (achievement.Progress >= achievement.Target)
            {
                achievement.Done = true;
                ShowPopup(achievement, true);//ráda popupu, ka ir izpildíts
                achievement.Progress = achievement.Target;//clip


                NewsListManager.Push(Lang.Get("News:Achievement |param| ready").Replace("|param|", achievement.Name), NewsListItemType.achievement, GameScreenType.Achievements);

                return;
            }


            if (achievement.ProgressDisplay == 2)
            { //svarígie achiivmenti - rádam katru progresu
                ShowPopup(achievement, false);
            }
            else if (achievement.ProgressDisplay == 1)
            { //mazák svarígie achívmenti - rádam júdźakmeńus
                float progressPrecentage = achievement.Progress / achievement.Target;

                if (progressPrecentageBefore < 0.25f && progressPrecentage >= 0.25f)
                { //25%
                    ShowPopup(achievement, false);
                }
                if (progressPrecentageBefore < 0.5f && progressPrecentage >= 0.5f)
                { //pársniegts 50% slieksnis
                    ShowPopup(achievement, false);
                }
                if (progressPrecentageBefore < 0.75f && progressPrecentage >= 0.75f)
                { //75%
                    ShowPopup(achievement, false);
                }
            }
            else
            { //nesvarígie, tiem neráda progresa zińojumus




            }



        }
        else
        {
            Debug.LogError("Neeksisteejosha achiivmenta progress \"" + name + "\" ");
        }


    }

    public static void ClaimReward(string name)
    {
        BikeDataManager.Achievements[name].Claimed = true;
        BikeDataManager.Coins += BikeDataManager.Achievements[name].RewardCoins;
        BikeDataManager.IncrementPlayerXP(BikeDataManager.Achievements[name].RewardPoints);
    }


    //@deprecated -- popupi atcelti
    private static void ShowPopup(AchievementRecord achievement, bool finished)
    {


        if (!finished && LastAchievementProgressShownInFrame == Time.frameCount)
        {
            return; //śajá kadrá jau viens progresa achívments ir bijis, ejam májás
        }
        LastAchievementProgressShownInFrame = Time.frameCount;


    }
}

}
