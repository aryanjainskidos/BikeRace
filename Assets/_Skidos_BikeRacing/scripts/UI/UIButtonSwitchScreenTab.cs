namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIButtonSwitchScreenTab : MonoBehaviour
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
    public string tab;
    public string subTab = "";

    //automatically add OnClick to unity ui button listeners
    void Awake()
    {
        Button btn = transform.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() => OnClick());
        }
    }

    void OnClick()
    {
        if (trigger == Trigger.OnClick)
        {
            UIManager.SwitchScreen(screen); //in the scene that loads screens ar runtime this handler is executed before UIButtonSwitchScreen click handler and messes up everything
            UIManager.SwitchScreenTab(screen, tab, subTab);
        }
    }

    void OnPress(bool isPressed)
    {

        if (isPressed && trigger == Trigger.OnPress)
        {
            UIManager.SwitchScreen(screen);
            UIManager.SwitchScreenTab(screen, tab, subTab);
        }

    }

}

}
