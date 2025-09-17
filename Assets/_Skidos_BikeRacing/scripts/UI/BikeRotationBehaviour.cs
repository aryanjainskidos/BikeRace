namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class BikeRotationBehaviour : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{

    public GameObject bike;

    //UIButton rotateButton;

    float defaultRotationSpeed = 10;
    public float currentRotationSpeed = 10;
    float bikeRotationSpeed = 10;

    [SerializeField]
    bool waitForInput = false;
    public float waitTime = 5;
    DateTime waitStart;
    [SerializeField]
    float rotationDir;

    RectTransform rectTransform;

    // Use this for initialization
    void Awake()
    {

        //        transform.FindChild ("BikeRotateButton").GetComponent<UIDragSimpleDelegate> ().dragDelegate = RotateBikeDrag;
        //        transform.FindChild ("BikeRotateButton").GetComponent<UIPressSimpleDelegate> ().pressDelegate = RotateBikePress;
        //        rotateButton = transform.FindChild ("BikeRotateButton").GetComponent<UIButton> ();
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {

        if (waitForInput)
        {
            TimeSpan diff = DateTime.Now.Subtract(waitStart);

            if (diff.TotalSeconds > waitTime)
            {
                bikeRotationSpeed = rotationDir * defaultRotationSpeed;
                waitForInput = false;
            }
        }

        if (bike != null)
        {
            currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, bikeRotationSpeed, Time.unscaledDeltaTime * 5);
            bike.transform.Rotate(Vector3.up * currentRotationSpeed * Time.unscaledDeltaTime);
        }

    }


    //TODO bind to an event
    void RotateBikePress(bool isPressed)
    {
        //        print("RotateBikePress");
        bikeRotationSpeed = 0;

        if (!isPressed)
        {
            waitForInput = true;
            waitStart = DateTime.Now;
            rotationDir = Mathf.Sign(currentRotationSpeed);
        }
        else
        {
            waitForInput = false;
        }

    }

    void RotateBikeDrag(Vector2 delta)
    {
        /* NGUI removed - this thing doesn't work, ignoring */
        //        Vector2 pos = UICamera.lastTouchPosition - (Vector2)UICamera.currentCamera.WorldToScreenPoint (rotateButton.transform.position);
        //        Bounds bounds = rotateButton.collider.bounds;
        //        Vector3 boundMaxScreen = UICamera.currentCamera.WorldToScreenPoint (bounds.max);
        //        Vector3 boundMinScreen = UICamera.currentCamera.WorldToScreenPoint (bounds.min);
        //
        //        float halfWidth = ((boundMaxScreen.x - boundMinScreen.x) / 2);
        //        float halfHeight = ((boundMaxScreen.y - boundMinScreen.y) / 2);
        //        
        //        if(Mathf.Abs(pos.x) < halfWidth && Mathf.Abs(pos.y) < halfHeight){
        if (localPointerPosition.x < -30 || localPointerPosition.x > 30)
        {
            if (Mathf.Abs(delta.x) > 10)
                delta.x = Mathf.Sign(delta.x) * 10;

            bikeRotationSpeed = -delta.x * 25;
        }
        else
            bikeRotationSpeed = 0;
    }

    public Vector2 delta;
    public Vector2 localPointerPosition;

    public void OnPointerUp(PointerEventData data)
    {
        //        print("OnPointerUp");
        RotateBikePress(false);
    }

    public void OnPointerDown(PointerEventData data)
    {
        //        print("OnPointerDown");
        RotateBikePress(true);
    }

    public void OnDrag(PointerEventData data)
    {
        delta = data.delta;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out localPointerPosition);

        RotateBikeDrag(data.delta);

    }

}

}
