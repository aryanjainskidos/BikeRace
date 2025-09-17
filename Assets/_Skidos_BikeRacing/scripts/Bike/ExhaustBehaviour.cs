namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class ExhaustBehaviour : MonoBehaviour
{

    ParticleSystem particles;

    void Awake()
    {
        particles = transform.Find("particles").GetComponent<ParticleSystem>();
        TurnOff();
    }

    void OnEnable()
    {
        TurnOff();
    }

    public void TurnOn()
    {
        if (!BikeDataManager.SettingsHD)
        { //nav HD, nav dúmu
            TurnOff();
            return;
        }


        particles.enableEmission = true;
        particles.Play();
    }

    public void TurnOff()
    {

        particles.Stop();
        particles.Clear();
        particles.enableEmission = false;
        if (particles.GetComponent<ParticleSystem>() != null)
        {
            particles.GetComponent<ParticleSystem>().Clear();
        }

    }

    public void StopEmission()
    {
        particles.enableEmission = false;
    }

    public void Reset()
    {
        TurnOff();
    }

}

}
