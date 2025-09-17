namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class FollowMainCamera : MonoBehaviour
{

    float cameraSize;
    Vector3 tmpPosition;
    Vector3 tmpScale;

    void Start()
    {
        cameraSize = 10;//Camera.main.orthographicSize;
                        //
                        //		tmpPosition = transform.position;
                        //		tmpPosition.y = GameManager.levelBounds.center.y;
                        //		transform.position = tmpPosition;

        //		Bounds b = GameManager.calculateBounds (transform);
        //
        //		tmpScale = transform.localScale;
        //		tmpScale.y = GameManager.levelBounds.size.y / b.size.y * 2;//((GameManager.levelBounds.size.y / 2.65f) / cameraSize);
        //		transform.localScale = tmpScale;
    }

    // Update is called once per frame
    void Update()
    {
        //		transform.position = 
        //			Camera.main.transform.position.x * Vector3.right +  
        //				Camera.main.transform.position.y * Vector3.up + 
        //				transform.position.z * Vector3.forward;
        //		transform.localScale = (Vector3.up + Vector3.right) * (Camera.main.orthographicSize / cameraSize) + Vector3.forward;

        tmpPosition = transform.position;
        tmpPosition.x = Camera.main.transform.position.x;

        transform.position = tmpPosition;

        tmpScale = transform.localScale;
        tmpScale.x = Camera.main.orthographicSize / cameraSize;

        transform.localScale = tmpScale;

    }
}
}
