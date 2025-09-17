namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using SimpleJSON;

public class PSBombRadial : MonoBehaviour, ISaveable
{

    public float radius = 10f;
    public float force = 100;
    public float lifeAfterExplosion = 0;
    public bool isKinematic = false;

    BombRadialBehaviour bb;

    void OnEnable()
    {

        Init();

    }

    void Init()
    {

        bb = transform.Find("BombRadial").GetComponent<BombRadialBehaviour>();
        UpdateChildren();

    }

    public void Load(JSONNode node)
    {

        radius = node["radius"].AsFloat;
        force = node["force"].AsFloat;
        lifeAfterExplosion = node["lifeAfterExplosion"].AsFloat;

        if (node["isKinematic"] != null)
            isKinematic = node["isKinematic"].AsBool;

        Init();

    }

    public JSONClass Save()
    {

        var J = new JSONClass();

        J["force"].AsFloat = force;
        J["radius"].AsFloat = radius;
        J["lifeAfterExplosion"].AsFloat = lifeAfterExplosion;
        J["isKinematic"].AsBool = isKinematic;

        return J;

    }

    void UpdateChildren()
    {

        bb.radius = radius;
        bb.force = force;
        bb.lifeAfterExplosion = lifeAfterExplosion;
        bb.isKinematic = isKinematic;

    }

    //This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
    void OnValidate()
    {

        //		print ("onValidate");
        if (bb == null)
        {
            bb = transform.Find("BombRadial").GetComponent<BombRadialBehaviour>();
        }

        if (bb != null)
        {
            UpdateChildren();
        }

    }

}

}
