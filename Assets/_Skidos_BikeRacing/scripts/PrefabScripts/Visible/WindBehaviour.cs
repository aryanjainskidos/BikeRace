namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class WindBehaviour : MonoBehaviour
{

    public float force = 100;

    GameObject player;
    GameObject[] bikeParts;

    void OnTriggerEnter2D(Collider2D coll)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        bikeParts = GameObject.FindGameObjectsWithTag("bike-part");
    }

    void OnTriggerStay2D(Collider2D coll)
    {

        //		if(coll.gameObject.tag == "bike-part" || coll.gameObject.tag == "Player") {

        print("blow with force" + force.ToString());
        //			Explode();
        //			
        //			lastExplodeTime = Time.time;

        player.GetComponent<Rigidbody2D>().AddForce(gameObject.transform.right * force);

        for (int i = 0; i < bikeParts.GetLength(0); i++)
        {

            if (bikeParts[i].GetComponent<Collider2D>() != null &&
               !bikeParts[i].GetComponent<Collider2D>().isTrigger &&
               bikeParts[i].GetComponent<Collider2D>().attachedRigidbody != null //&& (bikeParts[i].tag == "Player" || bikeParts[i].tag == "bike-part")
               )
            {

                bikeParts[i].GetComponent<Rigidbody2D>().AddForce(gameObject.transform.right * force);

            }

        }

        //		}
    }

}

}
