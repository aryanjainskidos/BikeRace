namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SaleButtonBehaviour : MonoBehaviour
{

    Image image;
    Button button;
    UIButtonGameCommand buttonGameCommand;
    BounceAndRepeatBehaviour tweenBehavior;

    GameObject innerImage;
    GameObject innerText;

    public bool anyPromo = true;
    public PromoSubPopups requiredPromo = PromoSubPopups.None;
    public PromoSubPopups centerOn = PromoSubPopups.Sale50;

    void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        buttonGameCommand = GetComponent<UIButtonGameCommand>();
        tweenBehavior = GetComponent<BounceAndRepeatBehaviour>();

        innerText = transform.Find("Text").gameObject;
        innerImage = transform.Find("Image").gameObject;

        image.enabled = false;
        button.enabled = false;
        buttonGameCommand.enabled = false;
        tweenBehavior.enabled = false;
        innerText.SetActive(false);
        innerImage.SetActive(false);
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
            tweenBehavior.enabled = true;
            innerText.SetActive(true);
            innerImage.SetActive(true);
        }
        else
        {
            image.enabled = false;
            button.enabled = false;
            buttonGameCommand.enabled = false;
            tweenBehavior.enabled = false;
            innerText.SetActive(false);
            innerImage.SetActive(false);
        }
        PopupPromoBehaviour.centerOn = centerOn;
    }
}

}
