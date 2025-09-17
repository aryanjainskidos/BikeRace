namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class SlideOutBehaviour : MonoBehaviour
{

    public bool auto = true;
    public float time = 0.5f;
    public float delay = 0;
    bool update = false;

    Vector2 fromAnchoredPosition;
    public bool horizontal = true;
    public bool vertical = false;

    public bool ignoreTimescale = true;

    public Vector2 toAnchoredPosition = Vector2.zero;
    RectTransform rectTransform;

    public iTween.EaseType easeType = iTween.EaseType.linear;

    void Awake()
    {

        if (Time.timeScale > 0)
        {
            iTween.Init(gameObject);
        }
        rectTransform = GetComponent<RectTransform>();
        fromAnchoredPosition = rectTransform.anchoredPosition;

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

            iTween.ValueTo(gameObject, iTween.Hash(
                "name", "slideOut_" + transform.name,
                "from", fromAnchoredPosition,
                "to", toAnchoredPosition,
                "time", time,
                "delay", delay,
                "onupdatetarget", gameObject,
                "onupdate", "OnTweenUpdate",
                "easetype", easeType, //iTween.EaseType.linear,
                "ignoretimescale", ignoreTimescale
                )
            );
        }
    }

    void OnTweenUpdate(Vector2 newValue)
    {
        //print(newValue);
        rectTransform.anchoredPosition = newValue;

    }

    public void Play()
    {
        if (enabled)
        {
            update = true;

            if (horizontal)
            {
                toAnchoredPosition.y = fromAnchoredPosition.y;
            }

            if (vertical)
            {
                toAnchoredPosition.x = fromAnchoredPosition.x;
            }

            OnTweenUpdate(fromAnchoredPosition);
            if (Time.timeScale > 0)
            {
                iTween.StopByName(gameObject, "slideOut_" + transform.name);

                iTween.Init(gameObject);
            }
        }
    }

    void OnDisable()
    {
        if (Time.timeScale > 0)
        {
            iTween.StopByName(gameObject, "slideOut_" + transform.name);
        }
    }

}

}
