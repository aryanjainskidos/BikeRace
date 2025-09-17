namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

//faux-static klase - nevienam nepieder, visi lieto
//
public enum GameScreenType
{
    Game = 0,
    Pause = 1,
    Levels = 2,
    Unzoomed = 3,
    PreGame = 4,
    PostGame = 5,
    PreGamePause = 6,
    Crash = 7,
    Menu = 8,
    Settings = 9,
    Garage = 10,
    Achievements = 11,
    Last = 12,
    PopupMultiplayerLoading = 13,

    MultiplayerPreGame = 14,
    MultiplayerPreGamePause = 15,
    MultiplayerGame = 16,
    MultiplayerGameReplay = 17,
    MultiplayerPause = 18,
    MultiplayerCrash = 19,
    MultiplayerPostGameFriend = 20,
    MultiplayerPostGameLeague = 21,
    MultiplayerPostGameRevanche = 22,
    MultiplayerPostGameReplay = 23,


    PopupUnlockLevels = 30,
    PopupPreGameLoading = 31,
    MultiplayerMenu = 32,
    MultiplayerLeagueSelection = 33,
    MultiplayerLeaderboards = 34,
    MultiplayerLeagueDifficulty = 35,
    MultiplayerLeagueResults = 36,
    PopupGenericError = 37,
    PopupUnlockBonusLevels = 38,
    PopupShop = 39,
    MultiplayerLevelInfo = 40,
    PopupMultiplayerLevelUp = 41,
    PopupLevelUp = 42,
    PopupRateUs = 43,
    PopupMultiplayerLeagueWon = 44,
    PopupInviteFriends = 45,
    PopupDownloadLatestVersion = 46,
    PopupLongLevel = 47,
    PopupNoBoosts = 48,
    PopupPromo = 49,
    PostGameLong = 50,
    PopupLoading = 51,
    PopupGiftCoin = 52,
    PopupGiftStyle = 53,
    PopupMultiplayerLocked = 54,
    PopupMultiplayerUnlocked = 55,
    PopupMultiplayerNewSeason = 56,
    Credits = 57,
    MultiplayerPreGameReplay = 58,
    PopupMultiplayerFriends = 59,
    //    PopupFastestBike = 60,
    //    PopupFreeCoins = 61,
    PopupSpinningWheel = 62,
    //    PopupInviteFriendsForPower = 63,
    //    PopupDoubleCoinWeekend = 64,
    PopupRestoreBackup = 65,
    //    PopupCollectStarsForPower = 66,

}

public class UIManager : MonoBehaviour
{

    static GameObject canvas;
    static CanvasBehaviour canvasBehaviour;
    static CanvasOneAtATimeBehaviour canvasOneBehaviour;
    static GameObject UIRoot;

    static Dictionary<GameScreenType, string> screenTypeToString;

    private delegate void CallAfterSwitchScreen();  //function to call after SwitchScreen method has switched screens (set in SwitchScreen)
    private static CallAfterSwitchScreen callAfterSwitchScreen;

    //static GameObject currentScreen;
    public static GameScreenType currentScreenType;

    //static GameObject lastScreen; // will need this for help menu
    public static GameScreenType lastScreenType;

    //    static MultiplayerPostGameBehaviour multiplayerPostGameBehaviour;
    //    static MultiplayerPostGameBehaviour multiplayerPostReplayBehaviour;
    //	static MultiplayerPostGameBehaviour multiplayerPostRevancheBehaviour;


    //śo izsauks globálais Startup skripts	
    public static void Init()
    {

        //map enum values to names
        screenTypeToString = new Dictionary<GameScreenType, string>();
        GameScreenType[] gameScreenTypeValues = (GameScreenType[])System.Enum.GetValues(typeof(GameScreenType));

        foreach (var item in gameScreenTypeValues)
        {
            screenTypeToString.Add(item, System.Enum.GetName(typeof(GameScreenType), item));
        }


        canvas = GameObject.Find("Canvas_game");
        if (canvas != null)
        {
            canvasBehaviour = canvas.GetComponent<CanvasBehaviour>();
            if (canvasBehaviour != null)
            {
                canvasBehaviour.Init();
                canvasBehaviour.GetScreenByName("Levels").GetComponent<LevelsBehaviour>().Init();
                canvasBehaviour.GetScreenByName("Garage").GetComponent<GarageBehaviour>().Init();
                canvasBehaviour.GetScreenByName("Achievements").GetComponent<AchievementsBehaviour>().Init();
            }
            else
            {
                canvasOneBehaviour = canvas.GetComponent<CanvasOneAtATimeBehaviour>();
                canvasOneBehaviour.Init();
            }
        }

        SwitchScreen(GameScreenType.Menu);
    }



    /**
	 * if forceToggle != null then forceToggle indicates whether to turn on or off selected screen
	 * "bool?" == nullable bool
	 */
    public static void ToggleScreen(GameScreenType screenType, bool? forceToggle = null)
    {

        if (canvasBehaviour != null)
        {
            if (forceToggle == null)
            { //normal toggling

                canvasBehaviour.ToggleScreenByName(screenTypeToString[screenType]);

            }
            else
            { //force show or force hide           

                if (forceToggle == true)
                {
                    canvasBehaviour.ShowScreenByName(screenTypeToString[screenType]);
                }
                else
                {
                    canvasBehaviour.HideScreenByName(screenTypeToString[screenType]);
                }
            }
        }
        else
        {
            if (forceToggle == null)
            { //normal toggling

                canvasOneBehaviour.ToggleScreenByName(screenTypeToString[screenType]);

            }
            else
            { //force show or force hide           

                if (forceToggle == true)
                {
                    canvasOneBehaviour.ShowScreenByName(screenTypeToString[screenType]);
                }
                else
                {
                    canvasOneBehaviour.HideScreenByName(screenTypeToString[screenType]);
                }
            }
        }

    }

    public static void SetScreenTimeout(GameScreenType screenType, float timeout)
    {

        Transform toggleScreen = (canvasBehaviour != null) ?
            canvasBehaviour.GetScreenByName(screenTypeToString[screenType]) :
                canvasOneBehaviour.GetScreenByName(screenTypeToString[screenType]);

        if (toggleScreen != null)
        {
            //TODO if more than one screen with timeout separate timeout logics into a separate script
            MultiplayerLoadingBehaviour mlb = toggleScreen.transform.GetComponent<MultiplayerLoadingBehaviour>();
            if (mlb != null)
            {
                mlb.timeout = timeout;
            }
        }
    }



    public static void SwitchScreen(GameScreenType screenName)
    {

#if UNITY_EDITOR
        // don't for a minute      
        BikeDataManager.Flush(); //in editor, save afer every screen 

#endif

        callAfterSwitchScreen = null;
        GameScreenType tmpScreenType = currentScreenType;


        if (tmpScreenType == GameScreenType.PostGame && screenName != GameScreenType.PreGame)
        {//also save in resetgame
            BikeGameManager.ExecuteCommand(GameCommand.SaveGameSP);
            BikeDataManager.Flush();
        }

        currentScreenType = screenName;
        //set the actual screen type of last as current BEFORE actually switching screens (OnEnable kicks in immediately after)
        if (screenName == GameScreenType.Last)
        {
            currentScreenType = lastScreenType;
        }

        lastScreenType = tmpScreenType;

        if (canvas != null)
        {
            if (canvasBehaviour != null)
            {
                canvasBehaviour.SwitchScreenByName(screenTypeToString[screenName]);
            }
            else
            {
                canvasOneBehaviour.SwitchScreenByName(screenTypeToString[screenName]);
            }
        }


        if (callAfterSwitchScreen != null)
        {
            callAfterSwitchScreen();
        }
    }

    public static void SwitchScreenTab(GameScreenType screenType, string tabName, string subTabName = "")
    {

        GameObject screen = (canvasBehaviour != null) ?
            canvasBehaviour.GetScreenByName(screenTypeToString[screenType]).gameObject :
                canvasOneBehaviour.GetScreenByName(screenTypeToString[screenType]).gameObject;

        if (screen != null)
        {
            TabSwitchBehaviour tsb = screen.GetComponent<TabSwitchBehaviour>();
            if (tsb != null)
            {
                tsb.Switch(tabName, subTabName);
            }
        }
    }

    public static void Reinit()
    {

        if (canvas != null)
        {
            if (canvasBehaviour != null)
            {
                canvasBehaviour.GetScreenByName("Levels").GetComponent<LevelsBehaviour>().Init();
                canvasBehaviour.GetScreenByName("Garage").GetComponent<GarageBehaviour>().Init();
            }
        }
    }



    public static Sprite GetLevelBadgeSprite(int level)
    {

        Sprite sprite = null;
        string spriteName = "";

        if (level > 9)
        { //platinum
            spriteName = "MP_Badge_Platinum";
        }
        else
        {
            if (level > 6)
            { //gold
                spriteName = "MP_Badge_Gold";
            }
            else
            {
                if (level > 3)
                { //silver
                    spriteName = "MP_Badge_Silver";
                }
                else
                {
                    if (level > 0)
                    { //bronze
                        spriteName = "MP_Badge_Bronze";
                    }
                    else
                    {
                        spriteName = "MP_Badge_GreyedOut";
                    }
                }
            }
        }

        if (spriteName != "")
        {
            //            sprite = Resources.Load<Sprite>("visuals/Sprites/GUI_sprites/" + spriteName);
            sprite = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/MP/MultiplayerEssentials", spriteName);
        }
        else print("sprite couldn't be found");

        return sprite;
    }

    public static Sprite GetLevelNumberSprite(int level, bool grey = false)
    {

        Sprite sprite = null;
        string spriteName = "";

        spriteName = "MP_Badge_lvl_" + level;
        if (grey)
            spriteName += "_bw";

        if (spriteName != "")
        {
            sprite = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/MP/MultiplayerEssentials", spriteName);
        }
        else print("sprite couldn't be found");

        return sprite;
    }

    public static Sprite GetBoostSprite(string value, bool threeD = true)
    {
        print("@not-being-used ???");
        Sprite sprite = null;
        string spriteName = "";

        switch (value)
        {
            case "ice":
                spriteName = "BoostIco_Freeze";
                break;
            case "magnet":
                spriteName = "BoostIco_Magnet";
                break;
            case "invincibility":
                spriteName = "BoostIco_Armor";
                break;
            case "fuel":
                spriteName = "BoostIco_Fuel";
                break;
            default:
                break;
        }

        if (threeD)
        {
            spriteName += "_3D";
        }

        if (spriteName != "")
        {
            if (!threeD)
            {
                sprite = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/GUI-ico", spriteName);
            }
            else
            {
                sprite = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/BoostIco3D/" + spriteName, spriteName);
            }
        }
        else print("sprite couldn't be found");

        return sprite;

    }

    public static Sprite GetRideSprite(string value)
    {
        Sprite sprite = null;
        string spriteName = "";

        switch (value)
        {
            case "coin":
                spriteName = "Coins";
                break;
            default:
                break;
        }

        if (spriteName != "")
        {
            sprite = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/RideEssentials", spriteName);
        }
        else print("sprite couldn't be found");

        return sprite;

    }

}

}
