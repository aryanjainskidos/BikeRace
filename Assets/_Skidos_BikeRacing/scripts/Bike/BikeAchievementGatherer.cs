namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class BikeAchievementGatherer : MonoBehaviour
{

    BikeControl controlScript;

    BikeTriggerCollision wheelA; //front wheel touching ground
    BikeTriggerCollision wheelB;
    float bodyScale; //X & Y scale of main body bike - used to get real units driven

    float airTime = 0;
    float airTimeOnGoing = 0;
    float distance = 0;
    float distanceWheelieA = 0;
    float distanceWheelieB = 0;
    int jumps = 0;

    float distanceFromStart = 0;
    bool frontWheelTouchedGround = false;
    int didLevelOnBackWheel = 0;

    float backflips;
    float backflipsOnGoing;
    bool backflipCheck90 = false;
    bool backflipCheck180 = false;
    bool backflipCheck270 = false;


    float frontflips;
    float frontflipsOnGoing;
    bool frontflipCheck90 = false;
    bool frontflipCheck180 = false;
    bool frontflipCheck270 = false;


    bool controllerFound = true;

    void Start()
    {

        controlScript = transform.GetComponent<BikeControl>();
        if (controlScript == null)
        {
            Debug.LogWarning("Nav BikeControl skripta, nebuus achiivmentu :(");
            controllerFound = false;
        }

        wheelA = transform.Find("wheel_front").Find("wheel_front_trigger").GetComponent<BikeTriggerCollision>();
        wheelB = transform.Find("wheel_back").Find("wheel_back_trigger").GetComponent<BikeTriggerCollision>();



        bodyScale = transform.localScale.x;
        StartCoroutine(FlushBikeAchievements());

    }


    void Update()
    {
        if (!controllerFound)
        {
            return;
        }

        if (UIManager.currentScreenType == GameScreenType.PreGame)
        {
            airTimeOnGoing = 0;//reset airtime (to prevent it being caried over from last ride - where he crashed or just restarted)
            backflipsOnGoing = 0;
            distanceFromStart = 0;
            frontWheelTouchedGround = false;
        }

        if (UIManager.currentScreenType == GameScreenType.PostGame)
        {
            if (!frontWheelTouchedGround)
            {
                didLevelOnBackWheel++;
                frontWheelTouchedGround = true;
            }
        }


        if (UIManager.currentScreenType == GameScreenType.PostGame || !controlScript.fly)
        {  //finish screen || stopped flying
            if (airTimeOnGoing > 0)
            {
                //print("airtime " + airTimeOnGoing );

                airTime = airTimeOnGoing; //end and register the jump		
                if (airTimeOnGoing > 0.5f)
                { // big enough to be classified as a jump (not just riding over a bump)
                    jumps++;
                }
                airTimeOnGoing = 0;
            }

            if (backflipsOnGoing > 0)
            {
                backflips = backflipsOnGoing; //cache in all succesful backflips
                backflipsOnGoing = 0;
                //print("cached in " + backflips + " backflips");
            }

            if (frontflipsOnGoing > 0)
            {
                frontflips = frontflipsOnGoing;
                frontflipsOnGoing = 0;
                //print("cached in " + frontflips + " frontflips");
            }
        }

        //GameManager.bikeDied

        if (controlScript.fly)
        { //flying (or falling >:)
            airTimeOnGoing += Time.deltaTime;
        }
        else
        {//any of wheel is touching ground
            distance += GetComponent<Rigidbody2D>().linearVelocity.magnitude * Time.deltaTime;
        }
        if (wheelA.colliding && !wheelB.colliding)
        {//only front wheel
            distanceWheelieA += GetComponent<Rigidbody2D>().linearVelocity.magnitude * Time.deltaTime;
        }
        if (wheelB.colliding && !wheelA.colliding)
        { //only back wheel
            distanceWheelieB += GetComponent<Rigidbody2D>().linearVelocity.magnitude * Time.deltaTime;
        }


        //bekflipam ir 3 secígi kontrolpunkti, kur vidéjais ir jáveic gaisá
        if (transform.eulerAngles.z >= 90 && transform.eulerAngles.z < 180)
        {
            backflipCheck90 = true;
        }
        if (backflipCheck90 && transform.eulerAngles.z >= 180 && transform.eulerAngles.z < 270 && controlScript.fly)
        {
            backflipCheck180 = true;
        }
        if (backflipCheck180 && transform.eulerAngles.z >= 270 && transform.eulerAngles.z < 360)
        {
            backflipCheck270 = true;
        }
        if (backflipCheck270)
        {
            backflipsOnGoing++;
            backflipCheck90 = false;
            backflipCheck180 = false;
            backflipCheck270 = false;
            frontflipCheck90 = false;
            frontflipCheck180 = false;
            frontflipCheck270 = false;
            ///print("backflips++ " + backflipsOnGoing);
        }


        if (transform.eulerAngles.z >= 270 && transform.eulerAngles.z < 360)
        {
            frontflipCheck270 = true;
        }
        if (frontflipCheck270 && transform.eulerAngles.z >= 180 && transform.eulerAngles.z < 270 && controlScript.fly)
        {
            frontflipCheck180 = true;
        }
        if (frontflipCheck180 && transform.eulerAngles.z >= 90 && transform.eulerAngles.z < 180)
        {
            frontflipCheck90 = true;
        }
        if (frontflipCheck90)
        {
            frontflipsOnGoing++;
            backflipCheck90 = false;
            backflipCheck180 = false;
            backflipCheck270 = false;
            frontflipCheck90 = false;
            frontflipCheck180 = false;
            frontflipCheck270 = false;
            //print("frontflips++ " + frontflipsOnGoing);
        }

        distanceFromStart += GetComponent<Rigidbody2D>().linearVelocity.magnitude * Time.deltaTime;
        if (distanceFromStart < 1)
        { //ne tálák ká 1m no starta (vnk źonlgéśana uz pakaĺéjá rata arí palielina śo attálumu):

        }
        else
        {
            if (wheelA.colliding)
            { // tálák no starta nedríkst pieskárties ar priekśéjo ratu zemei
                if (!BikeGameManager.playerState.finished)
                { //péc finiśa neskaitam pieskárśanos
                    frontWheelTouchedGround = true;
                }
            }
        }

    }


    //once a second flush achieviements to achievement manager
    IEnumerator FlushBikeAchievements()
    {
        while (true)
        {

            if (jumps > 0)
            {
                AchievementManager.AchievementProgress("jumps", jumps);
                AchievementManager.AchievementProgress("jumps__2", jumps);
                AchievementManager.AchievementProgress("jumps__3", jumps);
                jumps = 0;
            }

            if (airTime > 0)
            {
                AchievementManager.AchievementProgress("airtime", airTime);
                AchievementManager.AchievementProgress("airtime__2", airTime);
                AchievementManager.AchievementProgress("airtime__3", airTime);
                airTime = 0;
            }

            if (distance > 0)
            {
                float d = (distance / bodyScale) * BikeDataManager.UNITS_TO_METERS;
                AchievementManager.AchievementProgress("distance", d);
                AchievementManager.AchievementProgress("distance__2", d);
                AchievementManager.AchievementProgress("distance__3", d);
                distance = 0;
            }

            if (distanceWheelieA > 0)
            {
                float d = (distanceWheelieA / bodyScale) * BikeDataManager.UNITS_TO_METERS;
                AchievementManager.AchievementProgress("wheelie_front", d);
                AchievementManager.AchievementProgress("wheelie_front__2", d);
                AchievementManager.AchievementProgress("wheelie_front__3", d);
                distanceWheelieA = 0;
            }

            if (distanceWheelieB > 0)
            {
                float d = (distanceWheelieB / bodyScale) * BikeDataManager.UNITS_TO_METERS;
                AchievementManager.AchievementProgress("wheelie_back", d);
                AchievementManager.AchievementProgress("wheelie_back__2", d);
                AchievementManager.AchievementProgress("wheelie_back__3", d);
                distanceWheelieB = 0;
            }

            if (backflips > 0)
            {
                AchievementManager.AchievementProgress("backflips", backflips);
                AchievementManager.AchievementProgress("backflips__2", backflips);
                AchievementManager.AchievementProgress("backflips__3", backflips);
                backflips = 0;
            }
            if (frontflips > 0)
            {
                AchievementManager.AchievementProgress("frontflips", frontflips);
                AchievementManager.AchievementProgress("frontflips__2", frontflips);
                AchievementManager.AchievementProgress("frontflips__3", frontflips);
                frontflips = 0;
            }
            if (didLevelOnBackWheel > 0)
            {
                AchievementManager.AchievementProgress("wheelie_level", didLevelOnBackWheel);

                if (LevelManager.CurrentLevelName == "a___001")
                {
                    AchievementManager.AchievementProgress("wheelie_level__2", didLevelOnBackWheel);
                }

                if (LevelManager.CurrentLevelName == "a___063")
                {
                    AchievementManager.AchievementProgress("wheelie_level__3", didLevelOnBackWheel);
                }
                didLevelOnBackWheel = 0;
            }



            yield return new WaitForSeconds(1); //one sec
        }
    }


}

}
