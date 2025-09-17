namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class CustomizeBehaviour : MonoBehaviour
{

    public delegate void OnDisableDelegate();
    public OnDisableDelegate onDisableDelegate;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDisable()
    {
        //TODO reset bike colors
        if (onDisableDelegate != null)
            onDisableDelegate();
    }

}

}
