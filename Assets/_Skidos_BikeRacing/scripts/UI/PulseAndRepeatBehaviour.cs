namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class PulseAndRepeatBehaviour : MonoBehaviour
{

    public bool auto = true;
    bool update = false;

    RectTransform rectTransform;

    public Vector3 fromLocalScale = Vector3.zero;
    public Vector3 toLocalScale = Vector3.zero;
    public Vector3 scaleBy = Vector3.zero;

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

        fromLocalScale = rectTransform.localScale;

        if (scaleBy != Vector3.zero)
        {
            toLocalScale = fromLocalScale + scaleBy;
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

    void OnTweenUpdate(Vector3 newValue)
    {

        rectTransform.localScale = newValue;

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
                    "name", "pulse_" + transform.name,
                    "from", rectTransform.localScale,
                    "to", bounce % 2 == 0 ? toLocalScale : fromLocalScale,
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

        //        iTween.StopByName(gameObject, "pulse_"+transform.name);
        //        rectTransform.anchoredPosition = fromAnchoredPosition;
    }

    void OnDisable()
    {
        if (Time.timeScale > 0)
        {
            CancelInvoke("OnTweenReverse");
            CancelInvoke("Play");
            iTween.StopByName(gameObject, "pulse_" + transform.name);
        }
    }

    void OnEnable()
    {
        if (Time.timeScale > 0)
        {
            iTween.StopByName(gameObject, "pulse_" + transform.name);
            rectTransform.localScale = fromLocalScale;

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
