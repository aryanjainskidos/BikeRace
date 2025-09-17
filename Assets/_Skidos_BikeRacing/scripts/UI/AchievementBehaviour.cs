namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AchievementBehaviour : MonoBehaviour
{

    public string key;

    public delegate void ClaimDelegate();
    public ClaimDelegate claimDelegate;

    Text titleText;
    Text progressText;
    Text coinText;
    Text pointText;
    //Text claimButtonText;

    Button claimButton;

    Slider progressBar;

    public AchievementRecord Record;

    // Use this for initialization
    void Awake()
    {
        titleText = transform.Find("TitleText").GetComponent<Text>();
        progressText = transform.Find("ProgressText").GetComponent<Text>();
        coinText = transform.Find("CoinText").GetComponent<Text>();
        pointText = transform.Find("PointText").GetComponent<Text>();

        progressBar = transform.Find("Slider").GetComponent<Slider>();

        claimButton = transform.Find("ClaimButton").GetComponent<Button>();
        claimButton.onClick.AddListener(() => OnClaimClicked());

        //claimButtonText = transform.FindChild("ClaimButton/Text").GetComponent<Text>();
        //		claimButtonText.text = Lang.Get("Achievements:Claim");

    }


    void OnClaimClicked()
    {
        //  print(key + " claimed");
        AchievementManager.ClaimReward(key);
        claimButton.gameObject.SetActive(false);

        if (BikeDataManager.FirstClaim)
        {
            BikeDataManager.FirstClaim = false;
        }

        if (claimDelegate != null)
        {
            claimDelegate();
        }

        TelemetryManager.EventAchievementClaiming(key);

        SoundManager.Play("Claim");

    }

    public void setData(AchievementRecord record)
    {
        titleText.text = record.Name;
        progressText.text = Mathf.FloorToInt(record.Progress) + "/" + record.Target;
        coinText.text = record.RewardCoins.ToString();
        pointText.text = Lang.Get("Achievements:|param| pts").Replace("|param|", record.RewardPoints.ToString());

        if (record.Progress >= record.Target && !record.Claimed)
        {
            claimButton.gameObject.SetActive(true);
        }
        else
        {
            claimButton.gameObject.SetActive(false);
        }

        progressBar.value = (float)(record.Progress / record.Target);

        Record = record;

    }
}

}
