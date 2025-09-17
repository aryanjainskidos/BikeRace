namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TierPanelBehaviour : MonoBehaviour
{

    Image coinImage;
    Image timeImage;

    Text timeText;
    Text coinText;

    Color32 passColor;
    Color32 lockColor;

    bool initialized = false;

    void Awake()
    {

        coinImage = transform.Find("CoinImage").GetComponent<Image>();
        timeImage = transform.Find("TimeImage").GetComponent<Image>();

        timeText = transform.Find("TimeText").GetComponent<Text>();
        coinText = transform.Find("CoinText").GetComponent<Text>();

        lockColor = new Color32(255, 0, 0, 255);//75,75,75,255
        passColor = new Color32(75, 75, 75, 255);//102,208,0,255

        initialized = true;
    }

    public void SetData(float time, float timeTier, int coin, int coinTier)
    {

        if (!initialized)
        {
            Awake();
        }

        if (timeTier >= 0)
        {
            timeImage.enabled = true;
            timeText.enabled = true;
            timeText.text = timeTier.ToString("F2");

            if (time <= timeTier)
            { //pass
                timeImage.color = passColor;
                timeText.color = passColor;
            }
            else
            {
                timeImage.color = lockColor;
                timeText.color = lockColor;
            }
        }
        else
        {
            timeImage.enabled = false;
            timeText.enabled = false;
        }

        coinText.text = coinTier.ToString();

        if (coin >= coinTier)
        { //pass
            coinImage.color = passColor;
            coinText.color = passColor;
        }
        else
        {
            coinImage.color = lockColor;
            coinText.color = lockColor;
        }
    }
}

}
