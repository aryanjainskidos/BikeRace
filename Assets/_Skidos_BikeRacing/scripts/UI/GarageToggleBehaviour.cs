namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GarageToggleBehaviour : MonoBehaviour
{

    Text textOn;
    Text textOff;

    // Use this for initialization
    void Awake()
    {
        textOn = transform.Find("Off/On/Text").GetComponent<Text>();
        textOff = transform.Find("Off/Text").GetComponent<Text>();
    }

    public void SetLabels(string text)
    {
        textOn.text = textOff.text = text;
    }
}
}
