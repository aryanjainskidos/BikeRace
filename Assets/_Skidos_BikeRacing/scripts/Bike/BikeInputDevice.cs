namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class BikeInputDevice : MonoBehaviour
{

    public bool logInput = true;
    public bool passInputToControl = true;

    BikeControl control;

    BikeStateData stateData;

    Vector2 touch0Position;
    Vector2 touch1Position;
    float middleOfTheScrrenX;
    float verticalAxis;

    int lastLoggedStuntID = -1;
    bool finishEventLogged = false;

    bool loggingCompleted = false;

    public int finishEventFrame = -1;

    public int crashEventFrame;
    public int wheelRotationAtCrash;

    public int frameNumber = 0;

    JSONClass inputLog;
    JSONClass inputLogLast;

    //    public BikeInputDevice (BikeStateData stateData, BikeControl control) {
    //        
    //        Debug.Log("Start");
    //        this.control = control;
    //        this.stateData = stateData;
    //
    //        middleOfTheScrrenX = Screen.width / 2;
    //
    //        inputLog = new JSONClass();
    //
    //    }

    void Awake()
    {

        //Debug.Log("Awake");
        this.control = transform.GetComponent<BikeControl>();
        this.stateData = transform.GetComponent<BikeStateData>();

        middleOfTheScrrenX = Screen.width / 2;

        inputLog = new JSONClass();
        inputLogLast = new JSONClass();


    }

    public string GetDataString()
    {

        //Debug.Log("GetDataString");
        if (inputLog.ToString().Length <= 2)
        { //tukśs JSONs
            return inputLogLast.ToString(); // tad dos iepriekśéjo  (péc MP kreśa inpuuts tiek noresetots pirms tas tiek noseivots)
        }
        return inputLog.ToString();
    }

    public JSONClass GetDataJSON()
    {

        //Debug.Log("GetDataJSON");
        if (inputLog.ToString().Length <= 2)
        { //tukśs JSONs
            return inputLogLast; // tad dos iepriekśéjo  (péc MP kreśa inpuuts tiek noresetots pirms tas tiek noseivots)
        }
        return inputLog;
    }


    public void Log()
    { // log input and events on Fixed Update or on reset

        if (logInput && !loggingCompleted)
        {

            //info on every frame:
            LogInput(control.InputAccelerometerX,
                     control.InputTouchLeft,
                     control.InputTouchRight,
                     control.transform.position,
                     control.GetComponent<Rigidbody2D>().rotation,
                     control.fly);

            //rare events: 
            if (stateData.stuntID != lastLoggedStuntID)
            { //starting or ending stunt
                LogEvent("stunt", stateData.stuntID, frameNumber);
                lastLoggedStuntID = stateData.stuntID;
            }

            if (stateData.finished && !finishEventLogged)
            { // finishing
                LogEvent("finish", 1, frameNumber);
                finishEventFrame = frameNumber;
                finishEventLogged = true;
            }

            if (stateData.dead && !stateData.finished)
            { //crash
                LogEvent("crash", (int)-control.wheelB.GetComponent<Rigidbody2D>().rotation, frameNumber);
                crashEventFrame = frameNumber;
                wheelRotationAtCrash = (int)-control.wheelB.GetComponent<Rigidbody2D>().rotation;
            }

            if ((stateData.finished && finishEventLogged && Mathf.Abs(control.bodyVelocityX) < 0.01f) || stateData.dead)
            {
                loggingCompleted = true;
            }

            frameNumber++;
        }

    }

    void LogInput(float rotation, bool buttonA, bool buttonB, Vector2 bikePos, float bikeRot, bool bikeFly)
    {
        if (inputLog != null)
        {
            //inputi
            inputLog["rotation"][-1].AsFloat = rotation;
            inputLog["buttonA"][-1].AsBool = buttonA;
            inputLog["buttonB"][-1].AsBool = buttonB;
            //mocha apréḱinátie parametri
            inputLog["bikePos"][-1] = JSONHelper.Vector2ToJSONStringPrec(bikePos);
            inputLog["bikeRot"][-1].AsFloat = bikeRot;
            inputLog["bikeFly"][-1].AsBool = bikeFly;
        }
        else
        {
            Debug.LogWarning("inputLog == null");
        }
    }

    void LogEvent(string name, int value, int frameNum)
    {
        if (inputLog != null)
        {
            inputLog["events"]["name"][-1] = name;
            inputLog["events"]["value"][-1].AsInt = value;
            inputLog["events"]["frameNum"][-1].AsInt = frameNum;
        }
        else
        {
            Debug.LogWarning("inputLog == null");
        }
    }

    public void Pass()
    { // process on update

        if (passInputToControl)
        {

            if (BikeDataManager.SettingsAccelerometer)
            {

                //Device::Accelerometer
                control.InputAccelerometerX = Input.acceleration.x;

                //Device::Buttons
                touch0Position = Input.touchCount > 0 ? Input.GetTouch(0).position : Vector2.right * middleOfTheScrrenX;
                touch1Position = Input.touchCount > 1 ? Input.GetTouch(1).position : Vector2.right * middleOfTheScrrenX;
                if (touch0Position.x < 0)
                {
                    touch0Position.x = Screen.width + touch0Position.x;
                }
                if (touch1Position.x < 0)
                {
                    touch1Position.x = Screen.width + touch1Position.x;
                }
                if (touch0Position.x < middleOfTheScrrenX || touch1Position.x < middleOfTheScrrenX)
                {
                    control.InputTouchLeft = true;
                }
                else
                {
                    control.InputTouchLeft = false;
                }
                if (touch0Position.x > middleOfTheScrrenX || touch1Position.x > middleOfTheScrrenX)
                {
                    control.InputTouchRight = true;
                }
                else
                {
                    control.InputTouchRight = false;
                }

            }
            else
            {
                #region on_screen_control
                //Fake Keyboard::fakeAccelerometer
                verticalAxis = UIInput.GetAxis(UIInput.VERTICAL);
                if (verticalAxis != 0)
                {
                    control.InputAccelerometerX = -verticalAxis; //bultińa uz augśu: gázties atpakaĺ
                }

                //Fake Keyboard::buttons 
                if (UIInput.GetAxis(UIInput.BRAKE) > 0)
                {
                    control.InputTouchLeft = true;
                }
                else
                {
                    control.InputTouchLeft = false;
                }
                if (UIInput.GetAxis(UIInput.ACCELERATE) > 0)
                {
                    control.InputTouchRight = true;
                }
                else
                {
                    control.InputTouchRight = false;
                }
                #endregion
            }

            //Keyboard::fakeAccelerometer
            verticalAxis = Input.GetAxis("Vertical");
            if (verticalAxis != 0)
            {
                control.InputAccelerometerX = -verticalAxis; //bultińa uz augśu: gázties atpakaĺ
            }


            //Keyboard::buttons  (ja vien uz ieríces netiek nekas spiests)
            if (!control.InputTouchLeft && !control.InputTouchRight)
            {
                try
                {
                    if (Input.GetAxis("Break") > 0)
                    {
                        control.InputTouchLeft = true;
                    }
                    else
                    {
                        control.InputTouchLeft = false;
                    }
                }
                catch (System.ArgumentException)
                {
                    // Input axis "Break" not configured, use alternative input
                    if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift))
                    {
                        control.InputTouchLeft = true;
                    }
                    else
                    {
                        control.InputTouchLeft = false;
                    }
                }
                try
                {
                    if (Input.GetAxis("Accelerate") > 0)
                    {
                        control.InputTouchRight = true;
                    }
                    else
                    {
                        control.InputTouchRight = false;
                    }
                }
                catch (System.ArgumentException)
                {
                    // Input axis "Accelerate" not configured, use alternative input
                    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                    {
                        control.InputTouchRight = true;
                    }
                    else
                    {
                        control.InputTouchRight = false;
                    }
                }
            }

            firstInputReceived = true;
        }

    }

    public void Reset()
    { //player should be reset before AI, gets called before start

        //Debug.Log("Reset()");

        inputLogLast = inputLog;
        inputLog = new JSONClass();


        frameNumber = 0;

        loggingCompleted = false;

        finishEventFrame = -1;
        finishEventLogged = false;

        crashEventFrame = -1;
        wheelRotationAtCrash = 0;

        firstInputReceived = false;

    }

    public float unscaledFixedDeltaTime = 0;
    public int counter;
    //    public int counter2;

    void FixedUpdate()
    {

        //        Log();
        if (firstInputReceived) // don't log before you get first input
        {

            if (Time.timeScale != 1)
            {

                if (counter % Mathf.RoundToInt(1 / Time.timeScale) == 0)
                    Log();

                counter++;

            }
            else
            {
                Log();

                if (counter != 0)
                {
                    counter = 0;
                }
            }

        }

    }

    bool firstInputReceived = false;

    void Update()
    {
        Pass();

        //        if(Time.timeScale != 1) {
        //            counter2++;
        //        }
    }

}

}
