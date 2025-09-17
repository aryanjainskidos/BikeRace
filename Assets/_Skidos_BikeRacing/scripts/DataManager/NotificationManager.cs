namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable 0219 //shut up about unused variables


/**
 * lokálás notifikácijas
 */
public class NotificationManager : MonoBehaviour
{

    static int interval = 30; //reizi cik sekundés párskatít visas notifikácijas
    static int Prize24h = 2500; //bonuss par atrgieśanos péc 24h prombútnes

    public static void Init()
    {
        //print("NotificationManager init");

        //		GameObject.Find("Main Camera").GetComponent<MonoBehaviour>().StartCoroutine(Reschedule());

    }

    private static IEnumerator Reschedule()
    {

        //		int i = 0;
        //		#if UNITY_IOS
        //		UnityEngine.iOS.LocalNotification notif;
        //		#elif UNITY_ANDROID && !UNITY_EDITOR
        //		AndroidJavaObject ajc = new AndroidJavaObject("com.zeljkosassets.notifications.Notifier");
        //		#endif
        //
        //
        //		while(true){
        //			yield return new WaitForSeconds(interval* 0.2f); //mazliet pagaida sákumá, ilgák pagaida beigás
        //			i++;
        //
        //            ScheduleNotifications();
        //		
        //
        //			yield return new WaitForSeconds(interval* 0.8f); //mazliet pagaida sákumá, ilgák pagaida beigás
        //		}

        yield break;
    }

    public static void ScheduleNotifications()
    {

        //        #if UNITY_IOS
        //        UnityEngine.iOS.LocalNotification notif;
        //        #elif UNITY_ANDROID && !UNITY_EDITOR
        //        AndroidJavaObject ajc = new AndroidJavaObject("com.zeljkosassets.notifications.Notifier");
        //        #endif
        //
        //        /*
        //	     * pirms rada jaunas notifikácijas
        //	     * jápárbauda vai nav jáiedod balva par atgrieśanos, sańemot 24h notifikáciju 
        //	     * 
        //	     */
        //        
        //        
        //        #if UNITY_IOS
        //        //uz iOS vienkárśi atcelj visas
        //        UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
        //        //print("CancelAllLocalNotifications");
        //        
        //        //izveido notifikáciju, kas noreseto ciparinju pie ikonas (citádi tas múźígi bús 1)
        //        notif = new UnityEngine.iOS.LocalNotification();
        //        notif.fireDate = System.DateTime.Now;
        //        notif.applicationIconBadgeNumber = -1;
        //        notif.hasAction = false;
        //        UnityEngine.iOS.NotificationServices.ScheduleLocalNotification( notif );
        //        
        //        #elif UNITY_ANDROID && !UNITY_EDITOR
        //        //Androídam ir jáatceĺ katra notifikácija individuáli (to daru automátiski, kad uzsetoju nákamo)
        //        
        //        #endif
        //        //uzliek par jaunu visas
        //        
        //        // "bustińś gatavs!"
        //        BoostRecord boostRecord;
        //        foreach (KeyValuePair<string, BoostRecord> record in DataManager.Boosts)
        //        {
        //            boostRecord = record.Value;
        //            if(boostRecord.FarmingTimestamp != System.DateTime.MinValue) {
        //                
        //                System.DateTime readyAt = boostRecord.FarmingTimestamp + boostRecord.FarmingDuration;
        //                System.TimeSpan timeLeft = readyAt - System.DateTime.Now;
        //                if(timeLeft.TotalSeconds > interval+(interval/4)){//nezińos par pabeigtu bústińu, ja tas bus tik dríz gatavs, ka nebús iespéja atcelt pazińojumu nákamajá ciklá
        //                    
        //                    int seconds = (int) timeLeft.TotalSeconds;
        //                    string titleA = Lang.Get("Notifications:Title:Android:Boost |param| is ready").Replace("|param|", boostRecord.Name );
        //                    string textA = Lang.Get("Notifications:Body:Android:Boost |param| is ready").Replace("|param|", boostRecord.Name ); 
        //                    string textIOS = Lang.Get("Notifications:iOS:Boost |param| is ready").Replace("|param|", boostRecord.Name ); 
        //                    
        //                    #if UNITY_IOS
        //                    notif = new UnityEngine.iOS.LocalNotification();
        //                    notif.fireDate = System.DateTime.Now.AddSeconds( seconds );
        //                    notif.alertBody = textIOS;
        //                    notif.soundName = "play_wrum_notification.wav"; //XCode projektá pirms palaiśanas jáieliek failińś ar nosaukumu "play_wrum_notification.wav"   oriǵináli atrodas: /Assets/Resources/Sounds/UI/play_wrum.wav
        //                    UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notif);
        //                    #elif UNITY_ANDROID && !UNITY_EDITOR
        //                    ajc.CallStatic("cancelAndSetNotification", titleA, titleA, textA, seconds, 443 + boostRecord.ID, 1); //unikáls notifikácijas numurs katram bústińam
        //                    #endif
        //                    //print(boostRecord.Name + " => " + timeLeft   + " ++");
        //                } else {
        //                    //atceĺ Androída notifikáciju par bústińu, kas dríz bús gatavs
        //                    #if UNITY_ANDROID && !UNITY_EDITOR
        //                    ajc.CallStatic("cancelAndSetNotification", "", "", "", 1, 443 + boostRecord.ID, 0); 
        //                    #endif
        //                    //print(boostRecord.Name + " => " + timeLeft   + " --");
        //                }
        //                
        //                
        //                
        //            }else {
        //                //atceĺ Androída notifikáciju par bústińu, kas netiek audzéts
        //                #if UNITY_ANDROID && !UNITY_EDITOR
        //                ajc.CallStatic("cancelAndSetNotification", "", "", "", 1, 443 + boostRecord.ID, 0); 
        //                #endif
        //            }
        //        }

    }

    public static void RequestPermission()
    {
#if UNITY_IOS
        /* Note: calling this method will set the requested notification types for both local and remote notifications, overriding any previous calls. 
         * Call to NotificationServices.RegisterForRemoteNotificationTypes() is still required to enable remote notifications.
         */
        //        NotificationServices.RegisterForLocalNotificationTypes(LocalNotificationType.Alert | 
        //                                                               LocalNotificationType.Badge | 
        //                                                               LocalNotificationType.Sound);

        //        UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | 
        //                                                                UnityEngine.iOS.NotificationType.Badge | 
        //                                                                UnityEngine.iOS.NotificationType.Sound);
#endif
    }

    //    public static bool GavePermission() {
    //#if UNITY_IOS
    //        return UnityEngine.iOS.NotificationServices.enabledRemoteNotificationTypes != UnityEngine.iOS.NotificationType.None;
    //#else
    //        return true;
    //#endif
    //    }


}

}
