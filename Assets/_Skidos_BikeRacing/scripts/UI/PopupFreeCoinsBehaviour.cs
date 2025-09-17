namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupFreeCoinsBehaviour : MonoBehaviour
{

    Button facebookButton;
    Button twitterButton;
    Button vkButton;

    GameObject _2Buttons;
    GameObject _3Buttons;

    int coins;
    Text coinsText;

    void Awake()
    {
        facebookButton = transform.Find("FBButton").GetComponent<Button>();
        twitterButton = transform.Find("TwitterButton").GetComponent<Button>();
        vkButton = transform.Find("3Buttons/VKButton").GetComponent<Button>();

        coinsText = transform.Find("DefaultPanel/CoinsText").GetComponent<Text>();

        // facebookButton.onClick.AddListener(ShareManager.ShareToFacebookForFreeCoins);
        // twitterButton.onClick.AddListener(ShareManager.ShareToTwitterForFreeCoins);
        // vkButton.onClick.AddListener(ShareManager.ShareToVKForFreeCoins);

        _2Buttons = transform.Find("2Buttons").gameObject;
        _3Buttons = transform.Find("3Buttons").gameObject;

    }

    bool update = true;
    void OnEnable()
    {
        coinsText.text = ShareManager.ShareCoinReward.ToString();
        update = true;

        if (Startup.Initialized)
        {

#if !UNITY_IOS
			bool ruNotIos = DataManager.Country == "RU" || Lang.language == "ru";//valsts ir krievija (śo noskaidro appas sákumá, bet ne uzreiz, tápéc te uz katru Enable párbauda) vai valoda krievu
#else
            bool ruNotIos = false;
#endif


            if (ruNotIos)
            {
                // #if !UNITY_IOS
                // ShareManager.SetupVK();//ja nu nav
                // #endif
                vkButton.gameObject.SetActive(true);
                _2Buttons.SetActive(false);
                _3Buttons.SetActive(true);
            }
            else
            {
                vkButton.gameObject.SetActive(false);
                _2Buttons.SetActive(true);
                _3Buttons.SetActive(false);
            }


        }

    }

    void Update()
    {
        if (update)
        {
            coinsText.text = ShareManager.ShareCoinReward.ToString();
            update = false;
        }
    }

}

}
