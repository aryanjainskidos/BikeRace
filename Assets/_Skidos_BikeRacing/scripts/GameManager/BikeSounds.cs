namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

/**
 * Pieskata Baika dzinéja skańas, lietojot FSM
 * 
 * pieder BikeSounds objektam (prefabam, kas palikts zem kameras)
 * svarígi nonuljljot pozícijas vektoru
 * śim objektam pieder arí 2 AudioSource komponenti ar LOOP=TRUE un PLAY_ON_AWAKE=TRUE
 * 
 * AudioClip vietás saliek failińus
 * 
 * 
 */

public class BikeSounds : MonoBehaviour
{

    //inspektorá norádíti skańu faili
    [Header("Tikai baika motora/uzvedības savstarpēji izslēdzošās skaņas")]
    public AudioClip SIdle;
    public AudioClip SFast;
    public AudioClip SFastLoop;
    public AudioClip SSlow;
    public AudioClip SSlowLoop;
    public AudioClip SJump;
    public AudioClip SBreak;
    public AudioClip SAfterCrash; //nevis pati kreśa skańa, bet lúpojamais péc kreśa
    public AudioClip SBubbles;


    private BikeCamera bikeCamera;
    private BikeControl bikeControl;
    private RiderAnimationControl riderAnimationControl = null;
    private bool noRiderAnimationControl = false;
    private bool triedReseting = false;
    AudioSource[] audioSources; //objektam piederośi komponenti;  2 gab, lai varétu krosfeidot
    int activeAudioSource = 0; // 0 vai 1  -- kurś audiosource ir pédéjais lietotais
    AudioClip activeClip = null; //kurś klips paślaik skan (ja krosfeido - tad kurś skanés)


    private bool needBikeAudio = false;
    private bool initialized = false;
    private int endLoopId = 0;

    private bool firstGear = true;

    private BikeTriggerCollision frontWheelTrigger;
    private BikeTriggerCollision backWheelTrigger;


    [Header("Config")]
    public int speedToUpShift = 25; //pie káda átruma *gaisá* párslégs uz 2. párnesumu
    public int speedToDownShift = 15; //pie káda átruma ir jápárslédz atpakaĺ uz 1. parnesumu
    [Space(10)]
    [Range(0.01f, 0.2f)]
    public float XFadePeriod = 0.05f; //cik sekundes ilgi krosfeidot  | ap 0.05 ir ok; liekot kaut ko lieláku, var rasties spoku skańas :D
    [Space(10)]
    [Range(0.01f, 0.5f)]
    public float minJumpAirtime = 0.1f; //cik ilgi jábút gaisá, lai sáktu atskańot ESAM-GAISÁ skanju | BikeControl skriptá ir hárdkodéta lídzíga vértíba lai noteiktu min lidojuma ilgumu, péc kura spélét piezeméśanás skanju
    public float minJumSpeed = 10f; //cik átri ir jábrauc, lai gaisá esot sáktu atskańot ESAM-GAISÁ skanju

    //śo izsauc LoadLevel funkcijas paśás beigás - kad mocim jásák rúkt
    public void Init()
    {
        initialized = false;
        noRiderAnimationControl = false;

        bikeCamera = Camera.main.GetComponent<BikeCamera>();
        if (bikeCamera.target == null)
        {
            return;  //no bike
        }


        backWheelTrigger = bikeCamera.target.transform.Find("wheel_back/wheel_back_trigger").GetComponent<BikeTriggerCollision>();
        frontWheelTrigger = bikeCamera.target.transform.Find("wheel_front/wheel_front_trigger").GetComponent<BikeTriggerCollision>();

        bikeControl = bikeCamera.target.GetComponent<BikeControl>();
        audioSources = GetComponents<AudioSource>();


        needBikeAudio = true;
        initialized = true;
        firstGear = true;
    }

    void Update()
    {
        //print("BikeSounds::Screen=" + UIManager.currentScreenType.ToString());
        if (!initialized)
        {
            return;
        }

        //if (SkidosUI.Instance.IsEnabled())
        {
            //StopEverything();
            //return;
        }

        //tikai braukśanas ekrános vajag skańu
        //print("@todo -- BikeSounds:: waitlisteet pareizos MP braukshanas ekraanus");
        if (UIManager.currentScreenType == GameScreenType.Game || UIManager.currentScreenType == GameScreenType.MultiplayerGame
           || UIManager.currentScreenType == GameScreenType.PreGame || UIManager.currentScreenType == GameScreenType.PreGamePause || UIManager.currentScreenType == GameScreenType.MultiplayerPreGame
           || UIManager.currentScreenType == GameScreenType.PostGame
           || UIManager.currentScreenType == GameScreenType.MultiplayerPostGameFriend
           || UIManager.currentScreenType == GameScreenType.MultiplayerPostGameLeague
           || UIManager.currentScreenType == GameScreenType.MultiplayerPostGameRevanche
           || UIManager.currentScreenType == GameScreenType.Pause || UIManager.currentScreenType == GameScreenType.MultiplayerPause
           || UIManager.currentScreenType == GameScreenType.Crash || UIManager.currentScreenType == GameScreenType.MultiplayerCrash
           )
        {
            needBikeAudio = true;
        }
        else
        {
            needBikeAudio = false;
        }

        if (!BikeDataManager.SettingsSfx)
        {
            needBikeAudio = false;
        }

        //nedod skańu, kamér límenis nav ieládéts (ekráns ir pareizs - bet pa virsu tam ir loading popups)
        if (!BikeGameManager.initialized)
        {
            needBikeAudio = false;
        }

        if (!needBikeAudio)
        {
            StopEverything();
            return;
        }


        if (backWheelTrigger == null)
        { //brauciena vidú ir nomainíts baiks - jáatrod par jaunu
            if (!triedReseting)
            {
                Init();
                triedReseting = true; //lai vairs neméǵinátu resetot, jo acímredzami, kaut kas nav labi
            }
        }
        else
        {
            triedReseting = false;//lai nákamreiz, kad vajag, méǵinátu resetot
        }


        //------ne-FSM skanjas-------------
        //ir tikko piezeméjies, bet ne údení (un nav miris) (un kontroles skripts ieslégs - tátad spélétájs kontrolé)
        if (bikeControl != null && bikeControl.enabled && !BikeGameManager.playerState.dead && bikeControl.justlanded && backWheelTrigger.collName != "WaterZone" && frontWheelTrigger.collName != "WaterZone")
        {
            SoundManager.Play("BikeLanding");
        }



        if (riderAnimationControl == null && !noRiderAnimationControl)
        { //nav braucéja skripts && nav teikts, lai to nemeklé
          //Debug.Log("Hmm, riderAnimationControl skripts pazudis - varbút player_parts ir noresetots");
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                riderAnimationControl = GameObject.FindGameObjectWithTag("Player").transform.Find("Player_parts").GetComponent<RiderAnimationControl>();
            }
            if (riderAnimationControl == null)
            { //neizdevás atrast, bet nemeklés vairák kamér neieládés par jaunu - varbút tagad ir MP replejs or smtn
                noRiderAnimationControl = true;
                if (Debug.isDebugBuild)
                {
                    print("nebúst stuntu skańju");
                }
            }
        }

        //tikko sákts triks
        if (riderAnimationControl != null && riderAnimationControl.trickJustStarted)
        {
            SoundManager.Play("Trick");
        }





        //átrumpárslégśana ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑ (tikai gaisá)
        if (bikeControl.fly)
        {
            if (firstGear && bikeControl.bodyVelocity > speedToUpShift)
            { //gaisá párslédz 2. párnesumá, ja ir gana liels átrums
                firstGear = false;
            }
        }

        //átrumpárslégśana  ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
        if (!firstGear && bikeControl.bodyVelocity < speedToDownShift)
        {
            firstGear = true;
        }


        //ja ir nokreśojis, tad dod lúpojamu náves skańu - sirdspukstus
        if (BikeGameManager.playerState.dead)
        {
            if (BikeGameManager.playerState.drowned)
            {
                PlayThisInstead(SBubbles);
            }
            else
            {
                PlayThisInstead(SAfterCrash);
            }
            return;
        }


        //pauzes u.c. logi, kad vajag IDLE skańu nevis patieso
        if (UIManager.currentScreenType == GameScreenType.PreGame || UIManager.currentScreenType == GameScreenType.PreGamePause || UIManager.currentScreenType == GameScreenType.MultiplayerPreGame
           || UIManager.currentScreenType == GameScreenType.PostGame
           || UIManager.currentScreenType == GameScreenType.MultiplayerPostGameFriend
           || UIManager.currentScreenType == GameScreenType.MultiplayerPostGameLeague
           || UIManager.currentScreenType == GameScreenType.MultiplayerPostGameRevanche
           || UIManager.currentScreenType == GameScreenType.MultiplayerPostGameReplay
           || UIManager.currentScreenType == GameScreenType.Pause || UIManager.currentScreenType == GameScreenType.MultiplayerPause
           )
        {
            //print("play idle in " + UIManager.currentScreenType.ToString());
            PlayThisInsteadForced(SIdle);
            return;
        }

        //----visiem turpmákajiem FSM keisiem jábút savstarpéji izslédzośiem un pareizá secíbá (lai nebútu salikti IFi járaksta ;)


        //uz zemes un bremzé (kamér ir átrums = pakaljéjais rats nestáv uz vietas ) 
        if (bikeControl.InputTouchLeft && !bikeControl.fly && Mathf.Abs(bikeControl.wheelB.GetComponent<Rigidbody2D>().linearVelocity.x) > 0.5f)
        {
            PlayThisInstead(SBreak);
            return;
        }

        //uz zemes un negázé [po:/nebremzé]
        if (!bikeControl.InputTouchRight /*&& !bikeControl.InputTouchLeft */ && !bikeControl.fly)
        {
            PlayThisInstead(SIdle);
            return;
        }


        //gaisá
        if (bikeControl.fly && bikeControl.airtime > minJumpAirtime && bikeControl.bodyVelocity > minJumSpeed)
        {
            PlayThisInstead(SJump);
            return;
        }

        //uz zemes, gázé, nebremzé
        if (bikeControl.InputTouchRight && !bikeControl.InputTouchLeft && !bikeControl.fly)
        {
            if (firstGear)
            {
                PlayThisInstead(SSlow);
            }
            else
            {
                PlayThisInstead(SFast);
            }
            return;
        }



    }

    private void PlayThisInstead(AudioClip newClip)
    {
        if (activeClip != newClip)
        { //ja jau neskan izvélátá skańa
          //if(activeClip!=null){print("BikeSounds::OldSound=" + activeClip.name);}
          //print("BikeSounds::NewSound=" + newClip.name);

            activeClip = newClip;
            //StopCoroutine("XFadeAudioSources");
            StartCoroutine(QueueEndLoopSound(newClip));

            int nextAudioSource = 1 - activeAudioSource; // swapo 1 ⇄ 0
            audioSources[nextAudioSource].clip = newClip; //nákamajá audioSourcé ieliek vélamo failińu
                                                          //audioSources[activeAudioSource].Stop();


            StartCoroutine(XFadeAudioSources());//korutína apklusinás tagadéjo audioSourci un uzgriezís skaĺák nákamo (un nomainís skaitítáju - aktívo audioSourci)

            //vajadzígs 1x sesijá 
            //audioSources[0].Play();
            audioSources[nextAudioSource].Play();

        }
    }

    //bez X-feidinga; nokillo X-Feidinga ko-rutínu
    private void PlayThisInsteadForced(AudioClip newClip)
    {
        if (activeClip != newClip || !audioSources[0].isPlaying)
        { //ja izvélétais klips nav zlikts ká aktívais || vai arí ir uzlikts, bet vnk nogriezts 

            //if(activeClip!=null){print("BikeSounds::FORCE::OldSound=" + activeClip.name);}
            //print("BikeSounds::FORCE::NewSound=" + newClip.name);


            activeClip = newClip;
            //StopCoroutine("XFadeAudioSources");
            ///StartCoroutine(QueueEndLoopSound(newClip));

            activeAudioSource = 0;
            audioSources[0].clip = newClip;
            audioSources[0].volume = 1;

            audioSources[1].clip = newClip;
            audioSources[1].volume = 0;


            //vajadzígs 1x sesijá 
            audioSources[0].Play();
            audioSources[1].Play();
        }
    }


    private IEnumerator XFadeAudioSources()
    {
        float fTimeCounter = 0f;

        int nextAudioSource = 1 - activeAudioSource; // swapo 1 ⇄ 0

        /*string s = "BikeSounds::CHANGE ... MUSIC!  " +activeAudioSource+"<>"+nextAudioSource + "  ";
		if(audioSources[activeAudioSource].clip != null) {s += audioSources[activeAudioSource].clip.name; }
		s += " => ";
		if(audioSources[nextAudioSource].clip != null) {s += audioSources[nextAudioSource].clip.name; }
		print(s);*/


        while (!(Mathf.Approximately(fTimeCounter, 1f)))
        { //kamér nav 1 [clamp nodrośina, ka bús viens]
            fTimeCounter = Mathf.Clamp01(fTimeCounter + Time.unscaledDeltaTime * (1 / XFadePeriod)); //XFadePeriod ir sekundés, parasti kádas 0.05, te paŕvérśu uz "cik reizes sekundé"
            audioSources[activeAudioSource].volume = 1f - fTimeCounter;
            audioSources[nextAudioSource].volume = fTimeCounter;

            //			print("V("+activeAudioSource+")=" + audioSources[activeAudioSource].volume);
            //			print("V("+nextAudioSource+")=" + audioSources[nextAudioSource].volume);
            yield return new WaitForSeconds(XFadePeriod / 4f);
        }


        activeAudioSource = nextAudioSource;
    }


    /**
	 * péc aktívás skańas klipa beigám nomainís klipu [aktívajá audioSourcé] uz izvélétás skańas predefinéto (if-any) end-loop klipu
	 */
    private IEnumerator QueueEndLoopSound(AudioClip clip)
    {
        //yield break;//stooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooop

        //StopCoroutine("QueueEndLoopSound");

        endLoopId++;
        int localEndLoopId = endLoopId;

        AudioClip endLoopClip = null;

        //hardcoding FTW
        if (clip == SSlow)
        {
            endLoopClip = SSlowLoop;
        }
        else if (clip == SFast)
        {
            endLoopClip = SFastLoop;
        }
        else if (clip == SJump)
        {
            endLoopClip = SIdle; //lecienam biegás lúpo idling
        }

        if (endLoopClip == null)
        {
            yield break;
        }

        yield return new WaitForSeconds(clip.length); //nogaida lídz skanjas klipa beigám
                                                      //print("BikeSounds::changed from " + clip.name + " to loopable " + endLoopClip.name + "  after " + clip.length + " sec");
        if (localEndLoopId == endLoopId)
        { //nomainís klipu uz lúpojamo klipu, ja vien śis ir pats pédéjais pieprasíjums 
            audioSources[activeAudioSource].clip = endLoopClip; //nomaina to uz izvéléto beigu lúpojamo klipińu
            audioSources[activeAudioSource].Play(); //dunno vai vajag
        }
        else
        {
            //print("skip");
        }


    }

    //negaranté, ka aptur visu :\
    private void StopEverything()
    {
        Debug.LogError("Every Sound is stopped");
        //	Debug.Log("BIKESOUNDS STOP EVERYTHING!!!!");
        endLoopId++;
        //StopCoroutine("XFadeAudioSources");
        //StopCoroutine("QueueEndLoopSound");

        if (audioSources[0] != null && audioSources[0].isPlaying)
        {
            audioSources[0].Stop();
            audioSources[0].volume = 0;
        }
        if (audioSources[1] != null && audioSources[1].isPlaying)
        {
            audioSources[1].Stop();
            audioSources[1].volume = 0;
        }


    }

}




}
