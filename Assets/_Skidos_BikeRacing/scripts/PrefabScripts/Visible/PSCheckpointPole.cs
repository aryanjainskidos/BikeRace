namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using SimpleJSON;

public enum CheckpointGroup
{
    None,
    Group_1,
    Group_2,
    Group_3,
    Group_4,
    Group_5,
    Group_6,
    Group_7,
    Group_8,
    Group_9,
    Group_10,
}

public class PSCheckpointPole : MonoBehaviour, ISaveable
{

    public CheckpointGroup group;

    CheckpointPoleBehaviour bb;

    void OnEnable()
    {

        Init();

    }

    void Init()
    {

        bb = transform.Find("Checkpoint_Pole").GetComponent<CheckpointPoleBehaviour>();
        UpdateChildren();

    }

    public void Load(JSONNode node)
    {

        group = (CheckpointGroup)node["group"].AsInt;

        Init();

    }

    public JSONClass Save()
    {

        var J = new JSONClass();

        J["group"].AsInt = (int)group;

        return J;

    }

    void UpdateChildren()
    {

        bb.group = group;

    }

    //This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
    void OnValidate()
    {

        //		print ("onValidate");
        if (bb == null)
        {
            bb = transform.Find("Checkpoint_Pole").GetComponent<CheckpointPoleBehaviour>();
        }

        if (bb != null)
        {
            UpdateChildren();
        }

    }

}

}
