namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using SimpleJSON;

/// <summary>
/// ...
/// </summary>

public class PSCoinCrate : MonoBehaviour, ISaveable
{

    public int coinCount = 1;

    CoinCrateBehaviour ccb;

    void OnEnable()
    {

        Init();

    }

    void Init()
    {

        ccb = transform.Find("CoinCrate").GetComponent<CoinCrateBehaviour>();
        UpdateChildren();

    }

    public void Load(JSONNode node)
    {

        coinCount = node["coinCount"].AsInt;

        Init();

    }

    public JSONClass Save()
    {

        var J = new JSONClass();

        J["coinCount"].AsInt = coinCount;

        return J;

    }

    void UpdateChildren()
    {

        ccb.coinCount = coinCount;

    }

    //This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
    void OnValidate()
    {

        //      print ("onValidate");
        if (ccb == null)
        {
            ccb = transform.Find("CoinCrate").GetComponent<CoinCrateBehaviour>();
        }

        if (ccb != null)
        {
            UpdateChildren();
        }

    }

}

}
