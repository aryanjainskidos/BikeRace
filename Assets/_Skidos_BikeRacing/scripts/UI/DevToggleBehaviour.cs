namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DevToggleBehaviour : MonoBehaviour
{
    Toggle toggle;

    //UIButtonGameCommand gameCommand;

    void Awake()
    {
        toggle = transform.GetComponent<Toggle>();
        //gameCommand = transform.GetComponent<UIButtonGameCommand>();
    }


    void Update()
    {
        if (BikeGameManager.developmentMode != toggle.isOn)
        {
            toggle.isOn = BikeGameManager.developmentMode;
        }
    }
}

}
