namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class BombFakeBehaviour : MonoBehaviour
{

    public float lifeAfterExplosion = 0;
    public BombGroup group;
    public Animator anim;
    public Transform blast;
    public Transform bomb;

    void Start()
    {
        if (transform.parent != null)
        {
            blast = transform.parent.Find("Blast_animation");
            bomb = transform.parent.Find("animation");
            anim = blast.GetComponent<Animator>();
        }
    }

    public void Explode()
    {

        if (lifeAfterExplosion > 0)
        {
            float randomTimeOffset = (Random.value - 0.5f) * lifeAfterExplosion;
            Invoke("Remove", lifeAfterExplosion + randomTimeOffset);
        }

    }

    void Remove()
    {

        if (gameObject.transform.parent != null)
        {

            if (gameObject.GetComponent<Collider2D>() != null)
            {
                gameObject.GetComponent<Collider2D>().enabled = false;
            }

            if (gameObject.GetComponent<Renderer>() != null)
            {
                gameObject.GetComponent<Renderer>().enabled = false;
            }

            BikeGameManager.ShowChildren(bomb, false);

            BikeGameManager.ShowChildren(blast, true);
            anim.enabled = true;
            anim.Play("Blast", -1, 0);
        }

    }

    public void Reset()
    {

        if (gameObject.GetComponent<Collider2D>() != null)
        {
            gameObject.GetComponent<Collider2D>().enabled = true;
        }

        if (gameObject.GetComponent<Renderer>() != null)
        {
            gameObject.GetComponent<Renderer>().enabled = true;
        }

        BikeGameManager.ShowChildren(bomb, true);

        if (anim != null)
        {

            BikeGameManager.ShowChildren(blast, false);
            anim.enabled = false;
        }
        //		exploded = false;

    }

}

}
