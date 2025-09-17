namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuyFastestButtonBehaviour : MonoBehaviour
{

    Image image;
    Button button;
    UIButtonGameCommand buttonGameCommand;
    //    BounceAndRepeatBehaviour tweenBehavior;

    GameObject innerImage;
    GameObject innerButtonImage;
    GameObject innerText;

    public bool anyPromo = true;
    public PromoSubPopups requiredPromo = PromoSubPopups.None;
    public PromoSubPopups centerOn = PromoSubPopups.Sale50;

    void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        buttonGameCommand = GetComponent<UIButtonGameCommand>();
        //        tweenBehavior = GetComponent<BounceAndRepeatBehaviour>();

        innerText = transform.Find("Text").gameObject;
        innerImage = transform.Find("Image").gameObject;
        innerButtonImage = transform.Find("ButtonImage").gameObject;

        image.enabled = false;
        button.enabled = false;
        buttonGameCommand.enabled = false;
        //        tweenBehavior.enabled = false;
        innerText.SetActive(false);
        innerImage.SetActive(false);
        innerButtonImage.SetActive(false);
    }

    void OnEnable()
    {
        if (
            (anyPromo && PopupPromoBehaviour.ArePromosAvailable()) || //default show if any 
            (!anyPromo && requiredPromo != PromoSubPopups.None && PopupPromoBehaviour.IsPromoAvailable(requiredPromo)) //show if a specific promo is available
            )
        {
            image.enabled = true;
            button.enabled = true;
            buttonGameCommand.enabled = true;
            //            tweenBehavior.enabled = true;
            innerText.SetActive(true);
            innerImage.SetActive(true);
            innerButtonImage.SetActive(true);
        }
        else
        {
            image.enabled = false;
            button.enabled = false;
            buttonGameCommand.enabled = false;
            //            tweenBehavior.enabled = false;
            innerText.SetActive(false);
            innerImage.SetActive(false);
            innerButtonImage.SetActive(false);
        }
        PopupPromoBehaviour.centerOn = centerOn;
    }
}

}
