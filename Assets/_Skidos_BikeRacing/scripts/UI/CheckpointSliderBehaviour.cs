namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum CheckpointSliderState
{
    Locked,
    Unlocked,
    Selected,
}

public class CheckpointSliderBehaviour : MonoBehaviour
{

    public int level;
    public int value;

    public CheckpointSliderState state = CheckpointSliderState.Locked;

    Color32 lockedTextColor = new Color32(214, 214, 214, 255);
    Color32 unlockedTextColor = new Color32(24, 238, 17, 255);

    Color32[] barColor = new Color32[]{
        new Color32(90,90,90,255),//locked
        new Color32(253,245,23,255),//unlocked 1
        new Color32(254,207,8,255),
        new Color32(251,186,2,255),
        new Color32(252,165,2,255),
        new Color32(249,143,2,255),
        new Color32(252,122,9,255),
        new Color32(250,107,3,255),
        new Color32(252,83,2,255),
        new Color32(238,63,1,255),
        new Color32(227,4,2,255)
    };

    Color32[] levelTextColor = new Color32[]{
        new Color32(108,108,108,255),//locked
        new Color32(253,245,23,255),//unlocked 1
        new Color32(239,178,8,255),
        new Color32(241,157,11,255),
        new Color32(249,141,2,255),
        new Color32(242,120,2,255), //5
        new Color32(238,111,13,255),
        new Color32(241,83,7,255),
        new Color32(206,56,1,255),
        new Color32(215,3,4,255),
        new Color32(201,4,1,255)
    };


    Slider slider;

    Text coinText;
    //    Image coinImage;

    Text levelText;

    Image barImage;

    Image tickImage;

    bool initialized = false;

    Animator checkpointAnim;
    AnimatorStateInfo checkpointAnimState;

    public bool animationEnded = false;

    void Awake()
    {

        tickImage = transform.Find("Fill Area/Fill/TickImage").GetComponent<Image>();

        levelText = transform.Find("Fill Area/Fill/LevelText").GetComponent<Text>();

        coinText = transform.Find("Fill Area/Fill/CoinText").GetComponent<Text>();
        //        coinImage = transform.FindChild("Fill Area/Fill/CoinImage").GetComponent<Image>();

        barImage = transform.Find("Fill Area/Fill/BarImage").GetComponent<Image>();

        checkpointAnim = transform.Find("Fill Area/Fill/Checkpoint/Checkpoint_Pole").GetComponent<Animator>();

        checkpointAnim.GetComponent<GenericAnimationEvents>().finishDelegate = OnFinish;

        slider = transform.GetComponent<Slider>();

        initialized = true;
        animationEnded = false;

        if (slider.value < 0.2f)
        {
            levelText.enabled = false;
        }
        levelText.text = "" + level;



        coinText.text = value.ToString();

    }

    public float normTime = 0;

    //    void Update() {
    //
    //        if(checkpointAnim.enabled && !animationEnded) {
    //            checkpointAnimState = checkpointAnim.GetCurrentAnimatorStateInfo(0);
    //            normTime = checkpointAnimState.normalizedTime;
    //            
    //            if (checkpointAnimState.normalizedTime >= 1) { // if the last star finished animating animate the crate
    //                animationEnded = true;
    //            }
    //        }
    //
    //    }

    void OnFinish()
    {
        animationEnded = true;
    }

    void OnEnabled()
    {
        animationEnded = false;
    }

    void OnEnable()
    {
        int visualMultiplier = 1;
        if (CentralizedOfferManager.IsDoubleCoinWeekendOn())
        { //ja ir dubult-koinu wíkends, tad te vizuáli palielina koinu apjomu
            visualMultiplier = 2;
        }
        coinText.text = (value * visualMultiplier).ToString();
    }

    public void Reset()
    {

        animationEnded = false;
        //        checkpointAnim.Play("Checkpoint", -1, 0);
    }

    public void StopAnimation()
    {
        //        checkpointAnim.Play("Checkpoint", -1, 0);
        checkpointAnim.speed = 0;
    }

    public void PlayAnimation()
    {
        checkpointAnim.speed = 1;
        checkpointAnim.Play("Checkpoint", -1, 0);
    }

    public void SetSliderValue(float normalizedValue)
    {
        slider.value = normalizedValue;
    }

    public void SetState(CheckpointSliderState value)
    {

        if (!initialized)
        {
            Awake();
        }

        switch (value)
        {
            case CheckpointSliderState.Locked:
                Lock();
                tickImage.enabled = false;
                coinText.enabled = true;
                break;

            case CheckpointSliderState.Unlocked:
                Unlock();
                tickImage.enabled = true;
                coinText.enabled = false;
                break;

            case CheckpointSliderState.Selected:
                Unlock();
                tickImage.enabled = false;
                coinText.enabled = true;
                break;
            default:
                break;
        }
        state = value;
        //        SetLevel(level);
    }

    void Lock()
    {

        coinText.color = lockedTextColor;
        barImage.color = barColor[0];
        levelText.color = levelTextColor[0];

        //        checkpointAnim.speed = 0;
        checkpointAnim.enabled = false;
    }

    void Unlock()
    {

        coinText.color = unlockedTextColor;
        barImage.color = barColor[level];
        levelText.color = levelTextColor[level];

        checkpointAnim.enabled = true;
        //        checkpointAnim.speed = 1;
        //        checkpointAnim.Play("Checkpoint", -1, 0);
    }

}
}
