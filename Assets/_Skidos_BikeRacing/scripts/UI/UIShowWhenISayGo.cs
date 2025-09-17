namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class UIShowWhenISayGo : MonoBehaviour
{


    GameObject Rider;

    void Awake()
    {

        foreach (Transform child in transform)
        {
            Rider = child.gameObject;
            break;
        }
        Rider.SetActive(false);
    }


    void OnEnable()
    {
        Rider.SetActive(false);
        StartCoroutine(ActivateLater());
    }


    IEnumerator ActivateLater()
    {

        while (true)
        {

            if (!Startup.Initialized)
            { //wait until everything is loaded
                yield return new WaitForSeconds(0.3f);
                continue;
            }

            Rider.SetActive(true);
            break;
        }


    }

}

}
