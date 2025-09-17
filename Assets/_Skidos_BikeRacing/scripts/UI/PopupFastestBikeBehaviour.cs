namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupFastestBikeBehaviour : MonoBehaviour
{

    Button closeButton;

    void Awake()
    {
        closeButton = transform.Find("CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(OnCancelClick);
    }

    void OnEnable()
    {
        BikeDataManager.ShowGiftStyle = true;
        BikeDataManager.GiftStyleIndex = 9;
    }

    void OnCancelClick()
    {
        print("OnCancelClick");
        BikeDataManager.ShowGiftStyle = false;
    }
}

}
