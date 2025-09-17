namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using SimpleJSON;

public class PSWind : MonoBehaviour, ISaveable
{

    public float force = 100;

    WindBehaviour sb;


    public void Load(JSONNode node)
    {
        force = node["force"].AsFloat;
    }

    public JSONClass Save()
    {

        var J = new JSONClass();
        J["force"].AsFloat = force;
        return J;
    }


    GameObject player;
    GameObject[] bikeParts;

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.name == "entity_trigger")
        {
            player = GameObject.FindGameObjectWithTag("Player");
            bikeParts = GameObject.FindGameObjectsWithTag("bike-part");
            SoundManager.Play("Wind");
        }
    }

    void OnTriggerStay2D(Collider2D coll)
    {

        if (coll.name == "entity_trigger")
        {

            player.GetComponent<Rigidbody2D>().AddForce(gameObject.transform.right * force);

            for (int i = 0; i < bikeParts.GetLength(0); i++)
            {

                if (bikeParts[i].GetComponent<Collider2D>() != null &&
                   !bikeParts[i].GetComponent<Collider2D>().isTrigger &&
                   bikeParts[i].GetComponent<Collider2D>().attachedRigidbody != null)
                {

                    bikeParts[i].GetComponent<Rigidbody2D>().AddForce(gameObject.transform.right * force);

                }

            }
        }

    }

}

}
