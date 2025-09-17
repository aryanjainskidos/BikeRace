namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerBonusPanelBehaviour : MonoBehaviour
{

    public bool visible = true;

    Image image;

    Text timeText;

    Text powerText;

    public bool addPowerLabel = false;

    void Awake()
    {

        image = GetComponent<Image>();


        timeText = transform.Find("TimerText").GetComponent<Text>();
        powerText = transform.Find("Text").GetComponent<Text>();

        if (addPowerLabel)
        {
            powerText.text = Lang.Get("UI:Garage:Power") + "+" + (MultiplayerManager.PowerRating - MultiplayerManager.PermanentPowerRating).ToString(); //" +100";
        }
    }

    void SetVisibility(bool value)
    {
        visible = value;
        image.enabled = value;

        Utils.ShowChildrenGraphics(gameObject, value);
    }

    int displayedPower;

    void OnEnable()
    {
        MultiplayerManager.RecalculateMyPowerRating();
        if (BikeDataManager.PowerBoostEnabled && MultiplayerManager.PermanentPowerRating < 500)
        {
            SetVisibility(true);

            string powerString = "";

            if (addPowerLabel)
            {
                powerString += Lang.Get("UI:Garage:Power") + " "; //" +100";
            }
            print(MultiplayerManager.PowerRating - MultiplayerManager.PermanentPowerRating);
            displayedPower = (MultiplayerManager.PowerRating - MultiplayerManager.PermanentPowerRating);

            powerString += "+" + (MultiplayerManager.PowerRating - MultiplayerManager.PermanentPowerRating).ToString(); //" +100";
            powerText.text = powerString;

            StartCoroutine(UpdateWaitTimer());
        }
        else
        {
            SetVisibility(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (visible != BikeDataManager.PowerBoostEnabled || displayedPower != (MultiplayerManager.PowerRating - MultiplayerManager.PermanentPowerRating))
        {
            OnEnable();
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator UpdateWaitTimer()
    {

        while (BikeDataManager.PowerBoostEnabled)
        {

            System.TimeSpan timeTillEnd = System.TimeSpan.FromMinutes(BikeDataManager.POWER_BOOST_EXPIRTION_TIME).Subtract(System.DateTime.Now.Subtract(BikeDataManager.PowerBoostTimestamp));
            timeText.text = timeTillEnd.Minutes.ToString("D2") + ":" + timeTillEnd.Seconds.ToString("D2");

            yield return new WaitForSeconds(1);
        }

        OnEnable();
    }
}
}
