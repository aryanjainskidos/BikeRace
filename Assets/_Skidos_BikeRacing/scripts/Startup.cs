namespace vasundharabikeracing {

using UnityEngine;
using System;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//using Facebook.Unity;

/**
 * Dara globálas startup lietas,
 * pieder "Main Camera" geimobjektam
 */
public class Startup : MonoBehaviour
{
    public static bool Initialized = false;
    DateTime timeLostFocus = DateTime.Now;

    void Awake()
    {
        Lang.Init(); //śo vajag vél átrák piestartét - pirms daźi agrie UI sušķi pamostas
    }


    void Start()
    {
        Time.timeScale = 1;

        // FB.Init();
        //piestarté statiskos menedźerus, kas paśi nespéj piestartéties (jo nepieder gameObjektiem)
        BikeDataManager.Init();
        QualitySettingsManager.Init();
        UIManager.Init();
        TelemetryManager.Init();
        MultiplayerManager.Init();


        // TwitterManager.Init();

        GameObject.Find("Canvas_game").AddComponent<UIInput>();
        //nedaudz vélák izsaucamas lietas, kas nav tik svarígas, lai traucétu lielajiem procesiem
        StartCoroutine(DoInAJiffy()); //do it in next frame

        if (BikeDataManager.FirstTimer)
        {
            //izveido direktorijas, lai vélák ikreiz tás atverot nebútu jáskatás vai eksisté
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/sp-rides/"); //vienspélétája ride-failińiem
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/mp-cache/");//mulitpleijera nokachaatajiem un ierakstítajiem ride-failińiem (arí multipleijera nokachátajám bildém)
        }

        Initialized = true;

        NotificationManager.Init();
        // StoreManager.Init();

        MultiplayerManager.CleanUpOldFiles();

        // AdMobManager.Init();
        // HeyZapManager.Init(); //regulárie interstiśáli
        AdManager.Init();
        SpinManager.Init();
        CentralizedOfferManager.Init();
        DataBackupManager.Init();
        BikeForStarsOfferManager.Init();

    }



    void OnApplicationFocus(bool focus)
    {
        //Debug.Log("OnApplicationFocus " + focus);

        if (!focus)
        {
#if !UNITY_EDITOR
            if (Debug.isDebugBuild)
            {
                print("B) seivos gamedata.moto - on minimize");
            }
            BikeDataManager.Flush();
#else
            //print("Editoraa NESEIVOS gamedata.moto (appaa, seivotu)");
#endif


            timeLostFocus = DateTime.Now;
            //Debug.Log("no focus");
        }
        else
        {
            double noFocusDuration = (DateTime.Now - timeLostFocus).TotalSeconds;
            //Debug.Log("please focus " +  noFocusDuration);

            if (noFocusDuration > 60 * 5)
            { //ja appa ir 5min samazináta, tad noreseto reklámas taimerus
                AdManager.ResetTimer();
            }

        }
    }

    //iOS applications are usually suspended and do not quit, 
    //On Windows Store Apps and Windows Phone 8.1 there's no application quit event
    void OnApplicationQuit()
    {
        //Debug.Log("OnApplicationQuit");
        SpinManager.OnQuit();
    }

    void OnApplicationPause(bool pause)
    {
        //Debug.Log("OnApplicationPause " + pause);
        SpinManager.OnPause(pause);
    }


    private IEnumerator DoInAJiffy()
    {

        yield return null;
        PlayerCountryManager.Init();


        //yield return new WaitForSeconds(1);
        AdMobConverionTrackingManager.PokeGoogle();

        yield return new WaitForSeconds(2);
        TelemetryManager.EventStartup();

        yield return new WaitForSeconds(1);

    }

    int wait = 1;
    void Update()
    {
        if (Initialized && BikeGameManager.commandQueue.Count > 0)
        {
            if (wait <= 0)
            {
                //print( "delayed GameManager.ExecuteCommand" + ((int)GameManager.commandQueue.Peek()) );
                BikeGameManager.ExecuteCommand(BikeGameManager.commandQueue.Peek());
                wait = 1;
            }
            else
            {
                wait--;
            }
        }
    }

}
}
