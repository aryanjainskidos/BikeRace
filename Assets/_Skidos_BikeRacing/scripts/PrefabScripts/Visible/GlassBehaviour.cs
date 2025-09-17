namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class GlassBehaviour : MonoBehaviour
{

    public float lifeAfterExplosion = 5;
    public bool animateOnCollision = true;
    bool exploded = false;

    Collider2D[] colliders;

    Transform parts;
    Animator anim;

    bool invoke = false;

    void Start()
    {
        parts = transform.Find("parts");
        anim = parts.GetComponent<Animator>();
        anim.speed = 0;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {

        if (!exploded)
        {

            if (coll.gameObject.tag == "bike-part" || coll.gameObject.tag == "Player")
            {
                Explode();
                exploded = true;
            }

        }

    }

    public void Explode()
    {
        if (animateOnCollision)
        {
            anim.speed = 1;
            SoundManager.Play("GlassBreaking");
        }

        if (lifeAfterExplosion > 0)
        { //animate at the beginning of life cycle
            invoke = true;
            Invoke("Remove", lifeAfterExplosion);
        }

    }

    void Remove()
    {

        if (invoke && gameObject.transform.parent != null)
        {

            if (!animateOnCollision)
            { //animate at the end of life cycle
                anim.speed = 1;
                SoundManager.Play("GlassBreaking");
            }

            if (gameObject.GetComponent<Collider2D>() != null)
            {
                gameObject.GetComponent<Collider2D>().enabled = false;
            }

            //			if (gameObject.renderer != null) {
            //				gameObject.renderer.enabled = false;
            //			}

            //			GameManager.ShowChildren(parts, false);
        }

    }

    public void Reset()
    {

        if (gameObject.GetComponent<Collider2D>() != null)
        {
            gameObject.GetComponent<Collider2D>().enabled = true;
        }

        //		if (gameObject.renderer != null) {
        //			gameObject.renderer.enabled = true;
        //		}

        //		GameManager.ShowChildren(parts, true);

        if (anim != null)
        {
            anim.Play("IceShatter", -1, 0);
            anim.speed = 0;
        }

        exploded = false;
        invoke = false;

    }

}


}
