namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CanvasBehaviour : MonoBehaviour
{

    Dictionary<string, Transform> screens;
    public Transform lastDisplayedScreen;
    public Transform displayedScreen;

    public void Init()
    {
        //        print("CanvasBehaviour::Init");

        screens = new Dictionary<string, Transform>();
        foreach (Transform child in transform)
        {
            screens.Add(child.name, child);
            child.gameObject.SetActive(true);//make sure to enable it if it is disabled
            child.gameObject.SetActive(false);

            if (displayedScreen == null)
            {
                displayedScreen = child;
            }

            if (child.name == "Debug")
            { //always enable "Debug" container
                child.gameObject.SetActive(true);
            }
        }
    }

    public void SwitchScreenByName(string screenName)
    {

        Transform tmpScreen = null;

        if (screens.ContainsKey(screenName) && displayedScreen != screens[screenName])
        { //do not save screen as last if it's the same one
            tmpScreen = displayedScreen;
        }

        if (displayedScreen != null)
        {
            displayedScreen.gameObject.SetActive(false);
        }

        if (screens.ContainsKey(screenName))
        {
            displayedScreen = screens[screenName];
            displayedScreen.gameObject.SetActive(true);
        }
        else
        {
            if (screenName == "Last")
            {
                displayedScreen = lastDisplayedScreen;
                displayedScreen.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("CanvasBehaviour::Unknown screen name " + screenName);
            }
        }

        if (tmpScreen != null)
        {
            lastDisplayedScreen = tmpScreen;
        }
        SoundManager.ChangeAmbienceForMenu();
    }

    public void ShowScreenByName(string screenName)
    {

        if (screens.ContainsKey(screenName))
        {
            screens[screenName].gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("CanvasBehaviour::Unknown screen name " + screenName);
        }
        SoundManager.ChangeAmbienceForMenu();
    }

    public void HideScreenByName(string screenName)
    {

        if (screens.ContainsKey(screenName))
        {
            screens[screenName].gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("CanvasBehaviour::Unknown screen name " + screenName);
        }
        SoundManager.ChangeAmbienceForMenu();
    }

    public void ToggleScreenByName(string screenName)
    {

        if (screens.ContainsKey(screenName))
        {
            GameObject screen = screens[screenName].gameObject;
            screen.SetActive(!screen.activeSelf);
        }
        else
        {
            Debug.LogWarning("CanvasBehaviour::Unknown screen name " + screenName);
        }
        SoundManager.ChangeAmbienceForMenu();
    }

    public Transform GetScreenByName(string screenName)
    {

        Transform screen = null;

        if (screens.ContainsKey(screenName))
        {
            screen = screens[screenName];
        }
        else
        {
            Debug.LogWarning("CanvasBehaviour::Unknown screen name " + screenName);
        }

        return screen;

    }

    public GameScreenType gst = GameScreenType.Menu;
    void Update()
    {
        if (gst != UIManager.currentScreenType)
        {
            gst = UIManager.currentScreenType;
        }
    }

}
}
