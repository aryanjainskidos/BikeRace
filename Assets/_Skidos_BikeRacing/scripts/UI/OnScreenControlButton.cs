namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class OnScreenControlButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public Action upAction;
    public Action downAction;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (downAction != null)
        {
            downAction();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (upAction != null)
        {
            upAction();
        }
    }
}

}
