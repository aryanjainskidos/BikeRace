namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClaimPanelBehaviour : MonoBehaviour
{

    GameObject coinPanel;
    Text coinPanelText;

    GameObject bikeBeachImage;
    GameObject bikeTouristImage;

    GameObject coinPanelCupImage;

    GameObject coinPanelCoinImage;

    void Awake()
    {
        coinPanel = transform.Find("WheelPrize/Star/CoinCupContainer").gameObject;
        coinPanelText = coinPanel.transform.Find("Text").GetComponent<Text>();
        coinPanelCupImage = coinPanel.transform.Find("CupImage").gameObject;
        coinPanelCoinImage = coinPanel.transform.Find("CoinImage").gameObject;

        bikeBeachImage = transform.Find("WheelPrize/Star/BikeBeachImage").gameObject; //BikeBeachImage
        bikeTouristImage = transform.Find("WheelPrize/Star/BikeTouristImage").gameObject; //BikeTouristImage
    }

    void OnEnable()
    {

        bool showCoinPanel = true;
        switch (SpinManager.prize)
        {
            case SpinPrizeType.CoinsX100:
                coinPanelText.text = "100";
                break;
            case SpinPrizeType.CoinsX1000:
                coinPanelText.text = "1000";
                break;
            case SpinPrizeType.CoinsX100000:
                coinPanelText.text = "100000";
                break;
            case SpinPrizeType.CoinsX2500:
                coinPanelText.text = "2500";
                break;
            case SpinPrizeType.CoinsX500:
                coinPanelText.text = "500";
                break;
            case SpinPrizeType.CupsX100:
                coinPanelText.text = "100";
                break;
            case SpinPrizeType.CupsX200:
                coinPanelText.text = "200";
                break;
            default:
                showCoinPanel = false;
                break;
        }

        bool cups = (SpinManager.prize == SpinPrizeType.CupsX100 || SpinManager.prize == SpinPrizeType.CupsX200);

        coinPanelCupImage.SetActive(cups);
        coinPanelCoinImage.SetActive(!cups);

        coinPanel.SetActive(showCoinPanel);
        bikeBeachImage.SetActive((SpinManager.prize == SpinPrizeType.BikeBeach));
        bikeTouristImage.SetActive((SpinManager.prize == SpinPrizeType.BikeTourist));

    }

    // Update is called once per frame
    void Update()
    {

    }
}

}
