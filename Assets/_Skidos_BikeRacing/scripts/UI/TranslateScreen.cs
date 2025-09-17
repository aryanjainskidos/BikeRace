namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class TranslateScreen : MonoBehaviour
{


    void Awake()
    {
        Lang.TranslateScreen(transform);
    }

}
}
