namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

/**
 * pikseljperfekta kamera 
 * tikai spraitiem 
 * svarígi: + pixels per unit = 1 (tekstúras iestatíjumos)
 */
public class PixelPerfectCam : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Camera>().orthographicSize = Screen.height / 2f;
    }
}

}
