namespace vasundharabikeracing {

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class OnScreenControlBehaviour : MonoBehaviour
{

    OnScreenControlButton upButton;
    OnScreenControlButton downButton;
    OnScreenControlButton accelerateButton;
    OnScreenControlButton brakeButton;

    RectTransform accelerateButtonRect;
    Camera uiCamera;

    // Use this for initialization
    void Awake()
    {
        upButton = transform.Find("UpButton").GetComponent<OnScreenControlButton>();
        downButton = transform.Find("DownButton").GetComponent<OnScreenControlButton>();
        accelerateButton = transform.Find("AccelerateButton").GetComponent<OnScreenControlButton>();
        brakeButton = transform.Find("BrakeButton").GetComponent<OnScreenControlButton>();

        accelerateButtonRect = accelerateButton.GetComponent<RectTransform>();
        uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();

        upButton.downAction = OnUpDown;
        downButton.downAction = OnDownDown;
        upButton.upAction = downButton.upAction = OnUpOrDownUp;

        accelerateButton.downAction = OnAccelerateDown;
        accelerateButton.upAction = OnAccelerateUp;

        brakeButton.downAction = OnBrakeDown;
        brakeButton.upAction = OnBrakeUp;
    }

    void OnUpDown()
    {
        UIInput.SetAxis(UIInput.VERTICAL, 1);
    }

    void OnDownDown()
    {
        UIInput.SetAxis(UIInput.VERTICAL, -1);
    }

    void OnUpOrDownUp()
    {
        UIInput.SetAxis(UIInput.VERTICAL, 0);
    }

    void OnAccelerateDown()
    {
        UIInput.SetAxis(UIInput.ACCELERATE, 1);
    }

    void OnAccelerateUp()
    {
        UIInput.SetAxis(UIInput.ACCELERATE, 0);
    }

    void OnBrakeDown()
    {
        UIInput.SetAxis(UIInput.BRAKE, 1);
    }

    void OnBrakeUp()
    {
        UIInput.SetAxis(UIInput.BRAKE, 0);
    }

    void OnEnable()
    {
        UIInput.ResetAllAxes();

        if (BikeDataManager.SettingsAccelerometer)
        {
            upButton.gameObject.SetActive(false);
            downButton.gameObject.SetActive(false);
            accelerateButton.gameObject.SetActive(false);
            brakeButton.gameObject.SetActive(false);
        }
        else
        {
            upButton.gameObject.SetActive(true);
            downButton.gameObject.SetActive(true);
            accelerateButton.gameObject.SetActive(true);
            brakeButton.gameObject.SetActive(true);
        }
    }

    void OnDisable()
    {
        UIInput.ResetAllAxes();
    }

    Vector2 touchPosition;

    // Update is called once per frame
    public void Update()
    {

        //bool accelerationTouched = false;

        for (int i = 0; i < Input.touchCount; i++)
        {
            touchPosition = Input.GetTouch(i).position;

            bool rectangleContainsScreenPoint = RectTransformUtility.RectangleContainsScreenPoint(accelerateButtonRect, touchPosition, uiCamera);
            if (rectangleContainsScreenPoint)
            {

                //accelerationTouched = true;

            }
        }

        //simulate down event if touch is registered above the acceleration button, if axs is neutral or negative
        //if (accelerationTouched && UIInput.GetAxis(UIInput.ACCELERATE) <= 0)
        //{
        //    Debug.Log("First IF calling");
        //    var pointerEventData = new PointerEventData(EventSystem.current);
        //    ExecuteEvents.Execute(accelerateButton.gameObject, pointerEventData, ExecuteEvents.pointerDownHandler);
        //}

        ////simulate up event if touch is not registered above the acceleration button, but axis is not neutral
        //if (!accelerationTouched && UIInput.GetAxis(UIInput.ACCELERATE) > 0)
        //{
        //    Debug.Log("Second IF calling");
        //    var pointerEventData = new PointerEventData(EventSystem.current);
        //    ExecuteEvents.Execute(accelerateButton.gameObject, pointerEventData, ExecuteEvents.pointerUpHandler);
        //}
    }
}
}
