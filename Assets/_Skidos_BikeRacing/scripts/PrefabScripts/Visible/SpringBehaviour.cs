namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class SpringBehaviour : MonoBehaviour
{

    //public float radius = 10f;
    public float force = 100;

    GameObject player;
    GameObject[] bikeParts;
    public float lastExplodeTime = 0;
    public float explodeCooldown = 0.1f;

    void OnCollisionEnter2D(Collision2D coll)
    {

        //to avoid multiple explosions at the same time
        if ((lastExplodeTime == 0 || Time.time - lastExplodeTime >= explodeCooldown) && (coll.gameObject.tag == "bike-part" || coll.gameObject.tag == "Player"))
        {

            Explode();

            lastExplodeTime = Time.time;

        }

    }


    public void Explode()
    {

        //colliders = Physics2D.OverlapCircleAll (transform.position, radius);

        player = GameObject.FindGameObjectWithTag("Player");
        bikeParts = GameObject.FindGameObjectsWithTag("bike-part");

        player.GetComponent<Rigidbody2D>().AddForce(gameObject.transform.up * force);

        for (int i = 0; i < bikeParts.GetLength(0); i++)
        {

            if (bikeParts[i].GetComponent<Collider2D>() != null &&
               !bikeParts[i].GetComponent<Collider2D>().isTrigger &&
               bikeParts[i].GetComponent<Collider2D>().attachedRigidbody != null //&& (bikeParts[i].tag == "Player" || bikeParts[i].tag == "bike-part")
               )
            {

                bikeParts[i].GetComponent<Rigidbody2D>().AddForce(gameObject.transform.up * force);

            }

        }

    }

}
}
