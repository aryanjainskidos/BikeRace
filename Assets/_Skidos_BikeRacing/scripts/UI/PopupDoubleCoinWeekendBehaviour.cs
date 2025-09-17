namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupDoubleCoinWeekendBehaviour : MonoBehaviour
{


    Text timeText;


    void Awake()
    {
        timeText = transform.Find("TimeText").GetComponent<Text>();
    }


    void OnEnable()
    {
        StartCoroutine(Countdown());
    }


    IEnumerator Countdown()
    {
        while (true)
        {

            System.TimeSpan ttl = CentralizedOfferManager.GetDoubleCoinWeekendEndTime();
            if (ttl.TotalSeconds > 0)
            {
                timeText.text = "Event ends: " + Mathf.Floor((float)ttl.TotalHours).ToString("00") + ":" + ttl.Minutes.ToString("00") + ":" + ttl.Seconds.ToString("00");
            }
            else
            {
                timeText.text = "Event ends: 00:00:00";
                yield break;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }



}
}
