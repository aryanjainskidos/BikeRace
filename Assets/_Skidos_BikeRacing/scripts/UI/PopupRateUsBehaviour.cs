namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupRateUsBehaviour : MonoBehaviour
{

    Text text;

    // Use this for initialization
    void Awake()
    {
        text = transform.Find("Text").GetComponent<Text>();
    }

    // Update is called once per frame
    void OnEnable()
    {
        switch (BikeDataManager.RateUsShowCount)
        {
            case 1:
                text.text = Lang.Get("UI:PopupRateUs:MainText");
                break;
            case 2:
                text.text = Lang.Get("UI:PopupRateUs:MainTextReminder");
                break;
            default:
                text.text = "";
                break;
        }
    }
}

}
