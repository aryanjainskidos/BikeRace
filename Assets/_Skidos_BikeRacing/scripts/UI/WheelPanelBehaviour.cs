namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class WheelPanelBehaviour : MonoBehaviour
{

    GameObject cupImage;
    GameObject cupText;

    GameObject bikeImage;

    void Awake()
    {

        cupImage = transform.Find("Prizes/200Cup").gameObject;
        cupText = transform.Find("Prizes/200CupText").gameObject;
        bikeImage = transform.Find("Prizes/Bike").gameObject;
    }

    void OnEnable()
    {
        //TODO replace bike image with cups if necessary
        if (Startup.Initialized)
        {
            if (BikeDataManager.Styles[(int)BikeStyleType.Beach].Locked ||
                BikeDataManager.Styles[(int)BikeStyleType.Tourist].Locked)
            {
                cupImage.SetActive(false);
                cupText.SetActive(false);
                bikeImage.SetActive(true);
            }
            else
            {
                cupImage.SetActive(true);
                cupText.SetActive(true);
                bikeImage.SetActive(false);
            }
        }
    }

}

}
