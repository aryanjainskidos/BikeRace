namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupUnlockBonusLevelsBehaviour : MonoBehaviour
{

    Image bikeImage;
    Image nextBikeImage;

    Text infoText;
    GameObject infoPanel;

    GameObject giftPanel;
    GameObject defaultPanel;

    GameObject _3Buttons;
    GameObject _2Buttons;


    void Awake()
    {
        giftPanel = transform.Find("GiftPanel").gameObject;
        defaultPanel = transform.Find("DefaultPanel").gameObject;

        bikeImage = transform.Find("GiftPanel/BikeImage").GetComponent<Image>();
        nextBikeImage = transform.Find("GiftPanel/InfoPanel/NextBikeImage").GetComponent<Image>();
        infoPanel = transform.Find("GiftPanel/InfoPanel").gameObject;
        infoText = transform.Find("GiftPanel/InfoPanel/InfoText").GetComponent<Text>();

        _3Buttons = transform.Find("3Buttons").gameObject;
        _2Buttons = transform.Find("2Buttons").gameObject;
    }

    void OnEnable()
    {



        int giftID = BikeDataManager.GetLevelGiftID(BikeGameManager.SelectedLevelName);
        if (giftID > -1 && BikeDataManager.LevelGifts.ContainsKey(giftID))
        {
            //set the appropriate graphics
            giftPanel.SetActive(true);
            defaultPanel.SetActive(false);

            LevelGiftRecord record = BikeDataManager.LevelGifts[giftID];
            bikeImage.sprite = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/StyleGifts/" + record.SpriteName, record.SpriteName);

            if (BikeDataManager.LevelGifts.ContainsKey(giftID + 1))
            {
                record = BikeDataManager.LevelGifts[giftID + 1];

                infoPanel.SetActive(true);
                //                nextBikeImage.enabled = true;
                nextBikeImage.sprite = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/StyleGifts/" + record.SpriteName, record.SpriteName);

                infoText.text = Lang.Get("New style after level").Replace("|param|", record.LevelDisplayName);
            }
            else
            {
                //                nextBikeImage.enabled = false;
                //                infoText.text = "";
                infoPanel.SetActive(false);
            }

        }
        else
        {
            giftPanel.SetActive(false);
            defaultPanel.SetActive(true);
        }

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
                _2Buttons.SetActive(false);
                _3Buttons.SetActive(true);
            }
            else
            {
                _2Buttons.SetActive(true);
                _3Buttons.SetActive(false);
            }

        }

    }


    void OnDisable()
    {

    }

}

}
