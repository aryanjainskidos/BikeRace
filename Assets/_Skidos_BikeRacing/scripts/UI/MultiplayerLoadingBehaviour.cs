namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class MultiplayerLoadingBehaviour : MonoBehaviour
{

    bool timeouted = false;
    public float timeout = 1;
    GameObject errorPanel;

    // MultiplayerLoadingBehaviour.ErrorMessage = "Error connecting to store";
    public static string ErrorMessage = "";
    //Image background;

    GameObject progressTextBottom;

    DateTime start;

    void Awake()
    {
        errorPanel = transform.Find("ErrorPanel").gameObject;
        //background = transform.GetComponent<Image>();
        progressTextBottom = transform.Find("ProgressTextBottom").gameObject;
    }

    void OnEnable()
    {
        if (Startup.Initialized)
        {
            //start countdown
            start = DateTime.Now;
            timeouted = false;
            errorPanel.SetActive(false);

        }

    }

    void OnDisable()
    {
        errorPanel.SetActive(false);
    }

    void Update()
    {

        if (!timeouted && timeout >= 0)
        {
            TimeSpan diff = DateTime.Now.Subtract(start);

            if (diff.TotalSeconds > timeout)
            {
                progressTextBottom.SetActive(false);
                errorPanel.SetActive(true);
                if (ErrorMessage != "")
                {
                    errorPanel.transform.Find("InfoText").GetComponent<Text>().text = ErrorMessage;
                }
                else
                {
                    errorPanel.transform.Find("InfoText").GetComponent<Text>().text = Lang.Get("UI:MultiplayerPopupLoading:Fail");
                }

                timeouted = true;
            }
        }
    }

}

}
