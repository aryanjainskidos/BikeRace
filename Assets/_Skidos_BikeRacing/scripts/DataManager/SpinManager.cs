namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using Random = UnityEngine.Random;

public enum SpinPrizeType
{
    None = 0,//never
    CoinsX100 = 1,//never
    CoinsX500 = 2,
    CoinsX1000 = 3,
    CoinsX2500 = 4,
    CoinsX100000 = 5,//once
    CupsX100 = 6,
    CupsX200 = 7,
    BikeBeach = 8,//Beach, Tourist, 200 MP cups
    BikeTourist = 9,//Beach, Tourist, 200 MP cups
}

public class SpinManager
{

    const float MINUTES_TO_KEEP_PROGRESS_IF_INACTIVE = 10;//this is how long you can be inactive without losing progress

    //0, 2, 2, 5, 10, 30
    static SpinEntity[] firstSpinProgression;
    //0, 3, 5, 10, 30
    static SpinEntity[] defaultSpinProgression;
    //jackpot has it's own logic

    public static Action onReward;

    static bool saveQuitTimestamp = true;

    public static SpinPrizeType prize; //NB pay attention that GetReward gets called, otherwise this will always stay the same

    public static bool initialized = false;

    static SpinEntity[] activeProgression;

    public static bool spinInProgress = false;

    public static int spinSession = 0;
    static int spinProgress = 0;//don't use directly
    static DateTime lastSpinTimestamp;

    public static int SpinProgress
    {
        get
        {
            int tmp = 0;
            if (activeProgression != null)
            {
                tmp = spinProgress < activeProgression.Length ?
                    spinProgress : activeProgression.Length - 1;//if larger than the array, reset to the last element
            }
            return tmp;
        }
        set
        {
            spinProgress = value;
        }
    }

    public static void DefineSpins()
    {
        firstSpinProgression = new SpinEntity[] {
            new SpinEntity(0, new SpinPrizeType[]{ SpinPrizeType.CoinsX1000 } ),
            new SpinEntity(2f, new SpinPrizeType[]{ SpinPrizeType.CoinsX2500 } ),
            new SpinEntity(2f, new SpinPrizeType[]{ SpinPrizeType.CoinsX1000 } ),
            new SpinEntity(5f, new SpinPrizeType[]{ SpinPrizeType.BikeBeach } ),
            new SpinEntity(10f, new SpinPrizeType[]{ SpinPrizeType.CoinsX2500 } ),
            new SpinEntity(30f, new SpinPrizeType[]{ SpinPrizeType.CoinsX1000, SpinPrizeType.CoinsX2500, SpinPrizeType.CupsX100 } )
        };

        defaultSpinProgression = new SpinEntity[] {
            new SpinEntity(0, new SpinPrizeType[]{ SpinPrizeType.CoinsX1000, SpinPrizeType.CoinsX2500 } ),
            new SpinEntity(3f, new SpinPrizeType[]{ SpinPrizeType.CoinsX2500, SpinPrizeType.CupsX100, SpinPrizeType.CoinsX1000 } ),
            new SpinEntity(5f, new SpinPrizeType[]{ SpinPrizeType.BikeBeach, SpinPrizeType.BikeTourist, SpinPrizeType.CupsX200 }, false ),
            new SpinEntity(10f, new SpinPrizeType[]{ SpinPrizeType.CoinsX2500, SpinPrizeType.CoinsX500 } ),
            new SpinEntity(30f, new SpinPrizeType[]{ SpinPrizeType.CoinsX1000, SpinPrizeType.CoinsX2500, SpinPrizeType.CupsX100 } )
        };
    }

    //TODO MP cups only given if mp is unlocked

    const string QUIT_TIMESTAMP_KEY = "QuitTimestamp";
    const string SPIN_TIMESTAMP_KEY = "SpinTimestamp";
    const string PAUSE_TIMESTAMP_KEY = "PauseTimestamp";

    const string SPIN_SESSION_KEY = "SpinSession";
    const string SPIN_PROGRESS_KEY = "SpinProgress";

    public const string SPIN_TILL_SKIP_KEY = "SpinTillSkip";

    const string SPIN_SINCE_JACKPOT_KEY = "SpinSinceJackpot";

    static int giveJackpotAtSpin = 34;  // >
    static int giveJackpotAtSpinLastChance = 45; // ==

    //check quit timestamp on init
    public static void Init()
    {
        //Debug.Log("SpinManager::Init");

        spinSession = 0;
        if (PlayerPrefs.HasKey(SPIN_SESSION_KEY))
        {
            spinSession = PlayerPrefs.GetInt(SPIN_SESSION_KEY);
        }

        //Debug.Log("spinSession " + spinSession + " " + PlayerPrefs.HasKey( SPIN_SESSION_KEY ) );
        //Debug.Log("PlayerPrefs.HasKey( QUIT_TIMESTAMP_KEY ) " + PlayerPrefs.HasKey( QUIT_TIMESTAMP_KEY ) );

        if (!PlayerPrefs.HasKey(QUIT_TIMESTAMP_KEY))
        {
            OnPause(false);
        }

        if (spinSession > 0)
        {
            if (PlayerPrefs.HasKey(QUIT_TIMESTAMP_KEY))
            {
                CheckTimestamp(PlayerPrefs.GetString(QUIT_TIMESTAMP_KEY));
                PlayerPrefs.DeleteKey(QUIT_TIMESTAMP_KEY);
            }
            else
            {
                //Debug.LogWarning(QUIT_TIMESTAMP_KEY + " absent");
            }
            //default progression
            activeProgression = defaultSpinProgression;
        }
        else
        {
            //init first spin progression
            if (PlayerPrefs.HasKey(QUIT_TIMESTAMP_KEY))
            {
                CheckTimestamp(PlayerPrefs.GetString(QUIT_TIMESTAMP_KEY));
                PlayerPrefs.DeleteKey(QUIT_TIMESTAMP_KEY);
            }
            activeProgression = firstSpinProgression;
        }

        initialized = true;
    }

    public static bool CanHazSpin()
    {

        bool value = false;

        if (initialized)
        {
            TimeSpan timeSinceSpin = GetTimeSinceSpin();

            if (timeSinceSpin.TotalMinutes >= activeProgression[SpinProgress].time)
            {
                value = true;
            }
        }

        //        if (!value)
        //        {
        //            Debug.Log("Can't has che... spin");
        //        }

        return value;
    }

    public static TimeSpan GetTimeSinceSpin()
    {
        return lastSpinTimestamp != null ? DateTime.Now - lastSpinTimestamp : TimeSpan.FromMinutes(0);
    }

    public static TimeSpan GetTimeTillSpin()
    {

        TimeSpan timeSinceSpin = GetTimeSinceSpin();

        float minutesTillSpin = activeProgression[SpinProgress].time;

        return (timeSinceSpin.TotalMinutes <= minutesTillSpin) ? TimeSpan.FromMinutes(minutesTillSpin).Subtract(timeSinceSpin) : TimeSpan.FromMinutes(0);
    }

    public static void GivePrize()
    {

        switch (prize)
        {
            case SpinPrizeType.BikeBeach:
                BikeDataManager.Styles[(int)BikeStyleType.Beach].Locked = false;
                break;
            case SpinPrizeType.BikeTourist:
                BikeDataManager.Styles[(int)BikeStyleType.Tourist].Locked = false;
                break;
            case SpinPrizeType.CoinsX100:
                BikeDataManager.Coins += 100;
                break;
            case SpinPrizeType.CoinsX500:
                BikeDataManager.Coins += 500;
                break;
            case SpinPrizeType.CoinsX1000:
                BikeDataManager.Coins += 1000;
                break;
            case SpinPrizeType.CoinsX100000:
                BikeDataManager.Coins += 100000;
                break;
            case SpinPrizeType.CoinsX2500:
                BikeDataManager.Coins += 2500;
                break;
            case SpinPrizeType.CupsX100:
                MultiplayerManager.GiveCups(100);
                break;
            case SpinPrizeType.CupsX200:
                MultiplayerManager.GiveCups(200);
                break;
            default:
                break;
        }

        //TODO save the moto file?

        lastSpinTimestamp = DateTime.Now;

        //DONE save last spin timestamp
        PlayerPrefs.SetString(SPIN_TIMESTAMP_KEY, lastSpinTimestamp.ToString());
        PlayerPrefs.SetInt(SPIN_PROGRESS_KEY, ++SpinProgress);
        PlayerPrefs.Save();

        if (onReward != null)
        {
            onReward();
        }
    }

    public static SpinPrizeType GetPrize()
    { //TODO if this is to be called only once per spin, store the reward somewhere 

        //Debug.Log("GetPrize");

        prize = GetSpinPrize();

        //Debug.Log("You got " + System.Enum.GetName(typeof(SpinPrizeType), prize));

        return prize;
    }

    static SpinPrizeType GetSpinPrize()
    {
        SpinPrizeType prize = SpinPrizeType.None;
        List<SpinPrizeType> prizes = new List<SpinPrizeType>(activeProgression[SpinProgress].prizes);

        CleanPrizeList(ref prizes);

        if (prizes.Count == 0)
        {
            //take next levels prizes, this should be recursive or sth, but we only check the next level cause our case is not that genericand we rely on the data to be correct
            if (spinProgress < activeProgression.Length - 1)
            { //if next exists
                prizes = new List<SpinPrizeType>(activeProgression[spinProgress + 1].prizes);
                CleanPrizeList(ref prizes);
            }
        }

        if (prizes.Count > 0)
        {
            if (activeProgression[SpinProgress].random)
            {
                int prizeCount = prizes.Count;
                int randomPrizeIndex = Random.Range(0, prizeCount);

                prize = prizes[randomPrizeIndex];
            }
            else
            {
                //not random, in a sequence
                //right now only bikes use this feature
                prize = prizes[0];
            }
        }
        else
        {
            //well we tried to get some prizes, but i guess you're quite unlucky
            Debug.LogWarning("Two subsequent prize lists defined that have a potential to be empty, DISAPPOINTED!");
        }

        //DONE once after 34 and before 36 spins give the highest prize possible
        int spinsSinceJackpot = GetSpinsSinceJackpot();

        if (spinsSinceJackpot >= 0)
        {
            bool resetJackpotCounter = false;

            //if 45 reached without getting the prize give the prize now, otherwise flip a coin
            if (spinsSinceJackpot == giveJackpotAtSpinLastChance || (spinsSinceJackpot > giveJackpotAtSpin && Random.value > 0.5f))
            {
                prize = SpinPrizeType.CoinsX100000;
                resetJackpotCounter = true;
            }

            UpdateJackpotCounter(resetJackpotCounter); //reset stops the counting
        }

        return prize;
    }

    public static void UpdateJackpotCounter(bool reset = false)
    {
        int spinsSinceJackpot = GetSpinsSinceJackpot();

        if (reset)
            spinsSinceJackpot = -1;
        else
            spinsSinceJackpot++;

        PlayerPrefs.SetInt(SPIN_SINCE_JACKPOT_KEY, spinsSinceJackpot);
        PlayerPrefs.Save();
    }

    public static int GetSpinsSinceJackpot()
    {
        return PlayerPrefs.GetInt(SPIN_SINCE_JACKPOT_KEY, 0);
    }

    static void CleanPrizeList(ref List<SpinPrizeType> prizes)
    {
        //check if mp is not unlocked filter out the cup prizes
        if (!BikeDataManager.MultiplayerUnlocked)
        {
            for (int i = prizes.Count - 1; i >= 0; i--)
            {
                if (prizes[i] == SpinPrizeType.CupsX100 ||
                    prizes[i] == SpinPrizeType.CupsX200)
                {
                    prizes.RemoveAt(i);
                }
            }
        }

        for (int i = prizes.Count - 1; i >= 0; i--)
        {

            //if beach bike already unlocked, remove it
            if (prizes[i] == SpinPrizeType.BikeBeach &&
                !BikeDataManager.Styles[(int)BikeStyleType.Beach].Locked)
            {
                prizes.RemoveAt(i);
            }

            //if tourist bike already unlocked, remove it
            if (i < prizes.Count && //can be removed already
                prizes[i] == SpinPrizeType.BikeTourist &&
                !BikeDataManager.Styles[(int)BikeStyleType.Tourist].Locked)
            {
                prizes.RemoveAt(i);
            }

        }

        //Debug.Log("Prize Pool:");
        //        for (int i = 0; i < prizes.Count; i++)
        //        {
        //Debug.Log(i + " " + System.Enum.GetName(typeof(SpinPrizeType), prizes[i]));
        //        }
        //Debug.Log("-----------");
    }

    public static void OnQuit()
    {
        //save quit timestamp
        if (saveQuitTimestamp)
        {
            PlayerPrefs.SetString(QUIT_TIMESTAMP_KEY, DateTime.Now.ToString());
            PlayerPrefs.SetInt(SPIN_PROGRESS_KEY, SpinProgress);
            PlayerPrefs.SetInt(SPIN_SESSION_KEY, spinSession);
            PlayerPrefs.DeleteKey(PAUSE_TIMESTAMP_KEY);
            PlayerPrefs.Save();
            //Debug.Log("Quit " + PlayerPrefs.GetString( QUIT_TIMESTAMP_KEY ));
        }
    }

    public static void OnPause(bool paused)
    {
        //save timestamp if paused
        if (paused)
        {
            PlayerPrefs.SetString(PAUSE_TIMESTAMP_KEY, DateTime.Now.ToString());
            PlayerPrefs.SetInt(SPIN_PROGRESS_KEY, SpinProgress);
            PlayerPrefs.SetInt(SPIN_SESSION_KEY, spinSession);
            PlayerPrefs.Save();

            //Debug.Log("Paused " + paused + " " + PlayerPrefs.GetString(PAUSE_TIMESTAMP_KEY));
        }
        else
        {
            //check timestamp if unpaused
            if (PlayerPrefs.HasKey(PAUSE_TIMESTAMP_KEY))
            {
                CheckTimestamp(PlayerPrefs.GetString(PAUSE_TIMESTAMP_KEY));
            }
        }
    }

    static void CheckTimestamp(string timestamp)
    {
        //Debug.Log("SpinManager::CheckTimestamp " + timestamp);

        DateTime timestampDate;
        bool validTimestamp = false;

        validTimestamp = DateTime.TryParseExact(timestamp, "G", CultureInfo.InvariantCulture, DateTimeStyles.None, out timestampDate);

        if (validTimestamp)
        {
            TimeSpan delta = DateTime.Now - timestampDate;

            if (delta.TotalMinutes >= MINUTES_TO_KEEP_PROGRESS_IF_INACTIVE)
            {
                CloseSession(); //close session if inactive for more than max minutes
            }
            else
            {
                ContinueSession();
            }

        }
        else
        {
            CloseSession();
        }
    }

    static void ContinueSession()
    {
        //load last spin timestamp
        //Debug.Log("You're safe");
        SpinProgress = PlayerPrefs.GetInt(SPIN_PROGRESS_KEY);
        spinSession = PlayerPrefs.GetInt(SPIN_SESSION_KEY);

        bool validSpinTimestamp = false;
        if (PlayerPrefs.HasKey(SPIN_TIMESTAMP_KEY))
        {
            validSpinTimestamp = DateTime.TryParseExact(PlayerPrefs.GetString(SPIN_TIMESTAMP_KEY), "G", CultureInfo.InvariantCulture, DateTimeStyles.None, out lastSpinTimestamp);
        }

        if (!validSpinTimestamp)
            lastSpinTimestamp = DateTime.Now;

        //        lastSpinTimestamp = PlayerPrefs.HasKey(SPIN_TIMESTAMP_KEY) ? DateTime.ParseExact( PlayerPrefs.GetString(SPIN_TIMESTAMP_KEY), "G", CultureInfo.InvariantCulture ) : DateTime.Now;

        //Debug.Log("ContinueSession: SpinProgress " + SpinProgress + " lastSpinTimestamp " + lastSpinTimestamp + " spinSession " + spinSession);
    }

    static void CloseSession()
    {
        //nuke everything
        //Debug.Log("Reseting progress");
        SpinProgress = 0;
        spinSession = PlayerPrefs.HasKey(SPIN_SESSION_KEY) ? PlayerPrefs.GetInt(SPIN_SESSION_KEY) : 0;
        spinSession++;
        lastSpinTimestamp = DateTime.Now;
        PlayerPrefs.SetInt(SPIN_PROGRESS_KEY, SpinProgress);
        PlayerPrefs.SetInt(SPIN_SESSION_KEY, spinSession);
        PlayerPrefs.DeleteKey(SPIN_TIMESTAMP_KEY);
        PlayerPrefs.Save();

        //Debug.Log("CloseSession: SpinProgress " + SpinProgress + " lastSpinTimestamp " + lastSpinTimestamp + " spinSession " + spinSession);
    }

    public static void CancelWaiting()
    {
        //put last spin timestamp further into the past, then the manager will think that it's time to spin again, if keeping all timestamps accurate were important an other solution might be necessary
        lastSpinTimestamp = lastSpinTimestamp.Subtract(GetTimeTillSpin()); //last timestamp - remaining time

        //save last spin timestamp
        PlayerPrefs.SetString(SPIN_TIMESTAMP_KEY, lastSpinTimestamp.ToString());
        PlayerPrefs.Save();

        //TODO record this event somehow, so that the skip waiting button knows when to show itself again
        //lastSpinWaitSkipped = true;
        PlayerPrefs.SetInt(SPIN_TILL_SKIP_KEY, 2);
        PlayerPrefs.Save();
    }

    public static void UpdateSkipCounter()
    {
        int spinsTillSkip = PlayerPrefs.GetInt(SPIN_TILL_SKIP_KEY, 0);
        if (spinsTillSkip > 0)
            spinsTillSkip--;
        PlayerPrefs.SetInt(SPIN_TILL_SKIP_KEY, spinsTillSkip);

        PlayerPrefs.Save();
    }

    public static int GetSpinsTillSkip()
    {
        return PlayerPrefs.GetInt(SPIN_TILL_SKIP_KEY, 0);
    }

    public static string GetSpinPrizeTypeName(SpinPrizeType type)
    {
        return System.Enum.GetName(typeof(SpinPrizeType), type);
    }

    public static string GetTelemetryMessage()
    {
        return "Spin completed with prize " + SpinManager.GetSpinPrizeTypeName(SpinManager.prize) + " session: " + spinSession + " spin progress in session: " + SpinProgress;
    }

    #region testing
    //will only get called if you maually check a checkbox in editor
    public static void Reset()
    {
        //Debug.Log("SpinManager::Reset");
        SpinProgress = 0;
        lastSpinTimestamp = DateTime.Now;
        //        PlayerPrefs.SetInt(SPIN_PROGRESS_KEY, 0);
        PlayerPrefs.DeleteKey(SPIN_PROGRESS_KEY);
        PlayerPrefs.DeleteKey(SPIN_SESSION_KEY);
        PlayerPrefs.DeleteKey(SPIN_TIMESTAMP_KEY);
        PlayerPrefs.DeleteKey(QUIT_TIMESTAMP_KEY);
        PlayerPrefs.DeleteKey(PAUSE_TIMESTAMP_KEY);
        PlayerPrefs.Save();

        saveQuitTimestamp = false;
    }
    #endregion
}

public class SpinEntity
{

    public float time;
    public SpinPrizeType[] prizes;
    public bool random;

    /*
     * waiting time in minutes
     * list of prizes
     * are prizes given out in a sequence or at random
     */
    public SpinEntity(float time, SpinPrizeType[] prizes, bool random = true)
    {
        this.random = random;
        this.prizes = prizes;
        this.time = time;
    }
}

}
