namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class GenericAnimationEvents : MonoBehaviour
{

    public delegate void SimpleDelegate();
    public SimpleDelegate finishDelegate;

    public void PlaySound(string soundName)
    {
        SoundManager.Play(soundName);
    }

    public void OnFinish()
    {
        if (finishDelegate != null)
        {
            finishDelegate();
        }
    }
}
}
