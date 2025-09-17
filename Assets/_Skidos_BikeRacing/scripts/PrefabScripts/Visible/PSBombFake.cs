namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using SimpleJSON;

public enum BombGroup
{
    None,
    Group_0,
    Group_1,
    Group_2,
    Group_3,
    Group_4,
    Group_5,
    Group_6,
    Group_7,
    Group_8,
    Group_9,
    Group_A,
    Group_B,
    Group_C,
    Group_D,
    Group_E,
    Group_F,
}

public class PSBombFake : MonoBehaviour, ISaveable
{

    public BombGroup group;
    public float lifeAfterExplosion = 0;

    BombFakeBehaviour bb;

    void OnEnable()
    {

        Init();

    }

    void Init()
    {

        bb = transform.Find("BombFake").GetComponent<BombFakeBehaviour>();
        UpdateChildren();

    }

    public void Load(JSONNode node)
    {

        lifeAfterExplosion = node["lifeAfterExplosion"].AsFloat;
        group = (BombGroup)node["group"].AsInt;

        Init();

    }

    public JSONClass Save()
    {

        var J = new JSONClass();

        J["lifeAfterExplosion"].AsFloat = lifeAfterExplosion;
        J["group"].AsInt = (int)group;

        return J;

    }

    void UpdateChildren()
    {

        bb.lifeAfterExplosion = lifeAfterExplosion;
        bb.group = group;

    }

    //This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
    void OnValidate()
    {

        //		print ("onValidate");
        if (bb == null)
        {
            bb = transform.Find("BombFake").GetComponent<BombFakeBehaviour>();
        }

        if (bb != null)
        {
            UpdateChildren();
        }

    }

}

}
