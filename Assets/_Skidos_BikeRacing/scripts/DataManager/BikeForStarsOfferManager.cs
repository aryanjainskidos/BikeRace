namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using Random = UnityEngine.Random;


public class BikeForStarsOfferManager
{

    //1) if offer expired without being completed, then set up a display time if display time is in the past
    //2) if offer start is earlier than offer display time and current time is later equals offer display, then offer is available, offer start is set to now
    //3) while time now is less than offer start + offer duration offer is available
    //4) once time now is later than offer start + offer duration offer is expired and in case offer wasn't completed set up display time 1)
    //5) if at any point when time now was less than offer start + offer duration player collected 20 stars, then offer is completed for all times


    const float OFFER_DELAY_IN_HOURS = 28;//0.05f;
    const float OFFER_DISPLAY_HOUR = 19;//16;//19;
    const float OFFER_DISPLAY_MINUTE = 0;//05;//19;

    const float OFFER_DURATION_IN_MINUTES = 120f;
    public const int STARS_TO_COLLECT = 20;

    const string OFFER_DISPLAY_TIMESTAMP_KEY = "OfferDisplayTimestamp";
    const string OFFER_START_TIMESTAMP_KEY = "OfferStartTimestamp";
    const string STAR_COUNT_KEY = "OfferStarCount";

    public const int PRIZE_BIKE = (int)BikeStyleType.Gold;

    public static DateTime offerNotificationTimestamp; //if this is not the default value, then notification manager should setup the notification
    public static DateTime offerStartTimestamp;
    static int starCountAtOfferStart = 0;


    public static int StarsCollectedSinceOfferStart
    {
        get
        {
            return BikeDataManager.Stars - starCountAtOfferStart;
        }
    }


    public static bool OfferExpired //start + duration < now
    {
        get
        {
            //now is later than offer end -> offer expired
            return DateTime.Now.CompareTo(offerStartTimestamp.AddMinutes(OFFER_DURATION_IN_MINUTES)) > 0;
        }
    }

    public static bool OfferAvailable // start < now < start + duration
    {
        get
        {
            //now is later than offer start but earlier than offer end -> offer available
            return DateTime.Now.CompareTo(offerStartTimestamp) > 0 && DateTime.Now.CompareTo(offerStartTimestamp.AddMinutes(OFFER_DURATION_IN_MINUTES)) < 0;
        }
    }

    public static bool OfferCompleted
    {
        get
        {
            //prize bike is already unlocked -> offer completed
            return !BikeDataManager.Styles[PRIZE_BIKE].Locked;
        }
    }

    public static TimeSpan TimeTillOfferEnd
    {
        get
        {
            TimeSpan timePassedSinceStart = DateTime.Now.Subtract(offerStartTimestamp);
            return (timePassedSinceStart.TotalMinutes < OFFER_DURATION_IN_MINUTES) ?
                        TimeSpan.FromMinutes(OFFER_DURATION_IN_MINUTES).Subtract(timePassedSinceStart) :
                        TimeSpan.FromMinutes(0);
        }
    }


    public static void GetNotificationDateTime()
    {

        if (!OfferCompleted) // skip if offer completed
        {
            //try to load 
            if (PlayerPrefs.HasKey(OFFER_DISPLAY_TIMESTAMP_KEY)) //load notification timestamp
            {
                var timestamp = PlayerPrefs.GetString(OFFER_DISPLAY_TIMESTAMP_KEY);
                DateTime.TryParseExact(timestamp, "G", CultureInfo.InvariantCulture, DateTimeStyles.None, out offerNotificationTimestamp);
            }

            //            Debug.Log("offerNotificationTimestamp " + offerNotificationTimestamp.ToString()); 
            //            Debug.Log("DateTime.Now " + DateTime.Now.ToString()); 
            //            Debug.Log("offerNotificationTimestamp.CompareTo(DateTime.Now) > 0" + (offerNotificationTimestamp.CompareTo(DateTime.Now) > 0)); 

            if (offerNotificationTimestamp == DateTime.MinValue || offerNotificationTimestamp.CompareTo(DateTime.Now) > 0)
            { // if notification TS in the future put it off once again
                //Debug.Log("future");
                CalculateNotificationTimestamp();

            }

        }
    }

    static void CalculateNotificationTimestamp()
    {
        DateTime then = DateTime.Now;
        //        Debug.Log("then " + then);

        then = then.AddHours(OFFER_DELAY_IN_HOURS);

        //Debug.Log("then + OFFER_DELAY_IN_HOURS " + then);

        if (then.Hour >= OFFER_DISPLAY_HOUR)
        { //TODO >=
            then = then.AddDays(1);
        }
        //Debug.Log("then + day(optional) " + then);

        offerNotificationTimestamp = then.Date.AddHours(OFFER_DISPLAY_HOUR).AddMinutes(OFFER_DISPLAY_MINUTE); // OFFER_DISPLAY_MINUTE //TODO save somewhere for rescheduling
                                                                                                              //Debug.Log("offerNotificationTimestamp " + offerNotificationTimestamp);

        PlayerPrefs.SetString(OFFER_DISPLAY_TIMESTAMP_KEY, offerNotificationTimestamp.ToString());
        PlayerPrefs.Save();
    }

    //load data and chec
    public static void Init()
    {
        LoadOfferData(); //load available data

        GetNotificationDateTime();
    }

    public static void Update()
    {

        GetNotificationDateTime(); //

        if (StartOffer())
        {
            //Debug.Log("StartOffer");
            //            UIManager.ToggleScreen(GameScreenType.PopupCollectStarsForPower, true);
        }
        //TODO check if bike should be awarded
        if (CompleteOffer())
        {
            //Debug.Log("CompleteOffer");
            //            UIManager.ToggleScreen(GameScreenType.PopupCollectStarsForPower, true);
        }

        if (OfferExpired && offerNotificationTimestamp.CompareTo(DateTime.Now) < 0) // if notification TS is old and offer expired, then 
        {
            //Debug.Log(" Update...");
            CalculateNotificationTimestamp();
        }
    }

    public static bool StartOffer()
    {

        bool offerStarted = false;

        //        bool offerStartBeforeOfferNotification = offerStartTimestamp.CompareTo(offerNotificationTimestamp) < 0;

        //        if ( !OfferAvailable && !OfferCompleted )  // && offerStartBeforeOfferNotification // prevent a new offer being started if one is already running or has been completed
        //        {
        //            bool bikeLocked = DataManager.Styles[PRIZE_BIKE].Locked;
        //            bool requredStarCountAvailable = DataManager.Stars + STARS_TO_COLLECT <= DataManager.StarsTotal;//TODO check if player can acually collect 20 stars
        //            
        //            if ( OfferExpired && requredStarCountAvailable) //if offer expired, but bike 
        //            {
        //                starCountAtOfferStart = DataManager.Stars;
        //                offerStartTimestamp = DateTime.Now;
        ////                offerExpired = false;
        //                
        //                SaveOfferData(); // save new offer data
        //
        //                offerStarted = true;
        //            }
        //        }

        bool notificationShown = DateTime.Now.CompareTo(offerNotificationTimestamp) > 0;
        bool requiredStarCountAvailable = BikeDataManager.Stars + STARS_TO_COLLECT <= BikeDataManager.StarsTotal;

        //Debug.Log("");

        if (!OfferCompleted && OfferExpired && notificationShown && requiredStarCountAvailable)
        {
            starCountAtOfferStart = BikeDataManager.Stars;
            offerStartTimestamp = DateTime.Now;

            SaveOfferData(); // save new offer data

            CalculateNotificationTimestamp(); //need to move the notification timestamp into the future, otherwise this finction will restart the event constantly

            TelemetryManager.EventBikeForStarsStarted();

            offerStarted = true;
        }

        return offerStarted;
    }

    public static bool CompleteOffer()
    {

        var offerCompleted = false;
        bool collectedAllStars = StarsCollectedSinceOfferStart >= STARS_TO_COLLECT;

        // check if the selected bike is still locked
        // check if 20 stars collected and award bike if it still locked
        if (collectedAllStars && OfferAvailable && !OfferCompleted)
        {
            offerCompleted = true;

            // unlock bike show popup
            BikeDataManager.Styles[PRIZE_BIKE].Locked = false;

            offerNotificationTimestamp = DateTime.MinValue;
            PlayerPrefs.SetString(OFFER_DISPLAY_TIMESTAMP_KEY, offerNotificationTimestamp.ToString());
            PlayerPrefs.Save();

            TelemetryManager.EventBikeForStarsCompleted();
        }

        return offerCompleted;
    }

    //do this at offer start and load it every
    public static void SaveOfferData()
    {

        PlayerPrefs.SetString(OFFER_START_TIMESTAMP_KEY, DateTime.Now.ToString());
        PlayerPrefs.SetInt(STAR_COUNT_KEY, starCountAtOfferStart);
        PlayerPrefs.Save();
        //Debug.Log("Quit " + PlayerPrefs.GetString( QUIT_TIMESTAMP_KEY ));
    }

    public static void LoadOfferData()
    {

        if (PlayerPrefs.HasKey(OFFER_START_TIMESTAMP_KEY))
        {
            var timestamp = PlayerPrefs.GetString(OFFER_START_TIMESTAMP_KEY);
            DateTime.TryParseExact(timestamp, "G", CultureInfo.InvariantCulture, DateTimeStyles.None, out offerStartTimestamp);
        }

        var starCount = PlayerPrefs.GetInt(STAR_COUNT_KEY, -1);
        starCountAtOfferStart = starCount >= 0 ? starCount : BikeDataManager.Stars;//if nothing is saved, set variable to current star count

        //        TimeSpan delta = DateTime.Now - offerStartTimestamp;
        //        offerExpired = delta.TotalMinutes >= OFFER_DURATION_IN_MINUTES; //if offer never started it will be categorised as expired
    }

    //    static void CheckTimestamp(string timestamp) {
    //        //Debug.Log("SpinManager::CheckTimestamp " + timestamp);
    //
    //        DateTime timestampDate;
    //        bool validTimestamp = false;
    //
    //        validTimestamp = DateTime.TryParseExact(timestamp, "G", CultureInfo.InvariantCulture, DateTimeStyles.None, out timestampDate);
    //
    //        if (validTimestamp)
    //        {
    //            TimeSpan delta = DateTime.Now - timestampDate;
    //            
    //            if (delta.TotalMinutes >= OFFER_DURATION_IN_MINUTES) {
    //                //TODO offer expired
    //            } else {
    //                //TODO offer can continue
    //            }
    //            
    //        } else {
    //            //TODO timestamp invalid
    //        }
    //    }

    #region testing
    //will only get called if you maually check a checkbox in editor
    public static void Reset()
    {
        PlayerPrefs.DeleteKey(OFFER_START_TIMESTAMP_KEY);
        PlayerPrefs.DeleteKey(OFFER_DISPLAY_TIMESTAMP_KEY);
        PlayerPrefs.DeleteKey(STAR_COUNT_KEY);
        PlayerPrefs.Save();
    }
    #endregion
}

}
