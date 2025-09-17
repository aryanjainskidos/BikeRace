namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class UIEnableDelegate : MonoBehaviour
{


    public delegate void EnableDelegate();
    public EnableDelegate enableDelegate;


    void OnEnable()
    {
        if (enableDelegate != null)
            enableDelegate();
    }

}

}
