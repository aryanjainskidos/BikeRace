namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using SimpleJSON;

public class PSPuddle : MonoBehaviour, ISaveable
{

    float scaleX = 1;
    float scaleY = 1;

    //	PuddleBehaviour sb;
    Transform bp;

    void OnEnable()
    {

        Init();

        if (transform.localScale != Vector3.one)
        {
            transform.localScale = Vector3.one;
        }

    }

    void Init()
    {

        bp = transform.Find("Puddle");
        //		sb = transform.FindChild ("Puddle").GetComponent<PuddleBehaviour> ();

        //		UpdateChildren ();

    }

    void Start()
    {

        if (transform.localScale != Vector3.one)
        {
            transform.localScale = Vector3.one;
        }

        bp.localScale = new Vector3(scaleX * bp.localScale.x, scaleY * bp.localScale.y, bp.localScale.z);
    }

    public void Load(JSONNode node)
    {

        scaleX = node["scaleX"].AsFloat;
        scaleY = node["scaleY"].AsFloat;

        Init();
    }

    public JSONClass Save()
    {

        var J = new JSONClass();

        J["scaleX"].AsFloat = transform.localScale.x * scaleX;
        J["scaleY"].AsFloat = transform.localScale.y * scaleY;

        return J;
    }

    //	void UpdateChildren() {
    //		sb.force = force;
    //		sb.group = group;
    //	}

    //	//This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
    //	void OnValidate(){
    //		print ("onValidate");
    //		if(sb == null) {
    //			bp = transform.FindChild ("Puddle");
    //			sb = transform.FindChild ("Puddle").GetComponent<PuddleBehaviour> ();
    //		}
    //		if(sb != null) {
    //			UpdateChildren ();
    //		}
    //	}

}

}
