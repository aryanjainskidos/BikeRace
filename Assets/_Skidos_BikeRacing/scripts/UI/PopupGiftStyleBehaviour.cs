namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupGiftStyleBehaviour : MonoBehaviour
{

    Image bikeImage;
    Image nextBikeImage;

    Text infoText;
    GameObject infoPanel;

    Button garageButton;

    void Awake()
    {
        bikeImage = transform.Find("BikeImage").GetComponent<Image>();
        nextBikeImage = transform.Find("InfoPanel/NextBikeImage").GetComponent<Image>();
        infoPanel = transform.Find("InfoPanel").gameObject;
        infoText = transform.Find("InfoPanel/InfoText").GetComponent<Text>();

        garageButton = transform.Find("CloseButton").GetComponent<Button>();
        garageButton.onClick.AddListener(OnCancelClick);
    }

    void OnEnable()
    {
        int giftID = BikeDataManager.GetLevelGiftID(BikeGameManager.lastLoadedLevelName);
        if (giftID > -1 && BikeDataManager.LevelGifts.ContainsKey(giftID))
        {
            //set the appropriate graphics

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
    }

    void OnCancelClick()
    {
        print("OnGarageClick");
        BikeDataManager.ShowGiftStyle = false;
    }
}

}
