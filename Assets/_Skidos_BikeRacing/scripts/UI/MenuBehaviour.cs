namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;


//poga, kas atver MP galveno ekránu
public class MenuBehaviour : MonoBehaviour
{
    GameObject rider;
    Animator riderAnim;
    //    Animator helmetAnim;
    Animator bodyAnim;
    Animator bikeAnim;

    GameObject levelsButton;
    Animator levelsButtonAnim;

    GameObject background;

    void Awake()
    {
        rider = transform.Find("HomeScreen_Rider3D").gameObject;

        if (rider != null)
        {
            riderAnim = rider.transform.Find("animation").GetComponent<Animator>();
            //            helmetAnim = rider.transform.FindChild("animation/MainRider3D/Garage_Helmet_Generic").GetComponent<Animator>();
            //            bodyAnim = rider.transform.FindChild("animation/MainRider3D/Garage_Body_Main").GetComponent<Animator>();
            //            bikeAnim = rider.transform.FindChild("animation/Regular3D/Garage_Bike_Generic").GetComponent<Animator>();
            //            helmetAnim = rider.transform.FindChild("animation/MainRider3D/Garage_Helmet_Generic").GetComponent<Animator>();
            bodyAnim = rider.transform.Find("animation/HomeRider").GetComponent<Animator>();
            bikeAnim = rider.transform.Find("animation/HomeBike").GetComponent<Animator>();

            riderAnim.speed = 0;
            //            helmetAnim.speed = 0;
            bodyAnim.speed = 0;
            bikeAnim.speed = 0;

            bodyAnim.gameObject.transform.localPosition = Vector3.zero;
            bikeAnim.gameObject.transform.localPosition = Vector3.zero;
        }

        if (background == null)
        {
            Transform tmp = transform.Find("Backdrop");
            background = (tmp != null) ? tmp.gameObject : null;

            if (background == null)
            {
                tmp = transform.Find("Background");
                background = (tmp != null) ? tmp.gameObject : null;
            }
        }


        levelsButton = transform.Find("LevelsButton").gameObject;
        if (levelsButton != null)
        {
            levelsButtonAnim = levelsButton.transform.Find("SphericPlay").GetComponent<Animator>();

            levelsButtonAnim.speed = 0;
        }
    }

    //just skip the first couple of updates
    [SerializeField]
    int numUpdate = 0;
    int skpUpdate = 5;
    void Update()
    {
        if (rider != null)
        {
            if (numUpdate >= skpUpdate)
            {
                //            rider.SetActive(true);
                riderAnim.speed = 1;
                //                helmetAnim.speed = 1;
                bodyAnim.speed = 1;
                bikeAnim.speed = 1;

                levelsButtonAnim.speed = 1;
            }

            if (numUpdate < skpUpdate)
            {
                numUpdate++;
            }
        }
        else
        {
            OnEnable();
        }

        if (bodyAnim != null && bodyAnim.transform.localPosition.x != 0)
        {
            bodyAnim.gameObject.transform.localPosition = Vector3.zero;
            bikeAnim.gameObject.transform.localPosition = Vector3.zero;
        }

    }

    void OnEnable()
    {
        if (rider == null)
        {
            rider = Instantiate(LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("Prefabs/UI/HomeScreen_Rider3D")) as GameObject;
            Debug.Log("<color=yellow>Prefab Loaded Name = </color>" + rider);
            //rider = Instantiate(Resources.Load("Prefabs/UI/HomeScreen_Rider3D")) as GameObject;
            rider.name = "HomeScreen_Rider3D";
            rider.transform.SetParent(transform);
            //            rider.transform.localPosition = new Vector3(-192.0f, -233.0f, -239.0f);
            rider.transform.localPosition = new Vector3(-134.0f, -229.0f, -239.0f);
            rider.transform.localScale = new Vector3(156.5484f, 156.5484f, 156.5484f);

            GameObject playAnimation = Instantiate(LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("SphericPlay")) as GameObject;
            Debug.Log("<color=yellow>Prefab Loaded Name = </color>" + playAnimation);
            //GameObject playAnimation = Instantiate (Resources.Load("Prefabs/UI/SphericPlay")) as GameObject;
            playAnimation.name = "SphericPlay";
            playAnimation.transform.SetParent(levelsButton.transform);
            playAnimation.transform.localPosition = new Vector3(-7.199799f, 4.828719f, -60.00009f);
            playAnimation.transform.localScale = new Vector3(107.0f, 107.0f, 107.0f);

            Awake();
        }

        if (background == null)
        {
            if (BikeDataManager.SettingsHD)
            {
                background = Instantiate(LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("MenuBackdrop")) as GameObject;
                Debug.Log("<color=yellow>Prefab Loaded Name = </color>" + background);
                //background = Instantiate(Resources.Load("Prefabs/UI/MenuBackdrop")) as GameObject;
                background.name = "Backdrop";
            }
            else
            {
                background = Instantiate(LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("MenuBackground")) as GameObject;
                Debug.Log("<color=yellow>Prefab Loaded Name = </color>" + background);
                background.name = "Background";
            }

            background.transform.SetParent(transform);
            background.transform.localPosition = Vector3.zero;
            background.transform.localScale = Vector3.one;
            background.transform.SetAsFirstSibling();
        }

    }

    void OnDisable()
    {
        if (rider != null)
        {
            Destroy(rider);
            Destroy(levelsButtonAnim.gameObject);
        }
    }

}

}
