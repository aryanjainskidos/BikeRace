namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class StarAnimationEvents : MonoBehaviour
{

    public delegate void FinishDelegate();
    public FinishDelegate finishDelegate;

    public void PlaySound()
    {
        SoundManager.Play("FinishStarPop");
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
