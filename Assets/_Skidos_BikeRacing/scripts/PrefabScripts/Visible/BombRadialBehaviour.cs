namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class BombRadialBehaviour : MonoBehaviour
{

    public float radius = 10f;
    public float force = 100;
    public float lifeAfterExplosion = 0;
    public bool isKinematic = false;

    public bool IsKinematic
    {
        get { return isKinematic; }
        set
        {
            isKinematic = value;
            GetComponent<Rigidbody2D>().isKinematic = isKinematic;
        }
    }

    bool invoke = false;
    bool exploded = false;
    public Animator anim;
    public Transform blast;


    Collider2D[] colliders;

    void Start()
    {
        blast = transform.Find("Blast_animation");
        anim = blast.GetComponent<Animator>();
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
        colliders = Physics2D.OverlapCircleAll(transform.position, radius);//, colliders

        for (int i = 0; i < colliders.GetLength(0); i++)
        {
            if (!colliders[i].isTrigger && colliders[i].attachedRigidbody != null && (colliders[i].tag == "Player" || colliders[i].tag == "bike-part"))
            {

                //fading force closer to edge
                Vector2 dir = (colliders[i].transform.position - gameObject.transform.position);
                dir.x = Mathf.Sign(dir.x) * (radius - Mathf.Abs(dir.x)) / radius;
                dir.y = Mathf.Sign(dir.y) * (radius - Mathf.Abs(dir.y)) / radius;

                colliders[i].GetComponent<Rigidbody2D>().AddForce(dir * force);
            }
        }

        if (lifeAfterExplosion > 0)
        {
            invoke = true;
            Invoke("Remove", lifeAfterExplosion);
        }
        SoundManager.Play("Explosion");

    }

    void Remove()
    {
        if (gameObject.transform.parent != null && invoke)
        {

            if (gameObject.GetComponent<Collider2D>() != null)
            {
                gameObject.GetComponent<Collider2D>().enabled = false;
            }

            if (gameObject.GetComponent<Renderer>() != null)
            {
                gameObject.GetComponent<Renderer>().enabled = false;
            }


            BikeGameManager.ShowChildren(blast, true);
            //			anim.enabled = true;
            anim.speed = 1;
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

        invoke = false;
        exploded = false;

        anim.Play("Blast", -1, 0);
        anim.speed = 0;

        GetComponent<Rigidbody2D>().isKinematic = true;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        GetComponent<Rigidbody2D>().angularVelocity = 0;
        GetComponent<Rigidbody2D>().isKinematic = false;

        GetComponent<Rigidbody2D>().isKinematic = isKinematic;
    }

}

}
