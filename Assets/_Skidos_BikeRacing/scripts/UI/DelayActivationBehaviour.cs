namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class DelayActivationBehaviour : MonoBehaviour
{

    GameObject callout;

    public bool auto = true;
    public float time = 0.5f;
    public float delay = 0;
    public string childName = "FinishCallout";
    bool update = false;

    bool initialized = false;

    // Use this for initialization
    void Awake()
    {
        if (childName != "" && transform.Find(childName) != null)
        {
            callout = transform.Find(childName).gameObject;
            callout.SetActive(false);

            iTween.Init(gameObject);
            initialized = true;
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
        callout.SetActive(true);
    }

    void OnTweenUpdate(float newValue)
    {
    }

    public void Play()
    {
        if (initialized)
        {
            update = true;

            iTween.StopByName("hide_" + transform.name);
            //        OnTweenUpdate(fromAlpha);
            callout.SetActive(false);
        }
    }
}

}
