namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class BikeTriggerCollision : MonoBehaviour
{

    public bool colliding = false;
    public bool collisionEntered = false;
    public string collName;
    public Collider2D coll2d;

    public bool collKinematic = false;

    void Awake()
    {
        //print ("ignore collisions");
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), transform.parent.GetComponent<Collider2D>(), true);
    }

    void OnTriggerEnter2D(Collider2D coll)
    {

        if (coll.gameObject.layer == 8)
        {
            //Debug.LogError("Physics - Collider hit layer - " + coll.gameObject.layer + " - " + coll.gameObject.name);
            return;
        }

        if (coll.tag != "Player" && coll.tag != "bike-part")
        {

            colliding = true;
            collisionEntered = true;
            collName = coll.name;
            coll2d = coll;

            if (coll2d.GetComponent<Rigidbody2D>() != null)
            {
                collKinematic = coll2d.GetComponent<Rigidbody2D>();
            }

        }

    }

    void OnTriggerExit2D(Collider2D coll)
    {

        if (coll.gameObject.layer == 8)
        {
            //Debug.LogError("Physics - Collider hit layer - " + coll.gameObject.layer + " - " + coll.gameObject.name);
            return;
        }

        if (coll.tag != "Player" && coll.tag != "bike-part")
        {

            colliding = false;
            collisionEntered = false;
            collName = "";

        }

    }

    void OnTriggerStay2D(Collider2D coll)
    {

        if (coll.gameObject.layer == 8)
        {
            //Debug.LogError("Physics - Collider hit layer - "+coll.gameObject.layer+" - "+coll.gameObject.name);
            return;
        }

        if (coll.tag != "Player" && coll.tag != "bike-part")
        {

            colliding = true;
            collName = coll.name;
            coll2d = coll;

        }
        else
            coll2d = null;

    }

    void FixedUpdate()
    {

        if (coll2d != null && coll2d.GetComponent<Rigidbody2D>() != null && coll2d.GetComponent<Rigidbody2D>().isKinematic != collKinematic)
        {
            //print("!!!changed iskinematic");
        }

        //account for destroyed objects or objects that changed kinematic before exiting
        if (colliding &&
                   (coll2d == null ||
                 !coll2d.GetComponent<Collider2D>().enabled ||
                 (coll2d.GetComponent<Rigidbody2D>() != null && coll2d.GetComponent<Rigidbody2D>().isKinematic != collKinematic)))
        {
            colliding = false;
            coll2d = null;
            collKinematic = false;
        }

        //		colliding = false;

    }
}
}
