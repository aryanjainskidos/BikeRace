namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MultiplayerRestartBehaviour : MonoBehaviour
{

    Text restartsText;

    Transform restartTransform;
    Button restartButton;
    Image restartButtonImage;

    Transform buyRestartsTransfrom;
    Button buyRestartsButton;

    UIButtonGameCommand restartButtonGameCommand;
    UIButtonSwitchScreen restartButtonSwitchScreen;

    int RestartPrice = 1000; //cena par 3 gab.

    bool updated = false;

    public bool changeAlphaWhenInactive = false;

    Transform restartForAdTransform;
    Button restartForAdButton;

    void Awake()
    {
        restartsText = transform.Find("RestartsText").GetComponent<Text>();

        restartTransform = transform.Find("RestartButton");
        if (restartTransform != null)
        {
            restartButton = restartTransform.GetComponent<Button>();
            restartButtonImage = restartTransform.GetComponent<Image>();
            restartButtonGameCommand = restartTransform.GetComponent<UIButtonGameCommand>();
            restartButtonSwitchScreen = restartTransform.GetComponent<UIButtonSwitchScreen>();
        }

        buyRestartsTransfrom = transform.Find("BuyRestartsButton");
        if (buyRestartsTransfrom != null)
        {
            buyRestartsButton = buyRestartsTransfrom.GetComponent<Button>();
            buyRestartsButton.onClick.AddListener(RestartForCoinsButtonHandler);
            buyRestartsButton.transform.Find("CoinText").GetComponent<Text>().text = RestartPrice.ToString();
        }

        restartForAdTransform = transform.Find("ReastartForAdButton");
        if (restartForAdTransform != null)
        {
            restartForAdButton = restartForAdTransform.GetComponent<Button>();
            restartForAdButton.onClick.AddListener(RestartForAdButtonHandler);
        }
    }

    void OnEnable()
    {
        UpdateRestartForAdButton();
    }

    void OnDisable()
    {
        updated = false;
    }

    void Update()
    {
        if (!updated && BikeGameManager.initialized)
        {
            restartsText.text = "x" + BikeGameManager.multiPlayerRestarts;

            if (restartButton != null)
            {
                if (BikeGameManager.multiPlayerRestarts <= 0)
                {
                    restartButton.enabled = false;
                    restartButtonImage.color = changeAlphaWhenInactive ? new Color(1, 1, 1, 0.5f) : new Color(0.5f, 0.5f, 0.5f, 1);
                    if (restartButtonGameCommand != null)
                        restartButtonGameCommand.enabled = false;
                    if (restartButtonSwitchScreen != null)
                        restartButtonSwitchScreen.enabled = false;
                }
                else
                {
                    restartButton.enabled = true;
                    restartButtonImage.color = Color.white;
                    if (restartButtonGameCommand != null)
                        restartButtonGameCommand.enabled = true;
                    if (restartButtonSwitchScreen != null)
                        restartButtonSwitchScreen.enabled = true;
                }
            }

            #region restart for ad
            UpdateRestartForAdButton();
            #endregion

            updated = true;
        }
    }

    void UpdateRestartForAdButton()
    {
        if (restartForAdTransform != null)
        {

            //after the race is finished reset the WatchedAdToGetAnExtraReplay flag, done in LevelManager

            if (BikeGameManager.multiPlayerRestarts <= 0 && !BikeDataManager.WatchedAdToGetAnExtraReplay)
            {
                restartForAdTransform.gameObject.SetActive(true);
            }
            else
            {
                restartForAdTransform.gameObject.SetActive(false);
            }
        }
    }

    void RestartForCoinsButtonHandler()
    {
        if (PurchaseManager.CoinPurchase(RestartPrice))
        {
            BikeGameManager.multiPlayerRestarts += 3;
            updated = false;
        }
    }

    int Reward = 1;

    void RestartForAdButtonHandler()
    {
        Debug.Log("RestartForAdButtonHandler()");
        //if haven't shown ad for this ride, otherwise then this can't be called because the button is not visible
        // HeyZapManager.ShowRewarded(IncentiveType.Replay, Reward, FinishedWatchingAd);
    }

    void FinishedWatchingAd()
    {
        if (restartForAdTransform != null)
        {
            Debug.Log("FinishedWatchingAd()");
            restartForAdTransform.gameObject.SetActive(false);
            BikeDataManager.WatchedAdToGetAnExtraReplay = true; // record that an ad has been watched during this race

            updated = false;
        }
    }
}

}
