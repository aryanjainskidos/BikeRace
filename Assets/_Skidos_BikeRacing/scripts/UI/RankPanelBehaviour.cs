namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RankPanelBehaviour : MonoBehaviour
{

    Text text;
    public int rank = -1;
    public int rankTo = -1;


    public bool auto = true;
    public bool tweening = false;

    //bool initialized = false;

    // Use this for initialization
    void Awake()
    {
        text = transform.Find("Text").GetComponent<Text>();
        //initialized = true;
    }

    void OnEnable()
    {
        if (rank == -1 && Startup.Initialized)
        {
            UpdateRating();
            InitData(MultiplayerManager.PermanentPowerRating);
        }
    }

    public void InitData(int value)
    {
        rank = rankTo = value;
        SetRank(rank);
    }

    // Update is called once per frame
    public void SetRank(int rank)
    {
        text.text = rank.ToString();
    }

    public void UpdateRating(int i = 0)
    {
        if (Startup.Initialized)
        {
            MultiplayerManager.RecalculateMyPowerRating();
            //TODO take into account that there are two components to power rating now, permanent and temporary
            //            SetRank(MultiplayerManager.PowerRating);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (auto && rankTo != MultiplayerManager.PermanentPowerRating)
        {
            rankTo = MultiplayerManager.PermanentPowerRating;
        }

        if (rankTo != rank && !tweening)
        {
            //            rankTo = DataManager.rank;

            //            if(rankTo > rank){ // play sound only for coin increase
            //                SoundManager.Play("CoinCounting");
            //            }

            iTween.ValueTo(gameObject, iTween.Hash(
                "from", rank,
                "to", rankTo,
                "time", 1f, // 1.7 sec, the same as sound length
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
        rank = newValue;
        SetRank(rank);
    }

}

}
