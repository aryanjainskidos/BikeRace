namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HideBehaviour : MonoBehaviour
{

    public bool auto = true;
    public float time = 0.5f;
    public float delay = 0;
    bool update = false;

    RectTransform rectTransform;

    MaskableGraphic graphic;

    void Awake()
    {

        graphic = GetComponent<Text>();
        if (graphic == null)
        {
            graphic = GetComponent<Image>();
        }

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
        if (update)
        {
            update = false;

            iTween.ValueTo(gameObject, iTween.Hash(
                "name", "hide_" + transform.name,
                "from", 0.0f,
                "to", 1.0f,
                "time", time,
                "delay", delay,
                "onupdatetarget", gameObject,
                "onupdate", "OnTweenUpdate",
                "oncomplete", "OnTweenComplete",
                "easetype", iTween.EaseType.linear,
                "ignoretimescale", true
                )
            );
        }
    }

    void OnTweenComplete()
    {

        graphic.enabled = false;

    }

    void OnTweenUpdate(float newValue)
    {
        //        
        //        rectTransform.anchoredPosition = newValue;
        //        
        //        Color tmpColor;
        //        
        //        tmpColor = graphic.color;
        //        tmpColor.a = newValue;
        //        graphic.color = tmpColor;
        //        
    }

    public void Play()
    {
        update = true;

        iTween.StopByName("hide_" + transform.name);
        //        OnTweenUpdate(fromAlpha);
        graphic.enabled = true;
    }

}

}
