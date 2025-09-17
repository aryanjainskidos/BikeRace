namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System;

public class SlideInBehaviour : MonoBehaviour
{

    public bool auto = true;
    public float time = 0.5f;
    public float delay = 0;
    bool update = false;

    public Vector2 fromAnchoredPosition = Vector2.zero;
    public bool horizontal = true;
    public bool vertical = false;

    public bool ignoreTimescale = true;

    public bool tweening = false;

    public Vector2 toAnchoredPosition;
    RectTransform rectTransform;

    public Action onTweenComplete;

    public iTween.EaseType easeType = iTween.EaseType.linear;

    void Awake()
    {

        //        if(Time.timeScale > 0) {
        //            iTween.Init(gameObject);
        //        }
        rectTransform = GetComponent<RectTransform>();
        toAnchoredPosition = rectTransform.anchoredPosition;

    }

    void OnEnable()
    {
        if (auto && enabled)
        {
            Play();
        }
    }

    void Update()
    {
        if (update && Time.timeScale > 0)
        {
            update = false;

            tweening = true;

            iTween.ValueTo(gameObject, iTween.Hash(
                "name", "slideIn_" + transform.name,
                "from", fromAnchoredPosition,
                "to", toAnchoredPosition,
                "time", time,
                "delay", delay,
                "onupdatetarget", gameObject,
                "onupdate", "OnTweenUpdate",
                "oncomplete", "OnTweenComplete",
                "easetype", easeType,//iTween.EaseType.linear,
                "ignoretimescale", ignoreTimescale
                )
            );
        }
    }

    void OnTweenUpdate(Vector2 newValue)
    {

        rectTransform.anchoredPosition = newValue;

    }

    void OnTweenComplete()
    {

        tweening = false;
        if (onTweenComplete != null)
        {
            onTweenComplete();
        }

    }

    public void Play()
    {
        if (Time.timeScale > 0)
        {
            if (enabled)
            {
                update = true;

                if (horizontal)
                {
                    fromAnchoredPosition.y = toAnchoredPosition.y = rectTransform.anchoredPosition.y;
                }

                if (vertical)
                {
                    fromAnchoredPosition.x = toAnchoredPosition.x = rectTransform.anchoredPosition.x;
                }

                tweening = false;
                iTween.StopByName(gameObject, "slideIn_" + transform.name);
                OnTweenUpdate(fromAnchoredPosition);
            }
        }
        else
        {
            OnTweenUpdate(toAnchoredPosition);
        }
    }

    void OnDisable()
    {
        if (Time.timeScale > 0)
        {
            iTween.StopByName(gameObject, "slideIn_" + transform.name);
            tweening = false;
        }
    }

}
}
