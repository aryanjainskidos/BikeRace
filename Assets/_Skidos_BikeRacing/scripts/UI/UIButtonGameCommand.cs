namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIButtonGameCommand : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{

    public enum Trigger
    {
        OnClick = 0,
        OnNothing = 1,
        OnMouseOut = 2,
        OnPress = 3,
        OnRelease = 4,
    }

    public Trigger trigger = Trigger.OnClick;

    public GameCommand command;

    void Start() { }

    //for toggle and button, toggle doesn't have OnClick
    public void OnPointerClick(PointerEventData eventData)
    {

        //        Debug.Log("UIButtonGameCommand::Pointer Click: " + transform.name);
        OnClick();

    }

    public void OnPointerDown(PointerEventData eventData)
    {

        //        Debug.Log("UIButtonGameCommand::Pointer Press: " + transform.name);
        OnPress(true);

    }

    void OnClick()
    {

        if (enabled == true && trigger == Trigger.OnClick)
        {
            //            Debug.Log("UIButtonGameCommand::Click: " + transform.name);
            BikeGameManager.ExecuteCommand(command);
        }

    }

    void OnPress(bool isPressed)
    {

        if (enabled == true && isPressed && trigger == Trigger.OnPress)
        {
            //            Debug.Log("UIButtonGameCommand::Press: " + transform.name);
            BikeGameManager.ExecuteCommand(command);
        }

    }

}

}
