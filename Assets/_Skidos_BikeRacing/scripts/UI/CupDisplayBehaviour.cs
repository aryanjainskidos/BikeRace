namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CupDisplayBehaviour : MonoBehaviour
{

    Text cupText;
    public int cups = -1;
    public int cupsTo = -1;

    public bool auto = true;
    bool tweening = false;

    Image levelBackgroundImage;
    Image levelNumberImage;


    void Awake()
    {
        cupText = transform.Find("Text").GetComponent<Text>();

        //levelBackgroundImage = transform.FindChild("LevelBackgroundImage").GetComponent<Image>();
        levelBackgroundImage = transform.Find("Button/LevelShieldImage").GetComponent<Image>();
        levelNumberImage = transform.Find("Button/LevelNumberImage").GetComponent<Image>();
    }

    void OnEnable()
    {
        if (auto)
        {
            InitData(MultiplayerManager.Cups);
        }
        //print("MultiplayerManager.Cups=" + MultiplayerManager.Cups);
    }

    public void InitData(int value)
    {
        cups = cupsTo = value;
        SetData(cups);
    }


    void Update()
    {
        if (auto && cupsTo != MultiplayerManager.Cups)
        {
            cupsTo = MultiplayerManager.Cups;
        }

        if (cupsTo != cups && !tweening)
        {

            //            iTween.ValueTo(gameObject, iTween.Hash(
            //                "from", cups,
            //                "to", cupsTo,
            //                "time", 1f,
            //                "onupdatetarget", gameObject,
            //                "onupdate", "TweenOnUpdateCallBack",
            //                "oncomplete", "TweenOnCompleteCallBack",
            //                "easetype", iTween.EaseType.easeOutQuad,
            //                "ignoretimescale", true
            //                )
            //            );

            //            tweening = true;

            TweenOnUpdateCallBack(cupsTo);
        }
    }

    //    void TweenOnCompleteCallBack(){
    //        tweening = false;
    //    }

    void TweenOnUpdateCallBack(int newValue)
    {
        cups = newValue;
        //        coinText.text = coins.ToString();
        SetData(cups);
    }

    void SetData(int value)
    {
        cupText.text = value.ToString();

        levelBackgroundImage.sprite = UIManager.GetLevelBadgeSprite(MultiplayerManager.MPLevel);
        levelNumberImage.sprite = UIManager.GetLevelNumberSprite(MultiplayerManager.MPLevel);
    }
}
}
