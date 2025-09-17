namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using SimpleJSON;


/**
 * prasís MP serverim pie startupa - káda ir spélétája valsts
 * neprasís atkároti, ja noskaidros
 */

public class PlayerCountryManager : MonoBehaviour
{

    public static void Init()
    {
        if (BikeDataManager.Country == "")
        {
            GameObject.Find("Main Camera").GetComponent<MonoBehaviour>().StartCoroutine(Check());
        }
    }


    private static IEnumerator Check()
    {

        WWW www = new WWW(MultiplayerManager.ServerUrlMP + "/country");
        yield return www;

        if (www.error == null)
        {
            JSONNode N = JSON.Parse(www.text);

            if (N["country"] != null && N["country"] != "--" && ((string)N["country"]).Length == 2)
            { // atbilde "--" nozímé, ka nav identificéta valsts
                BikeDataManager.Country = N["country"];
                //Debug.Log("PlayerCountryManager::dabuuju valsti:"+DataManager.Country );
            }
            else
            {
                //Debug.LogWarning("PlayerCountryManager::Not a country:" + N["country"]);
            }
        }
        else
        {
            //Debug.LogError("PlayerCountryManager::err=" + www.error);
        }
    }
}

}
