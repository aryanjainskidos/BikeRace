namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIButtonToggleScreen : MonoBehaviour
{

    public enum Trigger
    {
        OnClick,
        OnMouseOver,
        OnMouseOut,
        OnPress,
        OnRelease,
    }

    public Trigger trigger = Trigger.OnClick;

    public GameScreenType screen;

    //automatically add OnClick to unity ui button listeners
    void Awake()
    {
        Button btn = transform.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() => OnClick());
        }
    }

    public void OnClick()
    {

        //print ("UIButtonSwitchScreen");
        if (enabled && trigger == Trigger.OnClick)
        {
            UIManager.ToggleScreen(screen);
        }

    }

    void OnPress(bool isPressed)
    {

        if (enabled && isPressed && trigger == Trigger.OnPress)
        {
            UIManager.ToggleScreen(screen);
        }

    }

}

}
