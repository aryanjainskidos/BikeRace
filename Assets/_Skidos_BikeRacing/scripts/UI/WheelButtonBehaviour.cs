namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WheelButtonBehaviour : MonoBehaviour
{

    //    public int coins = 7500;
    //    public bool multiplayer = false;

    Image image;
    Button button;

    Image winImage;
    Image waitImage;
    Image glowImage;

    Text winText;
    Text waitText;

    public bool visible = true;

    UIClickSound clickSound;

    SlideInBehaviour slideTween;
    BounceAndRepeatBehaviour bounceTween;
    PulseAndRepeatBehaviour glowTween;

    RectTransform rectTransform;

    RectTransform glowRectTransform;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        image = GetComponent<Image>();

        winImage = transform.Find("WinImage").GetComponent<Image>();
        waitImage = transform.Find("WaitImage").GetComponent<Image>();
        glowImage = transform.Find("GlowImage").GetComponent<Image>();

        winText = transform.Find("WinText").GetComponent<Text>();
        waitText = transform.Find("WaitText").GetComponent<Text>();
        clickSound = GetComponent<UIClickSound>();

        slideTween = GetComponent<SlideInBehaviour>();
        glowTween = glowImage.GetComponent<PulseAndRepeatBehaviour>();
        bounceTween = GetComponent<BounceAndRepeatBehaviour>();
        bounceTween.enabled = false;

        slideTween.onTweenComplete += OnSlideTweenComplete;

        rectTransform = GetComponent<RectTransform>();
        glowRectTransform = glowImage.GetComponent<RectTransform>();
    }

    void SetVisibility(bool value)
    {
        visible = value;
        image.enabled = value;
        button.enabled = value;

        Utils.ShowChildrenGraphics(gameObject, value);
    }

    void OnEnable()
    {

        if (SpinManager.initialized)
        {
            //            if (DataManager.Levels["a___002"].Tried) {
            //                SetVisibility(true);
            //            } else {
            //                SetVisibility(false);
            //            }
            SetVisibility(false);

            if (visible)
            {
                Actualize();
            }

            SpinManager.onReward += Actualize; //listen only when enabled
        }
    }

    //update state
    void Actualize()
    {
        //Debug.Log("WheelButtonBehaviour::Actualize " + transform.parent.name);

        StopAllCoroutines();

        bool readyToSpin = SpinManager.CanHazSpin(); //TODO get it from a spin manager or sth

        winText.enabled = readyToSpin;
        winImage.enabled = readyToSpin;
        glowImage.enabled = readyToSpin;

        tweenBounce = readyToSpin;// will only enable bounce if  a spin is ready

        waitText.enabled = !readyToSpin;
        waitImage.enabled = !readyToSpin;

        //        button.enabled = readyToSpin;
        //        clickSound.enabled = readyToSpin;

        if (!readyToSpin)
        {
            //            System.TimeSpan timeTillSpin = SpinManager.GetTimeTillSpin();
            //            waitText.text = timeTillSpin.Minutes + ":" + timeTillSpin.Seconds;
            //            if (bounceTween.enabled) {
            //                bounceTween.enabled = false;
            //                rectTransform.anchoredPosition = slideTween.toAnchoredPosition;
            //            }
            if (glowTween.enabled)
            {
                glowTween.enabled = false;
                glowRectTransform.localScale = glowTween.fromLocalScale;
            }

            StartCoroutine(UpdateWaitTimer());
        }

    }

    private IEnumerator UpdateWaitTimer()
    {
        while (!SpinManager.CanHazSpin())
        {

            System.TimeSpan timeTillSpin = SpinManager.GetTimeTillSpin();
            waitText.text = timeTillSpin.Minutes.ToString("D2") + ":" + timeTillSpin.Seconds.ToString("D2");

            yield return new WaitForSeconds(1);
        }

        Actualize();
    }

    bool tweenBounce = false;
    void OnSlideTweenComplete()
    {
        if (tweenBounce)
        {
            //            if (!slideTween.tweening && !bounceTween.enabled) {
            //                //Debug.Log("finished sliding");
            //                slideTween.enabled = false;
            //
            //                bounceTween.Init();
            //
            //                bounceTween.enabled = true;
            //                tweenBounce = false;
            //            }

            if (!slideTween.tweening && !glowTween.enabled)
            {
                //                //Debug.Log("finished sliding");

                glowTween.enabled = true;
                tweenBounce = false;
            }
        }
    }

    void OnDisable()
    {
        slideTween.enabled = true;
        //        bounceTween.enabled = false;
        glowTween.enabled = false;
        StopAllCoroutines();
        SpinManager.onReward -= Actualize;
    }

    void OnClick()
    {
        Actualize();
    }
}

}
