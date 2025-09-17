namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

/**
 * @note -- baikam pieder poligonáls trigeris, kas dzívo citá slání ká páréjá fizika - "Entities"
 * visiem objektiem, ar ko mocis varés mijiedarboties, arí ir jádzívo śajá slání
 * 
 * @note -- śis objekts hierarhijá dzívo paraléli baikam, śis skripts katrá UPDATE() objektu novieto baika pozícijá  (ja objekts dzívotu hierarhijá zem baika, tad mainot formu mainítos baika braukśanas parametri ):
 */
public class BikeFreezeTrigger : MonoBehaviour
{

    //	public float defaultRadius = 0;

    void Start()
    {

        //		defaultRadius = GetComponent<CircleCollider2D> ().radius;
        //		if(GameManager.entityTrigger != null)
        //			Physics2D.IgnoreCollision (collider2D, GameManager.entityTrigger.collider2D);

    }


    //	void OnTriggerEnter2D(Collider2D coll) {
    //
    //		if(coll.tag == "Pickup") { //monétas uz citi paceĺami śtruntińi
    //
    //			PickupManager.PickUp(coll.gameObject);
    //
    //		}
    //		
    //	}


    void Update()
    {

        if (BikeGameManager.player != null)
            transform.position = BikeGameManager.player.transform.position; //novieto objektu baika pozícijá

    }

    public void Reset()
    {

        //		if (defaultRadius != 0) {
        //
        //			GetComponent<CircleCollider2D> ().radius = defaultRadius;
        //
        //		}

    }

}

}
