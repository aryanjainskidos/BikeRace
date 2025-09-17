namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerLevelInfoBehaviour : MonoBehaviour
{

    Text titleText;
    Text levelText;
    Text progressText;
    Text coinText;
    Slider progressBar;
    [SerializeField]
    int playerXP;
    [SerializeField]
    int playerXPTo;
    [SerializeField]
    int playerLevel;
    [SerializeField]
    bool tweening = false;


    void Awake()
    {
        titleText = transform.Find("TitleText").GetComponent<Text>();
        levelText = transform.Find("LevelText").GetComponent<Text>();
        progressText = transform.Find("ProgressText").GetComponent<Text>();
        coinText = transform.Find("CoinText").GetComponent<Text>();

        progressBar = transform.Find("Slider").GetComponent<Slider>();
    }

    public void Init()
    {
        playerXP = playerXPTo = BikeDataManager.PlayerXP;
        playerLevel = BikeDataManager.PlayerXPLevel;
        SetData(playerXP, playerLevel);
    }

    //    public void Actualize() {
    //        SetData(DataManager.PlayerXP, DataManager.PlayerXPLevel);
    //    }


    void Update()
    {
        if (playerXP != BikeDataManager.PlayerXP && !tweening)
        {

            playerXPTo = BikeDataManager.PlayerXP;

            iTween.ValueTo(gameObject, iTween.Hash(
                "from", playerXP,
                "to", playerXPTo,
                "time", 0.5f,
                "onupdatetarget", gameObject,
                "onupdate", "TweenOnUpdateCallBack",
                "oncomplete", "TweenOnCompleteCallBack",
                "easetype", iTween.EaseType.linear,
                "ignoretimescale", true
                )
                           );

            tweening = true;
        }
    }

    void TweenOnCompleteCallBack()
    {
        tweening = false;
    }

    void TweenOnUpdateCallBack(int newValue)
    {
        playerXP = newValue;

        if (playerLevel + 1 < BikeDataManager.PlayerXPLevels.Count)
        {//is there a next level
            if (playerXP >= BikeDataManager.PlayerXPLevels[playerLevel + 1].XP)
            {//does the player meet the requirements            
                playerLevel++;//advance level
                              //TODO play some cool animation

                BikeDataManager.Coins += BikeDataManager.PlayerXPLevels[playerLevel].Reward.Coin;

                //after animation popup:
                UIManager.ToggleScreen(GameScreenType.PopupLevelUp, true);
                SoundManager.Play("LevelUp");
            }
        }
        SetData(playerXP, playerLevel);

    }

    void SetData(int xp, int level)
    {
        if (level < BikeDataManager.PlayerXPLevels.Count)
        {
            titleText.text = BikeDataManager.PlayerXPLevels[level].Title;
            levelText.text = Lang.Get("Achievements:(LVL - |param|)").Replace("|param|", (playerLevel + 1).ToString());

            RewardRecord reward = BikeDataManager.PlayerXPLevels[playerLevel].Reward;
            if (reward != null)
            { //if there is next level and it has a reward
                coinText.text = "+" + reward.Coin;
            }
            else
            {
                coinText.text = "";
            }

            if (level + 1 < BikeDataManager.PlayerXPLevels.Count)
            {
                /*
				 * visible XP_NOW/XP_GOAL:
				 * subtract current level XP 
				 * e.g: 0/70 for lvl6 not 200/270
				 */
                int xpNow = xp - BikeDataManager.PlayerXPLevels[level].XP;
                int xpGoal = BikeDataManager.PlayerXPLevels[level + 1].XP - BikeDataManager.PlayerXPLevels[level].XP;
                //
                progressText.text = xpNow + "/" + xpGoal;
                //                progressBar.value = (float)xp / DataManager.PlayerXPLevels[level + 1].XP;
                progressBar.value = (float)xpNow / xpGoal;
                //				//print("set_xp:" + xp + "  total_xp:" +  DataManager.PlayerXPLevels[level + 1].XP);
                //				//print("xpNow:" + xpNow+ "  xpGoal:" + xpGoal);
            }
            else
            {
                progressText.text = Lang.Get("Achievements:MAX");
                progressBar.value = 1;
                coinText.text = "-";

            }
            //print("progressBar" + progressBar.value);

        }

    }
}

}
