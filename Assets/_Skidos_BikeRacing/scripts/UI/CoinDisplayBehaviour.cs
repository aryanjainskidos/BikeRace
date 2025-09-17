namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CoinDisplayBehaviour : MonoBehaviour
{

    Text coinText;
    public int coins = -1;
    public int coinsTo = -1;

    public bool auto = true;
    public bool tweening = false;
    //TODO allow manual change where it's necessary to animate the coin number

    bool initialized = false;

    // Use this for initialization
    void Awake()
    {
        coinText = transform.Find("Text").GetComponent<Text>();
        initialized = true;
    }

    void OnEnable()
    {
        if (auto)
        {
            InitData(BikeDataManager.Coins);
        }
    }

    public void InitData(int value)
    {
        Debug.Log("<color=blue>TInitData::::---</color>" + value);
        coins = coinsTo = value;
        SetData(coins);
    }

    // Update is called once per frame
    void Update()
    {
        if (auto && coinsTo != BikeDataManager.Coins)
        {
            Debug.Log("<color=blue>COIN VALUE CHANGED: toooo :::---</color>");
            coinsTo = BikeDataManager.Coins;
        }

        if (coinsTo != coins && !tweening)
        {
            //            coinsTo = DataManager.Coins;
            Debug.Log("<color=blue>COIN VALUE CHANGED::::---</color>");
            if (coinsTo > coins)
            { // play sound only for coin increase
                SoundManager.Play("CoinCounting");
            }

            iTween.ValueTo(gameObject, iTween.Hash(
                "from", coins,
                "to", coinsTo,
                "time", 1.7f, // 1.7 sec, the same as sound length
                "onupdatetarget", gameObject,
                "onupdate", "TweenOnUpdateCallBack",
                "oncomplete", "TweenOnCompleteCallBack",
                "easetype", iTween.EaseType.easeOutQuad,
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
        Debug.Log("<color=blue>TweenOnUpdateCallBack::::---</color>" + newValue);
        coins = newValue;
        //        coinText.text = coins.ToString();
        SetData(coins);
    }

    void SetData(int value)
    {
        Debug.Log("<color=blue>SET DATA::::---</color>" + value);
        if (!initialized)
        {
            Awake();
        }
        coinText.text = value.ToString();
    }
}
}
