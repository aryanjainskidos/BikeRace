namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeOutBehaviour : MonoBehaviour
{

    public bool auto = true;
    public float time = 0.5f;
    public float delay = 0;
    bool update = false;

    public float fromAlpha = 1;

    public float toAlpha = 0;
    //    RectTransform rectTransform;

    bool initialized = false;

    MaskableGraphic graphic;

    void Awake()
    {

        graphic = GetComponent<Text>();
        if (graphic == null)
        {
            graphic = GetComponent<Image>();
        }
        initialized = true;
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
        if (update && Time.timeScale > 0)
        {
            update = false;

            iTween.ValueTo(gameObject, iTween.Hash(
                "name", "fade_" + transform.name,
                "from", fromAlpha,
                "to", toAlpha,
                "time", time,
                "delay", delay,
                "onupdatetarget", gameObject,
                "onupdate", "OnTweenUpdate",
                "easetype", iTween.EaseType.easeOutQuad,
                "ignoretimescale", true
                )
            );
        }
    }

    Color tmpColor;
    void OnTweenUpdate(float newValue)
    {

        tmpColor = graphic.color;
        tmpColor.a = newValue;
        graphic.color = tmpColor;

    }

    public void Play()
    {
        update = true;

        Reset();
    }

    public void Reset()
    {
        if (initialized)
        {
            if (Time.timeScale > 0)
            {
                iTween.StopByName(gameObject, "fade_" + transform.name);
                OnTweenUpdate(fromAlpha);
            }
        }
    }

    //    public void OnDisable() {
    //        iTween.StopByName(gameObject, "move_"+transform.name);
    //    }

}

}
