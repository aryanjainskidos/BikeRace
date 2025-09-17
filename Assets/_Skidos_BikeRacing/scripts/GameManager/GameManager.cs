namespace vasundharabikeracing
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using System.Text.RegularExpressions;
    // using CandyCoded.HapticFeedback;
    // using GameAnalyticsSDK;
    //using SimpleJSON;
    // using Facebook.Unity;

    /**
     * faux-static klase (visas metodes ir statiskas) nepieder nevieam geimobjektam
     * pieskata moça braucienu śajá límení (tiek noresetots pie límeńa ieládes)
     */

    public enum GameCommand
    { //@note -- explicit numbering prevents UnityUI form messing up if new entries are inserted in the middle
        TogglePause = 0,
        Reset = 1,
        ToggleUnzoom = 2,
        PauseOff = 3,
        PauseOn = 4,
        ResetPreGame = 5,
        None = 6,
        ToggleSingleplayerGhost = 7,
        BulletTimeOn = 8,
        BulletTimeOff = 9,
        ToggleDevelopmentMode = 10,
        ResetPlayerData = 11,
        HardReset = 12,
        ToggleSettingsMusic = 13,
        ToggleSettingsSfx = 14,
        ToggleSettingsHD = 15,
        SettingsResetTeams = 16,
        SettingsGoToSupportPage = 17,
        SettingsFBLogOut = 18,
        ShareFB = 19,
        ShareTwitter = 20,
        Rate = 21,
        DownloadLatestVersion = 22,
        UnlockAllTracks = 23,
        PlaySpecificLevel = 24,
        OpenPopupPromo = 25,
        KillBike = 26,
        FBLogin = 27,
        SaveGameSP = 28,
        GiveFreeCoins = 29,
        GiveFreeStyle = 30,
        MultiplayerQuit = 31,
        OpenFBPage = 32,
        KillLevel = 33,
        ToggleSettingsControl = 34,
        SettingsRestorePurchases = 35,
        ShowAdTestScreen = 36,
        ShareVK = 37,
        SkidosMain = 38,
    }


    public class BikeGameManager : MonoBehaviour
    {

        public static bool initialized = false;

        public static bool developmentMode = false; //changable in settings; not saved in DataManager
        public static bool singlePlayer; //how are AI bike treated - as opponent or just a replay | also will achi be collected
        public static int multiPlayerRestarts; // how many are left
        public static int singlePlayerRestarts = -1; // how many are left

        public static string mpLastSaveGameName;
        public static byte[] mpLastSaveGameData;

        public static bool longLevel;

        public static bool gamePaused = false;

        public static GameCommand lastCommand;

        public static Bounds levelBounds; //perhaps this should be in the level info?

        public static GameObject[] ais;
        public static List<BikeStateData> aiStates;
        public static List<Transform> aiRiders;
        public static List<Transform> aiRiderOriginals;
        public static List<BikeInputFile> aiInputs;
        public static List<BikeControl> aiControls;

        public static GameObject player;
        public static BikeStateData playerState;
        public static BikeInputDevice playerInput;
        public static BikeControl playerControl;

        public static Transform playerRider;
        public static Transform playerRiderOriginal;
        public static Transform playerRagdoll;

        public static GameObject entityTrigger;
        public static GameObject magnetTrigger;
        public static GameObject ragdollEntityTrigger;

        static GameObject crash;

        static GameObject[] glass;
        static GameObject[] radialBombs;
        static GameObject[] fakeBombs;
        static GameObject[] bombPlanks;
        static GameObject[] checkpointPoles;
        static GameObject[] checkpointZones;
        static GameObject[] birds;
        static GameObject[] birdZones;
        static GameObject[] puddles;
        static GameObject[] leafPiles;

        public static Timer timer;
        public static float TimeElapsed
        {
            get { return timer != null ? timer.TimeElapsed : 0; }
        }

        //	public static int StarsEarned; //calculated in AwardPlayer() after each race

        static Material bubbleMaterial;
        static Material zMaterial;

        public static LevelInfo levelInfo;

        static GameObject start;
        static Vector3 startPos;

        static int lastCheckpointID = -1;
        static int startCheckpointID = -1;
        public static int bestCheckpoints = 0;
        public static int checkpointsReached = 0;
        static List<int> reachedCheckpointIDs;

        static DirtBehaviour dirtBehaviourBack;
        static DirtBehaviour dirtBehaviourFront;
        static ExhaustBehaviour exhaustBehaviour;

        public static string SelectedLevelName = ""; //uzsed to pass info to gameCommand

        public static bool reloadBikeAtRestart = false;

        public static Queue<GameCommand> commandQueue = new Queue<GameCommand>();

        public static string lastLoadedLevelName = "";
        public static bool lastLoadedLevelTried = true;


        //śo izsauc LevelLoad metode
        public static void Init(bool _singlePlayer)
        {

            //Debug.Log("GameManager::Init");
            initialized = true;

            commandQueue = new Queue<GameCommand>();

            reloadBikeAtRestart = false;

            singlePlayer = _singlePlayer;
            multiPlayerRestarts = 3;

            startCheckpointID = lastCheckpointID = -1;
            checkpointsReached = 0;
            reachedCheckpointIDs = new List<int>();

            timer = GameObject.Find("LevelContainer").GetComponent<Timer>();

            Transform geocontainer = GameObject.Find("LevelContainer").transform.Find("GeoContainer");// First find a center for your bounds.
            levelBounds = calculateBounds(geocontainer);

            levelInfo = GameObject.Find("LevelContainer").GetComponent<LevelInfo>();
            Debug.Log($"[GameManager] Found LevelInfo: {levelInfo != null}");
            if (levelInfo != null)
            {
                Debug.Log($"[GameManager] LevelName: '{levelInfo.LevelName}'");
            }
            
            longLevel = (BikeGameManager.levelInfo != null && !string.IsNullOrEmpty(BikeGameManager.levelInfo.LevelName))
    ? BikeGameManager.levelInfo.LevelName.ToLower().Contains("long")
    : false;

            // Check if levelInfo is null or LevelName is null/empty
            if (levelInfo == null)
            {
                Debug.LogError("❌ LevelInfo component not found on LevelContainer!");
                return;
            }
            
            if (string.IsNullOrEmpty(levelInfo.LevelName))
            {
                Debug.LogError("❌ LevelInfo.LevelName is null or empty! This should have been set during level deserialization.");
                return;
            }

            lastLoadedLevelName = levelInfo.LevelName;
          if (!BikeDataManager.Levels.TryGetValue(levelInfo.LevelName, out var levelData))
{
    Debug.LogWarning($"⚠️ Level '{levelInfo.LevelName}' was not found in BikeDataManager.Levels — auto-registering default.");
    levelData = new LevelRecord(); // default data
    BikeDataManager.Levels[levelInfo.LevelName] = levelData;
}
lastLoadedLevelTried = levelData.Tried;


            //        DataManager.Levels[levelInfo.LevelName].Tried = true;//if the level has been loaded at least once, then mark it as tried

            singlePlayerRestarts = levelInfo.AllowedRestartCount + (levelInfo.AllowedRestartCount > 0 ? 1 : 0);

            start = GameObject.FindWithTag("StartZone");

            startPos = start.transform.position;
            startPos.z = -3;
            start.transform.position = startPos;

            //		if(player == null){
            //			return;
            //		}

            InitPlayer();

            if (aiRiderOriginals != null)
            {
                for (int i = 0; i < aiRiderOriginals.Count; i++)
                {
                    if (aiRiderOriginals[i] != null)
                    {
                        Destroy(aiRiderOriginals[i].gameObject); //delete previous unneeded AI bike gameobjects
                    }
                }
            }

            ais = GameObject.FindGameObjectsWithTag("AI"); //empty array if no AIs found
            aiStates = new List<BikeStateData>();
            aiRiders = new List<Transform>();
            aiRiderOriginals = new List<Transform>();
            aiInputs = new List<BikeInputFile>();
            aiControls = new List<BikeControl>();
            for (int i = 0; i < ais.Length; i++)
            {
                aiStates.Add(ais[i].GetComponent<BikeStateData>());

                aiRiders.Add(ais[i].transform.Find("Player_parts"));

                Transform rider = Instantiate(
                    aiRiders[i],
                    aiRiders[i].transform.localPosition,
                    aiRiders[i].transform.localRotation
                    ) as Transform;
                aiRiderOriginals.Add(rider);

                aiRiderOriginals[i].transform.parent = aiRiders[i].parent.parent;
                ShowChildren(aiRiderOriginals[i], false);

                aiRiderOriginals[i].GetComponent<RiderAnimationControl>().enabled = false;
                aiRiderOriginals[i].GetComponent<Animator>().enabled = false;
                aiInputs.Add(ais[i].GetComponent<BikeInputFile>());
                aiControls.Add(ais[i].GetComponent<BikeControl>());
            }

            if (player != null && playerRiderOriginal != null)
            {
                crash = (GameObject)Instantiate(LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("CrashObj"));
                Debug.Log("<color=yellow>Prefab Loaded Name = </color>" + crash);
                //crash = (GameObject)Instantiate(Resources.Load("Prefabs/AnimatedObjects/Crash"));
                crash.transform.parent = playerRiderOriginal.transform.parent.parent;
            }
            else if (player != null && playerRiderOriginal == null)
            {
                Debug.LogError("BikeGameManager: playerRiderOriginal is null, cannot create crash object");
            }
            //		ShowChildren(crash.transform, false);

            glass = GameObject.FindGameObjectsWithTag("Glass");
            radialBombs = GameObject.FindGameObjectsWithTag("BombRadial");
            fakeBombs = GameObject.FindGameObjectsWithTag("BombFake");
            bombPlanks = GameObject.FindGameObjectsWithTag("BombPlank");
            checkpointPoles = GameObject.FindGameObjectsWithTag("CheckpointPole");
            checkpointZones = GameObject.FindGameObjectsWithTag("CheckpointZone");
            puddles = GameObject.FindGameObjectsWithTag("Puddle");
            leafPiles = GameObject.FindGameObjectsWithTag("LeafPile");
            birds = GameObject.FindGameObjectsWithTag("Bird");
            birdZones = GameObject.FindGameObjectsWithTag("BirdZone");

            //reset boosts before reseting game
            BoostManager.ResetBoosts();
            selectedBoosts.Clear();

            PauseGameSet(true);
            ResetGame();

            //		MatchPreToLevelInfo.Init ();//let the post game screen find the level container, so that it can update according to it´s data
            //		MatchPostToLevelInfo.Init ();//let the post game screen find the level container, so that it can update according to it´s data

            //bubbleMaterial = (Material)Resources.Load("visuals/Materials/particle_Bubble", typeof(Material));
            //zMaterial = (Material)Resources.Load("visuals/Materials/particle_Z", typeof(Material));

            bubbleMaterial = (Material)LoadAddressable_Vasundhara.Instance.GetMaterial_Resources("particle_Bubble");
            zMaterial = (Material)LoadAddressable_Vasundhara.Instance.GetMaterial_Resources("particle_Z");

            Debug.Log("<color=yellow>Mat Loaded Name = </color>" + bubbleMaterial);
            Debug.Log("<color=yellow>Mat Loaded Name = </color>" + zMaterial);
            //        Camera.main.GetComponent<BikeCamera>().SetTarget(ais[0]);


            foreach (var boost in BikeDataManager.Boosts)
            {

                if (boost.Value.Active)
                {
                    BoostManager.DeactivateBoost(boost.Key);
                }
                boost.Value.Active = false;
                boost.Value.Selected = false;
            }

            //        if (levelInfo.type == "long") {
            PickupManager.ResetPickups();
            //        }

            if (playerControl != null)
            {

                Match match = Regex.Match(levelInfo.LevelName.ToLower(), "(a_multiplayer|a_mp|mp_test)");

                if (!singlePlayer || match.Success) //TODO levelInfo.LevelName only for testing the mp upgrade values in sp
                {

                    int upgradeLevel;
                    foreach (var upgradeKeyValuePair in BikeDataManager.Upgrades)
                    {
                        UpgradeRecord upgrade = upgradeKeyValuePair.Value;
                        if (upgrade.Availability == UpgradeAvailabilityType.MultiplayerOnly)
                        {

                            upgradeLevel = BikeDataManager.Bikes[BikeDataManager.MultiplayerPlayerBikeRecordName].Upgrades[upgradeKeyValuePair.Key]; // getting the total upgrades

                            switch ((UpgradeType)upgradeKeyValuePair.Key)
                            {
                                case UpgradeType.Acceleration:
                                    playerControl.accelerationBoostCoef = upgrade.Values[upgradeLevel];
                                    break;
                                case UpgradeType.AccelerationStart:
                                    playerControl.accelerationStartBoostCoef = upgrade.Values[upgradeLevel];
                                    break;
                                case UpgradeType.AdditionalRestarts:
                                    multiPlayerRestarts += (int)upgrade.Values[upgradeLevel];
                                    break;
                                case UpgradeType.BreakSpeed:
                                    playerControl.brakeForceBoostCoef = upgrade.Values[upgradeLevel];
                                    break;
                                case UpgradeType.MaxSpeed:
                                    playerControl.maxVelocityXBoostCoef = upgrade.Values[upgradeLevel];
                                    break;
                                default:
                                    print("Upgrade undefined");
                                    break;
                            }
                        }
                    }
                    //            
                }
                else
                {
                    //TODO deactivate multiplayer upgrades
                }
            }

            //assign ids to checkpoints, may be more appropriate in level manager
            GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("CheckpointZone");

            for (int i = 0; i < checkpoints.Length; i++)
            {
                checkpoints[i].GetComponent<PSCheckpoint>().id = i;
            }


        }

        private static void InitPlayer()
        {

            player = GameObject.FindWithTag("Player");
            Debug.Log($"Finding player - brr -> {player == null} {player.gameObject.name}");
            if (player != null)
            {

                playerState = player.GetComponent<BikeStateData>();
                playerInput = player.GetComponent<BikeInputDevice>();
                playerControl = player.GetComponent<BikeControl>();

                playerRider = player.transform.Find("Player_parts");
                
                if (playerRider == null)
                {
                    Debug.LogError("BikeGameManager: Player_parts not found in player transform!");
                    return;
                }

                if (playerRiderOriginal != null)
                {
                    Destroy(playerRiderOriginal.gameObject);
                }

                playerRiderOriginal = Instantiate(
                    playerRider,
                    playerRider.transform.localPosition,
                    playerRider.transform.localRotation
                    ) as Transform;

                playerRiderOriginal.transform.parent = playerRider.parent.parent;
                ShowChildren(playerRiderOriginal, false);

                if (playerRiderOriginal.GetComponent<RiderAnimationControl>() != null)
                {
                    playerRiderOriginal.GetComponent<RiderAnimationControl>().enabled = false;
                }
                else print("no RiderAnimationControl");

                dirtBehaviourBack = player.transform.Find("wheel_back").GetComponent<DirtBehaviour>();
                dirtBehaviourFront = player.transform.Find("wheel_front").GetComponent<DirtBehaviour>();

                ExhaustBehaviour eb = player.transform.GetComponentInChildren<ExhaustBehaviour>();
                //            if(player.transform.FindChild("ExhaustSmoke")) {
                //                exhaustBehaviour = player.transform.FindChild("ExhaustSmoke").GetComponent<ExhaustBehaviour>();
                if (eb != null)
                {
                    exhaustBehaviour = eb;
                    exhaustBehaviour.TurnOff();
                }
                else print("no ExhaustSmoke");

                styleBoosts = BikeDataManager.Styles[BikeDataManager.Bikes[BikeDataManager.SingleplayerPlayerBikeRecordName].StyleID].Boosts;//save it until the bike gets reloaded, otherwise bike stays, boosts change
            }
        }

        private static void DeinitPlayer()
        {

            if (player != null)
            {
                DestroyImmediate(player.transform.parent.gameObject);
            }

            playerState = null;
            playerInput = null;
            playerControl = null;

            if (playerRider != null)
            {
                DestroyImmediate(playerRider.gameObject);
            }
            //        playerRider = player.transform.FindChild ("Player_parts");

            if (playerRiderOriginal != null)
            {
                DestroyImmediate(playerRiderOriginal.gameObject);
            }

            dirtBehaviourBack = null;
            dirtBehaviourFront = null;
        }

        public static Bounds calculateBounds(Transform t)
        {

            Vector3 center = Vector3.zero;

            center = calculateCenter(t);

            Bounds bounds = new Bounds(center, Vector3.zero);
            Bounds tmpBounds;

            foreach (Transform child in t)
            {
                if (child.childCount > 0)
                {
                    tmpBounds = calculateBounds(child);
                    if (tmpBounds.size != Vector3.zero)
                    {
                        bounds.Encapsulate(tmpBounds);
                    }

                }
                else if (child.gameObject.GetComponent<Renderer>() != null)
                {
                    bounds.Encapsulate(child.gameObject.GetComponent<Renderer>().bounds);
                }
                else if (child.GetComponent<Collider2D>() != null)
                {
                    bounds.Encapsulate(child.GetComponent<Collider2D>().bounds);
                }
            }

            return bounds;
        }

        static Vector3 calculateCenter(Transform t)
        {

            int countChildren = 0;
            Vector3 center = Vector3.zero;
            Vector3 tmpCenter;

            foreach (Transform child in t)
            {
                if (child.childCount > 0)
                {

                    tmpCenter = calculateCenter(child);

                    if (tmpCenter != Vector3.zero)
                    {
                        center += tmpCenter;
                        countChildren++;
                    }

                }
                else if (child.gameObject.GetComponent<Renderer>() != null)
                {
                    center += child.gameObject.GetComponent<Renderer>().bounds.center;
                    countChildren++;
                }
                else if (child.GetComponent<Collider2D>() != null)
                {
                    center += child.GetComponent<Collider2D>().bounds.center;
                    countChildren++;
                }
            }

            if (countChildren > 0)
            {
                center /= countChildren; //center is average center of children
            }

            return center;

        }

        static List<string> selectedBoosts = new List<string>();
        public static List<string> styleBoosts;

        //on GameScreen Enable
        public static void StartGame()
        { //List<string> selectedBoosts = null
          //        BoostManager.ResetBoosts();

            foreach (var boost in BikeDataManager.Boosts)
            {

                //check if can activate boost
                if (boost.Value.Selected)
                { //this boost was selected
                  //                print(boost.Key + " selected, activating" + boost.Value.Active);
                    if (boost.Value.Number > 0 && !boost.Value.Active)
                    { // it's not active, enable
                        BoostManager.ActivateBoost(boost.Key);
                        boost.Value.Number--;

                        AchievementManager.AchievementProgress("boost_tried", 1);
                    } // if was selected and active do nothing, the case when returning from pause

                }
                else
                {
                    //                print(boost.Key + " not selected, deactivating");
                    BoostManager.DisableBoostEffect(boost.Key); //wasn't selected, disable(actually shouldn't be enabled)
                    if (boost.Value.Active)
                    { //can't be active and deselected
                        BoostManager.DeactivateBoost(boost.Key);
                    }
                }

            }

            //        List<string> styleBoosts = DataManager.Styles[DataManager.Bikes[DataManager.SingleplayerPlayerBikeRecordName].StyleID].Boosts;
            if (styleBoosts != null)
            {
                foreach (var item in styleBoosts)
                {
                    BoostManager.ActivateBoost(item, false);
                }
            }

        }

        //called after level load and after every restart
        public static void ResetGame()
        {
            //print("ResetGame");
            Debug.LogError("Method called __>");

            SoundManager.StopSfx(); //aptur visas skanjas skanjas, daźreiz vajag


            if (reloadBikeAtRestart)
            {
                print("RELOAD THE WHOLE BIKE...");
                DeinitPlayer();
                BikeManager.LoadBike(true, null, BikeDataManager.SingleplayerPlayerBikeRecordName);
                InitPlayer();
                reloadBikeAtRestart = false;
                if (player.transform.parent.Find("magnet_trigger") != null)
                {
                    magnetTrigger = player.transform.parent.Find("magnet_trigger").gameObject;
                }
            }

            if (initialized)
            {
                if (singlePlayer)
                {
                    if (levelInfo.type != "long")
                    {
                        PickupManager.ResetPickups();
                    }
                    else
                    {
                        //long level additional reset
                        if (singlePlayerRestarts == 0 || playerState.finished)
                        {
                            Debug.Log("reset checkpoints");

                            SaveGameSP();

                            PickupManager.ResetPickups();
                            start.transform.position = startPos;
                            singlePlayerRestarts = levelInfo.AllowedRestartCount;
                            startCheckpointID = lastCheckpointID = -1;

                            checkpointsReached = 0;
                            reachedCheckpointIDs = new List<int>();


                            //reset the pole animations
                            foreach (GameObject cp in checkpointPoles)
                            {
                                cp.GetComponent<CheckpointPoleBehaviour>().Reset();
                            }
                            foreach (GameObject cz in checkpointZones)
                            {
                                cz.GetComponent<PSCheckpoint>().Reset();
                            }

                            //                        string pidData = DataManager.UnzipFileToString(Application.persistentDataPath + "/sp-rides/" + LevelManager.CurrentLevelName + ".bytes");

                            //                        string pidData = DataManager.LoadLevelString(LevelManager.CurrentLevelName);
                            //                        aiInputs[0].SetData(pidData); //set this ride as input for AI bike

                            BikeDataManager.LoadLevelGhost(LevelManager.CurrentLevelName);
                            aiInputs[0].SetData(BikeDataManager.GetGhostRecordInputJSON()); //set this ride as input for AI bike
                        }
                        else
                        {

                            if (levelInfo.AllowedRestartCount >= 0 && singlePlayerRestarts > 0)
                            {
                                singlePlayerRestarts--;

                                if (Debug.isDebugBuild)
                                {
                                    print("lastCheckpointID: " + lastCheckpointID + "  startCheckpointID: " + startCheckpointID);
                                }

                                SaveGameSP(); //if there was a restart try saving data

                                //reset boosts
                                PickupManager.ResetBoostPickups();

                                if (lastCheckpointID >= 0 && lastCheckpointID != startCheckpointID)
                                { //if at least one checkpoint reached and it's not the one player started from, load appropriate ghost data

                                    //load data from file

                                    //                                string pidData = DataManager.UnzipFileToString(Application.persistentDataPath + "/sp-rides/" + LevelManager.CurrentLevelName + "_cp_" + lastCheckpointID + ".bytes");

                                    //                                string pidData = DataManager.LoadLevelString(LevelManager.CurrentLevelName + "_cp_" + lastCheckpointID);
                                    //                                
                                    //                                print("ResetGame" + Application.persistentDataPath + "/sp-rides/" + LevelManager.CurrentLevelName + "_cp_" + lastCheckpointID + ".bytes =>" + pidData.Length);
                                    if (Debug.isDebugBuild)
                                    {
                                        print("ResetGame" + Application.persistentDataPath + "/sp-rides/" + LevelManager.CurrentLevelName + "_cp_" + lastCheckpointID + ".bytes =>");
                                    }

                                    BikeDataManager.LoadLevelGhost(LevelManager.CurrentLevelName + "_cp_" + lastCheckpointID);
                                    //                                
                                    aiInputs[0].ClearData(); //clear data, there could be no file, but there is no need to use old data
                                                             //                                aiInputs[0].SetData(pidData); //set this ride as input for AI bike
                                    aiInputs[0].SetData(BikeDataManager.GetGhostRecordInputJSON()); //set this ride as input for AI bike
                                                                                                    //                                
                                    startCheckpointID = lastCheckpointID;
                                }
                            }

                        }
                    }

                    Match match = Regex.Match(levelInfo.LevelName.ToLower(), "(a_multiplayer|a_mp|mp_test)");
                    if (!match.Success)
                    {//TODO levelInfo.LevelName only for testing the mp upgrade values in sp, only reset boosts in usual sp layers
                     //deactivate all boosts, that weren't selected
                        foreach (var boost in BikeDataManager.Boosts)
                        {
                            if (boost.Value.Active)
                            {
                                //                            print(boost.Key + " Active");
                                BoostManager.DeactivateBoost(boost.Key); //reactivate boosts after reset

                                if (boost.Value.Selected && boost.Value.Number != 0)
                                {
                                    BoostManager.EnableBoostEffect(boost.Key);
                                }
                            }

                        }
                    }

                }

                if (start != null)
                {
                    Debug.LogError("--------------------------------------__>");
                    if (player != null)
                    {

                        ResetMainCamera();

                        ResetTriggers();

                        //TODO save input if necessary
                        //also upon exiting PostGame screen
                        if (levelInfo.type != "long")
                        {
                            SaveGameSP();
                        }
                        //save end

                        playerInput.Reset();//first reset input, because it writes to file on reset
                        playerInput.passInputToControl = true;

                        playerState.Reset(); //DONE reset BikeStateData
                        playerControl.enabled = false;

                        player.transform.position = start.transform.position;//starta linijas pozícija un rotácija (bet ne izmérs)
                        player.transform.rotation = start.transform.rotation;
                        player.GetComponent<BikeResetForces>().Reset();//noreseto baika spékus un átrumu
                                                                       //TODO reset positions for bike children

                        playerControl.Reset();

                        dirtBehaviourBack.Reset();
                        dirtBehaviourFront.Reset();

                        //                    exhaustBehaviour.TurnOff();
                        if (exhaustBehaviour != null)
                            exhaustBehaviour.Reset();

                        ResetRider();

                        ResetRagdoll();

                        ResetInteractiveObjects();

                        ShowChildren(crash.transform, false);

                        ResetWaterZones();


                    }
                    else
                    {

                        if (ais.Length > 1)
                        {

                            if (Camera.main != null)
                            {
                                BikeCamera camScript = Camera.main.GetComponent<BikeCamera>();
                                if (camScript != null)
                                {
                                    camScript.SetTarget(ais[0]);
                                    camScript.SetSecondaryTarget(ais[1]);
                                    //                            camScript.Start(); 
                                }
                                else
                                {
                                    Debug.LogError("Galvenajai kameria nav \"BikeCamera\" skripta!");
                                }
                            }
                            else
                            {
                                Debug.LogError("Nav kameras, ko nu ?");
                            }

                        }
                        else
                        {
                            Debug.LogError("Nav baika, ko nolikt uz starta línijas");
                        }

                    }

                    ResetFinishFlag();

                }
                else
                {
                    Debug.LogError("Nav atrasts starta línijas prefabs");
                }


                for (int i = 0; i < ais.Length; i++)
                {
                    aiInputs[i].Reset();
                    aiStates[i].Reset();
                    aiControls[i].Reset();
                    ais[i].transform.position = start.transform.position;//starta linijas pozícija un rotácija (bet ne izmérs)
                    ais[i].transform.rotation = start.transform.rotation;

                    ResetAIRiders();

                    if (aiInputs[i].totalFrameCount <= 0)
                    {
                        ShowChildren(ais[i].transform, false);
                    }
                }

                if (ais.Length > 0 && singlePlayer)
                {
                    if (BikeDataManager.SettingsSPGhost && aiInputs[0].totalFrameCount > 0)
                    { //save previous state somewhere? 
                        ShowChildren(ais[0].transform, true);
                    }
                    else
                    {
                        ShowChildren(ais[0].transform, false);
                    }
                }

                if (timer != null)
                {
                    timer.TimerStart();
                }
            }
        }

        public static void SaveGameSP()
        {
            //print("SaveGameSP");

            if (playerInput != null && singlePlayer && BikeDataManager.SettingsSPGhost)
            { //singleplayer: save ride-file for each level and only if better than last ride (attached to AI bike)

                //if reset from a static function
                if (!playerState.finished && playerState.dead)
                {
                    playerInput.Log();
                }

                if (ais.Length == 0)
                {
                    Debug.LogWarning("Nevar saglabaat SP braucienu - nav SP ghostinja!");
                    return;
                }

                bool playerCrashedBeforeFinish = !playerState.finished && playerState.dead && aiInputs[0].finishEventFrame < 0; //and AI didn´t finish
                bool playerCrashedLaterThanAI = playerInput.wheelRotationAtCrash > aiInputs[0].wheelRotationAtCrash;
                bool playerFinishedBeforeAI = playerInput.finishEventFrame < aiInputs[0].finishEventFrame || aiInputs[0].finishEventFrame < 0;

                if ((playerCrashedBeforeFinish && playerCrashedLaterThanAI) || (playerState.finished && playerFinishedBeforeAI))
                {

                    ////////////////////////////////////////////////////////////
                    //DONE save colors and upgrade progress on each ride save //
                    ////////////////////////////////////////////////////////////
                    if (Debug.isDebugBuild)
                    {
                        print("SaveGameSP: lastCheckpointID: " + lastCheckpointID + "  startCheckpointID: " + startCheckpointID);
                    }

                    string checkpointPostfix = startCheckpointID < 0 ? "" : "_cp_" + startCheckpointID;

                    //                DataManager.SaveLevelJSON(playerInput.GetDataJSON(), LevelManager.CurrentLevelName + checkpointPostfix);

                    //new way of saving
                    BikeDataManager.SetPlayerRecordInputJSON(playerInput.GetDataJSON());
                    BikeDataManager.SaveLevelGhost(LevelManager.CurrentLevelName + checkpointPostfix);
                    //

                    if (singlePlayer && aiInputs[0].totalFrameCount <= 0 && BikeDataManager.SettingsSPGhost)
                    {
                        ShowChildren(ais[0].transform, true);
                    }

                    aiInputs[0].SetData(playerInput.GetDataJSON());//set this ride as input for AI bike //one parsing operation less than using a string

                }
                else
                {
                    if (!playerState.finished)
                    {
                        //print("didn´t finish");
                    }
                    else
                    {
                        if (Debug.isDebugBuild)
                        {
                            print("took too long to finish " + playerInput.finishEventFrame.ToString() + " " + aiInputs[0].finishEventFrame.ToString());
                        }
                    }
                }

            }
        }

        /**
         * Saglabá MP spéles ride-failu ípaśajá direktoríjá
         * GameManager objektá ieliek pédéjá seivotá MP faila nosaukumu un sazipotos datus
         */
        public static void SaveGameMP()
        {
            //Debug.Log("SaveGame::MP");

            if (!MultiplayerManager.HasFB)
            {
                Debug.LogError("hmm, nav MP");
                return;
            }

            string pidData = playerInput.GetDataString();
            // mpLastSaveGameName = AccessToken.CurrentAccessToken.UserId.ToString() + "_" + System.DateTime.UtcNow.Ticks.ToString();
            //        DataManager.ZipStringToFile(pidData, Application.persistentDataPath + "/mp-cache/" + mpLastSaveGameName + ".bytes" );		//, true, out mpLastSaveGameData
            print("saved: " + mpLastSaveGameName + " data.len: " + pidData.Length);

            BikeDataManager.SetPlayerRecordInputJSON(playerInput.GetDataJSON(), false);
            BikeDataManager.SaveLevelGhost(mpLastSaveGameName, false);
        }

        static void ResetMainCamera()
        {
            Camera.main.GetComponent<BikeCamera>().SetTarget(player);
            Camera.main.GetComponent<BikeCamera>().Reset(); //noreseto kameru
        }

        static void ResetRagdoll()
        {
            GameObject riderRiggedGO = GameObject.FindWithTag("PlayerRagdoll");//GameObject.Find("LevelContainer/BikeContainer/Player_parts_rigged");
            if (riderRiggedGO != null)
                playerRagdoll = riderRiggedGO.transform;

            if (playerRagdoll != null)
            {
                ActivateRagdollPart(playerRagdoll, false);
            }
            else
            {
                Debug.LogError("restart the game");
            }

            ragdollEntityTrigger = playerRagdoll.Find("Core/entity_trigger").gameObject;
            ragdollEntityTrigger.GetComponent<Collider2D>().enabled = false;
            ragdollEntityTrigger.GetComponent<RagdollEntityTrigger>().Reset();
        }

        static void ResetFinishFlag()
        {
            //TODO stop animating the flag
            GameObject flag = GameObject.FindGameObjectWithTag("FinishLine");
            if (flag != null)
            {
                foreach (Transform child in flag.transform)
                {
                    Transform animation = child.Find("animation");

                    if (animation != null)
                    {

                        Animator anim = animation.GetComponent<Animator>();
                        if (anim != null)
                        {
                            anim.Play("Fireworks_v1", -1, 0);
                            anim.enabled = false;
                        }

                        ShowChildren(animation, false);

                    }
                }
            }
        }

        static void ResetWaterZones()
        {
            //reset water zones
            GameObject[] waterZones = GameObject.FindGameObjectsWithTag("WaterZone");
            foreach (GameObject waterZone in waterZones)
            {
                waterZone.GetComponent<PSWater>().Reset();
            }
        }

        static void ResetAIRiders()
        {
            for (int i = 0; i < ais.Length; i++)
            {
                if (Application.isPlaying)
                {
                    Destroy(aiRiders[i].gameObject);
                }
                else if (Application.isEditor)
                {
                    DestroyImmediate(aiRiders[i].gameObject);
                }

                aiRiders[i] = Instantiate(
                    aiRiderOriginals[i],
                    Vector3.zero,
                    Quaternion.identity) as Transform;




                aiRiders[i].transform.parent = ais[i].transform;
                aiRiders[i].transform.localPosition = aiRiderOriginals[i].position;
                aiRiders[i].transform.localScale = aiRiderOriginals[i].localScale;
                aiRiders[i].name = "Player_parts";
                aiRiders[i].GetComponent<Animator>().enabled = true;
                aiRiders[i].GetComponent<RiderAnimationControl>().enabled = true;
                aiRiders[i].GetComponent<RiderAnimationControl>().Init();

                ShowChildren(aiRiders[i], true);

                if (aiRiders[i] != null)
                {
                    aiRiders[i].GetComponent<RiderAnimationControl>().Reset();
                }
                else
                {
                    print("why is airider null?");
                }
            }
        }

        static void ResetRider()
        {
            ShowChildren(playerRider, true);

            playerRider = player.transform.Find("Player_parts");

            if (Application.isPlaying)
            {
                Destroy(playerRider.gameObject);
            }
            else if (Application.isEditor)
            {
                DestroyImmediate(playerRider.gameObject);
            }

            if (playerRiderOriginal != null)
            {
                playerRider = Instantiate(
                    playerRiderOriginal,
                    Vector3.zero,
                    Quaternion.identity) as Transform;

                playerRider.transform.parent = player.transform;
                playerRider.transform.localPosition = playerRiderOriginal.position;
                playerRider.transform.localScale = playerRiderOriginal.localScale;
                playerRider.name = "Player_parts";
            }
            else
            {
                Debug.LogError("BikeGameManager: playerRiderOriginal is null, cannot create playerRider");
                return;
            }

            var riderAnimationControl = playerRider.GetComponent<RiderAnimationControl>();
            if (riderAnimationControl != null)
            {
                riderAnimationControl.enabled = true;
                riderAnimationControl.Init();
            }

            ShowChildren(playerRider, true);

            if (playerRider != null && riderAnimationControl != null)
            {
                riderAnimationControl.Reset();
            }
        }

        static void ResetTriggers()
        {
            
            Debug.Log($"Player NUll 1 ->  {player ==null}");
            Debug.Log($"Player NUll 2->  {player.transform ==null}");
            Debug.Log($"Player NUll 3->  {player.transform.Find("entity_trigger") ==null}");

            entityTrigger = player.transform.Find("entity_trigger").gameObject;
            entityTrigger.GetComponent<Collider2D>().enabled = true;
            entityTrigger.GetComponent<BikeEntityTrigger>().Reset();

            if (player.transform.parent.Find("magnet_trigger") != null)
            {
                magnetTrigger = player.transform.parent.Find("magnet_trigger").gameObject;
                magnetTrigger.GetComponent<Collider2D>().enabled = true;
                //			magnetTrigger.GetComponent<BikeMagnetTrigger>().Reset();
            }
        }

        static void ResetInteractiveObjects()
        {
            //reset interactive objects
            BombRadialBehaviour brb;
            foreach (GameObject b in radialBombs)
            {
                brb = b.GetComponent<BombRadialBehaviour>();
                if (b != null && brb != null)
                {
                    brb.Reset();
                }
                else print("Radial bomb or it's behaviour component doesn't exist");
            }

            foreach (GameObject b in fakeBombs)
            {
                b.GetComponent<BombFakeBehaviour>().Reset();
            }

            foreach (GameObject p in bombPlanks)
            {
                p.GetComponent<BombPlankBehaviour>().Reset();
            }

            foreach (GameObject g in glass)
            {
                g.GetComponent<GlassBehaviour>().Reset();
            }

            foreach (GameObject pu in puddles)
            {
                pu.GetComponent<PuddleBehaviour>().Reset();
            }

            foreach (GameObject lp in leafPiles)
            {
                lp.GetComponent<LeafPileBehaviour>().Reset();
            }

            foreach (GameObject bi in birds)
            {
                bi.GetComponent<BirdBehaviour>().Reset();
            }

            foreach (GameObject bz in birdZones)
            {
                bz.GetComponent<PSBirdZone>().Reset();
            }
        }

        static void ActivateRagdollPart(Transform t, bool activate)
        {

            foreach (Transform child in t)
            {
                if (child.gameObject.activeSelf)
                {

                    if (child.childCount > 0)
                    {
                        ActivateRagdollPart(child, activate);
                    }

                    if (child.GetComponent<Collider2D>() != null)
                    {
                        child.GetComponent<Collider2D>().enabled = activate;
                    }

                    if (child.GetComponent<Renderer>() != null)
                    {
                        child.GetComponent<Renderer>().enabled = activate;
                    }

                    if (child.GetComponent<Rigidbody2D>() != null)
                    {

                        child.GetComponent<Rigidbody2D>().isKinematic = !activate;

                    }

                    ParticleSystem particleSystem = child.GetComponent<ParticleSystem>();
                    if (particleSystem != null)
                    {
                        particleSystem.Clear();
                    }

                }
            }
        }

        public static void PauseGameToggle()
        {
            PauseGameSet(!gamePaused);
        }

        public static void PauseGameSet(bool on)
        {
            if (initialized)
            {
                if (on)
                {
                    Time.timeScale = 0;

                    if (playerControl != null)
                        playerControl.enabled = false;

                    if (playerInput != null)
                        playerInput.enabled = false;
                    //if(timer != null) {
                    timer.Pause();
                    //}

                    if (ais != null)
                    {
                        for (int i = 0; i < ais.Length; i++)
                        {
                            if (ais[0] != null)
                            {
                                aiInputs[0].enabled = false;
                                ais[0].GetComponent<BikeControl>().enabled = false;
                            }
                        }
                    }

                }
                else
                {
                    Time.timeScale = 1;

                    if (ais != null)
                    {
                        for (int i = 0; i < ais.Length; i++)
                        {
                            if (ais[0] != null)
                            {
                                aiInputs[0].enabled = true;
                                ais[0].GetComponent<BikeControl>().enabled = true;
                            }
                        }
                    }

                    if (playerInput != null)
                        playerInput.enabled = true;

                    if (playerControl != null)
                    {

                        playerControl.enabled = true;



                    }

                    //if(timer != null) {
                    timer.Unpause();
                    //}

                }
                gamePaused = on;
            }

        }

        public static void BikeJustDied()
        {

            if (!initialized || playerState.invincible)
            {
                //print("'Tis but a flesh wound");
                return;
            }

            //		if (!bikeDied) {// && UIManager.currentScreenType == GameScreenType.Game
            if (!playerState.dead)
            {// && UIManager.currentScreenType == GameScreenType.Game

                playerState.dead = true;

                if (timer != null)
                {
                    timer.TimerStop();
                }

                if (playerControl != null)
                {
                    playerControl.enabled = false;
                }

                if (playerRagdoll != null)
                {
                    Camera.main.GetComponent<BikeCamera>().SetTarget(playerRagdoll.Find("Core").gameObject);
                }

                playerState.drowned = false;

                if (playerRider.Find("Helmet").GetComponent<BikeDeathTrigger>().collTag == "WaterZone" ||
                   playerRider.Find("Core").GetComponent<BikeDeathTrigger>().collTag == "WaterZone")
                {
                    playerState.drowned = true;
                }

                if (exhaustBehaviour != null)
                    exhaustBehaviour.StopEmission();

                if (playerState.drowned)
                {
                    AchievementManager.AchievementProgress("drown", 1);
                    AchievementManager.AchievementProgress("drown__2", 1);
                    AchievementManager.AchievementProgress("drown__3", 1);

                }
                else
                {
                    AchievementManager.AchievementProgress("crash", 1);
                    AchievementManager.AchievementProgress("crash__2", 1);
                    AchievementManager.AchievementProgress("crash__3", 1);
                    SoundManager.Play("BikeCrash");
                    
                    // Haptic feedback for crash
                    if (BikeDataManager.SettingsSfx)
                    {
                        #if UNITY_ANDROID
                            HapticFeedback.MediumFeedback();
                        #elif UNITY_IOS
                            Handheld.Vibrate();
                        #endif
                    }
                }

                if (Debug.isDebugBuild)
                {
                    print(playerState.drowned ? "Drowned" : "Crashed");
                }

                //			player.transform.FindChild("entity_trigger").collider2D.enabled = false;
                if (magnetTrigger != null)
                {
                    magnetTrigger.GetComponent<Collider2D>().enabled = false;
                }
                entityTrigger.GetComponent<Collider2D>().enabled = false;
                ShowChildren(playerRider, false);

                if (!playerState.drowned)
                {
                    ShowChildren(crash.transform, true);
                    crash.transform.position = playerRider.Find("Helmet").position;
                    crash.transform.Find("animation").GetComponent<Animator>().Play("Crash", -1, 0);
                }


                if (playerRagdoll != null)
                {

                    foreach (Transform child in playerRagdoll)
                    {

                        if (child.gameObject.activeSelf)
                        {
                            ActivateRagdollPart(child, true);

                            if (child.GetComponent<Collider2D>() != null)
                            {
                                child.GetComponent<Collider2D>().enabled = true;
                            }

                            if (child.GetComponent<Renderer>() != null)
                            {
                                child.GetComponent<Renderer>().enabled = true;
                            }

                            if (child.GetComponent<Rigidbody2D>() != null)
                            {

                                child.position = playerRider.Find(child.name).position;
                                child.GetComponent<Rigidbody2D>().isKinematic = false;

                            }

                        }
                    }

                    playerRagdoll.Find("Helmet/FlightParticle").GetComponent<ParticleSystem>().GetComponent<Renderer>().material =
                        playerState.drowned ? bubbleMaterial : zMaterial;

                    ragdollEntityTrigger.GetComponent<Collider2D>().enabled = true;

                }

                if (singlePlayerRestarts == 0 && levelInfo.LevelName.ToLower().Contains("long"))
                {
                    AwardPlayer();
                }

            }

        }

        public static void ShowChildren(Transform t, bool show)
        {

            if (t != null)
            {

                foreach (Transform child in t)
                {

                    if (child.childCount > 0)
                    {
                        ShowChildren(child, show);
                    }

                    if (child.GetComponent<Collider2D>() != null)
                    {
                        child.GetComponent<Collider2D>().enabled = show;
                    }

                    if (child.GetComponent<Renderer>() != null)
                    {
                        child.GetComponent<Renderer>().enabled = show;
                    }

                }

            }

        }

        public static void AIJustFinished()
        {

            /*@todo -- śo jápártaisa -- jo ir vairáki AI baiki 

            if (ai != null) {
                if(!playerState.finished) {
                    //DO this will not do any more, since ghost is there in single player

                    if(!singlePlayer) {
                        aiState.starsEarned = 3;
                        playerState.starsEarned = 1;
                    }

                }
            }
        */
            if (ais != null)
            {
                if (ais.Length > 1)
                {
                    int finishCount = 0;
                    foreach (var aiState in aiStates)
                    {
                        if (aiState.finished)
                        {
                            finishCount++;
                        }
                    }

                    if (finishCount == 1)
                    {
                        StartFlagAnimation();
                    }
                }
            }
        }

        static void StartFlagAnimation()
        {
            GameObject flag = GameObject.FindGameObjectWithTag("FinishLine");

            if (flag != null)
            {
                foreach (Transform child in flag.transform)
                {
                    Transform animation = child.Find("animation");

                    if (animation != null)
                    {

                        ShowChildren(animation, true);

                        Animator anim = animation.GetComponent<Animator>();
                        if (anim != null)
                        {
                            anim.enabled = true;
                            anim.Play("Fireworks_v1", -1, 0);
                        }

                    }
                }
            }
        }

        public static void PlayerJustFinished()
        {

            //print("PlayerJustFinished");
            if (!playerState.finished)
            {

                playerState.finished = true;

                if (timer != null)
                    timer.TimerStop();

                if (playerControl != null)
                {
                    playerInput.passInputToControl = false;
                    playerControl.InputAccelerometerX = 0;
                    playerControl.InputTouchRight = false;
                    playerControl.InputTouchLeft = true;
                }

                if (levelInfo.CheckpointsInLevel > 0)
                {
                    checkpointsReached = 10;//count finish as a checkpoint
                }
                //DONE start animating the flag
                StartFlagAnimation();

                SoundManager.Play("FinishLineCrossed");
                
                // Haptic feedback for victory
                if (BikeDataManager.SettingsSfx)
                {
                    #if UNITY_ANDROID
                        HapticFeedback.LightFeedback();
                    #elif UNITY_IOS
                        Handheld.Vibrate();
                    #endif
                }

                if (singlePlayer)
                {
                    BikeGameManager.AwardPlayer();

                }
                else
                {

                    /*@todo -- śo jápártaisa -- jo ir vairáki AI baiki
                if(!aiState.finished) {
                    //TODO this will not do any more, since ghost is there in single player

    //                if(!singleplayer) {
                    playerState.starsEarned = 3;
    //                }
                    aiState.starsEarned = 1;
                }*/

                }

            }
        }


        public static void CheckBikeAgainstBounds()
        {
            if (player != null &&

                !(levelBounds.max.x > player.transform.position.x && //contains2d
                    levelBounds.max.y > player.transform.position.y &&
                     levelBounds.min.x < player.transform.position.x &&
                     levelBounds.min.y < player.transform.position.y) &&
                player.transform.position.y < levelBounds.max.y)
            {
                playerState.invincible = false;
                BikeJustDied();
                ExecuteCommand(GameCommand.Reset);
                if (singlePlayer)
                {
                    UIManager.SwitchScreen(GameScreenType.PreGame);
                }
                else
                {
                    UIManager.SwitchScreen(GameScreenType.MultiplayerPreGame);
                }
            }
        }


        /**
         * give player money and stars and save ride statistics
         * called on UI action
         */
        public static void AwardPlayer()
        {

            int prevBestStars = BikeDataManager.Levels[levelInfo.LevelName].BestStars;
            // Debug.Log("Finished level --- " + "LEVEL-" + levelInfo.LevelName.Remove(0,4));
            // GameAnalyticsSDK.Wrapper.GA_Wrapper.AddDesignEvent("LEVEL-" + levelInfo.LevelName.Remove(0, 4));
            AchievementManager.AchievementProgress("finish", 1);
            AchievementManager.AchievementProgress("finish__2", 1);
            AchievementManager.AchievementProgress("finish__3", 1);


            BikeGameManager.playerState.starsEarned = 0;

            if (PickupManager.CoinsCollected >= levelInfo.CoinPar && (BikeGameManager.TimeElapsed <= levelInfo.TimePar || levelInfo.TimePar < 0))
            {
                BikeGameManager.playerState.starsEarned = 1;
            }

            if (PickupManager.CoinsCollected >= levelInfo.CoinPar2 && (BikeGameManager.TimeElapsed <= levelInfo.TimePar2 || levelInfo.TimePar2 < 0))
            {
                BikeGameManager.playerState.starsEarned = 2;
            }

            if (PickupManager.CoinsCollected >= levelInfo.CoinPar3 && (BikeGameManager.TimeElapsed <= levelInfo.TimePar3 || levelInfo.TimePar < 0))
            {
                BikeGameManager.playerState.starsEarned = 3;
            }



            int betterThanPrevBest = BikeGameManager.playerState.starsEarned - BikeDataManager.Levels[levelInfo.LevelName].BestStars;
            if (betterThanPrevBest > 0)
            {
                BikeDataManager.Levels[levelInfo.LevelName].BestStars = BikeGameManager.playerState.starsEarned; //new record, write it down
                BikeDataManager.Stars += betterThanPrevBest; //a level can give max 3 stars: if last ride was 2 stars, now 3 stars -> give only 1 (he already has earned 2 stars from this level)

                AchievementManager.AchievementProgress("stars", betterThanPrevBest);
                AchievementManager.AchievementProgress("stars__2", betterThanPrevBest);
                AchievementManager.AchievementProgress("stars__3", betterThanPrevBest);
            }

            if (BikeDataManager.Levels[levelInfo.LevelName].BestTime == 0)
            {
                BikeDataManager.Levels[levelInfo.LevelName].BestTime = BikeGameManager.TimeElapsed; //no prev. best time
            }

            if (BikeDataManager.Levels[levelInfo.LevelName].BestTime > BikeGameManager.TimeElapsed)
            {
                BikeDataManager.Levels[levelInfo.LevelName].BestTime = BikeGameManager.TimeElapsed; //new best time

            }

            bestCheckpoints = BikeDataManager.Levels[levelInfo.LevelName].BestCheckpoints; //old best checkpoints, need for finish screen
            int checkpointCoinBonus = 0;
            if (BikeDataManager.Levels[levelInfo.LevelName].BestCheckpoints < BikeGameManager.checkpointsReached)
            {
                //            print("bestCheckpoints " + bestCheckpoints + " " + DataManager.Levels[levelInfo.LevelName].BestCheckpoints);
                //            print("GameManager.checkpointsReached " + GameManager.checkpointsReached);
                BikeDataManager.Levels[levelInfo.LevelName].BestCheckpoints = BikeGameManager.checkpointsReached; //new best coins

                //            int startI = bestCheckpoints > 0 ? bestCheckpoints - 1 : 0;
                for (int i = bestCheckpoints; i < checkpointsReached; i++)
                {
                    //                print("DataManager.CheckpointCoinBonus[i] "+DataManager.CheckpointCoinBonus[i]);
                    checkpointCoinBonus += BikeDataManager.CheckpointCoinBonus[i];
                }
            }

            if (BikeDataManager.Levels[levelInfo.LevelName].BestCoins < PickupManager.CoinsCollected)
            {
                BikeDataManager.Levels[levelInfo.LevelName].BestCoins = PickupManager.CoinsCollected; //new best coins
            }

            int coinMultiplier = 1;
            if (CentralizedOfferManager.IsDoubleCoinWeekendOn())
            {
                coinMultiplier = 2;
            }

            BikeDataManager.Coins += (PickupManager.CoinsCollected + PickupManager.CoinCrateAmount() + checkpointCoinBonus) * coinMultiplier;
            BikeDataManager.CoinsInLastLevel = (PickupManager.CoinsCollected + PickupManager.CoinCrateAmount() + checkpointCoinBonus) * coinMultiplier;

            PickupManager.CashCratesIn();

            if (prevBestStars == 0 && BikeGameManager.playerState.starsEarned > 0)
            { //trase izbraukta pirmo reizi UN ir pabeigta

                AchievementManager.AchievementProgress("finish_unq", 1);
                AchievementManager.AchievementProgress("finish_unq__2", 1);
                AchievementManager.AchievementProgress("finish_unq__3", 1);

                if (levelInfo.LevelName.ToLower().Contains("long"))
                { //TODO long levels don't have stars, so this probably shouldn't be here
                    AchievementManager.AchievementProgress("finish_long", 1);
                    AchievementManager.AchievementProgress("finish_long__2", 1);
                }

                if (levelInfo.LevelName.ToLower().Contains("bonus"))
                {
                    AchievementManager.AchievementProgress("finish_bonus", 1);
                    AchievementManager.AchievementProgress("finish_bonus__2", 1);
                }
            }


            TelemetryManager.EventFinished(LevelManager.CurrentLevelName, BikeGameManager.TimeElapsed, PickupManager.CoinsCollected + PickupManager.CoinCrateAmount() + checkpointCoinBonus, BikeGameManager.playerState.starsEarned);
            //print("finisheeja, bet veel neseivojam - seivos ieejot trases chuuskaa");
            //DataManager.Flush();
        }


        public static void ExecuteCommand(GameCommand rideCommand)
        {

            lastCommand = rideCommand;

            switch (rideCommand)
            {

                case GameCommand.Reset:

                    //on first call add the command to queue (queue is empty or not reset's turn)
                    if (commandQueue.Count == 0 || commandQueue.Peek() != GameCommand.Reset)
                    {
                        //print("Delay");
                        commandQueue.Enqueue(GameCommand.Reset);
                        UIManager.ToggleScreen(GameScreenType.PopupLoading, true);
                    }
                    else
                    { //if it's reset's turn, deque and execute, also hide the loading screen after finished
                      //print("Execute");
                        commandQueue.Dequeue();

                        if (!singlePlayer)
                        {
                            if (multiPlayerRestarts > 0)
                                multiPlayerRestarts--;
                        }

                        if (Time.timeScale != 1 && Time.timeScale != 0)
                        {
                            ExecuteCommand(GameCommand.BulletTimeOff);
                        }

                        PauseGameSet(true);
                        ResetGame();

                        SoundManager.Play("Restart");

                        UIManager.ToggleScreen(GameScreenType.PopupLoading, false);
                    }

                    break;

                case GameCommand.HardReset:
                    LevelManager.LoadLevel("", levelInfo.LevelName);

                    break;

                case GameCommand.TogglePause:
                    PauseGameToggle();
                    break;

                case GameCommand.PauseOff:
                    PauseGameSet(false);

                    if (exhaustBehaviour != null)
                    {
                        exhaustBehaviour.TurnOn();
                    }

                    if (playerRider != null)
                    {
                        playerRider.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
                    }

                    if (ais != null)
                        for (int i = 0; i < ais.Length; i++)
                        {
                            aiRiders[i].GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
                        }

                    break;

                case GameCommand.PauseOn:
                    PauseGameSet(true);

                    if (playerRider != null)
                        playerRider.GetComponent<Animator>().updateMode = AnimatorUpdateMode.Normal;

                    if (ais != null)
                    {
                        for (int i = 0; i < ais.Length; i++)
                        {
                            aiRiders[i].GetComponent<Animator>().updateMode = AnimatorUpdateMode.Normal;
                        }
                    }

                    break;

                case GameCommand.ToggleSingleplayerGhost:
                    BikeDataManager.SettingsSPGhost = !BikeDataManager.SettingsSPGhost;
                    break;

                case GameCommand.ToggleSettingsMusic:
                    BikeDataManager.SettingsMusic = !BikeDataManager.SettingsMusic;

                    if (!BikeDataManager.SettingsMusic)
                    {
                        SoundManager.StopAmbience(); //izslédzu múziku
                    }
                    else
                    {
                        SoundManager.StartAmbience();//@note -- nákamá múzika sáksies ká paredzéts [párslédzot logus], bet kamér ir setingos - tikmér bús klusums
                    }

                    break;

                case GameCommand.ToggleSettingsSfx:
                    BikeDataManager.SettingsSfx = !BikeDataManager.SettingsSfx;
                    if (!BikeDataManager.SettingsSfx)
                    {
                        SoundManager.StopSfx();
                    }
                    break;

                case GameCommand.ToggleSettingsHD:

                    if (commandQueue.Count == 0 || commandQueue.Peek() != GameCommand.ToggleSettingsHD)
                    {
                        print("Delay");
                        commandQueue.Enqueue(GameCommand.ToggleSettingsHD);
                        UIManager.ToggleScreen(GameScreenType.PopupLoading, true);
                    }
                    else
                    {
                        print("Execute");
                        commandQueue.Dequeue();

                        BikeDataManager.SettingsHD = !BikeDataManager.SettingsHD;
                        QualitySettingsManager.UpdateQualityLevel();

                        UIManager.ToggleScreen(GameScreenType.PopupLoading, false);
                    }



                    break;

                case GameCommand.ToggleSettingsControl:
                    BikeDataManager.SettingsAccelerometer = !BikeDataManager.SettingsAccelerometer;
                    break;

                case GameCommand.BulletTimeOn:
                    Time.timeScale = 0.25f;
                    Time.fixedDeltaTime = Time.timeScale * Time.fixedDeltaTime;
                    BikeDataManager.Levels[levelInfo.LevelName].BulletTimeUsed = true;
                    break;

                case GameCommand.BulletTimeOff:
                    Time.fixedDeltaTime = Time.fixedDeltaTime / Time.timeScale;
                    Time.timeScale = 1f;

                    break;

                case GameCommand.ToggleDevelopmentMode:
                    developmentMode = !developmentMode;
                    if (developmentMode)
                    {

                        BikeDataManager.CoinsWOAchievement += 1000000;
                        BikeDataManager.Boosts["ice"].Number = 11;
                        BikeDataManager.Boosts["magnet"].Number = 12;
                        BikeDataManager.Boosts["invincibility"].Number = 13;
                        BikeDataManager.Boosts["fuel"].Number = 35;
                        BikeDataManager.Boosts["ice"].Discovered = true;
                        BikeDataManager.Boosts["magnet"].Discovered = true;
                        BikeDataManager.Boosts["invincibility"].Discovered = true;
                        BikeDataManager.Boosts["fuel"].Discovered = true;

                    }
                    UIManager.Reinit();

                    break;

                case GameCommand.ResetPlayerData:
                    developmentMode = false;
                    BikeDataManager.ResetAllPlayerInfo();
                    UIManager.Reinit();
                    break;

                case GameCommand.SettingsResetTeams:
                    MultiplayerManager.ManualResetTeam();
                    break;

                case GameCommand.SettingsGoToSupportPage:
                    //SkidosParentalControl.INSTANCE.ShowParentalControl(() =>
                    //{
                    Application.OpenURL("http://skidos.com/support/");
                    //    SkidosUI.Instance.Toggle(false);
                    //}, OnParentControlFail, false);
                    break;

                case GameCommand.SettingsFBLogOut:
                    MultiplayerManager.FBLogOut();
                    break;

                case GameCommand.DownloadLatestVersion:
#if UNITY_ANDROID
				Application.OpenURL("market://details?id=skidos.bikeracing");
#elif UNITY_IOS
                    Application.OpenURL("itms-apps://itunes.apple.com/app/id947356923");
#else
                Application.OpenURL("https://itunes.apple.com/app/id947356923");
#endif
                    break;

                case GameCommand.Rate:
                    //vedam uz appstoru (nav zináms vai novértés ar ★★★★★, ja vispár novértés)

#if UNITY_ANDROID
				Application.OpenURL("market://details?id=skidos.bikeracing");
#elif UNITY_IOS
                    Application.OpenURL("itms-apps://itunes.apple.com/app/id947356923");
#else
                Application.OpenURL("https://itunes.apple.com/app/id947356923");
#endif
                    BikeDataManager.HasRatedUs = true;
                    UIManager.ToggleScreen(GameScreenType.PopupRateUs, false);//atgrieźoties appá aizveru popupu				
                    break;


                case GameCommand.ShareFB:
#if UNITY_EDITOR
                    Application.OpenURL("http://lmgtfy.com/?q=Nesatraucies+uz+appas+veers+valjaa+natiivo+FB+appu!");
                    BikeDataManager.Levels[SelectedLevelName].Shared = true;
                    UIManager.ToggleScreen(GameScreenType.PopupUnlockBonusLevels);
                    ExecuteCommand(GameCommand.PlaySpecificLevel);
#else
                //ShareManager.ShareToFacebook();
#endif
                    break;

                case GameCommand.ShareTwitter:
#if UNITY_EDITOR
                    print("SOOMLA only works on device");
                    BikeDataManager.Levels[SelectedLevelName].Shared = true;
                    UIManager.ToggleScreen(GameScreenType.PopupUnlockBonusLevels);
                    ExecuteCommand(GameCommand.PlaySpecificLevel);
#else
                // ShareManager.ShareToTwitter(); 
#endif
                    break;

                case GameCommand.ShareVK:
#if UNITY_EDITOR
                    BikeDataManager.Levels[SelectedLevelName].Shared = true;
                    UIManager.ToggleScreen(GameScreenType.PopupUnlockBonusLevels);
                    ExecuteCommand(GameCommand.PlaySpecificLevel);
#else
                // ShareManager.ShareToVK(); 
#endif
                    break;

                case GameCommand.UnlockAllTracks:

                    if (PurchaseManager.CoinPurchase(BikeDataManager.PriceUnlockAll))
                    {//nopirka

                        //atklája visus bústinjus - lai redzétu "recomended boosts" visás trasés, pat ja nav atklájis bústińus dabiská veidá
                        BikeDataManager.Boosts["ice"].Discovered = true;
                        BikeDataManager.Boosts["magnet"].Discovered = true;
                        BikeDataManager.Boosts["invincibility"].Discovered = true;
                        BikeDataManager.Boosts["fuel"].Discovered = true;

                        TelemetryManager.EventTracksUnlocking();

                        BikeDataManager.PaidLevelUnlock = true;
                        BikeDataManager.Flush();
                        UIManager.ToggleScreen(GameScreenType.PopupUnlockLevels); //aizvért popupu
                        UIManager.SwitchScreen(GameScreenType.Levels); // jápárládé límenju ekráns - lai visi límenji izskatítot unlokoti


                    }
                    else
                    {
                        //nenopirka
                        //print("lol, nenopirka");
                    }


                    break;

                case GameCommand.PlaySpecificLevel://paślaik śo lieto tikai popups pirms LONG límeńa 

                    UIManager.ToggleScreen(GameScreenType.PopupLongLevel, false); //AIZVERU popupu
                    UIManager.SwitchScreen(GameScreenType.PreGame);
                    LevelManager.LoadLevel("", SelectedLevelName);
                    SoundManager.Play("Play");
                    //			UIButtonLevelCommand.Play(SelectedLevelName); // ParamS1 satur izvélétá límeńa nosaukumu - tas tiek ievietots pirms popupa atvérśanas
                    break;

                case GameCommand.OpenPopupPromo:
                    PopupPromoBehaviour.ShowInGarage();
                    break;

                case GameCommand.KillBike:
                    playerState.invincible = false;
                    BikeJustDied();
                    break;

                case GameCommand.FBLogin:

                    MultiplayerManager.FBConnect();

                    break;

                case GameCommand.SaveGameSP:

                    //on first call add the command to queue (queue is empty or not reset's turn)
                    if (commandQueue.Count == 0 || commandQueue.Peek() != GameCommand.SaveGameSP)
                    {
                        //print("Delay");
                        commandQueue.Enqueue(GameCommand.SaveGameSP);
                        UIManager.ToggleScreen(GameScreenType.PopupLoading, true);
                    }
                    else
                    { //if it's reset's turn, deque and execute, also hide the loading screen after finished
                      //print("Execute");
                        commandQueue.Dequeue();
                        SaveGameSP();

                        LevelManager.KillLevel(); //pártrauc límeńa renderéśanu 

                        UIManager.ToggleScreen(GameScreenType.PopupLoading, false);

                    }


                    break;

                case GameCommand.GiveFreeCoins:

                    BikeDataManager.Coins += 2500;

                    break;

                case GameCommand.GiveFreeStyle:

                    int giftID = BikeDataManager.GetLevelGiftID(BikeGameManager.lastLoadedLevelName);

                    if (giftID > -1 && BikeDataManager.LevelGifts.ContainsKey(giftID))
                    {

                        int styleID = BikeDataManager.LevelGifts[giftID].StyleID;

                        if (BikeDataManager.Styles.ContainsKey(styleID))
                        {
                            BikeDataManager.GiftStyleIndex = styleID;
                        }

                        //print("giftID " + giftID + "; styleID " + styleID);

                    }

                    //TODO get rid of this
                    //                if(DataManager.Styles[DataManager.GiftStyleIndex].Locked){
                    BikeDataManager.Styles[BikeDataManager.GiftStyleIndex].Locked = false;
                    BikeDataManager.ShowGiftStyle = true;
                    //                }

                    break;

                case GameCommand.MultiplayerQuit:

                    MultiplayerManager.QuitGame();

                    break;

                case GameCommand.OpenFBPage:

                    //Application.OpenURL("http://www.facebook.com/bikeupgame");

                    break;

                case GameCommand.KillLevel:

                    LevelManager.KillLevel();

                    break;

                case GameCommand.SettingsRestorePurchases:
                    // StoreManager.RestorePurchases();
                    break;
                //              
                case GameCommand.ShowAdTestScreen:
                    AdManager.ShowAdTestScreen();
                    break;
                case GameCommand.SkidosMain:
                    //SkidosMathlab.Instance.ShowMathlabPanel();
                    break;
                default:
                    break;


            }


        }

        static void OnParentControlFail()
        {
            //SkidosUI.Instance.Toggle(false);
        }

        static void TurnOffBulletTime()
        {
            ExecuteCommand(GameCommand.BulletTimeOff);
        }

        public static void PlayerReachedCheckpoint(int id, Vector2 position)
        {

            if (id != lastCheckpointID)
            {
                SoundManager.Play("Checkpoint");
            }

            lastCheckpointID = id;
            Vector3 tmpStart = start.transform.position;
            tmpStart.x = position.x;
            tmpStart.y = position.y;
            start.transform.position = tmpStart;

            if (!reachedCheckpointIDs.Contains(id))
            {
                checkpointsReached++;
                reachedCheckpointIDs.Add(id);
            }
        }
    }

}
