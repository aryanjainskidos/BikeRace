namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class BikeInputFile : MonoBehaviour
{

    BikeControl control;
    BikeStateData stateData;

    public bool passInputToControl = true;

    bool inputEndReached = false;

    List<float> inputRotation; //kadra_numurs => inputa_vértíba_śajá_kadrá
    List<bool> inputButtonA;
    List<bool> inputButtonB;
    List<Vector2> bikePosition;
    List<float> bikeRotation;
    List<bool> bikeFlying;

    Dictionary<int, int> stuntEvents; //kadra numurs => stunta ID

    public int finishEventFrame = -1;

    public int crashEventFrame;
    public int wheelRotationAtCrash;

    public int frameNumber = 0;

    public int totalFrameCount = 0;

    bool initialized = false;
    bool initializedAtLeastOnce = false;

    JSONNode dataNode;

    void Awake()
    {
        this.control = transform.GetComponent<BikeControl>();
        this.stateData = transform.GetComponent<BikeStateData>();
    }

    public void SetData(string uncompressedJSONText)
    {

        if (uncompressedJSONText == "" || uncompressedJSONText == null)
        {
            //Debug.Log("No JSON for AI bike!");
            return;
        }

        dataNode = JSON.Parse(uncompressedJSONText);
        Initialize();


    }

    public void SetData(JSONClass jsonNode)
    {

        if (jsonNode == null)
        {
            //Debug.Log("No JSON for AI bike!");
            return;
        }

        dataNode = jsonNode;
        Initialize();


    }

    public void ClearData()
    {
        frameNumber = 0;
        totalFrameCount = 0;
        crashEventFrame = -1;
        finishEventFrame = -1;
        wheelRotationAtCrash = -1;

        if (initializedAtLeastOnce)
        {
            inputRotation.Clear();
            inputButtonA.Clear();
            inputButtonB.Clear();
            bikePosition.Clear();
            bikeRotation.Clear();
            bikeFlying.Clear();
            stuntEvents.Clear();
        }

        initialized = false;
        inputEndReached = false;
    }

    void Initialize()
    {

        totalFrameCount = dataNode["rotation"].Count;
        //Debug.Log("Initialize");
        inputRotation = new List<float>();
        for (int i = 0; i < dataNode["rotation"].Count; i++)
        {
            inputRotation.Add(dataNode["rotation"][i].AsFloat);
        }
        inputButtonA = new List<bool>();
        for (int i = 0; i < dataNode["buttonA"].Count; i++)
        {
            inputButtonA.Add(dataNode["buttonA"][i].AsBool);
        }
        inputButtonB = new List<bool>();
        for (int i = 0; i < dataNode["buttonB"].Count; i++)
        {
            inputButtonB.Add(dataNode["buttonB"][i].AsBool);
        }
        bikePosition = new List<Vector2>();
        for (int i = 0; i < dataNode["bikePos"].Count; i++)
        {
            bikePosition.Add(JSONHelper.Vector2FromJSONString(dataNode["bikePos"][i]));
        }
        bikeRotation = new List<float>();
        for (int i = 0; i < dataNode["bikeRot"].Count; i++)
        {
            bikeRotation.Add(dataNode["bikeRot"][i].AsFloat);
        }
        bikeFlying = new List<bool>();
        for (int i = 0; i < dataNode["bikeFly"].Count; i++)
        {
            bikeFlying.Add(dataNode["bikeFly"][i].AsBool);
        }

        crashEventFrame = -1;
        wheelRotationAtCrash = int.MinValue;

        finishEventFrame = -1;
        stuntEvents = new Dictionary<int, int>();
        for (int i = 0; i < dataNode["events"]["name"].Count; i++)
        {

            //              print(N["events"]["name"][i]);

            switch (dataNode["events"]["name"][i])
            {
                case "stunt":
                    stuntEvents[dataNode["events"]["frameNum"][i].AsInt] = dataNode["events"]["value"][i].AsInt;
                    break;
                case "finish":
                    finishEventFrame = dataNode["events"]["frameNum"][i].AsInt;
                    break;
                case "crash":
                    crashEventFrame = dataNode["events"]["frameNum"][i].AsInt;
                    wheelRotationAtCrash = dataNode["events"]["value"][i].AsInt;
                    break;

                default:
                    break;
            }

        }

        initialized = true;
        initializedAtLeastOnce = true;

    }

    public void Pass()
    {

        if (initialized && passInputToControl && !inputEndReached)
        {

            if (frameNumber >= inputRotation.Count)
            {

                inputEndReached = true;
                stateData.dead = true;
                control.Reset();

            }
            else
            {

                control.InputAccelerometerX = inputRotation[frameNumber];
                control.InputTouchLeft = inputButtonA[frameNumber];
                control.InputTouchRight = inputButtonB[frameNumber];

                control.transform.position = (Vector3)bikePosition[frameNumber];
                control.GetComponent<Rigidbody2D>().rotation = bikeRotation[frameNumber];

                control.fly = bikeFlying[frameNumber];

                if (stuntEvents != null && stuntEvents.ContainsKey(frameNumber))
                {
                    if (stuntEvents[frameNumber] >= 0)
                    {
                        stateData.stunt = true;
                    }
                    else
                    {
                        stateData.stunt = false;
                    }
                    stateData.stuntID = stuntEvents[frameNumber];

                }

                if (finishEventFrame >= 0 && finishEventFrame == frameNumber)
                {
                    control.Reset();
                    stateData.finished = true;
                    BikeGameManager.AIJustFinished(); //meh, tight coupling
                }

                if (crashEventFrame >= 0 && crashEventFrame == frameNumber)
                {
                    inputEndReached = true;
                    stateData.dead = true;
                    control.InputTouchRight = true;
                }

                frameNumber++;

            }
        }
    }

    public void PassLerp(float t)
    {

        if (initialized && passInputToControl && !inputEndReached)
        {

            if (frameNumber + 1 < inputRotation.Count)
            {

                control.InputAccelerometerX = Mathf.Lerp(inputRotation[frameNumber - 1], inputRotation[frameNumber], t);

                control.transform.position = (Vector3)Vector2.Lerp(bikePosition[frameNumber - 1], bikePosition[frameNumber], t);
                control.GetComponent<Rigidbody2D>().rotation = Mathf.Lerp(bikeRotation[frameNumber - 1], bikeRotation[frameNumber], t);
            }
        }
    }

    public void Reset()
    {

        frameNumber = 0;
        inputEndReached = false;

    }

    public int counter = 1; //don't reset to 0, otherwise will advance one frame at the very beginning of bullet time zone
    public float t;

    void FixedUpdate()
    {
        //        Pass();

        if (Time.timeScale != 1)
        {

            if (counter % Mathf.RoundToInt(1 / Time.timeScale) == 0)
            {
                Pass();
                t = 0;
            }
            else
            {
                float f = 1 / Time.timeScale;
                t = (counter % f) / f;
                PassLerp(t);// interpolate
            }

            counter++;

        }
        else
        {
            Pass();

            if (counter != 1)
            {
                counter = 1;
            }
        }
    }

}

}
