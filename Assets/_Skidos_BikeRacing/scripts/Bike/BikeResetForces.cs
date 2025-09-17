namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class BikeResetForces : MonoBehaviour
{

    // Use this for initialization
    Transform body;
    Transform wheelA;
    Transform wheelB;
    WheelJoint2D[] wheelJoints;

    void Start()
    {

        body = gameObject.transform;

        wheelA = body.Find("wheel_front");
        wheelB = body.Find("wheel_back");

        wheelJoints = body.GetComponents<WheelJoint2D>();
    }


    public void Reset()
    {

        if (body == null)
        { //sometimes Reset is being called before Start
            Start();
        }

        var bodyRig = body.GetComponent<Rigidbody2D>();
        var wheelARig = wheelA.GetComponent<Rigidbody2D>();
        var wheelBRig = wheelB.GetComponent<Rigidbody2D>();

        bodyRig.isKinematic = true;
        wheelARig.isKinematic = true;
        wheelBRig.isKinematic = true;

        bodyRig.linearVelocity = Vector2.zero;
        bodyRig.angularVelocity = 0;
        wheelARig.linearVelocity = Vector2.zero;
        wheelARig.angularVelocity = 0;
        wheelBRig.linearVelocity = Vector2.zero;
        wheelBRig.angularVelocity = 0;

        Vector3 tmpPos;
        foreach (var item in wheelJoints)
        {
            tmpPos = item.connectedBody.transform.localPosition;
            tmpPos.x = item.anchor.x;
            tmpPos.y = item.anchor.y;
            item.connectedBody.transform.localPosition = tmpPos;
        }

        bodyRig.isKinematic = false;
        wheelARig.isKinematic = false;
        wheelBRig.isKinematic = false;

    }
}

}
