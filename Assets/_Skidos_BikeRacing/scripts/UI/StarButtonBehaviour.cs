namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StarButtonBehaviour : MonoBehaviour
{

    //    public int coins = 7500;
    //    public bool multiplayer = false;

    Image image;
    Button button;

    Text timeText;
    Text starText;

    public bool visible = true;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        image = GetComponent<Image>();

        timeText = transform.Find("TimeText").GetComponent<Text>();
        starText = transform.Find("StarText").GetComponent<Text>();
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

        if (Startup.Initialized)
        {
            Actualize();
        }

    }

    //update state
    //this button does too much for a button
    void Actualize()
    {
        //Debug.Log("WheelButtonBehaviour::Actualize " + transform.parent.name);

        StopAllCoroutines();

        BikeForStarsOfferManager.Update();

        bool available = BikeForStarsOfferManager.OfferAvailable && !BikeForStarsOfferManager.OfferCompleted;

        SetVisibility(available); //TODO not completed and not expired

        starText.text = BikeForStarsOfferManager.StarsCollectedSinceOfferStart.ToString() + "/" + BikeForStarsOfferManager.STARS_TO_COLLECT;

        if (available)
        {
            StartCoroutine(UpdateWaitTimer());
        }

    }

    private IEnumerator UpdateWaitTimer()
    {
        while (BikeForStarsOfferManager.OfferAvailable)
        {

            System.TimeSpan timeTillSpin = BikeForStarsOfferManager.TimeTillOfferEnd;
            timeText.text = timeTillSpin.Hours.ToString("D2") + ":" + timeTillSpin.Minutes.ToString("D2") + ":" + timeTillSpin.Seconds.ToString("D2");

            yield return new WaitForSeconds(1);
        }

        Actualize();
    }



    void OnDisable()
    {
        StopAllCoroutines();
    }

    void OnClick()
    {
        Actualize();
    }

    #region testing
    public bool reset = false;
    public bool init = false;
    public string notifTime;
    public string startTime;
    public bool expired = false;
    public bool available = false;
    public bool completed = false;

    void Update()
    {
        if (reset)
        {
            BikeForStarsOfferManager.Reset();
            reset = false;
        }

        if (init)
        {
            BikeForStarsOfferManager.StartOffer();
            init = false;
        }

        notifTime = BikeForStarsOfferManager.offerNotificationTimestamp.ToString();
        startTime = BikeForStarsOfferManager.offerStartTimestamp.ToString();
        expired = BikeForStarsOfferManager.OfferExpired;
        available = BikeForStarsOfferManager.OfferAvailable;
        completed = BikeForStarsOfferManager.OfferCompleted;
    }
    #endregion
}

}
