namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

/**
 * Pieskata visas páréjás skańas [kas neietilpst baika motora FSM párzińá (BikeSounds skripts) ]
 * tikai ísás, kas jáieládé sákot spéli
 * 
 * śis menedźeris nav statisks - skripts pieder BikeSounds objektam
 * vienkárśíbas déĺ tam ir statisks interfeiss (publiskás metodes ir statiskas)
 * 
 * 
 * galvenajai kamerai izveido X gabalus AudioSource kompoenentus - skanjas kanálu - cik vienlaicígas skańas vajag atskanjot
 * 
 */
public class SoundManager : MonoBehaviour
{

    public LoadAddressable_Vasundhara loadAddressable_Vasundhara;
    private static int numChannels = 40; //vienlaiciigas skanjas
    private AudioSource[] channels;
    private int lastUsedChannel = 0;
    private int lastTimeCoinSoundPlayed = 0;

    private static int ambienceChannellA = numChannels; //ambience tiek atskańota uz 1 vai 2 kanáliem - pédéjiem 2, kurus parastás skańas nevar njemt sev
    private static int ambienceChannellB = numChannels + 1;

    private Dictionary<string, List<AudioClip>> Clips = new Dictionary<string, List<AudioClip>>();  //nosaukums => liste ar 1 vai vairákiem klipiem (ja vairáki, tad atskanjo RND)

    private static SoundManager instace; //sis skripts - piederośs  BikeSounds objektam

    private bool firstTimeMenu = true;

    private string CurrentAmbience = ""; //tagadéjá ieslégtá ambience (múzika) 
    private string LastAmbience = ""; //iepriekśéjá ieslégtá ("Last screen" support)

    bool skidosStatus;
    bool ambiencePlaying;

    //statiskás funkchas, kas gan śis bútu par menedźeri bez statiskám funkcijám!
    public static void Play(string clipName)
    {
        instace._Play(clipName);
    }
    public static void ChangeAmbienceForLevel(string levelName)
    {
        if (BikeDataManager.SettingsMusic)
        { //ja setingos izslégta múzika, tad pat neméǵinás slégt iekśá
            instace._ChangeAmbienceForLevel(levelName);
        }
    }
    public static void ChangeAmbienceForMenu()
    {
        if (BikeDataManager.SettingsMusic)
        {
            instace._ChangeAmbienceForMenu();
        }
    }
    public static void StopAmbience()
    {
        instace._StopAmbience();
    }
    public static void StartAmbience()
    {
        instace._StartAmbience();
    }
    public static void StopSfx()
    {
        instace._StopSfx();
    }
    public static float GetClipLength(string clipName)
    {
        return instace._GetClipLength(clipName);
    }



    void Awake()
    {
        loadAddressable_Vasundhara = GameObject.Find("Vasundhara_LoadAddressable").GetComponent<LoadAddressable_Vasundhara>();

        if (instace != null)
        {
            Debug.LogError("Singltons, draudziņ!"); //jau káds ir piestartéjis śí skripta instanci
        }
        instace = this;

        for (int i = 0; i < numChannels + 2; i++)
        { // +2 jo ir 2 papildus ambiences kanáli
            Camera.main.gameObject.AddComponent<AudioSource>(); //izveido komponentu
            Debug.LogError("Camera name: " + Camera.main.gameObject.name);
        }
        channels = Camera.main.gameObject.GetComponents<AudioSource>(); //dabú masívu ar visiem komponentiem
                                                                        //print("GameSounds::got " + channels.Length + " channels"); 



        //LoadClip("BikeCrash", "Bike/crash");

        //LoadClip("BikeLanding", "Bike/landing");
        //LoadClip("BikeLanding", "Bike/landing1");
        //LoadClip("BikeLanding", "Bike/landing2");

        //LoadClip("BoostOn", "Bike/set_boost");

        ////ar 1 nosaukumu ieládé vairákus skanju failus
        //LoadClip("Trick", "Bike/tricks1");
        //LoadClip("Trick", "Bike/tricks2");
        //LoadClip("Trick", "Bike/tricks3");
        //LoadClip("Trick", "Bike/tricks4");
        //LoadClip("Trick", "Bike/tricks5");
        //LoadClip("Trick", "Bike/tricks6");
        //LoadClip("Trick", "Bike/tricks7");

        ////-----------------------------------------------
        //LoadClip("Checkpoint", "World/checkpoint");

        //LoadClip("Explosion", "World/bomb_explosion");

        //LoadClip("GlassBreaking", "World/glass_breaking_1");
        //LoadClip("GlassBreaking", "World/glass_breaking_2");
        //LoadClip("GlassBreaking", "World/glass_breaking_3");
        //LoadClip("GlassBreaking", "World/glass_breaking_4");

        //LoadClip("Spikes", "World/spike_zone");

        //LoadClip("Splash", "World/water_splash");

        //LoadClip("Wind", "World/wind_zone");

        ////-------------skanjas skanju zonai------------------
        //LoadClip("Cow", "World/cow");
        //LoadClip("Car", "World/car_with_horn");
        //LoadClip("Heli", "World/helicopter");
        //LoadClip("Cave", "World/cave");
        //LoadClip("Birdie", "World/Birdie");


        ////-----------------------------------------------
        //LoadClip("PickupCoin", "Items/pick_up_coin");

        //LoadClip("PickupCoin2x", "Items/pick_up_2xcoin");
        //LoadClip("PickupCoin2x", "Items/pick_up_coin2x_v2");

        //LoadClip("PickupBox", "Items/pick_up_box");

        //LoadClip("PickupGoldenHelmet", "Items/pick_up_golden_helmet");


        ////-----------------------------------------------
        //LoadClip("Restart", "UI/restart");
        //LoadClip("Claim", "UI/claim");

        //LoadClip("Back", "UI/backward");
        //LoadClip("Click", "UI/forward");
        //LoadClip("Play", "UI/play_wrum");
        //LoadClip("CoinCounting", "UI/coin_counting");
        //LoadClip("BoostStart", "UI/start_boost");
        //LoadClip("BoostFinish", "UI/finish_now");
        //LoadClip("Confirm", "UI/upgrade_confirm_style");
        //LoadClip("Color", "UI/color_slide");
        //LoadClip("StyleLocked", "UI/style_locked");
        //LoadClip("StyleUnlock", "UI/unlocking_style");
        //LoadClip("LevelUp", "UI/level_up");
        ////finish
        //LoadClip("FinishCrateDrop", "UI/box_coming_in");
        //LoadClip("FinishCrateClick", "UI/tap_the_box");
        //LoadClip("FinishCrateOpen", "UI/box_open");
        //LoadClip("FinishLineCrossed", "UI/finish_line");
        //LoadClip("FinishStarPop", "UI/stars");
        //LoadClip("FinishTextShow", "UI/text");




        LoadClip("BikeCrash", "crash");

        LoadClip("BikeLanding", "landing");
        LoadClip("BikeLanding", "landing1");
        LoadClip("BikeLanding", "landing2");

        LoadClip("BoostOn", "set_boost");

        //ar 1 nosaukumu ieládé vairákus skanju failus
        LoadClip("Trick", "tricks1");
        LoadClip("Trick", "tricks2");
        LoadClip("Trick", "tricks3");
        LoadClip("Trick", "tricks4");
        LoadClip("Trick", "tricks5");
        LoadClip("Trick", "tricks6");
        LoadClip("Trick", "tricks7");

        //-----------------------------------------------
        LoadClip("Checkpoint", "checkpoint");

        LoadClip("Explosion", "bomb_explosion");

        LoadClip("GlassBreaking", "glass_breaking_1");
        LoadClip("GlassBreaking", "glass_breaking_2");
        LoadClip("GlassBreaking", "glass_breaking_3");
        LoadClip("GlassBreaking", "glass_breaking_4");

        LoadClip("Spikes", "spike_zone");

        LoadClip("Splash", "water_splash");

        LoadClip("Wind", "wind_zone");

        //-------------skanjas skanju zonai------------------
        LoadClip("Cow", "cow");
        LoadClip("Car", "car_with_horn");
        LoadClip("Heli", "helicopter");
        LoadClip("Cave", "cave");
        LoadClip("Birdie", "birdie");


        //-----------------------------------------------
        LoadClip("PickupCoin", "pick_up_coin");

        LoadClip("PickupCoin2x", "pick_up_2xcoin");
        LoadClip("PickupCoin2x", "pick_up_coin2x_v2");

        LoadClip("PickupBox", "pick_up_box");

        LoadClip("PickupGoldenHelmet", "pick_up_golden_helmet");


        //-----------------------------------------------
        LoadClip("Restart", "restart");
        LoadClip("Claim", "claim");

        LoadClip("Back", "backward");
        LoadClip("Click", "forward");
        LoadClip("Play", "play_wrum");
        LoadClip("CoinCounting", "coin_counting");
        LoadClip("BoostStart", "start_boost");
        LoadClip("BoostFinish", "finish_now");
        LoadClip("Confirm", "upgrade_confirm_style");
        LoadClip("Color", "color_slide");
        LoadClip("StyleLocked", "style_locked");
        LoadClip("StyleUnlock", "unlocking_style");
        LoadClip("LevelUp", "level_up");
        //finish
        LoadClip("FinishCrateDrop", "box_coming_in");
        LoadClip("FinishCrateClick", "tap_the_box");
        LoadClip("FinishCrateOpen", "box_open");
        LoadClip("FinishLineCrossed", "finish_line");
        LoadClip("FinishStarPop", "stars");
        LoadClip("FinishTextShow", "text");

    }

    //non-static -- lietośanai UI caur inspektoru;  kodá jálieto statiskais aliass:  SoundManager.Play();
    public void _Play(string clipName)
    {


        //if (SkidosUI.Instance.IsEnabled())
        {
            //Debug.LogError("INSIDE SKIDOS LOOP");
            //StopAmbience();
            //_StopSfx();
            //return;
        }
        //Debug.LogError("SOUNDMANAGER PLAY :::" + clipName);

        if (!BikeDataManager.SettingsSfx)
        { //setingos izslégta skanja, neatskańos neko
            return;
        }
        //Debug.LogError("SOUNDMANAGER SettingsSfx :::" + clipName);
        if (!Startup.Initialized)
        { //neatskańo pirms viss nav gatavs (UI menedźeris inicializéjoties kliksḱina uz savám pogám :D )
            return;
        }
        //Debug.LogError("SOUNDMANAGER Intialized :::" + clipName);
        if (clipName == "PickupCoin")
        { //moneetu skanjaam iipahs rezhiims - atsakaas atskanjot, ja daudz jau tiek atskanjotas
            if (Time.frameCount - lastTimeCoinSoundPlayed < 2)
            {
                return; //neatskanjos
            }
            lastTimeCoinSoundPlayed = Time.frameCount;
        }

        List<AudioClip> clipList;
        if (Clips.TryGetValue(clipName, out clipList))
        {

            int freeChannelId = GetFreeChannel();
            if (freeChannelId == -1)
            {
#if UNITY_EDITOR
                Debug.LogError("SoundManager::Nav brīvu kanālu! Skipojam skaņu " + clipName);
#endif
                return;
            }

            int x = Random.Range(0, clipList.Count - 1);
            channels[freeChannelId].clip = clipList[x]; //nejauśs klips, kas atrodas zem śí nosaukuma
            channels[freeChannelId].Play();

        }
        else
        {
            if (Debug.isDebugBuild)
            {
                Debug.LogError("SoundManager::Sound " + clipName + " not found");
            }
            return;
        }

    }


    private void _ChangeAmbienceForLevel(string levelName)
    {
        //print("SoundManager::_ChangeAmbienceForLevel: " + levelName);
        int worldNum = -1;

        if (levelName.Contains("a___"))
        { //SP

            try
            {
                string numbers = levelName.Replace("a___", "");
                if (numbers.Length > 3)
                {
                    numbers = numbers.Remove(3);
                }
                int num = int.Parse(numbers, System.Globalization.CultureInfo.InvariantCulture);

                worldNum = (num - 1) / 10; //  (num-1) =>  9. un 10. trase ir viená pasaulé, 11. ir jau nákamajá
                worldNum++; // lai 1-10 bútu pirmá nevis nulltá pasaule
            }
            catch
            {

                Debug.LogWarning("kreiss liimenis, nebuus pareiza muuzika");
                worldNum = -1;
            }

        }
        else if (levelName.Contains("a_mp_"))
        { //MP

            switch (levelName)
            {
                case "a_mp_001":
                    worldNum = 3;
                    break;
                case "a_mp_002":
                    worldNum = 1;
                    break;
                case "a_mp_003":
                    worldNum = 4;
                    break;
                case "a_mp_004":
                    worldNum = 2;
                    break;
                case "a_mp_005":
                    worldNum = 1;
                    break;
                case "a_mp_006":
                    worldNum = 7;
                    break;
                case "a_mp_007":
                    worldNum = 2;
                    break;
                case "a_mp_008":
                    worldNum = 6;
                    break;
                case "a_mp_009":
                    worldNum = 5;
                    break;
                case "a_mp_010":
                    worldNum = 4;
                    break;
                case "a_mp_011":
                    worldNum = 8;
                    break;
                case "a_mp_012":
                    worldNum = 5;
                    break;
                default:
                    if (Debug.isDebugBuild)
                    {
                        Debug.LogError("SoundManager::Nezināms MP līmenis");
                    }
                    break;

            }
        }
        else
        {
            if (Debug.isDebugBuild)
            {
                Debug.LogError("SoundManager::Neidentificējams līmeņa nosaukums");
            }
        }


        //uzzináts pasaules numurs -- tas jápártulko uz ambiences nosaukumu
        switch (worldNum)
        {
            case 1:
                _ChangeAmbienceTo("mountain");
                break;
            case 2:
                _ChangeAmbienceTo("jungle");
                break;
            case 3:
                _ChangeAmbienceTo("desert");
                break;
            case 4:
                _ChangeAmbienceTo("arctic");
                break;
            case 5:
                _ChangeAmbienceTo("mountain");
                break;
            case 6:
                _ChangeAmbienceTo("jungle_thunder");
                break;
            case 7:
                _ChangeAmbienceTo("space");
                break;
            case 8:
                _ChangeAmbienceTo("space");
                break;
            case 9:
                _ChangeAmbienceTo("space");
                break;
            default:
                if (Debug.isDebugBuild)
                {
                    Debug.LogWarning("SoundManager::nebūs ambiences");
                }
                break;

        }


    }

    private void _ChangeAmbienceTo(string worldName)
    {

        //print("SoundManager::_ChangeAmbienceTo: " + worldName);
        LastAmbience = CurrentAmbience;
        CurrentAmbience = worldName;

        _StopAmbience();
        Clips["Ambience"] = new List<AudioClip>(); // noresetoju ieládédot klipus (visas abmiences iet zem viena aliasa) + lai GC varétu tos aizvákt (nav garantéts, iespéjams, ka ieládétás skańas netiek izváktas)

        switch (worldName)
        {
            case "jungle":
                LoadClip("Ambience", "jungle_background"); //pirmais vienmér ir ilgstośais klips
                LoadClip("Ambience", "jungle1");// páréjie (if any) ir periodiskie atkártojamie efekti

                //LoadClip("Ambience", "Ambience/jungle_background"); //pirmais vienmér ir ilgstośais klips
                //LoadClip("Ambience", "Ambience/jungle1");// páréjie (if any) ir periodiskie atkártojamie efekti
                break;

            case "mountain":
                LoadClip("Ambience", "mountain");

                //LoadClip("Ambience", "Ambience/mountain");
                break;

            case "desert":
                LoadClip("Ambience", "desert");

                //LoadClip("Ambience", "Ambience/desert");
                break;

            case "arctic":
                LoadClip("Ambience", "arctic");

                //LoadClip("Ambience", "Ambience/arctic");
                break;

            case "jungle_thunder":
                LoadClip("Ambience", "jungle_background");
                LoadClip("Ambience", "thunder");

                //LoadClip("Ambience", "Ambience/jungle_background");
                //LoadClip("Ambience", "Ambience/thunder");
                break;

            case "space":
                LoadClip("Ambience", "space_background");
                LoadClip("Ambience", "space1");
                LoadClip("Ambience", "space2");
                LoadClip("Ambience", "space3");

                //LoadClip("Ambience", "Ambience/space_background");
                //LoadClip("Ambience", "Ambience/space1");
                //LoadClip("Ambience", "Ambience/space2");
                //LoadClip("Ambience", "Ambience/space3");
                break;



            //menjuchu dziesmas:
            case "garage":
                LoadClip("Ambience", "garage_background");
                LoadClip("Ambience", "garage1");
                LoadClip("Ambience", "garage2");
                LoadClip("Ambience", "garage3");

                //LoadClip("Ambience", "Ambience/garage_background");
                //LoadClip("Ambience", "Ambience/garage1");
                //LoadClip("Ambience", "Ambience/garage2");
                //LoadClip("Ambience", "Ambience/garage3");
                break;

            case "theme":
                LoadClip("Ambience", "theme");
                //LoadClip("Ambience", "Ambience/theme");
                break;

            default:
                Debug.LogError("Pieprasīta ambience nezināmai pasaulei: " + worldName);
                return;
        }

        StartCoroutine("PlayAmbienceMain");
        StartCoroutine("PlayAmbiencePeriodic");

    }

    /**
     * śis tiks izsaukts katru reizi mainot ekránu
     * nomainís múzíku (ambienci), ja pieprasítá jau neskan
     * (waitlistéti tikai ekráni, kuros ir jámaina múzika)
     */
    private void _ChangeAmbienceForMenu()
    {
        //print("SoundManager::Ambience for screen: " + UIManager.currentScreenType);
        string newAmbience = "";


        //atkártoti ejot INTRO ekráná - parastá dziesmińa
        if (UIManager.currentScreenType == GameScreenType.Menu && !firstTimeMenu)
        {
            newAmbience = "theme";
        }
        //tikai pirmo reizi esot INTRO - ípaśá dziesminja
        if (UIManager.currentScreenType == GameScreenType.Menu && firstTimeMenu)
        {
            firstTimeMenu = false;
            newAmbience = "theme";
        }


        //ieejot GARÁŹÁ
        if (UIManager.currentScreenType == GameScreenType.Garage)
        {
            newAmbience = "garage";
        }

        //izejot no BRAUCIENA (sp un mp)
        if (UIManager.currentScreenType == GameScreenType.Levels || UIManager.currentScreenType == GameScreenType.MultiplayerMenu)
        {
            newAmbience = "theme";
        }

        //parasti te ir péc garáźas - bús vai nu trases chúska vai brauciens - tá kas ielikta ká iepriekśéjá ambience
        if (UIManager.currentScreenType == GameScreenType.Last)
        { // "Last screen" is so 90s
            newAmbience = LastAmbience;
        }


        if (CurrentAmbience != newAmbience && newAmbience.Length > 0)
        { //ja ir jáatskańo CITA ambience, tikai tad nomaina
            _ChangeAmbienceTo(newAmbience);
        }

    }

    private void _StopAmbience()
    {

        //print("SoundManager::StopAmbience??");

        StopCoroutine("PlayAmbienceMain");
        StopCoroutine("PlayAmbiencePeriodic");

        //apturu skanjas abos kanálos, ko lietojuśas ambiences
        channels[ambienceChannellA].Stop();
        channels[ambienceChannellB].Stop();

    }
    private void _StartAmbience()
    {
        StartCoroutine("PlayAmbienceMain");
        StartCoroutine("PlayAmbiencePeriodic");
    }

    //aptur visus skańu kanálus (iznjemot múziku)
    private void _StopSfx()
    {
        Debug.LogError("Sound stops from script");
        for (int i = 0; i < numChannels; i++)
        {
            if (channels[i].isPlaying)
            {
                channels[i].Stop();
            }
        }
    }


    /**
     * ieládé skanjas klipu
     * 
     * alias - ká spélé sauks śo skańas klipu
     * path - ká saucas skańas fails (zem Sounds/ diras)
     */
    private void LoadClip(string alias, string path)
    {

        //vai śai skańai nav káds klips ieládéts

        if (!Clips.ContainsKey(alias))
        {
            Clips[alias] = new List<AudioClip>(); //nav neviens klips - jáizveido jauna klipu liste
        }
        //Debug.Log(alias + "Audio loading from = " + path);
        AudioClip c;
        //if (LoadAddressable_Vasundhara.Instance != null)
        //{

        c = loadAddressable_Vasundhara.GetAudio_Resources(path);
        Debug.Log(c.name);

        //AudioClip c = Resources.Load("Resources_moved/Sounds" + path) as AudioClip;

        if (c == null)
        {
            if (Debug.isDebugBuild)
            {
                Debug.LogError("Sound \"" + path + "\" not found");
            }
        }
        else
        {
            Clips[alias].Add(c);
        }
        //}
    }



    private int GetFreeChannel()
    {

        for (int i = lastUsedChannel; i < numChannels; i++)
        {//sák meklét no pédéjá atrastá ..
            if (!channels[i].isPlaying)
            {
                lastUsedChannel = i;
                return i;
            }
        }
        for (int i = 0; i < numChannels; i++)
        { // .. ja visi kanaali, liidz beigaam, pilni, saak no saakuma
            if (!channels[i].isPlaying)
            {
                lastUsedChannel = i;
                return i;
            }
        }
        return -1; //neatrada
    }


    /**
     * atkártoti atskańos galveno klipu no śís pasaules ambiences (AudioSource komponentiem nav ieslégts loop un śádi ir vienkárśák)
     * śo var pártraukt tikai ar ko-rutínas nokillośanu
     */
    private IEnumerator PlayAmbienceMain()
    {

        while (true)
        {


            channels[ambienceChannellA].clip = Clips["Ambience"][0]; //atskańo ambiences galveno klipu
            channels[ambienceChannellA].Play();
            //print("SoundManager::Ambiance::Play::Main" + " chanel:" + freeChannelId);					


            if (Clips["Ambience"].Count == 0 || Clips["Ambience"][0] == null)
            {//atskanjośanas laiká dziesminjaa ir pazudusi
             //print("SoundManager::Ambiance::ThisJustHappen");
                yield break;
            }

            yield return new WaitForSeconds(Clips["Ambience"][0].length); //péc klipa beigám, visu atkártos
                                                                          //print("SoundManager::Ambiance::Play::Over");
                                                                          //print("SoundManager::Ambiance::After::Main::" + (Clips["Ambience"][0].length) );

        }
    }


    private IEnumerator PlayAmbiencePeriodic()
    {

        if (Clips["Ambience"].Count <= 1)
        {
            yield break; //śai ambiencei nav periodisko skanju
        }

        int times = 0;

        while (true)
        {

            int x = Random.Range(1, Clips["Ambience"].Count - 1); //nejauśa viena periodiská skańa

            float justWait;
            if (times == 0)
            { //pirms pirmás atskańośanas pagaida ilgák
                justWait = Random.Range(2, 4);
            }
            else
            {
                justWait = Random.Range(0.5f, 2) * Clips["Ambience"][x].length; //páréjás reizés pagaídís 1/2 lídz 2x no klipa garuma
            }

            yield return new WaitForSeconds(justWait);
            //print("SoundManager::Ambiance::Play::Periodic:" + x + "  wait:" + justWait + " chanel:" + freeChannelId);


            channels[ambienceChannellB].clip = Clips["Ambience"][x]; //atskanjo nejauśu ambiences periodisko klipu
            channels[ambienceChannellB].Play();

            yield return new WaitForSeconds(Clips["Ambience"][x].length); //péc klipa beigám, visu atkártos

            times++;
        }
    }


    private float _GetClipLength(string clipName)
    {
        return Clips[clipName][0].length; //no safety >:)
    }

    private void Start()
    {
        //if (SkidosUI.Instance.IsEnabled())
        //{
        //    skidosStatus = true;
        //}
        //else
        //    skidosStatus = false;
        if (BikeDataManager.SettingsMusic)
        {
            ambiencePlaying = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

        AudioSource audio = GetComponent<AudioSource>();

        //if (audio != null)
        //{

        //    channels[ambienceChannellA].volume = 1f;
        //    //StartAmbience();

        //    audio.volume = 1;
        //    //MasterAudio.UnmuteEverything();
        //}
        //skidosStatus = true;
        //if (SkidosUI.Instance.IsEnabled() && skidosStatus == true)
        //{
        //    if (audio != null)
        //    {

        //        //StopAmbience();
        //        channels[ambienceChannellA].volume = 0f;

        //        audio.volume = 0;
        //        //MasterAudio.MuteEverything();
        //    }
        //    skidosStatus = false;
        //}

        //else if (!SkidosUI.Instance.IsEnabled() && skidosStatus == false)
        //{

        //}
    }

}







}
