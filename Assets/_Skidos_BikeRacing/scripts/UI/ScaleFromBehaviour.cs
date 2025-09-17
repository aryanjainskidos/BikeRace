namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class ScaleFromBehaviour : MonoBehaviour
{

    public bool auto = true;
    public float time = 0.5f;
    public float delay = 0;
    bool update = false;

    public Vector3 fromScale = Vector3.zero;

    public bool ignoreTimescale = true;

    Vector3 toScale;
    RectTransform rectTransform;

    public iTween.EaseType easeType = iTween.EaseType.linear;

    void Awake()
    {

        //        if(Time.timeScale > 0) {
        //            iTween.Init(gameObject);
        //        }
        rectTransform = GetComponent<RectTransform>();
        toScale = rectTransform.localScale;

    }

    void OnEnable()
    {
        if (auto)
        {
            Play();
        }
    }

    void Update()
    {
        if (Time.timeScale > 0)
        {
            if (update)
            {
                update = false;

                iTween.ValueTo(gameObject, iTween.Hash(
                    "name", "slideIn_" + transform.name,
                    "from", fromScale,
                    "to", toScale,
                    "time", time,
                    "delay", delay,
                    "onupdatetarget", gameObject,
                    "onupdate", "OnTweenUpdate",
                    "easetype", easeType,//iTween.EaseType.linear,
                    "ignoretimescale", ignoreTimescale
                    )
                );
            }
        }
    }

    void OnTweenUpdate(Vector3 newValue)
    {

        rectTransform.localScale = newValue;

    }

    public void Play()
    {
        update = true;

        if (Time.timeScale > 0)
        {
            iTween.StopByName(gameObject, "slideIn_" + transform.name);
            OnTweenUpdate(fromScale);
        }
    }

    void OnDisable()
    {
        if (Time.timeScale > 0)
        {
            iTween.StopByName(gameObject, "slideIn_" + transform.name);
        }
    }

}

}
