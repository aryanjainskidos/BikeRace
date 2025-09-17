namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WheelOfFortuneBehaviour : MonoBehaviour
{

    HingeJoint2D[] hingeJoints;
    public HingeJoint2D wheelHingeJoint;
    public HingeJoint2D pointerHingeJoint;

    public bool wheelStopped = false;
    public bool shouldStart = false;
    public bool shouldStop = false;

    Dictionary<SpinPrizeType, float> prizeAngles;

    public System.Action onSpinningComplete;

    // Use this for initialization
    void Awake()
    {
        hingeJoints = GetComponents<HingeJoint2D>();

        for (int i = 0; i < hingeJoints.Length; i++)
        {
            if (hingeJoints[i].connectedBody.name == "Wheel")
            {
                wheelHingeJoint = hingeJoints[i];
            }

            if (hingeJoints[i].connectedBody.name == "Pointer" && hingeJoints[i].connectedBody.gameObject.activeSelf)
            {
                pointerHingeJoint = hingeJoints[i];
            }
        }

        prizeAngles = new Dictionary<SpinPrizeType, float>() {
            {SpinPrizeType.CoinsX500, 0},
            {SpinPrizeType.None, 45},
            {SpinPrizeType.BikeBeach, 90},
            {SpinPrizeType.BikeTourist, 90},
            {SpinPrizeType.CupsX200, 90},
            {SpinPrizeType.CoinsX1000, 135},
            {SpinPrizeType.CoinsX100000, 180},
            {SpinPrizeType.CoinsX100, 225},
            {SpinPrizeType.CupsX100, 270},
            {SpinPrizeType.CoinsX2500, 315}
        };
    }

    public int framesToWaitUntillStart = 15;
    public int framesWaited = 0;

    void OnEnable()
    {
        if (SpinManager.initialized && SpinManager.CanHazSpin())
        {
            Reset();
            shouldStart = true;
        }
    }

    public float wheelJointSpeed;
    public float wheelJointAngle;
    public float pointerJointAngle;
    public float stopAngle;
    public float deltaAngle;
    public float wheelAngularVelocity;

    float prizeAngle = 0;

    int framesWithLowSpeed = 0;
    int maxFramesWithLowSpeed = 6;

    public bool useMotor = false;
    // Update is called once per frame
    void FixedUpdate()
    {

        if (shouldStart)
        {
            //            if (framesToWaitUntillStart == framesWaited && !wheelHingeJoint.useMotor)
            //                wheelHingeJoint.useMotor = true;

            if (framesToWaitUntillStart == framesWaited && !useMotor)
            {
                useMotor = true;
                shouldStart = false;
            }

            //            if (framesWaited <= framesToWaitUntillStart && wheelHingeJoint.useMotor)
            if (framesWaited <= framesToWaitUntillStart && !useMotor)
            {
                framesWaited++;
            }
        }

        if (useMotor)
        {
            if (wheelHingeJoint.connectedBody.angularVelocity < 150 ||
                wheelHingeJoint.connectedBody.angularVelocity > -150)
            {
                wheelHingeJoint.connectedBody.AddTorque(-10000);
            }
            wheelHingeJoint.connectedBody.angularVelocity = Mathf.Clamp(wheelHingeJoint.connectedBody.angularVelocity, -150, 150);
        }

        if (framesWaited >= framesToWaitUntillStart)
        {

            wheelJointSpeed = wheelHingeJoint.jointSpeed;
            wheelJointAngle = wheelHingeJoint.jointAngle % 360;
            pointerJointAngle = pointerHingeJoint.jointAngle;

            wheelAngularVelocity = wheelHingeJoint.connectedBody.angularVelocity;
            //*
            //            if (shouldStop && wheelJointSpeed >= -15f && pointerJointAngle <= 0  && wheelHingeJoint.connectedBody.angularDrag < 0.35f && !wheelHingeJoint.connectedBody.fixedAngle)
            //            {
            //                wheelHingeJoint.connectedBody.angularDrag = 0.5f;
            //            }

            //wheel stopped completely
            if (wheelJointSpeed >= 0 && pointerJointAngle <= 0 && wheelHingeJoint.connectedBody.constraints== RigidbodyConstraints2D.None)
            {
                CompleteSpin();
            }

            //if the wheel speed has been lower than 0.3 for more than 6 frames, then we should complete the spin, otherwise wait or increment the frame counter
            if (Mathf.Abs(wheelJointSpeed) <= 3f && framesWithLowSpeed < maxFramesWithLowSpeed)
            {
                framesWithLowSpeed++;
            }
            else
            {
                if (Mathf.Abs(wheelJointSpeed) <= 3f && framesWithLowSpeed >= maxFramesWithLowSpeed && //speed is less than 0.3, and waited for max frames
                    pointerJointAngle <= 0 && wheelHingeJoint.connectedBody.constraints== RigidbodyConstraints2D.None)
                { // the pointer is horizontal and wheel is not fixed
                    CompleteSpin();
                }
            }

            if (wheelJointSpeed == 0 && pointerJointAngle > 0 && wheelHingeJoint.connectedBody.constraints== RigidbodyConstraints2D.None)
            {
                //this is the case when the pointe stays on one of the points of the wheel, it should never happen
                if (Debug.isDebugBuild) { Debug.Log("FUCK!"); }
                CompleteSpin();//complete spin anyway
            }

            //            if (shouldStop && wheelHingeJoint.useMotor && Mathf.Abs(wheelJointAngle) > prizeAngle && Mathf.Abs(wheelJointAngle) < prizeAngle + 5)
            if (shouldStop && useMotor && Mathf.Abs(wheelJointAngle) > prizeAngle && Mathf.Abs(wheelJointAngle) < prizeAngle + 5)
            {
                stopAngle = wheelHingeJoint.jointAngle;
                //                wheelHingeJoint.useMotor = false;
                useMotor = false;
            }
            //*/  
        }

        if (reset)
        {
            Reset();
        }

        #region testing
        if (resetSpinManager)
        {
            ResetSpinManager();
        }
        #endregion
    }

    void CompleteSpin()
    {
        //print("wheelHingeJoint.rigidbody2D.fixedAngle " + wheelHingeJoint.connectedBody.fixedAngle);
        wheelHingeJoint.connectedBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        wheelStopped = true;

        deltaAngle = wheelHingeJoint.jointAngle - stopAngle;

        if (onSpinningComplete != null)
        {
            onSpinningComplete();
        }
    }

    public void Stop()
    {
        //print("stop");
        shouldStop = true;

        prizeAngle = (360 + prizeAngles[SpinManager.prize] + 25) % 360; //calculate when to stop, keep the result within [0,360), 15 is the offset, bcause the values in the LUT are easyer to read and understand the way they are
    }

    public bool reset = false;
    public void Reset()
    {

        wheelHingeJoint.connectedBody.constraints = RigidbodyConstraints2D.None;
        //        wheelHingeJoint.useMotor = true;
        useMotor = false;

        shouldStart = true;
        framesWaited = 0;
        framesWithLowSpeed = 0;

        wheelHingeJoint.connectedBody.rotation = 0;
        wheelHingeJoint.connectedBody.angularDamping = 0.25f;

        wheelStopped = false;
        shouldStop = false;

        reset = false;
    }

    #region testing
    public bool resetSpinManager = false;
    public void ResetSpinManager()
    {

        SpinManager.Reset();

        resetSpinManager = false;
    }
    #endregion

}

}
