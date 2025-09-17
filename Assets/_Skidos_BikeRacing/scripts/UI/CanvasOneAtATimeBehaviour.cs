namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CanvasOneAtATimeBehaviour : MonoBehaviour
{

    List<string> vipScreens;
    Dictionary<string, Transform> screens;
    public Transform lastDisplayedScreen;
    public string lastDisplayedScreenName;
    public Transform displayedScreen;
    public string displayedScreenName;

    bool removeDisabledScreensOnUpdate = false;

    List<string> garbageCollectionExceptions = new List<string>{
        "Game",
        "PreGame",
        "Crash",
        "MultiplayerGame",
        "MultiplayerPreGame", 
//        "PopupLoading"
    };

    public void Init()
    {
        //        print("CanvasBehaviour::Init");

        screens = new Dictionary<string, Transform>();
        vipScreens = new List<string>();
        foreach (Transform child in transform)
        {
            screens.Add(child.name, child);
            vipScreens.Add(child.name);

            child.gameObject.SetActive(true);//make sure to enable it if it is disabled

            if (child.name == "Garage")
            {
                child.GetComponent<GarageBehaviour>().Init();
            }

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

    void RemoveDisabledScreens(bool collectGarbage = true)
    {
        //print("RemoveDisabledScreens("+collectGarbage+")");

        if (screens.Count > 1)
        {
            List<Transform> toRemove = new List<Transform>();
            foreach (var item in screens)
            {
                if (!item.Value.gameObject.activeSelf)
                {
                    toRemove.Add(item.Value);
                }
            }

            foreach (var item in toRemove)
            {
                if (!vipScreens.Contains(item.name))
                {
                    screens.Remove(item.name);
                    if (Time.timeScale > 0)
                    {
                        iTween.Stop(item.gameObject, true);
                    }
                    Destroy(item.gameObject);
                }
            }

            if (collectGarbage)
                LevelManager.CleanUp();
        }
    }

    void LoadScreenByName(string screenName)
    {

        //TODO clean old ones out

        if (!screens.ContainsKey(screenName) && screenName != "Last")
        {
            //TODO make a list
            bool collectGarbage = ShouldCollectGarbageAfterScreen(screenName);

            RemoveDisabledScreens(collectGarbage);
            if (Time.timeScale > 0)
            {
                iTween.tweens.Clear();
            }

            Debug.Log("<color=yellow>Prefab is loading from = </color>" + screenName);

            GameObject prefab = LoadAddressable_Vasundhara.Instance.GetPrefab_Resources(screenName) as GameObject;

            Debug.Log("<color=yellow>Prefab Name = </color>" + prefab);
            //GameObject prefab = Resources.Load("Prefabs/UI/Screens/"+screenName) as GameObject;
            GameObject screen = Instantiate(prefab) as GameObject;
            screen.name = screenName;
            screen.transform.SetParent(transform);
            screen.transform.localPosition = prefab.transform.localPosition;
            screen.transform.localScale = prefab.transform.localScale;
            screen.GetComponent<RectTransform>().anchoredPosition = prefab.GetComponent<RectTransform>().anchoredPosition;
            screen.GetComponent<RectTransform>().sizeDelta = prefab.GetComponent<RectTransform>().sizeDelta;

            //            if (screenName == "Garage") {
            //                screen.GetComponent<GarageBehaviour>().Init();
            //            }
            if (screenName == "Achievements")
            {
                screen.GetComponent<AchievementsBehaviour>().Init();
            }

            //            screen.SetActive(false);
            //            screen.SetActive(true);

            if (screenName.ToLower().Contains("popup"))
            {
                screen.SetActive(false);
            }

            screen.transform.SetAsLastSibling();

            //only bring popups to top if the last loaded screen wasn't a popup
            if (!screenName.ToLower().Contains("popup"))
            {
                PutPopupsOnTop();
            }

            //            if(screens.Count > 1) {
            //                Transform tmp = null;
            //                oldestScreenName = "";
            //                foreach (var item in screens) {
            //                    if(tmp == null && !item.Value.gameObject.activeSelf) {
            //                        tmp = item.Value;
            //                        oldestScreenName = item.Key;
            //                    }
            //                }
            //                if(tmp != null) {
            //                    screens.Remove(oldestScreenName);
            //                    Destroy(tmp.gameObject);
            //                }
            //            }

            screens.Add(screenName, screen.transform);
        }
    }

    void PutPopupsOnTop()
    {
        foreach (var screenKVP in screens)
        {
            if (screenKVP.Key.ToLower().Contains("popup"))
            {
                screenKVP.Value.SetAsLastSibling();
            }
        }
    }

    public void SwitchScreenByName(string screenName)
    {
        if (Debug.isDebugBuild) { print("CanvasOneAtATimeBehaviour::SwitchScreenByName: " + screenName); }

        LoadScreenByName(screenName);

        Transform tmpScreen = null;

        if (screens.ContainsKey(screenName) && displayedScreen != screens[screenName])
        { //do not save screen as last if it's the same one
            tmpScreen = displayedScreen;
        }

        if (displayedScreen != null && displayedScreen.gameObject.activeSelf)
        {
            displayedScreen.gameObject.SetActive(false);
        }

        if (screens.ContainsKey(screenName))
        {
            displayedScreen = screens[screenName];
            displayedScreenName = screenName;
            if (!displayedScreen.gameObject.activeSelf)
                displayedScreen.gameObject.SetActive(true);
        }
        else
        {
            if (screenName == "Last")
            {
                //                displayedScreen = lastDisplayedScreen;
                //                lastDisplayedScreen = null;

                LoadScreenByName(lastDisplayedScreenName);
                displayedScreen = screens[lastDisplayedScreenName];
                displayedScreenName = lastDisplayedScreenName;
                lastDisplayedScreenName = "";

                if (!displayedScreen.gameObject.activeSelf)
                    displayedScreen.gameObject.SetActive(true);
            }
            else
            {
                if (Debug.isDebugBuild) { Debug.LogWarning("CanvasBehaviour::Unknown screen name " + screenName); }
            }
        }

        if (tmpScreen != null)
        {
            lastDisplayedScreen = tmpScreen;
            lastDisplayedScreenName = tmpScreen.name;
        }
        SoundManager.ChangeAmbienceForMenu();
        //RemoveDisabledScreens();
        removeDisabledScreensOnUpdate = true; //schedule RemoveDisabledScreens() to next update, if it's called rightaway it interferes with the click events that execute game commands
    }

    public void ShowScreenByName(string screenName)
    {
        if (Debug.isDebugBuild) { print("CanvasOneAtATimeBehaviour::ShowScreenByName: " + screenName); }
        LoadScreenByName(screenName);

        if (screens.ContainsKey(screenName))
        {
            screens[screenName].gameObject.SetActive(true);
        }
        else
        {
            if (Debug.isDebugBuild) { Debug.LogWarning("CanvasBehaviour::Unknown screen name " + screenName); }
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
            if (Debug.isDebugBuild) { Debug.LogWarning("CanvasBehaviour::Unknown screen name " + screenName); }
        }
        SoundManager.ChangeAmbienceForMenu();
    }

    public void ToggleScreenByName(string screenName)
    {
        if (Debug.isDebugBuild) { print("CanvasOneAtATimeBehaviour::ToggleScreenByName: " + screenName); }
        LoadScreenByName(screenName);

        if (screens.ContainsKey(screenName))
        {
            GameObject screen = screens[screenName].gameObject;
            screen.SetActive(!screen.activeSelf);

            if (screen.activeSelf)
            {
                if (screenName.ToLower().Contains("popup"))
                { //if a popup existed and is getting toggled bring it to front
                    if (Debug.isDebugBuild) { print("bring to front " + screenName); }
                    screen.transform.SetAsLastSibling();
                }
            }
        }
        else
        {
            if (Debug.isDebugBuild) { Debug.LogWarning("CanvasBehaviour::Unknown screen name " + screenName); }
        }
        SoundManager.ChangeAmbienceForMenu();
    }

    public Transform GetScreenByName(string screenName)
    {
        if (Debug.isDebugBuild) { print("CanvasOneAtATimeBehaviour::GetScreenByName: " + screenName); }
        //        LoadScreenByName(screenName);
        Transform screen = null;

        LoadScreenByName(screenName);

        if (screens.ContainsKey(screenName))
        {
            screen = screens[screenName];
        }
        else
        {
            if (Debug.isDebugBuild) { Debug.LogWarning("CanvasOneBehaviour::Unknown screen name " + screenName); }
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

        if (removeDisabledScreensOnUpdate)
        {

            bool collectGarbage = ShouldCollectGarbageAfterScreen(displayedScreenName);

            RemoveDisabledScreens(collectGarbage);
            removeDisabledScreensOnUpdate = false;
        }
    }

    //TODO make a list
    bool ShouldCollectGarbageAfterScreen(string screenName)
    {
        return !garbageCollectionExceptions.Contains(screenName);
    }

}

}
