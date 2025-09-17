namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeDisplayBehaviour : MonoBehaviour
{

    Text timeText;
    public float time = -1;
    public float timeTo = -1;

    public bool auto = true;
    //    bool tweening = false;
    //TODO allow manual change where it's necessary to animate the time number

    // Use this for initialization
    void Awake()
    {
        timeText = transform.Find("Text").GetComponent<Text>();
        timeText.text = "0";
    }

    void OnEnable()
    {
        if (auto)
        {
            InitData(BikeGameManager.TimeElapsed);
        }
    }

    public void InitData(float value)
    {
        time = timeTo = value;
        SetData(time);
    }

    void Update()
    {
        if (auto && timeTo != BikeGameManager.TimeElapsed)
        {
            timeTo = BikeGameManager.TimeElapsed;
        }

        //        if (timeTo != time && !tweening) {
        ////            timesTo = DataManager.times;
        //
        //            iTween.ValueTo(gameObject, iTween.Hash(
        //                "from", time,
        //                "to", timeTo,
        //                "time", 1f,
        //                "onupdatetarget", gameObject,
        //                "onupdate", "TweenOnUpdateCallBack",
        //                "oncomplete", "TweenOnCompleteCallBack",
        //                "easetype", iTween.EaseType.easeOutQuad
        //                )
        //            );
        //
        //            tweening = true;
        //        }
        if (timeTo != time)
        {
            TweenOnUpdateCallBack(timeTo);
        }
    }

    //    void TweenOnCompleteCallBack(){
    //        tweening = false;
    //    }
    //
    void TweenOnUpdateCallBack(float newValue)
    {
        time = newValue;
        SetData(time);
    }

    void SetData(float value)
    {
        timeText.text = value.ToString("F2");
    }
}
}
