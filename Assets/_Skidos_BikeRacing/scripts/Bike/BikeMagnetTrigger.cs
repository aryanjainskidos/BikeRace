namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

/**
 * @note -- baikam pieder poligonáls trigeris, kas dzívo citá slání ká páréjá fizika - "Entities"
 * visiem objektiem, ar ko mocis varés mijiedarboties, arí ir jádzívo śajá slání
 * 
 * @note -- śis objekts hierarhijá dzívo paraléli baikam, śis skripts katrá UPDATE() objektu novieto baika pozícijá  (ja objekts dzívotu hierarhijá zem baika, tad mainot formu mainítos baika braukśanas parametri ):
 */
public class BikeMagnetTrigger : MonoBehaviour
{

    public float defaultRadius = 0;

    void Start()
    {

        defaultRadius = GetComponent<CircleCollider2D>().radius;
        if (BikeGameManager.entityTrigger != null)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), BikeGameManager.entityTrigger.GetComponent<Collider2D>());
        }

    }


    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.layer == 0)
        {
            //Debug.LogError("Physics - Collider hit layer - "+coll.gameObject.layer+" - "+coll.gameObject.name);
            return;
        }

        if (coll.tag == "Pickup")
        { //monétas uz citi paceĺami śtruntińi
            PickupManager.PickUp(coll.gameObject);
        }

    }


    void Update()
    {

        if (BikeGameManager.player != null)
        {
            transform.position = BikeGameManager.player.transform.position; //novieto objektu baika pozícijá
            transform.rotation = BikeGameManager.player.transform.rotation;
        }

    }

    public void Reset()
    {

        if (defaultRadius != 0)
        {
            GetComponent<CircleCollider2D>().radius = defaultRadius;
        }

    }

}

}
