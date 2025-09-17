namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using SimpleJSON;

/// <summary>
/// PS Bullet Time.
/// on activating the trigger slows down time for ammount of time specified in the time variable
/// one bullet time per level in apps lifetime
/// </summary>

public class PSBirdZone : MonoBehaviour, ISaveable
{

    public bool visited = false;
    public BirdGroup group;
    GameObject[] birds;

    public void Load(JSONNode node)
    {

        //        time = node["time"].AsFloat;
        group = (BirdGroup)node["group"].AsInt;

    }

    public JSONClass Save()
    {

        var J = new JSONClass();
        J["group"].AsInt = (int)group;

        return J;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.layer == 0)
        {
            //Debug.LogError("Physics - Collider hit layer - "+coll.gameObject.layer+" - "+coll.gameObject.name);
            return;
        }

        if (coll.name == "entity_trigger")
        {

            if (!visited)
            {
                Activate();
            }

            visited = true;
        }
    }

    void Activate()
    {

        birds = GameObject.FindGameObjectsWithTag("Bird");

        BirdBehaviour cpb;
        foreach (GameObject pole in birds)
        {

            cpb = pole.GetComponent<BirdBehaviour>();

            if (cpb.group == group)
            {
                cpb.Activate();
            }

        }

    }

    public void Reset()
    {
        visited = false;
    }
}

}
