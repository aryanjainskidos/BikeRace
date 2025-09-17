namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MultiplayerLevelUpBehaviour : MonoBehaviour
{

    Image iconImage;
    Image numberImage;

    Text infoText;
    Text coinText;


    void Awake()
    {

        iconImage = transform.Find("BadgePanel/IconImage").GetComponent<Image>();
        numberImage = transform.Find("BadgePanel/NumberImage").GetComponent<Image>();

        //        infoText = transform.FindChild("InfoPanel/InfoText").GetComponent<Text>();
        coinText = transform.Find("InfoPanel/CoinText").GetComponent<Text>();

    }


    void OnEnable()
    {
        if (Startup.Initialized)
        {
            iconImage.sprite = UIManager.GetLevelBadgeSprite(MultiplayerManager.MPLevel);
            numberImage.sprite = UIManager.GetLevelNumberSprite(MultiplayerManager.MPLevel);

            //			infoText.text = Lang.Get("MP:Levels:CoinsPerWin:");
            //          coinText.text = MultiplayerManager.CoinsPerWin.ToString();
            coinText.text = "+" + MultiplayerManager.CoinsForLevellingUp.ToString();
        }
    }

}

}
