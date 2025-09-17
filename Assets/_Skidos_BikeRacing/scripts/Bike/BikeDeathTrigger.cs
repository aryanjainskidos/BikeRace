namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class BikeDeathTrigger : MonoBehaviour
{

    public string collName;
    public string collTag;

    void Awake()
    {
        if (gameObject.layer == 9)
        {
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            PolygonCollider2D polygonCollider = GetComponent<PolygonCollider2D>();

            if (boxCollider != null)
            {
                //boxCollider.enabled = false;
                Destroy(boxCollider);
            }

            if (polygonCollider != null)
            {
                //polygonCollider.enabled = false;
                Destroy(polygonCollider);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.layer == 8 || coll.gameObject.layer == 9)
        {
            //Debug.LogError("Physics - Collider hit layer - "+coll.gameObject.layer+" - "+coll.gameObject.name);
            return;
        }
        collName = coll.collider.name;
        collTag = coll.collider.tag;

        BikeGameManager.BikeJustDied();

    }

    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.gameObject.layer == 8 || coll.gameObject.layer == 9)
        {
            //Debug.LogError("Physics - Collider hit layer - "+coll.gameObject.layer+" - "+coll.gameObject.name);
            return;
        }
        collName = "";
        collTag = "";
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.layer == 8 || coll.gameObject.layer == 9)
        {
            //Debug.LogError("Physics - Collider hit layer - "+coll.gameObject.layer+" - "+coll.gameObject.name);
            return;
        }

        collName = coll.name;
        collTag = coll.tag;

        BikeGameManager.BikeJustDied();

    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.layer == 8 || coll.gameObject.layer == 9)
        {
            //Debug.LogError("Physics - Collider hit layer - "+coll.gameObject.layer+" - "+coll.gameObject.name);
            return;
        }
        collName = "";
        collTag = "";
    }

}

}
