namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour
{

    public float timerStart = 0;
    public float timerLast = 0;
    public bool timerRunning = false;

    public float TimeElapsed
    {
        get { return timerLast - timerStart; }
    }

    void Update()
    {
        TimerUpdate();
    }

    public void TimerUpdate()
    {
        if (timerRunning)
        {
            timerLast = Time.time;
        }
    }

    public void TimerStart()
    {
        timerRunning = true;
        timerLast = timerStart = Time.time;
    }

    public void TimerStop()
    {
        if (timerRunning)
        {
            timerRunning = false;
            timerLast = Time.time;
        }
    }

    public void TogglePause()
    {
        if (timerStart != 0 && timerLast != 0)
        {
            timerRunning = !timerRunning;
        }
    }

    public void Pause()
    {
        if (timerStart != 0 && timerLast != 0)
        {
            timerRunning = false;
        }
    }

    public void Unpause()
    {
        if (timerStart != 0 && timerLast != 0)
        {
            timerRunning = true;
        }
    }

}

}
