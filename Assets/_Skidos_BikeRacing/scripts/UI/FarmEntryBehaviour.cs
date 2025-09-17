namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public enum FarmEntryState
{
    Waiting,
    Farming,
    Locked,
}

public class FarmEntryBehaviour : MonoBehaviour
{

    public int upgradeID = -1;

    UIButtonSimpleDelegate finishClickDelegate;
    UIButtonSimpleDelegate produceClickDelegate;

    Transform finishButton;
    Transform produceButton;
    Transform upgradeButton;
    Transform progressPanel;

    Transform producePointer;
    Transform upgradePointer;
    Transform finishNowPointer;

    Image iconImage;
    Image lockImage;

    Text nameText;
    Text countText;
    Text lockText;

    Text finishTimeText;
    Text finishCoinText;
    Slider finishSlider;

    Text produceTimeText;

    public FarmEntryState state = FarmEntryState.Locked;
    string boostKey;
    BoostRecord record;

    //    TimeSpan duration;
    int price;

    // Use this for initialization
    void Awake()
    {
        finishButton = transform.Find("FinishButton");
        produceButton = transform.Find("ProduceButton");
        upgradeButton = transform.Find("UpgradeButton");
        progressPanel = transform.Find("ProgressPanel");

        producePointer = produceButton.Find("Pointer");
        producePointer.gameObject.SetActive(false);

        upgradePointer = upgradeButton.Find("Pointer");
        upgradePointer.gameObject.SetActive(false);

        finishNowPointer = finishButton.Find("Pointer");
        finishNowPointer.gameObject.SetActive(false);

        iconImage = transform.Find("IconImage").GetComponent<Image>();
        lockImage = transform.Find("LockImage").GetComponent<Image>();

        nameText = transform.Find("NameText").GetComponent<Text>();
        countText = transform.Find("CountText").GetComponent<Text>();
        lockText = transform.Find("LockText").GetComponent<Text>();

        finishTimeText = finishButton.Find("TimeText").GetComponent<Text>();
        finishCoinText = finishButton.Find("CoinText").GetComponent<Text>();
        finishSlider = finishButton.Find("Slider").GetComponent<Slider>();

        produceTimeText = produceButton.Find("TimeText").GetComponent<Text>();

        finishClickDelegate = finishButton.GetComponent<UIButtonSimpleDelegate>();
        produceClickDelegate = produceButton.GetComponent<UIButtonSimpleDelegate>();

        finishClickDelegate.buttonDelegate = OnFinishClick;
        produceClickDelegate.buttonDelegate = OnProduceClick;

    }

    void OnFinishClick()
    {

        TimeSpan diff = DateTime.Now.Subtract(record.FarmingTimestamp);
        int currentPrice = Mathf.CeilToInt(record.Price * (float)(1 - diff.TotalSeconds / record.FarmingDuration.TotalSeconds)); //calculate price based on time passed

        if (PurchaseManager.CoinPurchase(currentPrice))
        { //

            SwitchState(FarmEntryState.Waiting);

            record.Number += record.NumberPerFarming;//+;
            record.FarmingTimestamp = DateTime.MinValue;

            TelemetryManager.EventBoostEnd(record.ID, false); // ID,  waited/paid = true/false

            Actualize();

        }

    }

    void OnProduceClick()
    {

        SwitchState(FarmEntryState.Farming);
        record.FarmingTimestamp = DateTime.Now;

        TelemetryManager.EventBoostStart(record.ID);

        if (BikeDataManager.FirstProduce)
        { //if hasn't been produced yet
            if (BikeDataManager.FirstUpgrade)
            { //and hasn't been upgraded yet
                upgradePointer.gameObject.SetActive(true); // ShowUpgradePointer
            }
            BikeDataManager.FirstProduce = false;
        }

        producePointer.gameObject.SetActive(false);
        //print("record.FarmingTimestamp == DataManager.Boosts[boostKey].FarmingTimestamp " + record.FarmingTimestamp.Equals(DataManager.Boosts[boostKey].FarmingTimestamp) );

    }

    void OnEnable()
    { //a boost can't get unlocked in the screen, so it's enough to update it on enable
        Actualize();
    }

    void Actualize()
    {
        //print("FarmEntryBehaviour::Actualize");
        //print(upgradeID);

        if (upgradeID >= 0 && upgradeID < BoostLineupManager.BoostNames.Length)
        {
            boostKey = BoostLineupManager.BoostNames[upgradeID];

            if (BikeDataManager.Boosts.ContainsKey(boostKey))
            {

                if (record == null)
                {
                    record = BikeDataManager.Boosts[boostKey];
                }

                //print("record.Discovered "+record.Discovered);

                if (record.Discovered)
                {
                    if (record.FarmingTimestamp != DateTime.MinValue)
                    {
                        SwitchState(FarmEntryState.Farming);
                    }
                    else
                    {
                        SwitchState(FarmEntryState.Waiting);
                    }

                    countText.text = "x" + record.Number.ToString();
                    price = record.Price;
                    produceTimeText.text = record.FarmingDuration.Minutes.ToString("D2") + ":" + record.FarmingDuration.Seconds.ToString("D2");


                    ShowProducePointer(boostKey, record);

                    if (upgradePointer.gameObject.activeSelf)
                    { //if not hidden, hide upgrade pointer
                        upgradePointer.gameObject.SetActive(false);
                    }

                    ShowFinishNowPointer();

                }
                else
                {
                    SwitchState(FarmEntryState.Locked);
                }

            }
        }



    }

    void ShowFinishNowPointer()
    {
        //        print("ShowFinishNowPointer" + finishButton.gameObject.activeSelf);

        if (finishButton.gameObject.activeSelf)
        { //if finish button active
          //            print("DataManager.FirstFinishNow && !DataManager.FirstProduce " + DataManager.FirstFinishNow + " " + DataManager.FirstProduce);
            if (BikeDataManager.FirstFinishNow && !BikeDataManager.FirstProduce)
            {

                finishNowPointer.gameObject.SetActive(true);
                BikeDataManager.FirstFinishNow = false;
            }
        }
        else
        {
            finishNowPointer.gameObject.SetActive(false);
        }
    }

    void ShowProducePointer(string boostKey, BoostRecord record)
    {
        //TODO if entered from game and boost discovered and recomended in the level show tap tap pointer, if it's the first time that player clicked produce, show tap tap pointer on the upgrade button
        if (BikeGameManager.initialized)
        { //if game level is initialized
            if (record.Number == 0)
            { //if this boost number is 0
                bool suggested = false; //if it's recommended in this level
                switch (boostKey)
                {
                    case "ice":
                        suggested = BikeGameManager.levelInfo.SuggBoostIce;
                        break;
                    case "magnet":
                        suggested = BikeGameManager.levelInfo.SuggBoostMagnet;
                        break;
                    case "invincibility":
                        suggested = BikeGameManager.levelInfo.SuggBoostInvincibility;
                        break;
                    case "fuel":
                        suggested = BikeGameManager.levelInfo.SuggBoostFuel;
                        break;
                    default:
                        break;
                }

                if (suggested)
                {
                    producePointer.gameObject.SetActive(true);
                }
                else
                {
                    producePointer.gameObject.SetActive(false);
                }
            }
            else
            {
                producePointer.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {

        if (state != FarmEntryState.Locked)
        {

            if (record.FarmingTimestamp != DateTime.MinValue)
            {
                TimeSpan passed = DateTime.Now.Subtract(record.FarmingTimestamp);
                TimeSpan delta = record.FarmingDuration.Subtract(passed);

                finishTimeText.text = delta.Minutes.ToString("D2") + ":" + delta.Seconds.ToString("D2");
                finishSlider.value = (float)(passed.TotalSeconds / record.FarmingDuration.TotalSeconds);
                finishCoinText.text = Mathf.CeilToInt(price * (float)(1 - passed.TotalSeconds / record.FarmingDuration.TotalSeconds)).ToString();
            }
            else
            {
                if (state != FarmEntryState.Waiting)
                {
                    SwitchState(FarmEntryState.Waiting);
                    countText.text = "x" + record.Number.ToString();
                }
            }

        }

    }

    public void SwitchState(FarmEntryState newState)
    {
        switch (newState)
        {
            case FarmEntryState.Farming:

                finishButton.gameObject.SetActive(true);
                produceButton.gameObject.SetActive(false);

                if (lockImage.enabled)
                {
                    lockImage.enabled = false;
                    lockText.enabled = false;
                }

                break;

            case FarmEntryState.Waiting:

                finishButton.gameObject.SetActive(false);
                produceButton.gameObject.SetActive(true);

                if (state == FarmEntryState.Locked)
                {

                    upgradeButton.gameObject.SetActive(true);
                    progressPanel.gameObject.SetActive(true);

                    countText.enabled = true;
                    lockImage.enabled = false;
                    lockText.enabled = false;

                    Color colorr = iconImage.color;
                    colorr.a = 1;
                    iconImage.color = colorr;
                    nameText.color = colorr;
                }
                break;

            case FarmEntryState.Locked:
                lockImage.enabled = true;

                lockText.enabled = true;
                countText.enabled = false;

                finishButton.gameObject.SetActive(false);
                produceButton.gameObject.SetActive(false);
                upgradeButton.gameObject.SetActive(false);
                progressPanel.gameObject.SetActive(false);

                Color color = iconImage.color;
                color.a = 0.25f;
                iconImage.color = color;
                nameText.color = color;
                break;

            default:
                print("unregistered state");
                break;
        };

        state = newState;
    }

    //    void SetLock(bool on) {
    //        print("SetLock " + on);
    //        lockImage.enabled = on;
    //
    //        countText.enabled = !on;
    //
    //        finishButton.gameObject.SetActive(!on);
    //        produceButton.gameObject.SetActive(!on);
    //        upgradeButton.gameObject.SetActive(!on);
    //
    //        Color color;
    //        if (on)
    //        {
    //            color = iconImage.color;
    //            color.a = 0.25f;
    //            iconImage.color = color;
    //            nameText.color = color;
    //            state = FarmEntryState.Locked;
    //        } else {
    //            color = iconImage.color;
    //            color.a = 1f;
    //            iconImage.color = color;
    //            nameText.color = color;
    //            state = FarmEntryState.Waiting;
    //        }
    //    }

    public int UpgradeID
    {
        get
        {
            return upgradeID;
        }
        set
        {
            upgradeID = value;

            //            if (finishClickDelegate != null)
            //                finishClickDelegate.index = upgradeID;
            //            
            //            if (produceClickDelegate != null)
            //                produceClickDelegate.index = upgradeID;
        }
    }

    //    public UIClickIndexDelegate.IndexDelegate FinishDelegate
    //    {
    //        get
    //        {
    //            return finishClickDelegate != null ? finishClickDelegate.indexDelegate : null;
    //        }
    //        set
    //        {            
    //            if (finishClickDelegate != null)
    //                finishClickDelegate.indexDelegate = value;
    //        }
    //    }
    //    
    //    public UIClickIndexDelegate.IndexDelegate ProduceDelegate
    //    {
    //        get
    //        {
    //            return produceClickDelegate != null ? produceClickDelegate.indexDelegate : null;
    //        }
    //        set
    //        {            
    //            if (produceClickDelegate != null)
    //                produceClickDelegate.indexDelegate = value;
    //        }
    //    }


}
}
