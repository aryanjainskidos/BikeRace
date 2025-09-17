namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using SimpleJSON;

public class BikeCamera : MonoBehaviour
{

    public GameObject target;
    public GameObject targetSecondary;

    public float xSensitivity = 10;
    public float ySensitivity = 6;
    public float sizeSensitivity = 1;

    float cameraNewX;
    float cameraNewY;

    public float velocityThresholdLow = 1;
    public float velocityThresholdHigh = 40;
    float deltaVelocityThreshold;

    public float distanceThresholdHigh = 12.5f;

    public float maxCameraSize = 11;
    public float minCameraSize = 7;
    float deltaCameraSize;

    public float ratioSizeCorrection = 1.35f;
    float cameraSizeCorrection;

    float cameraNewZoom;

    bool reset = false;
    bool unzoom = false;
    Bounds unzoomBounds;
    float unzoomOrthoSize = 0;

    //this also will be called after new level is loaded [and new bike is created]
    public void SetTarget(GameObject target)
    {
        xSensitivity = 10;
        this.target = target;
        Start();
    }

    public void SetSecondaryTarget(GameObject target)
    {
        xSensitivity = 6;
        this.targetSecondary = target;
    }

    public void Start()
    {
        deltaVelocityThreshold = velocityThresholdHigh - velocityThresholdLow;
        deltaCameraSize = maxCameraSize - minCameraSize;

        if (Startup.Initialized)
        {
            PrepareUnzoomedVisuals(true); //daźreiz cauri límenja ǵeometrijai ir redzama kameras pamatkrása - śis tikai piekrásos pamatkrásu
        }
    }

    void Awake()
    {
        float minRatio = 1.33f; //4:3
        float maxRatio = 1.77f; //16:9
        float deltaRatio = maxRatio - minRatio;
        float ratio = (float)Screen.width / Screen.height;
        float ratioCoef = Mathf.Clamp01((ratio - minRatio) / deltaRatio); //0 for 4:3 and narrower, 1 for 16:9 and wider

        cameraSizeCorrection = ratioSizeCorrection * (1 - ratioCoef);//max correction value for 4:3, no correction for 16:9

        //Debug.Log("ratio " + ratio + " correction " + cameraSizeCorrection);
    }


    //looks awful, used only if game is paused (if game is runing FixedUpdate is used)
    void Update()
    {



        //if paused and reset is pressed camera is allowed to move to player position
        if (Time.timeScale == 0 && target && reset && UIManager.currentScreenType != GameScreenType.Unzoomed)
        {
            AdjustZoomToTargetVelocity(Time.unscaledDeltaTime * 3);
            AdjustPosition(target.transform.position.x + GetComponent<Camera>().orthographicSize - cameraSizeCorrection, target.transform.position.y, Time.unscaledDeltaTime * 3);
            unzoom = false;
        }
        else if (Time.timeScale == 0 && UIManager.currentScreenType == GameScreenType.Unzoomed)
        {

            if (!unzoom)
            { //will do this once per unzoom
                PrepareUnzoomedVisuals();

                //bounds tagad bús no starta lídz finiśam + iekljaujot eksplicítos UnzoomBoundsExtender objektus   (iepriekś viss límenis tika njemts vérá, rékjinot bounds)
                Vector3 startPos = GameObject.FindGameObjectWithTag("StartZone").transform.position;
                Vector3 finishPos = GameObject.FindGameObjectWithTag("Finish").transform.position;
                GameObject[] UnzoomBoundsExtenders = GameObject.FindGameObjectsWithTag("UnzoomBoundsExtender");

                unzoomBounds = new Bounds(startPos, Vector3.zero);
                unzoomBounds.Encapsulate(finishPos);
                for (int i = 0; i < UnzoomBoundsExtenders.Length; i++)
                {
                    unzoomBounds.Encapsulate(UnzoomBoundsExtenders[i].transform.position);
                }

                if (unzoomBounds.size.x > unzoomBounds.size.y)
                { //plats límenis
                    unzoomOrthoSize = 1.1f * (unzoomBounds.size.x * Screen.height / Screen.width * 0.5f);  //100% párliecináts par śo matemátiku   ( http://answers.unity3d.com/questions/760671/resizing-orthographic-camera-to-fit-2d-sprite-on-s.html )
                }
                else
                { //Vertikáls límenis
                    unzoomOrthoSize = 0.6f * unzoomBounds.size.y; //izskatás OK :D
                }

                //print("unzoomBounds:" + unzoomBounds.ToString());
                //print("Screen  h:" + Screen.height + " w:" + Screen.width);
            }

            AdjustPosition(unzoomBounds.center.x, unzoomBounds.center.y, Time.unscaledDeltaTime * 0.5f);
            AdjustZoom(unzoomOrthoSize, Time.unscaledDeltaTime * 0.5f); //izskatás OK :D


            unzoom = true;
        }

    }


    /**
	 * virs uz debesu objekta un zem tá ir redzams nekas (kas atśḱiras no debesu objekta gradienta krásám)
	 * 
	 * a) párkráso kameras pamatkrásu gradienta augśéjá krásá
	 * b) pievieno objektu zem kameras, párkráso to gradienta apakśéjá krásá, nopozicioné, lai tas aizklátu apakśéjo ekrána dalju (bet ne límeni)
	 *  
	 */
    void PrepareUnzoomedVisuals(bool justChangeCameraColor = false)
    {
        GameObject skyContainer = GameObject.Find("LevelContainer/SkyContainer");
        if (skyContainer != null)
        {
            if (skyContainer.transform.childCount == 0)
            {
                Debug.LogError("SkyContainer ir tukshs, izzuumoshana trauceeta!");
                return;
            }
            Transform someKindOfSky = skyContainer.transform.GetChild(0); //pirmais objekts debesu konteinerí (tur vienmér ir viens)
            Renderer skyrenderer = someKindOfSky.Find("Sky").GetComponent<Renderer>(); //debesím vienmér ir gradient-krásots objekts várdá "Sky" ar materiaĺu - Foliage Shader, kas satur 2 krásas
            Color top = skyrenderer.material.GetColor("_ColorTop");
            Color bottom = skyrenderer.material.GetColor("_ColorBottom");

            if (justChangeCameraColor)
            {
                //print("bottom:" + bottom);
                Camera.main.backgroundColor = bottom; //kamerai iedod apakśas krásu 
                return; // ar to pietiek, sagatavojot visu - unzoom kamerai dos augśéjo krásu
            }

            //apakśas krásińai:
            //samazina kośumu (desaturate)    
            float f = 0.25f; // desaturate by XX%  			http://stackoverflow.com/a/20820649/207757
            float L = 0.3f * bottom.r + 0.6f * bottom.g + 0.1f * bottom.b;
            float new_r = bottom.r + f * (L - bottom.r);
            float new_g = bottom.g + f * (L - bottom.g);
            float new_b = bottom.b + f * (L - bottom.b);

            //un padara tumśáku      			 			http://www.pvladov.com/2012/09/make-color-lighter-or-darker.html
            float correctionFactor = 0.3f;
            new_r *= correctionFactor;
            new_g *= correctionFactor;
            new_b *= correctionFactor;

            bottom = new Color(new_r, new_g, new_b);



            Camera.main.backgroundColor = top; //kamerai iedod augśéjo krásu


            Transform skyGradientBottomTr = Camera.main.transform.Find("SkyGradientBottom");
            GameObject skyGradientBottomGo;

            if (skyGradientBottomTr == null)
            {
                GameObject prefab = LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("SkyGradientBottom") as GameObject;
                Debug.LogError("Prefab is loading from = " + prefab);

                //GameObject prefab = Resources.Load("Prefabs/Skies/SkyGradientBottom") as GameObject;
                skyGradientBottomGo = Instantiate(prefab) as GameObject;
                skyGradientBottomGo.transform.position = new Vector3(0, -2500, 100);
                skyGradientBottomGo.transform.localScale = new Vector3(5000, 5000, 1);
                skyGradientBottomGo.transform.parent = Camera.main.transform;
                skyGradientBottomGo.name = "SkyGradientBottom";
            }
            else
            {
                skyGradientBottomGo = skyGradientBottomTr.gameObject;
            }
            Renderer skyGradientBottomRenderer = skyGradientBottomGo.GetComponent<Renderer>();
            skyGradientBottomRenderer.material.SetColor("_Color", bottom);


        }

    }

    void AdjustPosition(float x, float y, float timeDelta)
    {

        float boost = 1;
        if (target != null && target.GetComponent<Rigidbody2D>() != null && Mathf.Abs(target.GetComponent<Rigidbody2D>().linearVelocity.y) > 20)
        { //if falling faster than X, boost camera speed (only in Y direction)
            boost = (Mathf.Abs(target.GetComponent<Rigidbody2D>().linearVelocity.y) / 20);
        }

        cameraNewX = Mathf.Lerp(transform.position.x, x, timeDelta * xSensitivity);
        cameraNewY = Mathf.Lerp(transform.position.y, y, timeDelta * ySensitivity * boost);
        transform.position = new Vector3(cameraNewX, cameraNewY, transform.position.z);

    }

    void AdjustZoom(float zoom, float timeDelta)
    {
        GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, zoom, timeDelta * sizeSensitivity);
    }

    void AdjustZoomToTargetVelocity(float timeDelta)
    {
        if (target != null && target.GetComponent<Rigidbody2D>() != null && target.GetComponent<Rigidbody2D>().linearVelocity.x > velocityThresholdLow)
        {
            if (GetComponent<Camera>().orthographicSize > maxCameraSize + cameraSizeCorrection)
            {
                GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, maxCameraSize + cameraSizeCorrection, timeDelta * sizeSensitivity);
            }
            else
            {
                if (target.GetComponent<Rigidbody2D>().linearVelocity.x < velocityThresholdHigh)
                {
                    cameraNewZoom = minCameraSize + cameraSizeCorrection + deltaCameraSize * ((target.GetComponent<Rigidbody2D>().linearVelocity.x - velocityThresholdLow) / deltaVelocityThreshold);
                    GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, cameraNewZoom, timeDelta * sizeSensitivity);
                }
            }
        }
        else
        {
            if (GetComponent<Camera>().orthographicSize > minCameraSize + cameraSizeCorrection)
            {
                GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, minCameraSize + cameraSizeCorrection, timeDelta * sizeSensitivity);
            }
        }
    }

    void AdjustZoomToTargetDistance(float timeDelta)
    {
        if (target != null && targetSecondary != null)
        {
            if (GetComponent<Camera>().orthographicSize > maxCameraSize + cameraSizeCorrection)
            {
                GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, maxCameraSize + cameraSizeCorrection, timeDelta * sizeSensitivity);
            }
            else
            {
                //                if(target.rigidbody2D.velocity.x < velocityThresholdHigh) {
                //                    
                //                    cameraNewZoom = minCameraSize + deltaCameraSize * ((target.rigidbody2D.velocity.x - velocityThresholdLow) / deltaVelocityThreshold);
                //                    camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, cameraNewZoom, timeDelta * sizeSensitivity);
                //                }
                //TODO calculate camera zoom
                float distance = Mathf.Abs(target.transform.position.x - targetSecondary.transform.position.x);

                if (distance < distanceThresholdHigh)
                {
                    cameraNewZoom = minCameraSize + cameraSizeCorrection + deltaCameraSize * (distance / distanceThresholdHigh);
                    GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, cameraNewZoom, timeDelta * sizeSensitivity);
                }
            }
        }
        else
        {
            if (GetComponent<Camera>().orthographicSize > minCameraSize + cameraSizeCorrection)
            {
                GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, minCameraSize + cameraSizeCorrection, timeDelta * sizeSensitivity);
            }
        }
    }


    void FixedUpdate()
    {

        //does nothing if Time.timeScale == 0
        if (target)
        {
            if (targetSecondary == null)
            {
                AdjustZoomToTargetVelocity(Time.fixedDeltaTime);
                AdjustPosition(target.transform.position.x + GetComponent<Camera>().orthographicSize - cameraSizeCorrection, target.transform.position.y, Time.fixedDeltaTime);
            }
            else
            {
                //                float distance = Mathf.Abs(target.transform.position.x - targetSecondary.transform.position.x);
                //                Vector3 midpoint = (target.transform.position - targetSecondary.transform.position) * 0.5f + targetSecondary.transform.position;

                AdjustZoomToTargetDistance(Time.fixedDeltaTime);
                //                if(distance < distanceThresholdHigh)
                //                    AdjustPosition(midpoint.x + camera.orthographicSize / 2, midpoint.y, Time.fixedDeltaTime);
                //                else 
                //                    AdjustPosition(target.transform.position.x - distanceThresholdHigh / 2 + camera.orthographicSize / 2, target.transform.position.y, Time.fixedDeltaTime);
                AdjustPosition(target.transform.position.x + GetComponent<Camera>().orthographicSize - cameraSizeCorrection, target.transform.position.y, Time.fixedDeltaTime);
            }

            if (reset) reset = false;

        }
    }

    public void Reset()
    {
        reset = true;
    }

}

}
