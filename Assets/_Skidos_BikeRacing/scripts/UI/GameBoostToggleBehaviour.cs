namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameBoostToggleBehaviour : MonoBehaviour
{

    Text minusText;
    Text countText;
    public string key;
    public Toggle toggle;
    Image on;
    Image off;
    Image image;

    public bool initialized = false;

    RectTransform minusRectTransform;

    public void Awake()
    {
        countText = transform.Find("Text").GetComponent<Text>();
        minusText = transform.Find("MinusOne").GetComponent<Text>();
        minusRectTransform = minusText.GetComponent<RectTransform>();
        //        minusText.gameObject.SetActive(false);
        minusText.enabled = false;
        on = transform.Find("On").GetComponent<Image>();
        off = transform.Find("Off").GetComponent<Image>();
        image = transform.Find("Image").GetComponent<Image>();

        toggle = transform.GetComponent<Toggle>();

        initialized = true;

        if (Time.timeScale > 0)
        {
            iTween.Init(gameObject);
        }
    }

    public void SetCount(int value)
    {
        countText.text = "x" + value;
    }

    bool fadeOut = false;
    void Update()
    {

        /* for some reason if this is called from onEnable in GameBoostPanelBehaviour on the very first enable it doesn't work correctly, works fine aferwards though,
         * so delay the animation till the first update call
         */
        if (fadeOut && Time.timeScale > 0)
        {
            fadeOut = false;

            iTween.ValueTo(gameObject, iTween.Hash(
                "name", "fade",
                "from", 1f,
                "to", 0f,
                "time", 1.5f,
                "delay", 1f,
                "onupdatetarget", gameObject,
                "onupdate", "TweenOnUpdateCallBack",
                //            "oncomplete", "TweenOnCompleteCallBack",
                "easetype", iTween.EaseType.easeOutQuad
                //            "ignoretimescale", true
                )
                           );

            if (toggle.isOn)
            {

                iTween.ValueTo(gameObject, iTween.Hash(
                    "name", "minusFade",
                    "from", 1f,
                    "to", 0f,
                    "time", 0.5f,
                    "delay", 0.5f,
                    "onupdatetarget", gameObject,
                    "onupdate", "TweenOnUpdateCallBackMinusFade",
                    "easetype", iTween.EaseType.easeOutQuad
                    )
                               );

                iTween.ValueTo(gameObject, iTween.Hash(
                    "name", "minusMove",
                    "from", new Vector2(52.38f, -46.35011f),
                    "to", new Vector2(70f, -16f),
                    "time", 1f,
                    "onupdatetarget", gameObject,
                    "onupdate", "TweenOnUpdateCallBackMinusMove",
                    "easetype", iTween.EaseType.easeOutQuad
                    )
                               );
            }
        }
    }

    public void FadeOut()
    {
        fadeOut = true;
    }

    void TweenOnUpdateCallBack(float newValue)
    {
        Color tmpColor;

        tmpColor = off.color;
        tmpColor.a = newValue;
        off.color = tmpColor;

        tmpColor = on.color;
        tmpColor.a = newValue;
        on.color = tmpColor;

        tmpColor = image.color;
        tmpColor.a = newValue;
        image.color = tmpColor;

        tmpColor = countText.color;
        tmpColor.a = newValue;
        countText.color = tmpColor;

    }

    void TweenOnUpdateCallBackMinusFade(float newValue)
    {
        //        print("TweenOnUpdateCallBackMinusFade" + newValue);
        Color tmpColor;

        tmpColor = minusText.color;
        tmpColor.a = newValue;
        minusText.color = tmpColor;

    }

    void TweenOnUpdateCallBackMinusMove(Vector2 newValue)
    {

        minusRectTransform.anchoredPosition = newValue;

    }

    void OnEnable()
    {

        if (BikeDataManager.Boosts.Count > 0)
        {
            toggle.isOn = BikeDataManager.Boosts[key].Selected;
            if (toggle.isOn)
            {
                minusText.enabled = true;
            }
            else
            {
                minusText.enabled = false;
            }

            TweenOnUpdateCallBack(1f);
            if (Time.timeScale > 0)
            {
                iTween.StopByName("fade");

                iTween.StopByName("minusFade");
                iTween.StopByName("minusMove");
            }
            TweenOnUpdateCallBackMinusFade(1f);
            TweenOnUpdateCallBackMinusMove(new Vector2(52.38f, -46.35011f));

        }
    }

}

}
