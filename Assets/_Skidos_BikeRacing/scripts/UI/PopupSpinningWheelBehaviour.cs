namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupSpinningWheelBehaviour : MonoBehaviour
{

    Button stopButton;
    Button closeButton;

    Transform wheelOfFortuneWheel;
    WheelOfFortuneBehaviour wheelOfFortuneBehaviour;

    RectTransform wheelPanel;

    GameObject claimPanel;

    GameObject wheelPanelPrizes;

    Transform wheelOfFortunePointer;

    RectTransform wheelPointer;

    Button claimButton;

    Image wheelImage;

    Color wheelDark = new Color(0.3f, 0.3f, 0.3f);
    Color wheelBright = new Color(1, 1, 1);

    Image pointerImage;

    GameObject nextSpinPanel;

    Text nextSpinPanelTimeText;

    //    Button remindButton;

    RectTransform nextSpinPanelTextContainer;

    GameObject stopPointer;
    GameObject claimPointer;

    // Use this for initialization
    void Awake()
    {
        stopButton = transform.Find("StopButton").GetComponent<Button>();
        closeButton = transform.Find("CloseButton").GetComponent<Button>();

        claimPanel = transform.Find("ClaimPanel").gameObject;
        claimButton = claimPanel.transform.Find("ClaimButton").GetComponent<Button>();

        wheelOfFortuneWheel = transform.Find("WheelOfFortune/Wheel");
        wheelOfFortuneBehaviour = transform.Find("WheelOfFortune").GetComponent<WheelOfFortuneBehaviour>();
        wheelOfFortuneBehaviour.onSpinningComplete += OnSpinningComplete;

        wheelOfFortunePointer = transform.Find("WheelOfFortune/Pointer");

        wheelPanel = transform.Find("WheelPanel").GetComponent<RectTransform>();
        wheelImage = transform.Find("WheelPanel/Image").GetComponent<Image>();
        wheelPanelPrizes = transform.Find("WheelPanel/Prizes").gameObject;

        wheelPointer = transform.Find("PointerImage").GetComponent<RectTransform>();
        pointerImage = wheelPointer.GetComponent<Image>();

        nextSpinPanel = transform.Find("NextSpinPanel").gameObject;
        nextSpinPanelTimeText = nextSpinPanel.transform.Find("TextContainer/TimeText").GetComponent<Text>();
        nextSpinPanelTextContainer = nextSpinPanel.transform.Find("TextContainer").GetComponent<RectTransform>();
        //        remindButton = nextSpinPanel.transform.FindChild("RemindButton").GetComponent<Button>();

        stopPointer = transform.Find("Pointer").gameObject;
        claimPointer = claimPanel.transform.Find("Pointer").gameObject;

        stopButton.onClick.AddListener(OnStopClick);
        claimButton.onClick.AddListener(OnClaimClick);

        stopPointer.SetActive(false);
        claimPointer.SetActive(false);
        //        remindButton.onClick.AddListener(OnRemindClick);
    }

    void OnEnable()
    {

        SpinManager.spinInProgress = true;

        if (SpinManager.initialized)
        {
            if (SpinManager.CanHazSpin())
            {
                stopButton.gameObject.SetActive(true);
                closeButton.interactable = true;
                closeButton.GetComponent<UIClickSound>().enabled = true;

                claimPanel.SetActive(false);
                wheelPanelPrizes.SetActive(true);

                wheelImage.color = wheelBright;
                pointerImage.enabled = true;

                nextSpinPanel.SetActive(false);

                if (BikeDataManager.Levels["a___002"].Tried && !BikeDataManager.ShowedWoFPointers)
                {// && !DataManager.Levels["a___003"].Tried
                    stopPointer.SetActive(true);
                    claimPointer.SetActive(true);
                    closeButton.onClick.AddListener(MarkPointersAsShown);
                    claimButton.onClick.AddListener(MarkPointersAsShown);
                }
                else
                {
                    stopPointer.SetActive(false);
                    claimPointer.SetActive(false);
                }
            }
            else
            {
                ShowCountdown();
            }
        }


        //
        //#if UNITY_IOS && !UNITY_EDITOR
        //        if ( NotificationManager.GavePermission() ) {
        //            nextSpinPanelTextContainer.localPosition = Vector3.zero;
        //            remindButton.gameObject.SetActive(false);
        //        } else {
        //            nextSpinPanelTextContainer.localPosition = new Vector3(0, 36.3f, 0);
        //            remindButton.gameObject.SetActive(true);
        //        }
        //#else
        //        nextSpinPanelTextContainer.localPosition = Vector3.zero;
        //        remindButton.gameObject.SetActive(false);
        //#endif
    }

    void MarkPointersAsShown()
    {
        BikeDataManager.ShowedWoFPointers = true; // can't do it in OnEnable, because in the new scene it gets called twice whenever a screen is loaded
        closeButton.onClick.RemoveListener(MarkPointersAsShown);
        claimButton.onClick.RemoveListener(MarkPointersAsShown);
    }

    void OnStopClick()
    {
        SpinManager.GetPrize();

        wheelOfFortuneBehaviour.Stop();
        stopButton.gameObject.SetActive(false);
        closeButton.interactable = false;
        closeButton.GetComponent<UIClickSound>().enabled = false;

        if (stopPointer.activeSelf)
            stopPointer.SetActive(false);

        //DONE show correct prize image/text in the animation
    }

    void OnSpinningComplete()
    {
        closeButton.interactable = true;
        closeButton.GetComponent<UIClickSound>().enabled = true;
        claimPanel.SetActive(true);
        wheelPanelPrizes.SetActive(false);
    }

    void OnClaimClick()
    {

        SpinManager.UpdateSkipCounter();// this is for the skip button

        claimPanel.SetActive(false);
        wheelImage.color = wheelDark;
        pointerImage.enabled = false;
        nextSpinPanel.SetActive(true);

        SpinManager.GivePrize();
        NotificationManager.ScheduleNotifications();
        TelemetryManager.EventWOF(SpinManager.GetTelemetryMessage());

        StartCoroutine(UpdateWaitTimer());

        //DONE if the prize is a bike, then direct the player to the garage
        if (SpinManager.prize == SpinPrizeType.BikeBeach ||
           SpinManager.prize == SpinPrizeType.BikeTourist)
        {

            BikeDataManager.ShowGiftStyle = true;
            BikeDataManager.GiftStyleIndex = (SpinManager.prize == SpinPrizeType.BikeBeach) ?
                (int)BikeStyleType.Beach : (int)BikeStyleType.Tourist;

            var tabName = (UIManager.currentScreenType == GameScreenType.Levels ? "Career" : "Competitive");
            UIManager.ToggleScreen(GameScreenType.PopupSpinningWheel);
            UIManager.SwitchScreen(GameScreenType.Garage);
            UIManager.SwitchScreenTab(GameScreenType.Garage, "Customize", tabName); //DONE if in MP go to MP garage
        }

    }

    void ShowCountdown()
    {
        claimPanel.SetActive(false);
        wheelImage.color = wheelDark;
        pointerImage.enabled = false;
        wheelPanelPrizes.SetActive(false);
        nextSpinPanel.SetActive(true);
        stopButton.gameObject.SetActive(false);

        StartCoroutine(UpdateWaitTimer());
    }

    //    void OnRemindClick()
    //    {
    //        NotificationManager.RequestPermission();
    //    }

    // Update is called once per frame
    void Update()
    {
        wheelPanel.rotation = wheelOfFortuneWheel.rotation;
        wheelPointer.rotation = wheelOfFortunePointer.rotation;

        spinSession = SpinManager.spinSession;
        spinProgress = SpinManager.SpinProgress;
    }

    #region testing
    [SerializeField]
    int spinSession;
    [SerializeField]
    int spinProgress;
    #endregion

    void OnDisable()
    {
        StopAllCoroutines();

        SpinManager.spinInProgress = false;
    }

    private IEnumerator UpdateWaitTimer()
    {

        while (!SpinManager.CanHazSpin())
        {

            System.TimeSpan timeTillSpin = SpinManager.GetTimeTillSpin();
            nextSpinPanelTimeText.text = timeTillSpin.Minutes.ToString("D2") + ":" + timeTillSpin.Seconds.ToString("D2");

            yield return new WaitForSeconds(1);
        }

        wheelOfFortuneBehaviour.Reset();
        OnEnable();
    }
}


}
