namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIClickKeyDelegate : MonoBehaviour, IPointerClickHandler
{

    public string key;

    public delegate void KeyDelegate(string key);
    public KeyDelegate keyDelegate;

    void OnClick()
    {
        if (enabled == true && keyDelegate != null)
        {
            keyDelegate(key);
        }
        else print("keyDelegate is null " + key);
    }

    //for toggle
    public void OnPointerClick(PointerEventData eventData)
    {

        OnClick();

    }

}

}
