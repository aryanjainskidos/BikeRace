namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IgnoreChildrenCollisions : MonoBehaviour
{

    bool completed = false;
    //	Dictionary<string, Transform> childTransforms;
    //
    //	void Awake()
    //	{
    //		childTransforms = new Dictionary<string, Transform> ();
    //
    //		foreach (Transform child in transform)
    //		{
    //			childTransforms.Add(child.name, child.transform);
    //		}
    //	}
    //
    //	void OnDisable() {
    //
    //		foreach (Transform child in transform)
    //		{
    //			child.transform.position = childTransforms[child.name].position;
    //			child.transform.rotation = childTransforms[child.name].rotation;
    //		}
    //
    //	}

    // Use this for initialization
    void Start()
    {

        if (!completed)
        {
            Collider2D collA;
            Collider2D collB;

            foreach (Transform childA in transform)
            {
                if (childA.gameObject.activeSelf)
                {
                    collA = childA.GetComponent<Collider2D>();
                    if (collA != null)
                    {

                        foreach (Transform childB in transform)
                        {
                            if (childB.gameObject.activeSelf)
                            {
                                collB = childB.GetComponent<Collider2D>();

                                if (collB != null && collA != collB)
                                {
                                    Physics2D.IgnoreCollision(collA, collB);
                                }
                            }
                        }
                    }
                }
            }

            completed = true;
        }
    }
}

}
