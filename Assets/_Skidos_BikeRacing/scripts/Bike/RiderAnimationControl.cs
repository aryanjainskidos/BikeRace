namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class RiderAnimationControl : MonoBehaviour
{

    Animator animator;
    AnimatorStateInfo stateInfo;

    BikeControl bikeControl;
    BikeStateData stateData;
    //	BikeEntityTrigger entityTrigger;

    public bool stuntZoneAlreadyEntered = false;
    public bool trickJustStarted = false;

    public float randomRoll;

    public AnimatorOverrideController[] overrideControllers;

    int trickStartHash;
    int trickLoopHash;
    int trickEndHash;

    public bool flying;

    void Start()
    {
        animator = GetComponent<Animator>();

        trickStartHash = Animator.StringToHash("Base Layer.TrickStart");
        trickLoopHash = Animator.StringToHash("Base Layer.TrickLoop");
        trickEndHash = Animator.StringToHash("Base Layer.TrickEnd");

        if (overrideControllers.Length > 0)
        {
            animator.runtimeAnimatorController = overrideControllers[0];
        }

        Init();
    }

    public void Init()
    {

        if (transform.parent != null)
        {
            bikeControl = transform.parent.GetComponent<BikeControl>();
            stateData = transform.parent.GetComponent<BikeStateData>();
            //			entityTrigger = transform.parent.FindChild ("entity_trigger").GetComponent<BikeEntityTrigger> ();
        }
        else
        {
            print("no parent");
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (animator != null && bikeControl != null)
        {

            stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            animator.speed = Time.timeScale > 0 ? Time.timeScale : 1;

            if (bikeControl.InputTouchLeft)
            {
                animator.SetBool("Braking", true);
            }
            else
            {
                animator.SetBool("Braking", false);
            }


            if (!bikeControl.InputTouchLeft && bikeControl.InputTouchRight)
            {
                animator.SetBool("Accelerating", true);
            }
            else
            {
                animator.SetBool("Accelerating", false);
            }

            if (bikeControl.rotateCW)
            {
                animator.SetBool("LeaningForward", true);
            }
            else
            {
                animator.SetBool("LeaningForward", false);
            }

            if (bikeControl.rotateCCW)
            {
                animator.SetBool("LeaningBack", true);
            }
            else
            {
                animator.SetBool("LeaningBack", false);
            }

            if (bikeControl.fly)
            {
                animator.SetBool("Flying", true);
            }
            else
            {
                animator.SetBool("Flying", false);
            }

            if (stateData != null)
            {
                animator.SetBool("Finish", stateData.finished);
                if (stateData.finished)
                    animator.SetInteger("Stars", stateData.starsEarned); //TODO correct
            }

            flying = animator.GetBool("Flying");

            animator.SetFloat("Acceleration", bikeControl.lowPassValueX);

            trickJustStarted = false;
            if (bikeControl.fly)
            {
                //if flying can enter stunt

                //if in the stunt zone
                //				if (entityTrigger.collTag == "StuntZone") {
                if (stateData.stunt)
                {

                    //if not in one of the trick states can start trick
                    if (stateInfo.nameHash != trickStartHash &&
                        stateInfo.nameHash != trickLoopHash &&
                        stateInfo.nameHash != trickEndHash)
                    {

                        randomRoll = Random.value;

                        //roll if can do the trick and pick a random one
                        if (!animator.GetBool("TrickStart") && (randomRoll < bikeControl.stuntChance || stateData.stuntID > -1))
                        {

                            //pick stunt
                            if (overrideControllers.Length > 0)
                            {

                                if (stateData.stuntID < 0)
                                {
                                    stateData.stuntID = Random.Range(0, overrideControllers.Length);
                                }

                                //print(animIndex);
                                animator.runtimeAnimatorController = overrideControllers[stateData.stuntID];
                                stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                            }

                            animator.SetBool("TrickStart", true);
                            trickJustStarted = true;

                        }

                    }
                    else
                    {
                        //if in the loop state set update flags
                        if (stateInfo.nameHash == trickLoopHash)
                        {

                            animator.SetBool("TrickStart", false);
                            animator.SetBool("Trick", true);
                        }
                    }

                }
                else
                {
                    //if in one of the trick states stop trick
                    if (stateInfo.nameHash == trickStartHash ||
                        stateInfo.nameHash == trickLoopHash ||
                        stateInfo.nameHash == trickEndHash)
                    {

                        //and the trick state is not set
                        if (!animator.GetBool("TrickEnd"))
                            animator.SetBool("TrickEnd", true);

                        animator.SetBool("TrickStart", false);
                        animator.SetBool("Trick", false);

                        stateData.stuntID = -1;
                    }
                }
                //can also exit stunt
            }
            else
            {
                //if not flying can only exit stunt

                //if in one of the trick states
                if (stateInfo.nameHash == trickStartHash ||
                    stateInfo.nameHash == trickLoopHash ||
                    stateInfo.nameHash == trickEndHash)
                {

                    //and the trick state is not set
                    if (!animator.GetBool("TrickEnd"))
                        animator.SetBool("TrickEnd", true);

                    animator.SetBool("TrickStart", false);
                    animator.SetBool("Trick", false);
                }

            }

            if (bikeControl.fly)
            {
                animator.SetBool("Flying", true);
            }
            else
            {
                animator.SetBool("Flying", false);
            }

            flying = animator.GetBool("Flying");

            if (stateInfo.nameHash != trickStartHash &&
                stateInfo.nameHash != trickLoopHash &&
                stateInfo.nameHash != trickEndHash)
            {

                if (animator.GetBool("TrickEnd"))
                    animator.SetBool("TrickEnd", false);
            }

        }
    }

    //	
    //	// Update is called once per frame
    //	void Update () {
    //		if (animator != null && tabletControl != null) {
    //			stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    //
    ////			if ((stateInfo.nameHash == Animator.StringToHash("Base Layer.Idle_01")) ||
    ////			     (stateInfo.nameHash == Animator.StringToHash("Base Layer.Accelerate_02"))) {
    //
    //			if(tabletControl.touchLeft && !tabletControl.fly) {
    //				animator.SetBool("Braking", true);
    //			} else {
    //				animator.SetBool("Braking", false);
    //			}
    ////			}	
    //
    ////			if ((stateInfo.nameHash == Animator.StringToHash("Base Layer.Idle_01")) ||
    ////				(stateInfo.nameHash == Animator.StringToHash("Base Layer.Brake_02"))) {
    //
    //			if(!tabletControl.touchLeft && tabletControl.touchRight && !tabletControl.fly) {
    //				animator.SetBool("Accelerating", true);
    //			} else {
    //				animator.SetBool("Accelerating", false);
    //			}
    //			//			}
    //			
    //			if(tabletControl.rotateCW) {
    //				animator.SetBool("LeaningForward", true);
    //			} else {
    //				animator.SetBool("LeaningForward", false);
    //			}
    //			
    //			if(tabletControl.rotateCCW) {
    //				animator.SetBool("LeaningBack", true);
    //			} else {
    //				animator.SetBool("LeaningBack", false);
    //			}
    //
    //
    //			if(entityTrigger.collTag == "StuntZone") {
    //
    //				//roll chance once per stunt zone
    //				if (!stuntZoneAlreadyEntered) {
    //					stuntZoneAlreadyEntered = true;
    //
    //					randomRoll = Random.value;
    //
    //					//if just entered stunt zone && rolled a trick
    //					if(!animator.GetBool("TrickStart") && randomRoll < tabletControl.stuntChance) {
    //
    //						if(overrideControllers.Length > 0) {
    //							int animIndex = Random.Range(0, overrideControllers.Length);
    //							print(animIndex);
    //							animator.runtimeAnimatorController = overrideControllers[animIndex];
    //						}
    //
    ////						animator.SetBool("Trick", true);
    //						animator.SetBool("TrickStart", true);
    //					}
    //				}
    //
    //			} else {
    //
    //				if(animator.GetBool("Trick")) { //if left the stunt zone and still looping -> stop loop, continue to trick end
    //
    //					animator.SetBool("TrickStart", false);
    //					animator.SetBool("Trick", false);
    //					animator.SetBool("TrickEnd", true);
    //
    //				}
    //
    //				if(stuntZoneAlreadyEntered) {
    //					stuntZoneAlreadyEntered = false;
    //				}
    //
    //			}
    //
    //
    //			
    //			if (stateInfo.nameHash == Animator.StringToHash("Base Layer.TrickLoop")) {
    //				animator.SetBool("TrickStart", false);
    //				if(!animator.GetBool("TrickEnd"))
    //					animator.SetBool("Trick", true);
    //			}
    //
    //			if(animator.GetBool("TrickEnd")) { //if left the stunt zone and still looping -> stop loop, continue to trick end
    //				
    //				animator.SetBool("TrickStart", false);
    //				animator.SetBool("Trick", false);
    //				
    //			}
    //			
    //			if (stateInfo.nameHash == Animator.StringToHash("Base Layer.AirBlendTree") ||
    //			    stateInfo.nameHash == Animator.StringToHash("Base Layer.GroundBlendTree") ) {
    //				animator.SetBool("TrickEnd", false);
    //			}
    //
    //			if(tabletControl.fly) {
    //				animator.SetBool("Flying", true);
    //			} else {
    //				animator.SetBool("Flying", false);
    //			}
    //
    ////			if(!tabletControl.fly && stateInfo.nameHash != Animator.StringToHash("Base Layer.TrickEnd"))
    ////				animator.SetBool("TrickEnd", false);
    //
    //			animator.SetFloat("Acceleration", tabletControl.lowPassValueX);
    //
    //		}
    //	}

    public void Reset()
    {
        if (animator != null)
        {
            animator.SetBool("TrickStart", false);
            animator.SetBool("Trick", false);
            animator.SetBool("TrickEnd", false);

            animator.runtimeAnimatorController = overrideControllers[0];
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            animator.Play("Base Layer.GroundBlendTree", -1, 0);
            //			animator.Play("Base Layer.GroundBlendTree");
        }
    }
}

}
