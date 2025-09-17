namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using SimpleJSON;

public class PSWindView : MonoBehaviour, ISaveable
{

    public float rotationZ = 0;
    public float scaleY = 0;

    ParticleSystem ps;

    //	void OnEnable() {
    //		
    //		Init ();
    //		
    //	}

    void Init()
    {

        ps = transform.Find("particles").GetComponent<ParticleSystem>();
        UpdateChildren();

    }

    void Start()
    {

        UpdateChildren();
    }

    public void Load(JSONNode node)
    {

        rotationZ = node["rotationZ"].AsFloat;
        scaleY = node["scaleY"].AsFloat;

        Init();

    }

    public JSONClass Save()
    {

        var J = new JSONClass();

        J["rotationZ"].AsFloat = 360f - transform.localRotation.eulerAngles.z;
        J["scaleY"].AsFloat = transform.localScale.y;

        return J;
    }

    void UpdateChildren()
    {

        ps.startRotation = rotationZ * Mathf.Deg2Rad;
        ps.startLifetime = scaleY * 2.13f;


    }

    //This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
    void OnValidate()
    {

        //		print ("onValidate");
        if (ps == null)
        {
            ps = transform.Find("particles").GetComponent<ParticleSystem>();
        }

        if (ps != null)
        {
            rotationZ = 360f - transform.localRotation.eulerAngles.z;
            scaleY = transform.localScale.y;
            UpdateChildren();
        }

    }

}

}
