namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MultiplayerLeagueDifficultyBehaviour : MonoBehaviour
{

    Button playButton;
    int selectedTeam = -1;
    int selectedDifficulty = -1;

    MultiplayerDifficultyInfoPanelBehaviour winInfoPanel;
    MultiplayerDifficultyInfoPanelBehaviour loseInfoPanel;


    GameObject easyToggle;
    GameObject normalToggle;

    Text easyRank;
    Text normalRank;
    Text hardRank;

    Text powerText;

    GameObject teamScrollView;
    Transform teamScrollViewContent;

    void Awake()
    {
        winInfoPanel = transform.Find("WinInfoPanel").GetComponent<MultiplayerDifficultyInfoPanelBehaviour>(); //WinInfoPanel
        loseInfoPanel = transform.Find("LoseInfoPanel").GetComponent<MultiplayerDifficultyInfoPanelBehaviour>(); //LoseInfoPanel


        easyToggle = transform.Find("DifficultyScrollView/Content/EasyTeamToggle").gameObject;
        normalToggle = transform.Find("DifficultyScrollView/Content/NormalTeamToggle").gameObject;

        teamScrollView = transform.Find("TeamScrollView").gameObject;
        teamScrollViewContent = transform.Find("TeamScrollView/Content");

        easyRank = transform.Find("DifficultyScrollView/Content/EasyTeamToggle/Off/RankText").gameObject.GetComponent<Text>();
        normalRank = transform.Find("DifficultyScrollView/Content/NormalTeamToggle/Off/RankText").gameObject.GetComponent<Text>();
        hardRank = transform.Find("DifficultyScrollView/Content/HardTeamToggle/Off/RankText").gameObject.GetComponent<Text>();

        powerText = transform.Find("PowerText").gameObject.GetComponent<Text>();


    }

    void OnEnable()
    {

        if (playButton == null)
        {
            Transform playButtonTransform = transform.Find("PlayButton");
            playButtonTransform.GetComponent<UIButtonSimpleDelegate>().buttonDelegate = OnSelectClick;
            playButton = playButtonTransform.GetComponent<Button>();
        }
        playButton.enabled = false;


        if (Startup.Initialized)
        {

            easyRank.text = "";
            normalRank.text = "";
            hardRank.text = "";

            powerText.text = MultiplayerManager.PermanentPowerRating.ToString();

            if (MultiplayerManager.NumGames < 4)
            {
                teamScrollView.SetActive(false);
            }
            else if (!teamScrollView.activeSelf)
            {
                teamScrollView.SetActive(true);
            }

            UIManager.SetScreenTimeout(GameScreenType.PopupMultiplayerLoading, 10);// w/ timeouting
            UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, true); //loading overlay

            /*
			 * ieládé 3 lígas spéĺu kandidátus
			 * MultiplayerManager.LeagueCandidateOpponents[0..2]
			 */
            MultiplayerManager.GetLeagueGameCandidates(delegate ()
            {

                //kad dati lejupieládéti - es kandidátiem izveidoju [sléptus] pretinieku panelíśus - lai tie savos Updeitos párbaudítu vai ir viss gatavs un piestartétu spéli

                foreach (Transform item in transform)
                {
                    if (item.name == "FriendGameEntry(Clone)")
                    {
                        Destroy(item.gameObject);
                    }
                }

                GameObject prefab = LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("FriendGameEntry") as GameObject;
                Debug.Log("<color=yellow>Prefab Loaded Name = </color>" + prefab);
                //GameObject prefab = Resources.Load("Prefabs/UI/FriendGameEntry") as GameObject;

                for (int j = 0; j < MultiplayerManager.LeagueCandidateOpponents.Length; j++)
                {
                    if (MultiplayerManager.LeagueCandidateOpponents[j] != null)
                    {
                        GameObject entry = Instantiate(prefab) as GameObject;

                        entry.transform.SetParent(transform);
                        entry.transform.localScale = Vector3.one;
                        entry.GetComponent<RectTransform>().localPosition = new Vector2(9999, 9999 + j * 100);

                        FriendGameEntryBehaviour script = entry.GetComponent<FriendGameEntryBehaviour>();
                        script.AutoPlay = true; //sáks spéli tiklídz bús lejupieládéta visa info ...
                        script.AutoDownload = false; //... bet tá tiks lejupieladéta tikai, kad spélétájs bús gatavs sákt spéli
                        script.SetData(MultiplayerManager.LeagueCandidateOpponents[j]); //iedod pretinieka ierakstu - poga pati sev saliks datus

                        UpdateOnce();

                        UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, false); //aizveru loading ekránu
                    }
                    else
                    {
                        if (Debug.isDebugBuild) { Debug.LogWarning(j + ". liigas pretinieks nav atrasts :\\"); }
                    }
                }

            });





            int i = 1; //komandas ir no 1-4, bez spélétája izvélétás komandas
            UIClickIndexDelegate cid;
            Transform teamGrid = teamScrollViewContent;//transform.FindChild("TeamScrollView/Content");
            foreach (Transform item in teamGrid)
            {
                cid = item.GetComponent<UIClickIndexDelegate>();
                if (cid != null)
                {
                    if (cid.index == MultiplayerManager.PlayerTeamID)
                    {
                        //					i++; //skipo spélétája komandu
                        item.gameObject.SetActive(false);
                    }
                    else
                    {
                        item.gameObject.SetActive(true);
                        //cid.index = i; //already set in editor
                        cid.indexDelegate = OnTeamCheckboxClick;
                        item.GetComponent<Toggle>().isOn = false;
                        //item.transform.FindChild("NameLabel").GetComponent<UILabel>().text = "team:"+i;
                    }
                    i++;
                }
            }

            if (Startup.Initialized)
            {
                //randomize teams
                Transform visibleChild;
                for (int j = 0; j < 3; j++)
                {
                    visibleChild = teamGrid.GetChild(1);
                    if (Random.value < 0.5)
                    {
                        visibleChild.SetAsFirstSibling();
                    }
                    else
                    {
                        visibleChild.SetAsLastSibling();
                    }
                }
                //
                //select the team in the middle
                Transform tmp;
                int k = 0;
                for (int j = 0; j < 4; j++)
                {
                    tmp = teamGrid.GetChild(j);

                    if (tmp.gameObject.activeSelf && k < 2)
                    {

                        if (k == 1)
                        { //if second visible, then must be in the center
                            cid = tmp.GetComponent<UIClickIndexDelegate>();
                            tmp.GetComponent<Toggle>().isOn = true;
                            selectedTeam = cid.index;
                        }
                        k++;
                    }
                }
            }

            UpdateOnce();//unlock button


            //hide Easy/Normal if MP level is high enough
            if (BikeDataManager.PlayerMultiplayerLevels[MultiplayerManager.MPLevel - 1].HideMedium)
            {
                normalToggle.SetActive(false);
                selectedDifficulty = 2;//select the hard difficulty by default (cos normal is disabled)
            }
            else
            {
                normalToggle.SetActive(true);
                selectedDifficulty = 1;//select the normal difficulty
            }


            if (BikeDataManager.PlayerMultiplayerLevels[MultiplayerManager.MPLevel - 1].HideEasy)
            {
                easyToggle.SetActive(false);
            }
            else
            {
                easyToggle.SetActive(true);
                selectedDifficulty = 0; //if easy is available - select it
            }




            //reset difficulties
            i = 0;
            Transform difficultyGrid = transform.Find("DifficultyScrollView/Content");
            foreach (Transform item in difficultyGrid)
            {
                cid = item.GetComponent<UIClickIndexDelegate>();
                if (cid != null)
                {
                    cid.index = i;
                    cid.indexDelegate = OnDifficultyCheckboxClick;
                    if (i == selectedDifficulty)
                    {
                        item.GetComponent<Toggle>().isOn = true;
                    }
                    else
                    {
                        item.GetComponent<Toggle>().isOn = false;
                    }
                    i++;
                }
            }

        }

    }


    void OnSelectClick()
    {
        UIManager.SetScreenTimeout(GameScreenType.PopupMultiplayerLoading, 10);// w/ timeouting
        UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, true); //loading overlay

        //print("selectedDifficulty==" + selectedDifficulty);
        //print("selectedTeam==" + selectedTeam);
        //ir lejupieládéti 3 kandidáti (katrai grútíbai), pareizajam lieku lejupieládét datus (tiklídz tas bús daríts - sáksies spéle, jo vińam ir autoplay uzsetots)
        MultiplayerManager.LeagueCandidateOpponents[selectedDifficulty].UIEntry.DownloadData();
        MultiplayerManager.LeagueCandidateOpponents[selectedDifficulty].TeamID = selectedTeam; //heh, heh: pasaku ka savlaicígi noskaidrotajam pretiniekam ir tieśi tá komanda, ko cilvéks nupat izvéléjies :D

    }

    void OnTeamCheckboxClick(int index)
    {
        selectedTeam = index;
        UpdateOnce();
    }

    void OnDifficultyCheckboxClick(int index)
    {
        selectedDifficulty = index;
        UpdateOnce();
    }

    void UpdateOnce()
    {

        if (Startup.Initialized && !playButton.enabled && selectedTeam > 0 && selectedDifficulty >= 0)
        {
            playButton.enabled = true;
        }

        if (Startup.Initialized && selectedDifficulty >= 0)
        {
            winInfoPanel.SetCoins(MultiplayerManager.CoinsPerWin);
            loseInfoPanel.SetCoins(0);

            if (MultiplayerManager.LeagueCandidateOpponents.Length > selectedDifficulty &&
                MultiplayerManager.LeagueCandidateOpponents[selectedDifficulty] != null)
            {
                winInfoPanel.SetCups(MultiplayerManager.LeagueCandidateOpponents[selectedDifficulty].CupsTrackWin);
                loseInfoPanel.SetCups(-MultiplayerManager.LeagueCandidateOpponents[selectedDifficulty].CupsTrackLose);
            }
            else if (Debug.isDebugBuild) { print("selected difficulty doesn't have data"); }
        }

        //PowerRating text on Easy/Med/hard buttons
        if (MultiplayerManager.LeagueCandidateOpponents[0] != null)
        {
            easyRank.text = MultiplayerManager.LeagueCandidateOpponents[0].PowerRating.ToString();
        }
        if (MultiplayerManager.LeagueCandidateOpponents[1] != null)
        {
            normalRank.text = MultiplayerManager.LeagueCandidateOpponents[1].PowerRating.ToString();
        }
        if (MultiplayerManager.LeagueCandidateOpponents[2] != null)
        {
            hardRank.text = MultiplayerManager.LeagueCandidateOpponents[2].PowerRating.ToString();
        }


    }
}

}
