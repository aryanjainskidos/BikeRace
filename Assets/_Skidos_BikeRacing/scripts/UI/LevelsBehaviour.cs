namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class LevelsBehaviour : MonoBehaviour
{

    bool initialized = false;
    Dictionary<string, GameObject> buttons = new Dictionary<string, GameObject>();
    LevelDataContainer levelData;

    PlayerPointerBehaviour playerPointerBehaviour;

    Transform container;
    ScrollRect scrollRect;
    RectTransform scrollRectTransform;
    RectTransform contentRectTransform;
    RectTransform maskRectTransform;

    RectTransform lastLevelButton;

    string lastPlayedLevelName = "";

    bool aLevelWasLoaded;

    GameObject WoFPointer;
    Button WoFButton;

    void Awake()
    {
        //print("LevelsBehaviour::Awake");
        // Try to load from LoadAddressable_Vasundhara first, fallback to Resources
        if (LoadAddressable_Vasundhara.Instance != null)
        {
            levelData = LoadAddressable_Vasundhara.Instance.GetScriptableObjLevelData_Resources("LevelData");
        }
        
        // Fallback to Resources if LoadAddressable_Vasundhara failed or returned null
        if (levelData == null)
        {
            levelData = Resources.Load<LevelDataContainer>("Data/LevelData");
        }
        
        // If still null, create a default LevelDataContainer to prevent errors
        if (levelData == null)
        {
            Debug.LogWarning("LevelData not found in LoadAddressable_Vasundhara or Resources. Creating default LevelDataContainer.");
            levelData = ScriptableObject.CreateInstance<LevelDataContainer>();
            levelData.levelDataEntries = new System.Collections.Generic.List<LevelDataEntry>();
        }
        
        Debug.Log("<color=green>LevelData Loaded Successfully: </color>" + (levelData != null ? levelData.name : "null"));

        playerPointerBehaviour = transform.Find("ScrollView/Content/PlayerPointer").GetComponent<PlayerPointerBehaviour>(); //PlayerPointer

        container = transform.Find("ScrollView/Content");

        scrollRect = transform.Find("ScrollView").GetComponent<ScrollRect>();
        scrollRectTransform = scrollRect.transform as RectTransform;
        contentRectTransform = scrollRect.content;
        maskRectTransform = scrollRect.GetComponent<RectTransform>();//transform.FindChild("ScrollView").GetComponent<Mask>().rectTransform;

        WoFPointer = transform.Find("Pointer").gameObject;
        WoFButton = transform.Find("ButtonScrollView/Content/WheelButtonContainer/WheelButton").GetComponent<Button>();
    }

    public void Init()
    {

        if (levelData != null)
        {
            PopulateLevelList();
        }
        else
        {
            Debug.LogWarning("levelData == null");
        }

        initialized = true;
    }


    //only finds the buttons in scene
    void PopulateLevelList()
    {


        //Transform container = transform.Find("ScrollView/Content");
        buttons.Clear();

        foreach (KeyValuePair<string, LevelRecord> level in BikeDataManager.Levels)
        {
            Transform btnTR = container.Find(level.Key);

            if (btnTR == null)
            {
#if UNITY_EDITOR
                if (!level.Key.Contains("_mp_"))
                {
                    //Debug.LogWarning("Trases chuuskaa nav atrasts liimenis: " + level.Key); //súdzás, ka nav ir límenis, kam nav podzinja trases chúská (ignoréjot MP límenjus)
                    //@BTW -- trases chúská esośajám pogám ir jásaucás límenja várdá un skriptá ir jábút norádítam límenja várdam
                }
#endif
            }
            else
            {


                buttons.Add(level.Key, btnTR.gameObject);
            }

        }

    }


    public void Actualize()
    {
        if (initialized)
        {
            GameObject levelButtonObject;
            bool previousRegularLevelCleared = true;
            bool previousLevelTried = true;

            //Match match;
            LevelButtonType type;

            GameObject nextLevelButton = null;


            foreach (KeyValuePair<string, LevelRecord> level in BikeDataManager.Levels)
            {

                if (buttons.TryGetValue(level.Key, out levelButtonObject))
                { //not all levels have buttons

                    LevelButtonBehaviour levelButtonBehaviour = levelButtonObject.GetComponent<LevelButtonBehaviour>();
                    LevelLongButtonBehaviour levelLongButtonBehaviour = levelButtonObject.GetComponent<LevelLongButtonBehaviour>();

                    type = (levelButtonBehaviour != null) ? levelButtonBehaviour.type : levelLongButtonBehaviour.type;


                    UIButtonLevelCommand buttonLevelCommand = levelButtonObject.GetComponent<UIButtonLevelCommand>(); //TODO remove this
                    buttonLevelCommand.mode = LevelButtonMode.play;


                    //by default level is unlocked when previous regular one has been played (some restrictions on LONG levels later)
                    if (!previousRegularLevelCleared && !BikeGameManager.developmentMode && !BikeDataManager.PaidLevelUnlock)
                    {
                        buttonLevelCommand.mode = LevelButtonMode.locked;
                    }
                    else
                    {
                        buttonLevelCommand.mode = LevelButtonMode.play;
                    }

                    bool looksUnlocked = previousRegularLevelCleared || BikeDataManager.PaidLevelUnlock;

                    if (type != LevelButtonType.Long)
                    {
                        levelButtonBehaviour.SetState(looksUnlocked ? LevelButtonState.Unlocked : LevelButtonState.Locked);
                        levelButtonBehaviour.SetStars(level.Value.BestStars, level.Value.Tried);


                        if (type == LevelButtonType.Bonus)
                        { //bonus

                            //if previous level is not played - bonus level is locked
                            if (!previousRegularLevelCleared && !BikeGameManager.developmentMode && !BikeDataManager.PaidLevelUnlock)
                            {
                                buttonLevelCommand.mode = LevelButtonMode.locked;
                                //print("BONUSS: " + level.Key + "  LOCKED-PLAY-MORE");
                            }
                            else
                            { //prev level is played - check if he has liked-n-shared for this level                 
                                if (level.Value.Shared && !BikeGameManager.developmentMode)
                                {
                                    buttonLevelCommand.mode = LevelButtonMode.play;
                                    //print("BONUSS: " + level.Key + "  PLAY");
                                }
                                else
                                {
                                    buttonLevelCommand.mode = LevelButtonMode.lockedShare;
                                    //print("BONUSS: " + level.Key + "  LOCKED-LIKE-N-SHARE");
                                }
                            }

                        }

                        //check for explicitly locked levels (unlockable with stars) regular levels only
                        for (int i = 0; i < levelData.levelDataEntries.Count; i++)
                        {
                            if (levelData.levelDataEntries[i].name == level.Key)
                            { //current level is marked as locked
                                if (levelData.levelDataEntries[i].starsToUnlock > BikeDataManager.Stars && !BikeDataManager.PaidLevelUnlock)
                                { // is still locked  | @todo -- add MP win check
                                    if (previousRegularLevelCleared)
                                    { //only if the level is next to unlocked one

                                        buttonLevelCommand.mode = LevelButtonMode.lockedWorld;
                                        buttonLevelCommand.stars = levelData.levelDataEntries[i].starsToUnlock;

                                        levelButtonBehaviour.SetState(LevelButtonState.Locked);
                                        //                                    levelButtonObject.transform.FindChild("Unlocked").gameObject.SetActive(false); //no unique look, get a ui plane
                                        //                                  levelButtonObject.transform.FindChild("Locked").gameObject.SetActive(true); //also turn on LOCKED pic, for that unique look
                                    }
                                }
                            }
                        }
                    }

                    if (type == LevelButtonType.Long)
                    { //long
                        levelLongButtonBehaviour.SetState(looksUnlocked ? LevelButtonState.Unlocked : LevelButtonState.Locked);
                        levelLongButtonBehaviour.SetCheckpoints(level.Value.BestCheckpoints);

                        if (!previousRegularLevelCleared && !BikeGameManager.developmentMode && !BikeDataManager.PaidLevelUnlock)
                        {
                            buttonLevelCommand.mode = LevelButtonMode.locked;
                        }
                        else
                        {
                            buttonLevelCommand.mode = LevelButtonMode.popupLong; //mandatory popup bedore playing LONG level
                        }
                    }


                    //                if (previousRegularLevelCleared) { //TODO if previous is bonus and it's not cleared stay there
                    if (previousRegularLevelCleared && previousLevelTried && !level.Value.Tried)
                    { //TODO if previous is bonus and it's not cleared stay there
                        nextLevelButton = levelButtonObject;
                    }

                    //TODO if level tried but not finished still put a flag on it
                    if (previousRegularLevelCleared && previousLevelTried && level.Value.Tried && level.Value.BestTime == 0)
                    {
                        nextLevelButton = levelButtonObject;
                    }

                    //if regular and no stars - not cleared (skip bonus and long)
                    //                if (level.Value.BestStars < 1 && type == LevelButtonType.Regular) {
                    //                    if (level.Value.BestTime == 0 && type == LevelButtonType.Regular) { //everybody is a winner
                    if (!level.Value.Tried && type == LevelButtonType.Regular)
                    { //everybody is even more of a winner
                        previousRegularLevelCleared = false;
                    }

                    previousLevelTried = level.Value.Tried;

                }
            }

            //reposition the flag
            if (nextLevelButton != null)
            {
                    Debug.Log("next level button name = "+ nextLevelButton.name);
                Vector2 lvlBtnAncPos = nextLevelButton.GetComponent<RectTransform>().anchoredPosition;
                float angle = nextLevelButton.GetComponent<LevelButtonBehaviour>() != null ?
                    nextLevelButton.GetComponent<LevelButtonBehaviour>().flagAngle :
                        nextLevelButton.GetComponent<LevelLongButtonBehaviour>().flagAngle;

                if (Debug.isDebugBuild)
                {
                    print(nextLevelButton.name + " " + angle + " " + lvlBtnAncPos);
                }

                    Debug.Log("sending pos = " + lvlBtnAncPos);
                    //                if (lvlBtnAncPos != playerPointerBehaviour.anchoredPosition) {
                    //                    print(angle + " " + lvlBtnAncPos);
                    playerPointerBehaviour.SetAngle(angle);
                playerPointerBehaviour.SetPosition(lvlBtnAncPos, BikeGameManager.lastLoadedLevelName != "");
                playerPointerBehaviour.clickDelegate = nextLevelButton.GetComponent<UIButtonLevelCommand>().OnClick;

                //this is a callback in response to the pointer finishing moving on level 22long
                //TODO should get executed after the animation is done
                //                if (nextLevelButton.name == "a___022LONG" && DataManager.FirstLong) {
                //                    GameManager.SelectedLevelName = "a___022LONG";
                //                    UIManager.ToggleScreen(GameScreenType.PopupLongLevel);
                //                    DataManager.FirstLong = false;
                //                }
                //                }

                if (BikeGameManager.lastLoadedLevelName == "a___0155" && !BikeDataManager.CoinsGifted)
                {
                    UIManager.ToggleScreen(GameScreenType.PopupGiftCoin);
                    BikeDataManager.CoinsGifted = true;
                }

                if (BikeGameManager.lastLoadedLevelName == "a___006" && !BikeDataManager.FastestBikeShowed)
                {
                    //                    UIManager.ToggleScreen(GameScreenType.PopupFastestBike);
                    BikeDataManager.FastestBikeShowed = true;
                }

                //                if(GameManager.lastLoadedLevelName == "a___007" && !DataManager.StyleGifted && DataManager.Styles[DataManager.GiftStyleIndex].Locked){
                //                    UIManager.ToggleScreen(GameScreenType.PopupGiftStyle);
                //                    DataManager.StyleGifted = true;
                //                }

                if (BikeGameManager.lastLoadedLevelName == "a___0144" && !BikeDataManager.MultiplayerUnlockedPopupShowed)
                {
                    // UIManager.ToggleScreen(GameScreenType.PopupMultiplayerUnlocked);
                    BikeDataManager.MultiplayerUnlockedPopupShowed = true;
                    BikeDataManager.ShowMultiplayerButtonNotification = true;
                }

                if (BikeGameManager.lastLoadedLevelName == "a___0284" && !BikeDataManager.MultiplayerUnlockedPopupShowed2)
                {
                    // UIManager.ToggleScreen(GameScreenType.PopupMultiplayerUnlocked);
                    BikeDataManager.MultiplayerUnlockedPopupShowed2 = true;
                    BikeDataManager.ShowMultiplayerButtonNotification = true;
                }

                if (BikeGameManager.lastLoadedLevelName == "a___0384" && !BikeDataManager.MultiplayerUnlockedPopupShowed3)
                {
                    // UIManager.ToggleScreen(GameScreenType.PopupMultiplayerUnlocked);
                    BikeDataManager.MultiplayerUnlockedPopupShowed3 = true;
                    BikeDataManager.ShowMultiplayerButtonNotification = true;
                }

                if (BikeGameManager.lastLoadedLevelName == "a___0484" && !BikeDataManager.MultiplayerUnlockedPopupShowed4)
                {
                    // UIManager.ToggleScreen(GameScreenType.PopupMultiplayerUnlocked);
                    BikeDataManager.MultiplayerUnlockedPopupShowed4 = true;
                    BikeDataManager.ShowMultiplayerButtonNotification = true;
                }

                int giftID = BikeDataManager.GetLevelGiftID(BikeGameManager.lastLoadedLevelName);
                if (giftID > -1 && BikeDataManager.LevelGifts.ContainsKey(giftID))
                {
                    if (!BikeDataManager.LevelGifts[giftID].Gifted)
                    {
                        BikeGameManager.ExecuteCommand(GameCommand.GiveFreeStyle);
                        UIManager.ToggleScreen(GameScreenType.PopupGiftStyle);
                        BikeDataManager.LevelGifts[giftID].Gifted = true;
                    }
                }
            }

            //get the button on which to center
            //if at least one level was loaded previously and the level was tried before

            //            print("!" + GameManager.lastLoadedLevelName + " " + GameManager.lastLoadedLevelTried);
            if (BikeGameManager.lastLoadedLevelName != "" && BikeGameManager.lastLoadedLevelTried)
            {

                //                print("!!" + GameManager.lastLoadedLevelName + " " + GameManager.lastLoadedLevelTried + " " + aLevelWasLoaded);
                //                print("!!!buttons.ContainsKey(GameManager.lastLoadedLevelName) " + buttons.ContainsKey(GameManager.lastLoadedLevelName));
                if (buttons.ContainsKey(BikeGameManager.lastLoadedLevelName) //if a button for this level exists
                    && aLevelWasLoaded // and finished playing a level just now
                   )
                {
                    //center on this last played level 
                    //                    print("!! from a level " + LevelManager.CurrentLevelName);
                    lastPlayedLevelName = BikeGameManager.lastLoadedLevelName;
                    lastLevelButton = buttons[BikeGameManager.lastLoadedLevelName].GetComponent<RectTransform>();
                }
                else
                {
                    lastLevelButton = null;
                }
            }
            else
            {
                //if a level hasn't been played yet(just launched game) or the loaded level wasn't tried before
                if (nextLevelButton != null && //if the flag button is set
                    lastPlayedLevelName != nextLevelButton.name //and it's not te same button that was selected last time (don't snap if already snaped to this button once)
                   )
                {
                    //center on the flag button
                    //                    print("!! first time" + nextLevelButton.name);
                    lastPlayedLevelName = nextLevelButton.name;
                    lastLevelButton = nextLevelButton.GetComponent<RectTransform>();
                }
                else
                {
                    lastLevelButton = null;
                }
            }
        }
    }

    void Start()
    {
        if (Startup.Initialized && !initialized)
        {
            Init();
            OnEnable();
        }
    }

    void OnEnable()
    {
        if (Startup.Initialized && !initialized)
        {
            Init();
        }


        //testa zińa, izvákt
        //NewsListManager.Push("Atveert MP draugus [" + Random.Range(11,99) + "]", NewsListItemType.mpFriends, GameScreenType.MultiplayerMenu,"Friends");

        if (Startup.Initialized && initialized)
        {

            //            //ignore if already rated
            //            //first show if player xp lvl is at least 2 and hasn't been shown before or
            //            //second show after 25 lvl if the player hasn't rated yet
            //            if(!DataManager.HasRatedUs && (
            //                (DataManager.RateUsShowCount == 0 && DataManager.PlayerXPLevel > 1) || 
            //                (DataManager.RateUsShowCount == 1 && LevelManager.CurrentLevelName == "a___025")
            //             )){ //ir bijis vismaz 2 LVL-up (sák no nulltá límeńa) && neesam vél vinju aicinájuśi novértét spéli
            //				//DataManager.HasRatedUs = true; //pat, ja aizver popupu un neveŕté -- neprasísim nekad vairs
            //                print("OnEnable()");
            //				DataManager.RateUsShowCount++;
            //				UIManager.ToggleScreen(GameScreenType.PopupRateUs);
            //
            //			} else {
            //				//ja vien nav tikko parádíts RATE US
            //				PopupPromoBehaviour.ShowPromoIfScheduled(); //rádís reklámpopupu, ja vajag
            //			}

            aLevelWasLoaded = BikeGameManager.initialized;


            Actualize();

            //TODO if came from game screen, go to the last played level
            //TODO if first time after app start, go to the last open level
            if (lastLevelButton != null)
            {

                CenterOnItem(lastLevelButton);
            }

            //            if (DataManager.Levels["a___002"].Tried && !DataManager.ShowedWoFPointers) { // && !DataManager.Levels["a___003"].Tried
            //                WoFPointer.SetActive(true);
            //                WoFButton.onClick.AddListener(OnWoFClick);
            //            } else {
            //                WoFPointer.SetActive(false);
            //            }
            WoFPointer.SetActive(false);

        }
    }

    void Update()
    {
        BoostManager.FarmingUpdate();
    }

    void OnWoFClick()
    {
        if (WoFPointer.activeSelf)
        {
            WoFPointer.SetActive(false);
        }
        WoFButton.onClick.RemoveListener(OnWoFClick);
    }

    #region scroll rect horizontal item snapping methods
    public float scrollRectVerticalNormalizedPosition;
    public void CenterOnItem(RectTransform target)
    {
        //        print("CenterOnItem " + target.name + " " + activePresetIndex);
        // Item is here
        Vector3 itemCenterPositionInScroll = GetWorldPointInWidget(scrollRectTransform, GetWidgetWorldPoint(target));
        // But must be here
        Vector3 targetPositionInScroll = GetWorldPointInWidget(scrollRectTransform, GetWidgetWorldPoint(maskRectTransform));
        // So it has to move this distance
        float differenceY = targetPositionInScroll.y - itemCenterPositionInScroll.y;

        float normalizedDifferenceY = differenceY / (contentRectTransform.rect.size.y - scrollRectTransform.rect.size.y);
        float newNormalizedYPosition = scrollRect.verticalNormalizedPosition - normalizedDifferenceY;

        if (scrollRect.movementType != ScrollRect.MovementType.Unrestricted)
        {
            newNormalizedYPosition = Mathf.Clamp01(newNormalizedYPosition);
        }

        scrollRect.verticalNormalizedPosition = newNormalizedYPosition;

        scrollRectVerticalNormalizedPosition = scrollRect.verticalNormalizedPosition;
    }

    Vector3 GetWidgetWorldPoint(RectTransform target)
    {
        //pivot position + item size has to be included
        var pivotOffset = new Vector3(
            (0.5f - target.pivot.x) * target.rect.size.x,
            (0.5f - target.pivot.y) * target.rect.size.y,
            0f);
        var localPosition = target.localPosition + pivotOffset;
        return target.parent.TransformPoint(localPosition);
    }

    Vector3 GetWorldPointInWidget(RectTransform target, Vector3 worldPoint)
    {
        return target.InverseTransformPoint(worldPoint);
    }
    #endregion
}
}
