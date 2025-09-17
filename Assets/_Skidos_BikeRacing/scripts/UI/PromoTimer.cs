namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PromoTimer : MonoBehaviour
{

    Text[] timerTexts;
    bool started = false;
    System.DateTime endTime;


    void Awake()
    {
        Text[] _timerTexts = { //@note -- jáwaitlisté visi popupos atrodamie taimeri
			transform.Find("ScrollView/Content/PopupPromoSaleStyles/TimerText").GetComponent<Text>(),
            transform.Find("ScrollView/Content/PopupPromoSale50/TimerText").GetComponent<Text>(),
            transform.Find("ScrollView/Content/PopupPromoSale70/TimerText").GetComponent<Text>(),
        };
        timerTexts = _timerTexts;

    }


    void OnEnable()
    {
        if (Startup.Initialized)
        {
            if (!started)
            {
                StartCoroutine(UpdateTimer());
            }
        }
    }


    IEnumerator UpdateTimer()
    {
        while (true)
        {
            if (endTime < System.DateTime.Now)
            { //ir pienácis laiks - jáiedod vél 2 stundas
                endTime = System.DateTime.Now;
                endTime = endTime.AddHours(2);
            }

            System.TimeSpan diff = endTime - System.DateTime.Now;

            for (int i = 0; i < timerTexts.Length; i++)
            {
                timerTexts[i].text = diff.Hours.ToString("D2") + ":" + diff.Minutes.ToString("D2") + ":" + diff.Seconds.ToString("D2");
            }

            yield return new WaitForSeconds(0.3f);

        }
    }



}

}
