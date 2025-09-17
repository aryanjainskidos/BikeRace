namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

/**
 * @note -- baikam pieder poligonáls trigeris, kas dzívo citá slání ká páréjá fizika - "Entities"
 * visiem objektiem, ar ko mocis varés mijiedarboties, arí ir jádzívo śajá slání
 * 
 * @note -- śis objekts hierarhijá dzívo paraléli baikam, śis skripts katrá UPDATE() objektu novieto baika pozícijá  (ja objekts dzívotu hierarhijá zem baika, tad mainot formu mainítos baika braukśanas parametri ):
 */
public class BikeEntityTrigger : MonoBehaviour
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

        collName = coll.name;
        collTag = coll.tag;

        switch (coll.tag)
        {
            case "Finish": //sastop finiśa prefabu, kas tiek identificéts péc taga
                BikeGameManager.PlayerJustFinished();
                break;

            //            case "CheckpointZone": //sastop finiśa prefabu, kas tiek identificéts péc taga
            //                GameManager.PlayerReachedCheckpoint();
            //                break;  

            case "DeathZone":
                if (BikeGameManager.initialized)
                {
                    BikeGameManager.playerState.invincible = false;
                    BikeGameManager.BikeJustDied();
                    if (BikeGameManager.singlePlayerRestarts != 0)
                    {
                        BikeGameManager.ExecuteCommand(GameCommand.Reset);
                    }
                    else
                    {
                        //print("stop game for long");
                        BikeGameManager.ExecuteCommand(GameCommand.PauseOn);
                        UIManager.SwitchScreen(GameScreenType.PostGameLong);
                    }

                }
                break;

            case "SpikeZone":
                if (!BikeGameManager.playerState.invincible)
                {
                    BikeGameManager.BikeJustDied();
                    SoundManager.Play("Spikes");
                }
                break;

            case "StuntZone":
                if (BikeGameManager.playerState != null)
                    BikeGameManager.playerState.stunt = true;
                break;

            case "TutorialZone":
                string levelName = BikeGameManager.lastLoadedLevelName;
                if (levelName == "a___002" || levelName == "a___003" || levelName == "a___004" || levelName == "a___005" || levelName == "a___006" || levelName == "a___007" || levelName == "a___008")
                {
                    GameObject.Find("Canvas_game").transform.Find("Game/OnScreenControlPanel/DownButton/Pointer").gameObject.SetActive(true);
                }
                else if (levelName == "a___009" || levelName == "a___012")

                {
                    GameObject.Find("Canvas_game").transform.Find("Game/OnScreenControlPanel/BrakeButton/Pointer").gameObject.SetActive(true);
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

        if (coll.tag == "StuntZone")
        { //sastop finiśa prefabu, kas tiek identificéts péc taga

            BikeGameManager.playerState.stunt = false;

        }
        else if (coll.tag == "TutorialZone")
        {
            GameObject.Find("Canvas_game").transform.Find("Game/OnScreenControlPanel/DownButton/Pointer").gameObject.SetActive(false);
            GameObject.Find("Canvas_game").transform.Find("Game/OnScreenControlPanel/BrakeButton/Pointer").gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (BikeGameManager.player != null)
            transform.position = BikeGameManager.player.transform.position; //novieto objektu baika pozícijá

        if (!GetComponent<Collider2D>().enabled && collName != "")
        {
            //print("script was removed");
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
