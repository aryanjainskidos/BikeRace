namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class PSWater : MonoBehaviour, ISaveable
{

    public float force = 100;

    Transform ice;

    List<GameObject> iceBlocks;
    int iceBlockIndex = 0;
    bool invoke = false;

    GameObject splash;
    bool splashed = false;

    void Start()
    {

        ice = transform.Find("Ice");
        ice.localScale = new Vector3(GetComponent<Collider2D>().bounds.size.x, 1, 1);
        //		ice.renderer.material.mainTextureScale = new Vector2 (ice.transform.localScale.x, 1);
        //		ice.renderer.sortingOrder = 99;
        ice.position = new Vector3(GetComponent<Collider2D>().bounds.center.x, GetComponent<Collider2D>().bounds.max.y, ice.position.z);


        if (GetComponent<Renderer>() != null)
        {
            GetComponent<Renderer>().sortingLayerName = "Default";
            GetComponent<Renderer>().sortingOrder = 98;
        }

        if (gameObject.GetComponent<Collider2D>() != null)
        {
            gameObject.GetComponent<Collider2D>().isTrigger = true;
        }
        else
            print("water needs a collider");


        GameObject icePrefab = (GameObject)LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("Ice_Block");
        Debug.Log("<color=yellow>Prefab Loaded Name = </color>" + icePrefab);
        //GameObject icePrefab = (GameObject)Resources.Load("Prefabs/AnimatedObjects/Ice_Block", typeof(GameObject));

        Vector3 tmpPos = new Vector3(
                GetComponent<Collider2D>().bounds.min.x,
                GetComponent<Collider2D>().bounds.max.y,
                icePrefab.transform.position.z
            );


        int blockCount = Mathf.CeilToInt(GetComponent<Collider2D>().bounds.size.x / icePrefab.GetComponent<Renderer>().bounds.size.x);

        iceBlocks = new List<GameObject>();

        GameObject tmpIceBlock;
        for (int i = 0; i < blockCount; i++)
        {

            tmpIceBlock = Instantiate(
                icePrefab,
                tmpPos + Vector3.right * icePrefab.GetComponent<Renderer>().bounds.size.x * i,
                icePrefab.transform.rotation//tmpRot
                ) as GameObject;

            tmpIceBlock.transform.parent = ice;

            iceBlocks.Add(tmpIceBlock);

        }

        BikeGameManager.ShowChildren(ice.transform, false);

        GameObject splashPrefab = (GameObject)LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("Splash");
        Debug.Log("<color=yellow>Prefab is loading from = </color>" + splashPrefab);
        //GameObject splashPrefab = (GameObject)Resources.Load("Prefabs/AnimatedObjects/Splash", typeof(GameObject));

        splash = Instantiate(
            splashPrefab,
            tmpPos + Vector3.up * 0.3f,
            splashPrefab.transform.rotation//tmpRot
            ) as GameObject;

        splash.transform.parent = transform;

        for (int i = 1; i < 16; i++)
        {
            splash.transform.Find("Splash" + i).GetComponent<Animator>().speed = 0;//TODO save all animators for splash in an array
        }

    }

    public void Load(JSONNode node)
    {
        force = node["force"].AsFloat;

    }

    public JSONClass Save()
    {
        var J = new JSONClass();
        J["force"].AsFloat = force;
        return J;
    }

    public void Reset()
    {
        if (ice != null)
        {
            BikeGameManager.ShowChildren(ice.transform, false);
            ice.GetComponent<Collider2D>().enabled = false;

            iceBlockIndex = 0;
            invoke = false;
        }
        splashed = false;
    }

    void ShowNextIceBlock()
    {

        if (invoke && iceBlockIndex < iceBlocks.Count)
        {
            iceBlocks[iceBlockIndex].GetComponent<Renderer>().enabled = true;
            iceBlockIndex++;

            Invoke("ShowNextIceBlock", 0.05f);
        }

    }

    void Update()
    {

        if (BikeGameManager.player != null && BikeGameManager.player.transform.position.x > GetComponent<Collider2D>().bounds.min.x)
        {

            if (BikeDataManager.Boosts["ice"].Active)
            {

                ice.GetComponent<Collider2D>().enabled = true;

                invoke = true;
                ShowNextIceBlock();

            }

        }


    }

    void OnTriggerEnter2D(Collider2D coll)
    {

        //TODO play splash anim

        if (BikeDataManager.Boosts["ice"].Active)
        {

        }
        else if (!splashed)
        {
            Vector3 splashPos = splash.transform.position;
            splashPos.x = coll.transform.position.x;
            splash.transform.position = splashPos;

            for (int i = 1; i < 16; i++)
            {
                splash.transform.Find("Splash" + i).GetComponent<Animator>().speed = 1;
                splash.transform.Find("Splash" + i).GetComponent<Animator>().Play("LiquidSplash", -1, 0);
            }

            splashed = true;

            SoundManager.Play("Splash");
        }


    }

    void OnTriggerStay2D(Collider2D coll)
    {

        if (coll.GetComponent<Rigidbody2D>() != null)
        {
            coll.GetComponent<Rigidbody2D>().AddForce(-coll.GetComponent<Rigidbody2D>().linearVelocity * force * coll.GetComponent<Rigidbody2D>().mass);
        }
    }

}

}
