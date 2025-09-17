namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class PuddleBehaviour : MonoBehaviour
{

    ParticleSystem particles;
    Animator anim;

    [SerializeField]
    string triggeringObjectName = "";
    [SerializeField]
    bool triggered = false;

    float velocityThreshold = 2.0f;

    void Awake()
    {
        particles = transform.Find("Particles").GetComponent<ParticleSystem>();
        particles.Stop();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {

        if (!triggered && Mathf.Abs(BikeGameManager.playerControl.bodyVelocityX) > velocityThreshold)
        {// && (coll.gameObject.tag == "bike-part" || coll.gameObject.tag == "Player")

            triggered = true;
            triggeringObjectName = coll.name;

            particles.Play();
        }

    }

    void OnTriggerExit2D(Collider2D coll)
    {

        if (triggered && coll.name == triggeringObjectName)
        {
            triggeringObjectName = "";
            triggered = false;
        }

    }

    public void Reset()
    {

        particles.Clear();
        particles.Stop();

    }


}
}
