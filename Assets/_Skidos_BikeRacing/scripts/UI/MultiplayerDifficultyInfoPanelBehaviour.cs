namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MultiplayerDifficultyInfoPanelBehaviour : MonoBehaviour
{

    Text cupText;
    Text coinText;
    bool initialized = false;


    void Awake()
    {
        cupText = transform.Find("CupText").GetComponent<Text>();
        coinText = transform.Find("CoinText").GetComponent<Text>();
        initialized = true;

        SetCups(0);
        SetCoins(0);
    }

    public void SetCups(int value)
    {
        if (initialized)
        {
            cupText.text = (value > 0 && value != 0) ? "+" + value : value.ToString();
        }
    }

    public void SetCoins(int value)
    {
        if (initialized)
        {
            coinText.text = (value > 0) ? "+" + value : value.ToString();
        }
    }
}

}
