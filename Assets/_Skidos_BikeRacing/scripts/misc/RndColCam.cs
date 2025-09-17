namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class RndColCam : MonoBehaviour
{

    // Use this for initialization
    void Awake()
    {
        GetComponent<Camera>().backgroundColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
    }

}

}
