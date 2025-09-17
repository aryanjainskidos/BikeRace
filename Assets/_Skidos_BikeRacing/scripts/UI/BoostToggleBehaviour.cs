namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BoostToggleBehaviour : MonoBehaviour
{

    Text countText;
    public string key;
    public Toggle toggle;
    public UIClickIndexDelegate cid;

    public void Awake()
    {
        countText = transform.Find("Text").GetComponent<Text>();
        toggle = transform.GetComponent<Toggle>();
        cid = transform.GetComponent<UIClickIndexDelegate>();
    }

    public void SetCount(int value)
    {
        countText.text = "x" + value;
    }

}

}
