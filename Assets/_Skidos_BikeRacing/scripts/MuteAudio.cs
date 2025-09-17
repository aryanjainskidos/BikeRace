namespace vasundharabikeracing {
using System.Collections;
using System.Collections.Generic;
//using DarkTonic.MasterAudio;
using UnityEngine;

public class MuteAudio : MonoBehaviour
{
    bool skidosStatus;


    private void Start()
    {
        AudioSource audio = GetComponent<AudioSource>();

        if (audio != null)
        {
            audio.volume = 1;
            //MasterAudio.UnmuteEverything();
        }
        //if (SkidosUI.Instance.IsEnabled())
        //{
        //	skidosStatus = true;
        //}
        //else
        //	skidosStatus = false;
    }

    // Update is called once per frame
    void Update()
    {
        //AudioSource audio = GetComponent<AudioSource>();

        ////if (SkidosUI.Instance.IsEnabled() && skidosStatus == true)
        ////{
        ////	if (audio != null)
        ////	{
        ////		audio.volume = 0;
        ////		//MasterAudio.MuteEverything();
        ////	}
        ////	skidosStatus = false;
        ////}

        //if (audio != null)
        //{
        //    audio.volume = 1;
        //    //MasterAudio.UnmuteEverything();
        //}
    }
}

}
