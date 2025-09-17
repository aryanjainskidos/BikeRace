namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupGenericErrorBehaviour : MonoBehaviour
{


    public static string ErrorMessage = "";

    void OnEnable()
    {

        if (Startup.Initialized)
        {
            transform.Find("ErrorPanel/InfoText").GetComponent<Text>().text = ErrorMessage;
        }

    }

}

}
