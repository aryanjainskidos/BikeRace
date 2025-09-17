namespace vasundharabikeracing
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections;
    using SimpleJSON;

    public class MultiplayerLeaderboardsBehaviour : MonoBehaviour
    {

        System.DateTime lastGlobal = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
        System.DateTime lastLocal = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
        System.DateTime lastLeague = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
        System.DateTime lastFriends = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

        Text globalPositionText;
        Text localPositionText;
        Text leaguePositionText;
        Text friendsPositionText;

        GameObject globalPanel;
        GameObject localPanel;
        GameObject leaguePanel;
        GameObject friendsPanel;

        GameObject playerEntryPrefab;
        GameObject entryListGOGlobal;
        GameObject entryListGOLocal;
        GameObject entryListGOLeague;
        GameObject entryListGOFriends;

        Toggle globalToggle;
        Toggle localToggle;
        Toggle leagueToggle;
        Toggle friendsToggle;

        GameObject friendsEntriesPanel;
        GameObject logInButton;

        GameObject loadingImage;

        // Use this for initialization
        void Awake()
        {

            globalToggle = transform.Find("GlobalToggle").GetComponent<Toggle>();
            localToggle = transform.Find("LocalToggle").GetComponent<Toggle>();
            leagueToggle = transform.Find("LeagueToggle").GetComponent<Toggle>();
            friendsToggle = transform.Find("FriendsToggle").GetComponent<Toggle>();

            globalPositionText = transform.Find("GlobalPanel/YourPositionPanel/PositionText").GetComponent<Text>();
            localPositionText = transform.Find("LocalPanel/YourPositionPanel/PositionText").GetComponent<Text>();
            leaguePositionText = transform.Find("LeaguePanel/YourPositionPanel/PositionText").GetComponent<Text>();
            friendsPositionText = transform.Find("FriendsPanel/YourPositionPanel/PositionText").GetComponent<Text>();

            globalPanel = transform.Find("GlobalPanel").gameObject;
            localPanel = transform.Find("LocalPanel").gameObject;
            leaguePanel = transform.Find("LeaguePanel").gameObject;
            friendsPanel = transform.Find("FriendsPanel").gameObject;

            globalPanel.GetComponent<UIEnableDelegate>().enableDelegate = EnableLeaderboardTab;
            localPanel.GetComponent<UIEnableDelegate>().enableDelegate = EnableLeaderboardTab;
            leaguePanel.GetComponent<UIEnableDelegate>().enableDelegate = EnableLeaderboardTab;
            friendsPanel.GetComponent<UIEnableDelegate>().enableDelegate = EnableLeaderboardTab;

            playerEntryPrefab = LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("LeaderboardEntry") as GameObject;
            Debug.Log("<color=yellow>Prefab Loaded Name = </color>" + playerEntryPrefab);
            //playerEntryPrefab = Resources.Load("Prefabs/UI/LeaderboardEntry") as GameObject;

            entryListGOGlobal = globalPanel.transform.Find("LeaderboardEntriesPanel/ScrollView/Content").gameObject;
            entryListGOLocal = localPanel.transform.Find("LeaderboardEntriesPanel/ScrollView/Content").gameObject;
            entryListGOLeague = leaguePanel.transform.Find("LeaderboardEntriesPanel/ScrollView/Content").gameObject;
            entryListGOFriends = friendsPanel.transform.Find("LeaderboardEntriesPanel/ScrollView/Content").gameObject;

            friendsEntriesPanel = friendsPanel.transform.Find("LeaderboardEntriesPanel").gameObject;
            logInButton = friendsPanel.transform.Find("LogInButton").gameObject;

            loadingImage = transform.Find("LoadingImage").gameObject;

            iTween.RotateBy(loadingImage, iTween.Hash("z", -1.0f, "looptype", iTween.LoopType.loop, "easetype", iTween.EaseType.linear));
        }



        void OnEnable()
        {
            if (!globalToggle.isOn && globalPanel.activeSelf)
            {
                globalPanel.SetActive(false);
            }

            if (!localToggle.isOn && localPanel.activeSelf)
            {
                localPanel.SetActive(false);
            }

            if (!leagueToggle.isOn && leaguePanel.activeSelf)
            {
                leaguePanel.SetActive(false);
            }

            if (!friendsToggle.isOn && friendsPanel.activeSelf)
            {
                friendsPanel.SetActive(false);
            }
            //print("MultiplayerLeaderboardsBehaviour::Enabled");
        }

        void OnDisable()
        {
            //print("MultiplayerLeaderboardsBehaviour::Disabled");
        }

        void Update()
        {
            if (friendsPanel.activeSelf && logInButton.activeSelf && MultiplayerManager.HasFB)
            {
                friendsEntriesPanel.SetActive(true);
                logInButton.SetActive(false);
                EnableLeaderboardTab();
            }
        }

        public void EnableLeaderboardTab()
        {

            string type = "";
            GameObject list = null;
            System.DateTime lastTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

            if (globalPanel.activeSelf)
            {
                type = "global";
                list = entryListGOGlobal;
                lastTime = lastGlobal;
            }
            if (localPanel.activeSelf)
            {
                type = "local";
                list = entryListGOLocal;
                lastTime = lastLocal;
            }
            if (leaguePanel.activeSelf)
            {
                type = "league";
                list = entryListGOLeague;
                lastTime = lastLeague;
            }
            if (friendsPanel.activeSelf)
            {
                type = "friends";
                list = entryListGOFriends;
                lastTime = lastFriends;

                if (!MultiplayerManager.HasFB)
                {
                    friendsEntriesPanel.SetActive(false);
                    logInButton.SetActive(true);
                }
                else
                {
                    friendsEntriesPanel.SetActive(true);
                    logInButton.SetActive(false);
                }
            }

            if (list == null)
            {
                Debug.LogError("wrong leaderboard type");
                return;
            }

            list.transform.parent.GetComponent<ScrollViewMoveToTopBehaviour>().forceRecalculate = true;

            //print("Leaderboard tab:" + type  + " is active and we are loading data");

            System.TimeSpan diff = System.DateTime.Now.ToUniversalTime() - lastTime.ToUniversalTime();
            if (diff.TotalSeconds < 60)
            { //ja vél nav pagájuśas X sekundes - neprasa serverim atkártoti
                if (Debug.isDebugBuild) { print("Not reloading " + type + " leaderboard (only " + (diff.TotalSeconds) + " sec passed)"); }
                return;
            }

            //atvértam tabam pieraksta, kad pédéjoreiz atjaunoti dati
            if (globalPanel.activeSelf)
            {
                lastGlobal = System.DateTime.Now;
            }
            if (localPanel.activeSelf)
            {
                lastLocal = System.DateTime.Now;
            }
            if (leaguePanel.activeSelf)
            {
                lastLeague = System.DateTime.Now;
            }
            if (friendsPanel.activeSelf)
            {
                if (!MultiplayerManager.HasFB)
                {
                    if (Debug.isDebugBuild) { print("no FB - no friends"); }
                    return;
                }
                lastFriends = System.DateTime.Now;
            }




            if (Debug.isDebugBuild) { print("get leaderboard data: " + type); }


            //iztíra iepriekśéjo sarakstu
            foreach (Transform child in list.transform)
            {
                Destroy(child.gameObject);
            }


            WWWForm data = new WWWForm();
            data.AddField("Type", type);


            loadingImage.SetActive(true);

            StartCoroutine(MultiplayerManager.MPRequest("get_leaderboard", data, delegate (WWW www)
            {
            //vélreiz iztíra iepriekśéjo sarakstu
            foreach (Transform child in list.transform)
                {
                    Destroy(child.gameObject);
                }
            //@todo -- aizvákt loading śtruntu

            if (Debug.isDebugBuild) { print("get_leaderboard++ " + www.text); }
                try
                {
                    JSONNode N = JSON.Parse(www.text);


                    for (int i = 0; i < N["friends"].AsArray.Count; i++)
                    {

                        GameObject playerEntry;

                        playerEntry = Instantiate(playerEntryPrefab) as GameObject;
                        playerEntry.layer = 5;
                    //playerEntry.layer = LayerMask.NameToLayer("UI");
                    playerEntry.transform.SetParent(list.transform);
                        playerEntry.transform.localScale = Vector3.one;
                        playerEntry.GetComponent<RectTransform>().localPosition = Vector3.zero;

                        LeaderboardEntryBehaviour leb = playerEntry.GetComponent<LeaderboardEntryBehaviour>();

                        leb.SetName(N["friends"][i]["name"]);
                        leb.SetTeam(N["friends"][i]["team_id"].AsInt);
                        leb.SetCups(N["friends"][i]["cups"].AsInt);
                        leb.SetRank(MultiplayerManager.CalculatePowerRating(N["friends"][i]["player_upgrades"].AsIntArray)); //rank, lol, petiesíbá power rating, ko apréḱinás péc JSONá atrastajiem upgreidiem
                    leb.SetPosition(N["friends"][i]["rank"].AsInt);
                        leb.SetPicture(N["friends"][i]["picture"], N["friends"][i]["fbid"]); //sák lejupieládét bildi
                    leb.ShowHighlight(false);

                        if (N["friends"][i]["me"].AsInt == 1)
                        {
                            string yourPos = Lang.Get("Multiplayer:Top:YourPosition:") + " " + N["friends"][i]["rank"].AsInt.ToString();
                            if (type == "global")
                            {
                                globalPositionText.text = yourPos;
                            }
                            if (type == "local")
                            {
                                localPositionText.text = yourPos;
                            }
                            if (type == "league")
                            {
                                leaguePositionText.text = yourPos;
                            }
                            if (type == "friends")
                            {
                                friendsPositionText.text = yourPos;
                            }
                            leb.ShowHighlight(true);
                        }

                    }
                    loadingImage.SetActive(false);

                //scroll everything to top
                //				var pointerEventData = new PointerEventData(EventSystem.current);
                //				ExecuteEvents.Execute(list.transform.parent.gameObject, pointerEventData, ExecuteEvents.scrollHandler);
                list.transform.parent.GetComponent<ScrollViewMoveToTopBehaviour>().forceRecalculate = true;

                }
                catch (System.Exception e)
                {
                    if (Debug.isDebugBuild) { print("j) Wrong JSON:" + e.Message); }

                    MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
                //lai ráda erroru (pat ja ir loading)
                UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, true);
                    UIManager.SetScreenTimeout(GameScreenType.PopupMultiplayerLoading, 0);
                }

            }, delegate (WWW www)
            {
                if (Debug.isDebugBuild) { print("get_leaderboard-- " + www.text); }
                if (www.error.ToString().Contains("Internal Server Error"))
                    MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
                else if (www.error.ToString().Contains("404 Not Found"))
                    MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailBadConnection");
                else
                    MultiplayerLoadingBehaviour.ErrorMessage = Lang.Get("UI:MultiplayerPopupLoading:FailNoInternet");
            }));

        }

    }

}
