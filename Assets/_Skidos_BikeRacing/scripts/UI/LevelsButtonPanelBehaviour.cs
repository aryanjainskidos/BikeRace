namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class LevelsButtonPanelBehaviour : MonoBehaviour
{
    GameObject pointer;

    GameObject achievementNotification;
    GameObject garageNotification;
    // GameObject multiplayerNotification;

    // GameObject multiplayerButton;
    UIButtonSwitchScreen multiplayerButtonSwitchScreen;
    UIButtonToggleScreen multiplayerButtonToggleScreen;

    // Use this for initialization
    void Awake()
    {
        pointer = transform.Find("Pointer").gameObject;

        achievementNotification = transform.Find("AchievementButton/NotificationImage").gameObject;
        garageNotification = transform.Find("GarageButton/NotificationImage").gameObject;
        // multiplayerNotification = transform.Find("MultiplayerButton/NotificationImage").gameObject;

        // multiplayerButton = transform.Find("MultiplayerButton").gameObject;
        // multiplayerButtonSwitchScreen = multiplayerButton.GetComponent<UIButtonSwitchScreen>();
        // multiplayerButtonToggleScreen = multiplayerButton.GetComponent<UIButtonToggleScreen>();
    }

    // Update is called once per frame
    void OnEnable()
    {
        // if more than 4 unclaimed achievements, show pointer

        if (BikeDataManager.FirstClaim && BikeDataManager.CountUnclaimedAchievements() >= 4)
        {
            pointer.SetActive(true);
        }
        else
        {
            if (pointer.activeSelf)
            {
                pointer.SetActive(false);
            }
        }

        if (BikeDataManager.CountUnclaimedAchievements() > 0)
        {
            achievementNotification.SetActive(true);
        }
        else
        {
            if (achievementNotification.activeSelf)
            {
                achievementNotification.SetActive(false);
            }
        }

        if (BikeDataManager.ShowGarageButtonNotification) //if boost is ready
        {
            garageNotification.SetActive(true);
        }
        else
        {
            if (garageNotification.activeSelf)
            {
                garageNotification.SetActive(false);
            }
        }

        // if (DataManager.MultiplayerUnlocked && //multiplayer unlocked
        //     (NewsListManager.ActiveRides > 0 || NewsListManager.LeagueGamesPlayed == 0 || DataManager.ShowMultiplayerButtonNotification)) //if player hasn't played this season or new rides available
        // {
        //     multiplayerNotification.SetActive(true);
        // } else {
        //     if (multiplayerNotification.activeSelf) {
        //         multiplayerNotification.SetActive(false);
        //     }
        // }

        // if (DataManager.MultiplayerUnlocked) {
        //     //TODO disable popup
        //     multiplayerButtonSwitchScreen.enabled = true;
        //     multiplayerButtonToggleScreen.enabled = false;
        // } else {
        //     //TODO enable popup
        //     multiplayerButtonSwitchScreen.enabled = false;
        //     multiplayerButtonToggleScreen.enabled = true;
        // }
    }
}

}
