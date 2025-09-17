namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//for LevelUp screen not popup
public class LevelUpBehaviour : MonoBehaviour
{

    Text coinText;
    Text levelText;

    void Awake()
    {
        coinText = transform.Find("InfoPanel/CoinText").GetComponent<Text>();
        levelText = transform.Find("LevelText").GetComponent<Text>();
    }

    void OnEnable()
    {

        if (Startup.Initialized)
        {
            print("DataManager.PlayerXPLevel" + BikeDataManager.PlayerXPLevel);
            RewardRecord reward = BikeDataManager.PlayerXPLevels[BikeDataManager.PlayerXPLevel].Reward;
            if (reward != null)
            {
                coinText.text = "+" + reward.Coin;
            }

            levelText.text = Lang.Get("LevelUp:Subtitle: LVL |param1| - |param2|") //LVL 3 - STEELBENDER
                .Replace("|param1|", (BikeDataManager.PlayerXPLevel + 1).ToString())  //skaitam no 0, rádam no 1
                .Replace("|param2|", BikeDataManager.PlayerXPLevels[BikeDataManager.PlayerXPLevel].Title);

        }

    }

}
}
