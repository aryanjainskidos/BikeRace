namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class BikeStateData : MonoBehaviour
{

    public bool invincible = false; //will be set by BoostManager
    public bool dead = false;
    public bool drowned = false; //may not be accurate when (dead == false)
    public bool finished = false;
    public bool stunt = false;
    public int stuntID = -1;
    public int starsEarned = 0;
    // Use this for initialization
    //	void Start () {
    //	
    //	}
    //	
    //	// Update is called once per frame
    //	void Update () {
    //	
    //	}

    public void Reset()
    {
        invincible = false;
        dead = false;
        finished = false;
        stunt = false;
        stuntID = -1;
        starsEarned = 0;
    }

}

}
