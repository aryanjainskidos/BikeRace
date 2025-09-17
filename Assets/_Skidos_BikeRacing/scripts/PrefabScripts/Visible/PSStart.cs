namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using SimpleJSON;

public class PSStart : MonoBehaviour
{

    GameObject player;

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.layer == 0)
        {
            //Debug.LogError("Physics - Collider hit layer - "+coll.gameObject.layer+" - "+coll.gameObject.name);
            return;
        }

        if (coll.name == "entity_trigger")
        {
            player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<BikeControl>().useAccelerationStart = true;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.layer == 0)
        {
            //Debug.LogError("Physics - Collider hit layer - "+coll.gameObject.layer+" - "+coll.gameObject.name);
            return;
        }

        if (coll.name == "entity_trigger")
        {
            player.GetComponent<BikeControl>().useAccelerationStart = false;
        }
    }

}

}
