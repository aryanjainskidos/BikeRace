namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using SimpleJSON;

public class PSSpring : MonoBehaviour, ISaveable
{

    public float force = 100;

    SpringBehaviour sb;

    void OnEnable()
    {

        Init();

    }

    void Init()
    {

        sb = transform.Find("Spring").GetComponent<SpringBehaviour>();
        UpdateChildren();

    }

    public void Load(JSONNode node)
    {

        force = node["force"].AsFloat;

        Init();

    }

    public JSONClass Save()
    {

        var J = new JSONClass();

        J["force"].AsFloat = force;

        return J;
    }

    void UpdateChildren()
    {

        sb.force = force;

    }

    //This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
    void OnValidate()
    {

        //		print ("onValidate");
        if (sb == null)
        {
            sb = transform.Find("Spring").GetComponent<SpringBehaviour>();
        }

        if (sb != null)
        {
            UpdateChildren();
        }

    }

}

}
