namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIButtonSwitchScreen : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
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

    //for toggle and button, toggle doesn't have OnClick
    public void OnPointerClick(PointerEventData eventData)
    {

        //        Debug.Log("UIButtonSwitchScreen::Pointer Click: " + transform.name);
        OnClick();

    }

    public void OnPointerDown(PointerEventData eventData)
    {

        //        Debug.Log("UIButtonSwitchScreen::Pointer Press: " + transform.name);
        OnPress(true);

    }

    public void OnClick()
    {

        if (enabled && trigger == Trigger.OnClick)
        {
            //            Debug.Log("UIButtonSwitchScreen::Click: " + transform.name);
            ClickAction();
        }

    }

    void OnPress(bool isPressed)
    {

        if (enabled && isPressed && trigger == Trigger.OnPress)
        {
            //            Debug.Log("UIButtonSwitchScreen::Press: " + transform.name);
            ClickAction();
        }

    }

    private void ClickAction()
    {
        if (!enabled) return;

        if (screen == GameScreenType.Levels && UIManager.currentScreenType == GameScreenType.PostGame)
        {
            UIManager.SwitchScreen(screen);

                if (LevelManager.CurrentLevelName != "" && BikeDataManager.Levels.ContainsKey(LevelManager.CurrentLevelName))
                {
                    Debug.Log("If calledddd");
                    BikeDataManager.Levels[LevelManager.CurrentLevelName].Tried = true;
                }
                UIManager.SwitchScreen(GameScreenType.Levels);
                return;
        }

        UIManager.SwitchScreen(screen);



    }

}
}
