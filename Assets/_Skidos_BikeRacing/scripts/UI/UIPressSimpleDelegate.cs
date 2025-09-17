namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class UIPressSimpleDelegate : MonoBehaviour
{

    public delegate void SimplePressDelegate(bool isPressed);
    public SimplePressDelegate pressDelegate;

    void Start()
    {

    }

    void OnPress(bool isPressed)
    {
        if (enabled)
        {
            if (pressDelegate != null)
            {
                pressDelegate(isPressed);
            }
        }
    }
}

}
