namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class BikeControl : MonoBehaviour
{

    Transform body;
    public Transform wheelA; //front
    public Transform wheelB; //back

    public BikeTriggerCollision wheelCollisionA;
    public BikeTriggerCollision wheelCollisionB;

    public float stuntChance = 1;
    //	public int currentStunt = -1; //set by RiderAnimationControl script

    public float force = 1075;
    public float torqueFlying = 80;
    public float torqueFlytingForwardCoef = 1.3f;
    public float torqueGround = 950f;
    public float torqueStillCoef = 1.3f;

    public float accelerationThresholdTop = 0.5f;

    [Range(0, 1000)] public float brakeForce = 300f;
    public float windResistanceForce = -700f;

    float AccelerometerUpdateInterval = 1.0f / 60.0f;
    public float LowPassKernelWidthInSeconds = 0.25f;

    float LowPassFilterFactor; // tweakable
    public float lowPassValueX = 0;

    public float deltaAccelerationThreshold = 0.001f;
    public float deadZone = 0.025f;

    public float torque = 0;

    Vector2 accelerrationDirection;

    public float torqueReductionActiveAir = 0.1f;
    public float torqueReductionActiveGround = 0.1f;

    public float torqueReductionPassiveAir = 0.1f;
    public float torqueReductionPassiveGround = 0.175f; //regulates how fast the bike falls to the ground

    public float maxAngularVelocityAir = 180f;
    public float maxAngularVelocityGround = 70f;

    float bodyAbsAngularVelocity;

    float accelerationCoef;
    public float accelerationBoostCoef = 1;
    public float accelerationStartBoostCoef = 1;
    public bool useAccelerationStart = false;

    float windResistanceCoef;
    public float backWheelCoef = 1.1f;

    float accelerationX;
    float accelerationXlastFrame;
    float accelerationXcurrFrame;
    float deltaAcceleration;

    public bool rotateCW;
    public bool rotateCCW;
    public bool fly; //no wheel is touching ground
    public float airtime; //if flying - for how long
    public bool justlanded; //after a bit of flying


    public float bodyVelocity;
    public float bodyVelocityX;
    public float bodyVelocityY;
    public float angularVelocityBody;
    public float angularVelocityB;
    public float angularVelocityA;


    float maxVelocityX = 50.0f;
    public float maxVelocityXBoostCoef = 1.0f;

    public float brakeForceBoostCoef = 1.0f;

    float lastAngularVelocity = 0;

    public bool showDebug = false;

    [HideInInspector]
    public float InputAccelerometerX;//Input form real accelerometer, log or even emulated (keyboard)
    [HideInInspector]
    public bool InputTouchLeft;
    [HideInInspector]
    public bool InputTouchRight;

    bool allowTurning = false;


    void Start()
    {

        LowPassFilterFactor = AccelerometerUpdateInterval / LowPassKernelWidthInSeconds;
        lowPassValueX = InputAccelerometerX;

        body = transform;

        wheelA = body.Find("wheel_front");
        wheelB = body.Find("wheel_back");

        wheelCollisionA = body.Find("wheel_front").Find("wheel_front_trigger").GetComponent<BikeTriggerCollision>();
        wheelCollisionB = body.Find("wheel_back").Find("wheel_back_trigger").GetComponent<BikeTriggerCollision>();

    }

    //regular update for input and things that don´t require fixed updates
    void Update()
    {

        UpdateAcceleration();


        //		maxVelocityX = body.rigidbody2D.velocity.x > maxVelocityX ? body.rigidbody2D.velocity.x : maxVelocityX;
        bodyVelocityX = body.GetComponent<Rigidbody2D>().linearVelocity.x;
        bodyVelocityY = body.GetComponent<Rigidbody2D>().linearVelocity.y;
        bodyVelocity = Mathf.Abs(bodyVelocityX) + Mathf.Abs(bodyVelocityY);


        //print("x:" +bodyVelocityX + " + y: " + bodyVelocityY + " = " + bodyVelocity );

        angularVelocityB = -wheelB.GetComponent<Rigidbody2D>().angularVelocity;
        angularVelocityA = -wheelA.GetComponent<Rigidbody2D>().angularVelocity;
        angularVelocityBody = -body.GetComponent<Rigidbody2D>().angularVelocity;




    }

    //fixed update for physics
    void FixedUpdate()
    {

        if (!body.GetComponent<Rigidbody2D>().isKinematic)
        {

            UpdateWindResistance();

            UpdateMovemet();

            UpdateBraking();

            UpdateRotation();

            //a hack for bullet time
            if (Time.timeScale != 1)
            {
                Vector2 direction = -1 * body.GetComponent<Rigidbody2D>().linearVelocity.normalized;

                body.GetComponent<Rigidbody2D>().AddForce(direction * 700 * Time.timeScale * Time.deltaTime);
                wheelA.GetComponent<Rigidbody2D>().AddForce(direction * 700 * Time.timeScale * Time.deltaTime);
                wheelB.GetComponent<Rigidbody2D>().AddForce(direction * 700 * Time.timeScale * Time.deltaTime);
            }

        }
        else
        {

            if (InputTouchRight && !InputTouchLeft)
            {//!fly && 
                wheelA.Rotate(0, 0, -1000);
                wheelB.Rotate(0, 0, -1000);
            }
        }

    }

    void UpdateAcceleration()
    {

        accelerationX = Mathf.Abs(InputAccelerometerX) >= deadZone ? InputAccelerometerX : 0; //the value from the accelerometer
        lowPassValueX = Mathf.Lerp(lowPassValueX, accelerationX, LowPassFilterFactor); //low pass filter to smoothen out the noisy input
        accelerationX = Mathf.Abs(lowPassValueX) >= deadZone ? lowPassValueX : 0;

        //		accelerationDir = Mathf.Abs(lowPassValueX) >= deadZone ? Mathf.Sign(lowPassValueX) : 0;

        if (wheelCollisionA != null && wheelCollisionB != null)
        {
            fly = !(wheelCollisionB.colliding || wheelCollisionA.colliding) ? true : false;
        }

        //just landed after non-trivial jump
        justlanded = (!fly && airtime > 1f) ? true : false;


        if (fly)
        {
            rotateCW = lowPassValueX > 0 && Mathf.Abs(lowPassValueX) >= deadZone * .5 ? true : false;
            rotateCCW = lowPassValueX < 0 && Mathf.Abs(lowPassValueX) >= deadZone * .5 ? true : false;
            airtime += Time.deltaTime;
        }
        else
        {
            rotateCW = accelerationX > 0 && Mathf.Abs(lowPassValueX) >= deadZone * 2 ? true : false;
            rotateCCW = accelerationX < 0 && Mathf.Abs(lowPassValueX) >= deadZone * 2 ? true : false;
            airtime = 0;
        }


    }

    void UpdateWindResistance()
    {

        //wind resistance
        if (body.GetComponent<Rigidbody2D>().linearVelocity.x > 0)
        {

            windResistanceCoef = Mathf.Abs(body.GetComponent<Rigidbody2D>().linearVelocity.x) / 100;
            windResistanceCoef = windResistanceCoef > 0 ? windResistanceCoef : 0;
            body.GetComponent<Rigidbody2D>().AddForce(Vector2.right * windResistanceForce * windResistanceCoef * Time.deltaTime);

        }

    }

    void UpdateRotation()
    {

        accelerationXlastFrame = accelerationXcurrFrame;
        accelerationXcurrFrame = accelerationX;
        deltaAcceleration = (accelerationXcurrFrame - accelerationXlastFrame);  //shows whch way the device was rotated
        bodyAbsAngularVelocity = Mathf.Abs(body.GetComponent<Rigidbody2D>().angularVelocity);
        if (allowTurning)
        {
            if (Mathf.Abs(accelerationXcurrFrame) < deadZone || (Mathf.Abs(deltaAcceleration) > 0 && Mathf.Abs(accelerationXcurrFrame) > 0.2f))
            {
                allowTurning = false;
            }
        }
        // both wheels in the air
        if (!(wheelCollisionB.colliding || wheelCollisionA.colliding))
        {

            torque = (Mathf.Min(Mathf.Abs(accelerationXcurrFrame), accelerationThresholdTop) + (1 - accelerationThresholdTop)) * torqueFlying * Time.fixedDeltaTime;

            //rotate the body backwards if the device and bike are rotated in the opposite directions
            if (accelerationXcurrFrame != 0 && Mathf.Sign(accelerationXcurrFrame) == Mathf.Sign(body.GetComponent<Rigidbody2D>().angularVelocity))
            {
                body.GetComponent<Rigidbody2D>().AddTorque(-body.GetComponent<Rigidbody2D>().angularVelocity * torqueReductionActiveAir);
            }

            //if the acceleration is outside the dead zone OR delta acceleration is less than zero and greater than the threshold
            if (accelerationXcurrFrame <= -deadZone || (deltaAcceleration < 0 && Mathf.Abs(deltaAcceleration) > deltaAccelerationThreshold))
            {

                body.GetComponent<Rigidbody2D>().AddTorque(-body.GetComponent<Rigidbody2D>().angularVelocity * 0.1f);
                torque = 14.8f * (Mathf.Min(Mathf.Abs(accelerationXcurrFrame), accelerationThresholdTop)) * torqueFlying * Time.fixedDeltaTime;

                if (bodyAbsAngularVelocity < maxAngularVelocityAir && !allowTurning)
                {
                    body.GetComponent<Rigidbody2D>().AddTorque(torque); //rotateCCW
                }

                //CW rotation
            }
            else if (accelerationXcurrFrame >= deadZone || (deltaAcceleration > 0 && Mathf.Abs(deltaAcceleration) > deltaAccelerationThreshold))
            {

                body.GetComponent<Rigidbody2D>().AddTorque(-body.GetComponent<Rigidbody2D>().angularVelocity * 0.025f);
                torque *= torqueFlytingForwardCoef * 1.6f;

                if (bodyAbsAngularVelocity < maxAngularVelocityAir && !allowTurning)
                {
                    body.GetComponent<Rigidbody2D>().AddTorque(-torque); //rotateCW
                }
            }
            else
            {
                body.GetComponent<Rigidbody2D>().AddTorque(-body.GetComponent<Rigidbody2D>().angularVelocity * torqueReductionPassiveAir);
            }

            // at least one wheel on the ground
        }
        else
        {
            torque = (Mathf.Min(Mathf.Abs(accelerationXcurrFrame), accelerationThresholdTop) + (1 - accelerationThresholdTop)) * torqueGround * Time.fixedDeltaTime;
            allowTurning = true;
            //rotate the body backwards if the device and bike are rotated in the opposite directions
            if (wheelCollisionB.colliding)
            {

                if (accelerationXcurrFrame != 0 && Mathf.Sign(accelerationXcurrFrame) == Mathf.Sign(body.GetComponent<Rigidbody2D>().angularVelocity))
                {
                    body.GetComponent<Rigidbody2D>().AddTorque(-body.GetComponent<Rigidbody2D>().angularVelocity * torqueReductionActiveGround);
                }

            }

            //CCW rotation
            if (accelerationXcurrFrame <= -deadZone && deltaAcceleration < 0)
            {

                if ((Mathf.Abs(wheelB.GetComponent<Rigidbody2D>().linearVelocity.x) < 10f && wheelCollisionB.colliding) || (Mathf.Abs(wheelA.GetComponent<Rigidbody2D>().linearVelocity.x) < 10f && wheelCollisionA.colliding))
                {
                    torque *= torqueStillCoef;
                }

                if (bodyAbsAngularVelocity < maxAngularVelocityGround)
                {
                    body.GetComponent<Rigidbody2D>().AddTorque(torque);
                }

                //CW rotation
            }
            else if (accelerationXcurrFrame >= deadZone && deltaAcceleration > 0)
            {

                //only rotatate if both wheels not on the ground
                if (!(wheelCollisionA.colliding && wheelCollisionB.colliding) || ((wheelCollisionA.colliding && wheelCollisionB.colliding) && Mathf.Abs(body.GetComponent<Rigidbody2D>().rotation % 360) > 100))
                {

                    //if small speed, use a coefficient to increase torque
                    if ((Mathf.Abs(wheelB.GetComponent<Rigidbody2D>().linearVelocity.x) < 10f && wheelCollisionB.colliding) || (Mathf.Abs(wheelA.GetComponent<Rigidbody2D>().linearVelocity.x) < 10f && wheelCollisionA.colliding))
                    {
                        torque *= torqueStillCoef;
                    }

                    if (bodyAbsAngularVelocity < maxAngularVelocityGround)
                    {
                        body.GetComponent<Rigidbody2D>().AddTorque(-torque);
                    }
                }

            }
            else
            {
                //if accelerometer not used and bike is on the ground with back wheel, reduce the angular rotation (makes bike angle "sticky")
                if (wheelCollisionB.colliding)
                {
                    body.GetComponent<Rigidbody2D>().AddTorque(-body.GetComponent<Rigidbody2D>().angularVelocity * torqueReductionPassiveGround);
                }

            }
        }

        //reduce angular velocity if it´s too high
        if (Mathf.Abs(body.GetComponent<Rigidbody2D>().angularVelocity) >= maxAngularVelocityAir)
        {
            body.GetComponent<Rigidbody2D>().AddTorque(-body.GetComponent<Rigidbody2D>().angularVelocity * 0.01f);
        }

    }

    void UpdateMovemet()
    {

        //if key down and not braking start motor
        if (!InputTouchLeft && InputTouchRight)
        {

            if (wheelCollisionB.colliding)
            {

                //if body.rigidbody2D.velocity.x > 50 no force will be applied, 50 - maxVelocity at wich force is applied
                accelerationCoef = Mathf.Abs(body.GetComponent<Rigidbody2D>().linearVelocity.x) / (maxVelocityX * maxVelocityXBoostCoef);// 50;
                accelerationCoef = 1f - accelerationCoef * accelerationCoef;//at square 0, maximum force, decrease in force quadratic, the more velocity the less force is applied to bodies
                accelerationCoef = accelerationCoef > 0 ? accelerationCoef : 0; //clip negative values to 0

                if (wheelCollisionB.colliding && !wheelCollisionA.colliding)
                { //only back wheel on ground
                    accelerrationDirection = Vector2.right + Vector2.up * 0.175f;
                }
                else
                { //if both on ground or none
                    accelerrationDirection = Vector2.right * body.right.x + Vector2.up * body.right.y * 0.15f;
                }

                if (wheelCollisionB.colliding)
                {
                    // if on the back wheel give some more force
                    if (wheelCollisionB.colliding && !wheelCollisionA.colliding)
                    {
                        accelerationCoef *= backWheelCoef;
                    }
                    accelerationCoef *= (useAccelerationStart ? accelerationStartBoostCoef : accelerationBoostCoef);//in singleplayer fuel boosts acceleration, in multiplayer acceleration upgrade

                    body.GetComponent<Rigidbody2D>().AddForce(accelerrationDirection * force * 0.5f * accelerationCoef * Time.deltaTime);
                    wheelB.GetComponent<Rigidbody2D>().AddForce(accelerrationDirection * force * 0.5f * accelerationCoef * Time.deltaTime);

                }

            }

        }

    }

    //    int count = 0;
    void UpdateBraking()
    {

        if (InputTouchLeft)
        {

            //            print(wheelB.rigidbody2D.angularVelocity);
            if (wheelB.GetComponent<Rigidbody2D>().constraints != RigidbodyConstraints2D.FreezeRotation)
            {

                //                print(lastAngularVelocity);
                if (lastAngularVelocity == 0)
                {
                    lastAngularVelocity = wheelA.GetComponent<Rigidbody2D>().angularVelocity;
                }

                //brake both wheels
                wheelA.GetComponent<Rigidbody2D>().AddTorque(-1 * Mathf.Sign(wheelA.GetComponent<Rigidbody2D>().angularVelocity) * brakeForce * brakeForceBoostCoef * Time.fixedDeltaTime);
                wheelB.GetComponent<Rigidbody2D>().AddTorque(-1 * Mathf.Sign(wheelB.GetComponent<Rigidbody2D>().angularVelocity) * brakeForce * brakeForceBoostCoef * Time.fixedDeltaTime);

                //                print(wheelA.rigidbody2D.angularVelocity);

                //if rear wheel fully stopped, fix angle on both wheels          
                if (Mathf.Sign(lastAngularVelocity) != Mathf.Sign(wheelB.GetComponent<Rigidbody2D>().angularVelocity))
                {   //if(wheelB.rigidbody2D.angularVelocity <= 0) {
                    wheelB.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                    wheelA.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                }

                lastAngularVelocity = wheelA.GetComponent<Rigidbody2D>().angularVelocity;
                //                count++;
            }

        }
        else
        {

            //unlock wheels after braking
            if ((wheelB.GetComponent<Rigidbody2D>().constraints & RigidbodyConstraints2D.FreezeRotation) != 0 || (wheelA.GetComponent<Rigidbody2D>().constraints & RigidbodyConstraints2D.FreezeRotation) != 0)
            {
                wheelB.GetComponent<Rigidbody2D>().constraints= RigidbodyConstraints2D.None;
                wheelA.GetComponent<Rigidbody2D>().constraints= RigidbodyConstraints2D.None;
                wheelB.GetComponent<Rigidbody2D>().WakeUp();
                wheelA.GetComponent<Rigidbody2D>().WakeUp();

                lastAngularVelocity = 0;
                //                print(count);
                //                count = 0;
            }

        }

    }

    public void Reset()
    {
        fly = false;
        rotateCW = false;
        rotateCCW = false;
        InputTouchLeft = false;
        InputTouchRight = false;

        InputAccelerometerX = 0;
        accelerationX = 0;
        lowPassValueX = 0;
    }

}

}
