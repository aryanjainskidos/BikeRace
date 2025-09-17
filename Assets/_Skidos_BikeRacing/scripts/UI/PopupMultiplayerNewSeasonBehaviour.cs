namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupMultiplayerNewSeasonBehaviour : MonoBehaviour
{

    Text timeText;
    Text cupText;
    Text coinText;

    void Awake()
    {
        timeText = transform.Find("TimeText").GetComponent<Text>();

        cupText = transform.Find("CupText").GetComponent<Text>();
        cupText.text = "";

        coinText = transform.Find("CoinText").GetComponent<Text>();
        coinText.text = "";
    }

    public void SetCoins(int value)
    {
        coinText.text = "+" + value;
    }

    public void SetCups(int value)
    {
        cupText.text = "+" + value;
    }


    void OnEnable()
    {
        StartCoroutine(SeasonCountdown());
    }
    void OnDisable()
    {
        StopCoroutine(SeasonCountdown());
    }

    IEnumerator SeasonCountdown()
    {
        while (true)
        {

            System.TimeSpan t = new System.TimeSpan();
            if (MultiplayerManager.SeasonTTL != 0)
            { // ja nav sanemta info no servera, cikos beidzas sezona, tead neko nerádít
                t = MultiplayerManager.SeasonEndDate - System.DateTime.Now;
                if (t.Ticks < 0)
                { //sezona ir beigusies
                    t = new System.TimeSpan(); //noresetoju, lai nerádítu negatívu laiku, bet nulli
                    MultiplayerManager.SeasonTTL = 0;
                }
            }

            timeText.text = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", t.Days, t.Hours, t.Minutes, t.Seconds);
            yield return new WaitForSeconds(0.33f);

        }

    }


}

}
