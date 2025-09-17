namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GainedPowerPanelBehaviour : MonoBehaviour
{

    RectTransform rectTransform;

    public int rank = -1;
    public int rankTo = -1;

    //bool update = false;
    public bool auto = true;
    public bool tweening = false;

    public float time = 1;
    //    public float delay = 1;
    public iTween.EaseType easeType = iTween.EaseType.easeInOutElastic;
    public bool ignoreTimescale = true;

    //bool initialized = false;

    Text text;
    Text powerText;

    Image image;
    Image glowImage;
    List<FadeOutBehaviour> fadeOuts;

    void Awake()
    {
        iTween.Init(gameObject);
        rectTransform = GetComponent<RectTransform>();

        text = transform.Find("Text").GetComponent<Text>();
        powerText = transform.Find("PowerText").GetComponent<Text>();

        image = transform.Find("Image").GetComponent<Image>();
        glowImage = transform.Find("GlowImage").GetComponent<Image>();

        fadeOuts = new List<FadeOutBehaviour>();
        fadeOuts.Add(glowImage.GetComponent<FadeOutBehaviour>());
        fadeOuts.Add(image.GetComponent<FadeOutBehaviour>());
        fadeOuts.Add(text.GetComponent<FadeOutBehaviour>());
        fadeOuts.Add(powerText.GetComponent<FadeOutBehaviour>());

        //initialized = true;
    }

    void OnEnable()
    {
        if (rank == -1 && Startup.Initialized)
        {
            InitData(MultiplayerManager.PermanentPowerRating);
        }
        rectTransform.localScale = Vector3.zero;
    }

    public void InitData(int value)
    {
        rank = rankTo = value;
        SetRank(0);
    }

    void Update()
    {

        if (auto && rankTo != MultiplayerManager.PermanentPowerRating)
        {
            Reset();
            rankTo = MultiplayerManager.PermanentPowerRating;
        }

        if (rankTo != rank && !tweening)
        {

            iTween.ValueTo(gameObject, iTween.Hash(
                "name", "scale_" + transform.name,
                "from", 0,
                "to", 1,
                "time", time,
                "onupdatetarget", gameObject,
                "onupdate", "OnTweenUpdate",
                "oncomplete", "OnTweenComplete",
                "easetype", easeType,
                "ignoretimescale", ignoreTimescale
                )
            );

            SetRank(rankTo - rank);
            rank = rankTo;

            tweening = true;
        }
    }

    void OnTweenUpdate(float newValue)
    {
        rectTransform.localScale = Vector3.one * newValue;
    }

    void OnTweenComplete()
    {
        print("fade out");
        tweening = false;

        foreach (var item in fadeOuts)
        {
            item.Play();
        }

    }

    void Reset()
    {
        iTween.StopByName(gameObject, "scale_" + transform.name);

        foreach (var item in fadeOuts)
        {
            item.Reset();
        }

        InitData(rankTo);
        rectTransform.localScale = Vector3.zero;

        tweening = false;
    }

    public void SetRank(int rank)
    {
        powerText.text = "+" + rank.ToString();
    }

    void OnDisable()
    {
        Reset();
    }

}

}
