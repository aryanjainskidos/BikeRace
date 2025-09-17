namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

/**
 * @note -- baikam pieder poligonáls trigeris, kas dzívo citá slání ká páréjá fizika - "Entities"
 * visiem objektiem, ar ko mocis varés mijiedarboties, arí ir jádzívo śajá slání
 * 
 * @note -- śis objekts hierarhijá dzívo paraléli baikam, śis skripts katrá UPDATE() objektu novieto baika pozícijá  (ja objekts dzívotu hierarhijá zem baika, tad mainot formu mainítos baika braukśanas parametri ):
 */
public class RagdollEntityTrigger : MonoBehaviour
{

    public string collName;
    public string collTag;

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.layer == 0)
        {
            //Debug.LogError("Physics - Collider hit layer - "+coll.gameObject.layer+" - "+coll.gameObject.name);
            return;
        }

        //        print("trigger enter");

        collName = coll.name;
        collTag = coll.tag;

        switch (coll.tag)
        {

            case "DeathZone":
                //                print("DeathZone");
                if (BikeGameManager.initialized && BikeGameManager.playerState.dead)
                {//ragdoll fell into a deathzone

                    //                    if (GameManager.singlePlayerRestarts == 0) { //if in a long level go to finish 
                    //                        UIManager.SwitchScreen(GameScreenType.PostGameLong);
                    //                    } else {

                    if (BikeGameManager.singlePlayerRestarts != 0)
                    {//enough for a simple level, if during crash a deathzone has been entered, reset game and go to pre game screen
                        UIManager.SwitchScreen(GameScreenType.PreGame);
                        BikeGameManager.ExecuteCommand(GameCommand.Reset);
                    }
                    else
                    {
                        BikeGameManager.ExecuteCommand(GameCommand.PauseOn);
                        UIManager.SwitchScreen(GameScreenType.PostGameLong);
                        print("stop game for long");
                    }

                }
                break;

            default:
                break;
        }

    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.layer == 0)
        {
            //Debug.LogError("Physics - Collider hit layer - "+coll.gameObject.layer+" - "+coll.gameObject.name);
            return;
        }
        collName = "";
        collTag = "";
    }

    void Update()
    {
        //		if(GameManager.playerRagdoll != null) //moved automatically with Core(parent)
        //            transform.position = GameManager.playerRagdoll.transform.FindChild("Core").position; //novieto objektu baika pozícijá

        if (!GetComponent<Collider2D>().enabled && collName != "")
        {
            //			print("script was removed");
            Reset();
        }

    }

    public void Reset()
    {

        collName = "";
        collTag = "";
    }

}

}
