namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StyleBoostPanelBehaviour : MonoBehaviour
{

    Image image;
    List<Text> texts;

    void Awake()
    {
        image = GetComponent<Image>();
        texts = new List<Text>();
        for (int i = 1; i <= 3; i++)
        {
            texts.Add(transform.Find("ScrollView/Content/StyleBoostText" + i).GetComponent<Text>());
        }
    }

    public void SetRider(int styleID)
    {
        List<string> styleBoosts = BikeDataManager.Styles[styleID].Boosts;

        bool yesDesc = false;

        if (styleBoosts != null && yesDesc)
        {
            image.enabled = true;
            for (int i = 0; i < 3; i++)
            {
                texts[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < styleBoosts.Count; i++)
            {

                if (i < texts.Count)
                {
                    texts[i].gameObject.SetActive(true);
                    //"ice","magnet","invincibility","fuel"
                    switch (styleBoosts[i])
                    {
                        //                    case "ice":
                        //                        texts[i].text = Lang.Get("UI:Garage:StyleBoostIce");
                        //                        break;
                        case "magnet":
                            texts[i].text = Lang.Get("UI:Garage:StyleBoostMagnet");
                            break;
                        case "invincibility":
                            texts[i].text = Lang.Get("UI:Garage:StyleBoostInvincibility");
                            break;
                        case "fuel":
                            texts[i].text = BikeDataManager.Styles[styleID].Name == "Gold" ? Lang.Get("UI:Garage:StyleBoostFuelGold") : Lang.Get("UI:Garage:StyleBoostFuel");
                            break;
                        default:
                            texts[i].text = "";
                            break;
                    }

                }
            }

        }
        else
        {
            image.enabled = false;

            for (int i = 0; i < 3; i++)
            {
                texts[i].gameObject.SetActive(false);
            }
        }
    }
}

}
