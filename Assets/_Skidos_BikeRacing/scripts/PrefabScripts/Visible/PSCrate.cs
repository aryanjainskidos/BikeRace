namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using SimpleJSON;

/// <summary>
/// ...
/// </summary>

public class PSCrate : MonoBehaviour, ISaveable
{

    public int count = 1;

    CrateBehaviour ccb;

    void OnEnable()
    {
        Init();
    }


    void Init()
    {
        ccb = transform.GetComponentInChildren<CrateBehaviour>();
        UpdateChildren();
    }


    public void Load(JSONNode node)
    {
        if (node["count"] != null)
        {
            count = node["count"].AsInt;
        }
        Init();
    }


    public JSONClass Save()
    {
        var J = new JSONClass();
        J["count"].AsInt = count;
        return J;
    }


    void UpdateChildren()
    {
        ccb.count = count;
    }


    //This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
    void OnValidate()
    {

        if (ccb == null)
        {
            ccb = transform.GetComponentInChildren<CrateBehaviour>();
        }

        if (ccb != null)
        {
            UpdateChildren();
        }

    }

}

}
