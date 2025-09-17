namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using SimpleJSON;

/// <summary>
/// PS Bullet Time.
/// on activating the trigger slows down time for ammount of time specified in the time variable
/// one bullet time per level in apps lifetime
/// </summary>

public class PSCheckpoint : MonoBehaviour, ISaveable
{

    public int id = 0;
    public bool visited = false;
    public CheckpointGroup group;
    GameObject[] checkpointPoles;

    public void Load(JSONNode node)
    {

        //        time = node["time"].AsFloat;
        group = (CheckpointGroup)node["group"].AsInt;

    }

    public JSONClass Save()
    {

        var J = new JSONClass();

        //        J["time"].AsFloat = time;	
        J["group"].AsInt = (int)group;

        return J;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.name == "entity_trigger")
        {
            //            GameManager.ExecuteCommand(GameCommand.BulletTimeOn);
            ////            DataManager.Levels[levelInfo.LevelName].BulletTimeUsed = true;
            //            Invoke("TurnOffBulletTime", time);
            if (!visited)
            {
                Activate();
            }

            visited = true;
            BikeGameManager.PlayerReachedCheckpoint(id, transform.position);
        }
    }

    void Activate()
    {

        checkpointPoles = GameObject.FindGameObjectsWithTag("CheckpointPole");

        CheckpointPoleBehaviour cpb;
        foreach (GameObject pole in checkpointPoles)
        {

            cpb = pole.GetComponent<CheckpointPoleBehaviour>();

            if (cpb.group == group)
            {
                cpb.Activate();
            }

        }

    }

    //    void OnTriggerExit2D(Collider2D coll) {
    //        if (coll.name == "entity_trigger") {
    //            //          player = GameObject.FindGameObjectWithTag ("Player");
    //            //          bikeParts = GameObject.FindGameObjectsWithTag ("bike-part");
    //            GameManager.ExecuteCommand(GameCommand.BulletTimeOff);
    //        }
    //    }

    //    void TurnOffBulletTime() {
    //        GameManager.ExecuteCommand(GameCommand.BulletTimeOff);
    //    }

    public void Reset()
    {
        visited = false;
    }
}

}
