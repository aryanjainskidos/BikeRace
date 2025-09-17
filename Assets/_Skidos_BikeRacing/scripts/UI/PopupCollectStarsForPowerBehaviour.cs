namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupCollectStarsForPowerBehaviour : MonoBehaviour
{
    Text titleText;
    Text freeText;
    Text priceText;

    Text bubbleSmallText;
    Text bubbleLargeText;

    Text countdownText;

    Button closeButton;

    Button continueButton;
    Button garageButton;

    // Use this for initialization
    void Awake()
    {
        titleText = transform.Find("TitleText").GetComponent<Text>();
        freeText = transform.Find("FreeText").GetComponent<Text>();
        priceText = transform.Find("PriceText").GetComponent<Text>();

        bubbleSmallText = transform.Find("BubbleImage/SmallText").GetComponent<Text>();
        bubbleLargeText = transform.Find("BubbleImage/LargeText").GetComponent<Text>();

        countdownText = transform.Find("CountdownText").GetComponent<Text>();

        closeButton = transform.Find("CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(OnCloseClick);

        continueButton = transform.Find("ContinueButton").GetComponent<Button>();
        garageButton = transform.Find("GarageButton").GetComponent<Button>();

    }

    void OnEnable()
    {
        if (Startup.Initialized)
        {
            StopAllCoroutines();

            var completed = BikeForStarsOfferManager.OfferCompleted;

            continueButton.gameObject.SetActive(!completed);
            garageButton.gameObject.SetActive(completed);

            freeText.gameObject.SetActive(!completed);
            priceText.gameObject.SetActive(!completed);

            titleText.text = completed ?
                Lang.Get("UI:PopupCollectStarsForBike:Title:Completed") :
                    Lang.Get("UI:PopupCollectStarsForBike:Title:Uncompleted").Replace("|param|", BikeForStarsOfferManager.STARS_TO_COLLECT.ToString());
            bubbleSmallText.text = completed ? Lang.Get("UI:PopupCollectStarsForBike:BubbleSmall:Completed") : Lang.Get("UI:PopupCollectStarsForBike:BubbleSmall:Uncompleted");
            bubbleLargeText.text = completed ? Lang.Get("UI:PopupCollectStarsForBike:BubbleLarge:Completed") : Lang.Get("UI:PopupCollectStarsForBike:BubbleLarge:Uncompleted");

            if (completed)
            {
                BikeDataManager.ShowGiftStyle = true;
                BikeDataManager.GiftStyleIndex = BikeForStarsOfferManager.PRIZE_BIKE;

                countdownText.text = Lang.Get("UI:PopupCollectStarsForBike:Countdown:Completed");
            }
            else
            {
                StartCoroutine(UpdateWaitTimer());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCloseClick()
    {
        BikeDataManager.ShowGiftStyle = false;
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator UpdateWaitTimer()
    {

        while (!BikeForStarsOfferManager.OfferCompleted)
        {

            System.TimeSpan timeTillOfferEnd = BikeForStarsOfferManager.TimeTillOfferEnd;
            var time = timeTillOfferEnd.Hours.ToString("D2") + ":" + timeTillOfferEnd.Minutes.ToString("D2") + ":" + timeTillOfferEnd.Seconds.ToString("D2");
            countdownText.text = Lang.Get("UI:PopupCollectStarsForBike:Countdown:Uncompleted").Replace("|param|", time);

            yield return new WaitForSeconds(1);
        }

        OnEnable();
    }
}

}
