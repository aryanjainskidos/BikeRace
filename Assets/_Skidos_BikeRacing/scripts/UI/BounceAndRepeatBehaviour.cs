namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class BounceAndRepeatBehaviour : MonoBehaviour
{

    public bool auto = true;
    bool update = false;

    RectTransform rectTransform;

    public Vector2 fromAnchoredPosition = Vector2.zero;
    public Vector2 toAnchoredPosition = Vector2.zero;
    public Vector2 moveBy = Vector2.zero;

    void Awake()
    {
        //        if (Time.timeScale > 0) {
        //            iTween.Init(gameObject);
        //        }
        Init();
    }

    public void Init()
    {
        rectTransform = GetComponent<RectTransform>();

        fromAnchoredPosition = rectTransform.anchoredPosition;

        if (moveBy != Vector2.zero)
        {
            toAnchoredPosition = fromAnchoredPosition + moveBy;
        }
    }

    public float time = 1;
    public float delay = 1;
    public float startDelay = 1;
    public float startOffset = 1;
    public iTween.EaseType easeType = iTween.EaseType.linear;
    public iTween.EaseType easeType2 = iTween.EaseType.linear;
    public bool ignoreTimescale = true;

    //bool invoking = false;
    bool firstUpdate = true;
    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale > 0)
        {
            if (update)
            {
                update = false;

                Invoke("OnTweenReverse", firstUpdate ? startDelay : delay);
                firstUpdate = false;
            }
        }
    }

    void OnTweenUpdate(Vector2 newValue)
    {

        rectTransform.anchoredPosition = newValue;

    }

    public int bounce = 0;
    public int bounces = 6;

    void OnTweenReverse()
    {
        if (Time.timeScale > 0)
        {
            if (bounce < bounces)
            {
                iTween.ValueTo(gameObject, iTween.Hash(
                    "name", "badge_" + transform.name,
                    "from", rectTransform.anchoredPosition,
                    "to", bounce % 2 == 0 ? toAnchoredPosition : fromAnchoredPosition,
                    "time", time,
                    "onupdatetarget", gameObject,
                    "onupdate", "OnTweenUpdate",
                    "oncomplete", "OnTweenReverse",
                    "easetype", bounce % 2 == 0 ? easeType : easeType2,//iTween.EaseType.linear,
                    "ignoretimescale", ignoreTimescale
                    )
                );
                bounce++;
            }
            else
            {
                Play();
            }
        }
    }

    public void Play()
    {
        update = true;
        bounce = 0;

        //        iTween.StopByName(gameObject, "badge_"+transform.name);
        //        rectTransform.anchoredPosition = fromAnchoredPosition;
    }

    void OnDisable()
    {
        if (Time.timeScale > 0)
        {
            CancelInvoke("OnTweenReverse");
            CancelInvoke("Play");
            iTween.StopByName(gameObject, "badge_" + transform.name);
        }
    }

    void OnEnable()
    {
        if (Time.timeScale > 0)
        {
            iTween.StopByName(gameObject, "badge_" + transform.name);
            rectTransform.anchoredPosition = fromAnchoredPosition;

            CancelInvoke("OnTweenReverse");
            CancelInvoke("Play");

            if (auto)
            {
                //            Play();
                update = false;
                firstUpdate = true;
                Invoke("Play", startOffset);
            }
        }
        else
        {
            update = false;
            //            iTween[] tweens = GetComponents<iTween>();
            //            for (int i = 0; i < tweens.Length; i++) {
            //                DestroyImmediate(tweens[i]);
            //            }
        }
    }
}

}
