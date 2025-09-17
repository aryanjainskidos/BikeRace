namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIClickIndexDelegate : MonoBehaviour, IPointerClickHandler
{

    public int index;

    public delegate void IndexDelegate(int index);
    public IndexDelegate indexDelegate;

    void OnClick()
    {

        if (enabled == true && indexDelegate != null)
            indexDelegate(index);
        else print("indexDelegate is null " + index);
    }

    //for toggle
    public void OnPointerClick(PointerEventData eventData)
    {

        OnClick();

    }

}

}
