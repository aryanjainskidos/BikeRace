namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class AdMobConverionTrackingManager : MonoBehaviour
{


    public static void PokeGoogle()
    {


        //print("AdMobConverionTrackingManager A");
#if UNITY_IOS
        //Camera.main.GetComponent<MonoBehaviour>().StartCoroutine(AdMobConverionTrackingManager._PokeGoogle());
#else
				//párájám platformám pagaidám neko nedaru
#endif

    }



    public static IEnumerator _PokeGoogle()
    {
#if UNITY_IOS
        string idfa = "";//UnityEngine.iOS.Device.advertisingIdentifier;
        if (idfa == null)
        {
            idfa = "lol";
        }
        string md5idfa = Md5.Md5Sum(idfa);

        string url = "https://www.googleadservices.com/pagead/conversion/965227517/"
            + "?label=74IWCI_c3FsQ_eegzAM"
            + "&muid=" + md5idfa
            + "&bundleid=com.FunGenerationLab.BikeUp" //śis ir músu bundle ID ?
            + "&idtype=idfa" //neaiztikt
            + "&lat=1" //1 vai 0
            ;


        if (Debug.isDebugBuild) { print("AdMobConverionTrackingManager::url:\n" + url); }
        var www = new WWW(url);

        yield return www;

        /* i don't care
		if(www.error == null) {
			if(Debug.isDebugBuild){print("AdMobConverionTrackingManager::response="+www.text);}
		} else {
			if(Debug.isDebugBuild){print("AdMobConverionTrackingManager::error="+www.error);}
		}*/
#endif

        yield break;
    }

}


}
