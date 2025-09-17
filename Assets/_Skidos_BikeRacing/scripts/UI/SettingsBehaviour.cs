namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class SettingsBehaviour : MonoBehaviour
{

    private bool initialized = false;
    private bool afterFirstEnabled = false;

    Button restoreButton;
    Text restoreText;

    void Awake()
    {
        restoreButton = transform.Find("ButtonPanel/RestoreButton").GetComponent<Button>();
        restoreText = restoreButton.transform.Find("Text").GetComponent<Text>();

#if UNITY_ANDROID
        //hide restore and move the other buttons
        restoreButton.gameObject.SetActive(false);
        
//        var teamsButton = transform.Find("ButtonPanel/TeamsButton").GetComponent<RectTransform>();
//        var supportButton = transform.Find("ButtonPanel/supportButtonButton").GetComponent<RectTransform>();
        // var logOutButton = transform.Find("ButtonPanel/LogOutButton").GetComponent<RectTransform>();
        var creditsButton = transform.Find("ButtonPanel/CreditsButton").GetComponent<RectTransform>();

//        teamsButton.anchoredPosition = new Vector2(-150f, -72);
//        supportButton.anchoredPosition = new Vector2(0f, -72);
        // logOutButton.anchoredPosition = new Vector2(75.6f, -72);
        creditsButton.anchoredPosition = new Vector2(150f, -72);//228.6f, -72);
#endif
    }

    void OnEnable()
    {
        afterFirstEnabled = true;

    }

    //settings closed
    void OnDisable()
    {
        if (Startup.Initialized)
        {
            BikeDataManager.Flush();
        }
    }

    void Update()
    {
        if (!initialized && afterFirstEnabled)
        {
            initialized = true;

            transform.Find("ButtonPanel/GhostToggle").GetComponent<Toggle>().isOn = BikeDataManager.SettingsSPGhost;
            transform.Find("ButtonPanel/MusicToggle").GetComponent<Toggle>().isOn = BikeDataManager.SettingsMusic;
            transform.Find("ButtonPanel/SFXToggle").GetComponent<Toggle>().isOn = BikeDataManager.SettingsSfx;
            transform.Find("ButtonPanel/GraphicsToggle").GetComponent<Toggle>().isOn = BikeDataManager.SettingsHD;
            transform.Find("ButtonPanel/ControlToggle").GetComponent<Toggle>().isOn = BikeDataManager.SettingsAccelerometer;

#if UNITY_IOS
            //            if (StoreManager.initialized) {
            //                print("StoreManager.TransactionsAlreadyRestored() " + StoreManager.TransactionsAlreadyRestored );
            //                restoreButton.interactable = !StoreManager.TransactionsAlreadyRestored;
            //                if (StoreManager.TransactionsAlreadyRestored) {
            //                    restoreText.text = Lang.Get("UI:Settings:Restored");
            //                }
            //            }
#endif
        }
#if UNITY_IOS
        //        if (StoreManager.TransactionsAlreadyRestored && restoreButton.interactable) {
        //            restoreButton.interactable = false;
        //            restoreText.text = Lang.Get("UI:Settings:Restored");
        //        }
#endif
    }
}

}
