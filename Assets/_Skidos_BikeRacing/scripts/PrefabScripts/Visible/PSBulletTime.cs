namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using SimpleJSON;

/// <summary>
/// PS Bullet Time.
/// on activating the trigger slows down time for ammount of time specified in the time variable
/// one bullet time per level in apps lifetime
/// </summary>

public class PSBulletTime : MonoBehaviour, ISaveable
{

    public float time = 1;

    LevelInfo levelInfo;

    void Awake()
    {
        levelInfo = GameObject.Find("LevelContainer").GetComponent<LevelInfo>();
    }

    public void Load(JSONNode node)
    {
        time = node["time"].AsFloat;
    }

    public JSONClass Save()
    {
        var J = new JSONClass();
        J["time"].AsFloat = time;
        return J;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.name == "entity_trigger" && !BikeDataManager.Levels[levelInfo.LevelName].BulletTimeUsed)
        { //nezinu kápéc jácheko vai ir jau lietots śajá límení (jo tas netiek pie restarta noresetots)
            BikeGameManager.ExecuteCommand(GameCommand.BulletTimeOn);//ieslédz bulettaimu
            Invoke("TurnOffBulletTime", time); //efekts beigsies péc x sekundém
        }
    }


    void TurnOffBulletTime()
    {
        BikeGameManager.ExecuteCommand(GameCommand.BulletTimeOff);
    }

}

}
