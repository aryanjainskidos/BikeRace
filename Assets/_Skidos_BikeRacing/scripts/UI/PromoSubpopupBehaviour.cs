namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Globalization;
/// <summary>
/// fake timer removed because we don't want to get in trouble.
/// </summary>
public class PromoSubpopupBehaviour : MonoBehaviour
{

    public PromoSubPopups subpopupType;
    public double availableForHours = 2;
    //    DateTime endTime;

    Text timerText;

    void Awake()
    {
        //        timerText = transform.FindChild("TimerText").GetComponent<Text>();
    }

    // Use this for initialization
    void OnEnable()
    {
        //        //if promo activated and subpopup been activated get the end time
        //        if( PlayerPrefs.GetInt("promo" + subpopupType.ToString(), 0) == 1  ) {
        //
        //            if (!PlayerPrefs.HasKey(name+"EndTime"))
        //            {
        ////                endTime = System.DateTime.Now.AddHours(availableForHours);
        ////                PlayerPrefs.SetString(name+"EndTime", endTime.ToString());
        //                ResetTimer();
        //            } else {
        //                endTime = DateTime.ParseExact(PlayerPrefs.GetString(name+"EndTime"), "G", CultureInfo.InvariantCulture);
        //
        //                //reset timer
        //                if (endTime <= DateTime.Now) {
        //                    ResetTimer();
        //                }
        //            }
        //            //print(name+"EndTime: "+ endTime);
        //
        //            StartCoroutine(UpdateTimer());
        //        }
    }

    //    void ResetTimer() {
    //        endTime = DateTime.Now.AddHours(availableForHours);
    //        PlayerPrefs.SetString(name+"EndTime", endTime.ToString());
    //        PlayerPrefs.Save();
    //    }

    void OnDisable()
    {
        //        StopAllCoroutines();
    }

    //    IEnumerator UpdateTimer(){
    //        while(true){
    //            
    //            TimeSpan diff = endTime - System.DateTime.Now;
    //
    //            if(endTime > DateTime.Now){
    //                timerText.text = diff.Hours.ToString("D2") + ":" + diff.Minutes.ToString("D2") + ":" + diff.Seconds.ToString("D2");
    //            } else {
    //                timerText.text = "00:00:00";
    //                ResetTimer();
    //            }
    //
    //            yield return new WaitForSeconds(1);
    //            
    //        }
    //    }
}

}
