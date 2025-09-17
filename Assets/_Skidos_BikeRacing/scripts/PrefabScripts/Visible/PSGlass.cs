namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using SimpleJSON;

public class PSGlass : MonoBehaviour, ISaveable
{

    public float lifeAfterExplosion = 5;
    public bool animateOnCollision = true;

    GlassBehaviour bb;

    void OnEnable()
    {
        Init();
    }

    void Init()
    {

        bb = transform.Find("Glass").GetComponent<GlassBehaviour>();
        UpdateChildren();

    }

    public void Load(JSONNode node)
    {

        lifeAfterExplosion = node["lifeAfterExplosion"].AsFloat;
        if (node["animateOnCollision"] != null)
            animateOnCollision = node["animateOnCollision"].AsBool;
        Init();

    }

    public JSONClass Save()
    {

        var J = new JSONClass();
        J["lifeAfterExplosion"].AsFloat = lifeAfterExplosion;
        J["animateOnCollision"].AsBool = animateOnCollision;
        return J;

    }

    void UpdateChildren()
    {
        bb.lifeAfterExplosion = lifeAfterExplosion;
        bb.animateOnCollision = animateOnCollision;
    }

    //This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
    void OnValidate()
    {

        //		print ("onValidate");
        if (bb == null)
        {
            bb = transform.Find("Glass").GetComponent<GlassBehaviour>();
        }

        if (bb != null)
        {
            UpdateChildren();
        }

    }

}
}
