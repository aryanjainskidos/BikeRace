namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerPointerBehaviour : MonoBehaviour
{
    public Vector2 anchoredPosition
    {
        get { return rectTransform.anchoredPosition; }
    }

    RectTransform rectTransform;
    RectTransform playerCircleRectTransform;
    Button playerCircleButton;

    public delegate void SimpleClickDelegate();
    public SimpleClickDelegate clickDelegate;

    ColorMePrettyUI playerCircleColorer;

    GameObject helmet;
    string loadedHelmetPrefabName = "";

    // Use this for initialization
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        playerCircleRectTransform = transform.Find("Visual/PlayerPointer/PlayerCircle").GetComponent<RectTransform>();
        playerCircleButton = transform.Find("Visual/PlayerPointer/PlayerCircle").GetComponent<Button>();
        playerCircleColorer = playerCircleRectTransform.GetComponent<ColorMePrettyUI>();

        playerCircleButton.onClick.AddListener(OnClick);
    }

    void OnEnable()
    {
        if (Startup.Initialized)
        {


            int styleID = BikeDataManager.Bikes[playerCircleColorer.selectedRecord].StyleID;

            if (helmet == null || loadedHelmetPrefabName != BikeDataManager.Styles[styleID].LevelsPrefabName)
            {

                Destroy(helmet);

                loadedHelmetPrefabName = BikeDataManager.Styles[styleID].LevelsPrefabName;
                Debug.Log("<color=yellow>Prefab is loading from = </color>" + loadedHelmetPrefabName);
                helmet = (GameObject)Instantiate(LoadAddressable_Vasundhara.Instance.GetPrefab_Resources(loadedHelmetPrefabName));
                Debug.Log("<color=yellow>Prefab Loaded Name = </color>" + helmet);
                //helmet = (GameObject)Instantiate(Resources.Load("Prefabs/Riders/" + loadedHelmetPrefabName));
                helmet.transform.SetParent(playerCircleRectTransform);
                helmet.transform.localPosition = new Vector3(-2.5f, 2.8f, 0);// Vector3.zero;
                helmet.transform.localScale = Vector3.one;
                helmet.transform.localRotation = Quaternion.identity;
            }

            playerCircleColorer.Run();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (rectTransform.localRotation.eulerAngles.z != -playerCircleRectTransform.localRotation.eulerAngles.z)
        {
            Quaternion tmpRotation = playerCircleRectTransform.localRotation;
            tmpRotation.eulerAngles = new Vector3(0, 0, -rectTransform.localRotation.eulerAngles.z);
            playerCircleRectTransform.localRotation = tmpRotation;
        }
    }

    public void SetAngle(float angle)
    {
        try
        {
            Quaternion tmpRotation;
            tmpRotation = rectTransform.localRotation;
            tmpRotation.eulerAngles = new Vector3(0, 0, angle);
            rectTransform.localRotation = tmpRotation;

            tmpRotation = playerCircleRectTransform.localRotation;
            tmpRotation.eulerAngles = new Vector3(0, 0, -angle);
            playerCircleRectTransform.localRotation = tmpRotation;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }

    }

    Vector2 fromAnchoredPosition;

    public void SetPosition(Vector2 pos, bool animate)
    {
            try
            {
                rectTransform.anchoredPosition = pos;
                //TODO animate
                if (animate)
                {
                    //fromAnchoredPosition = rectTransform.anchoredPosition;

                    //iTween.ValueTo(gameObject, iTween.Hash(
                    //    "name", "move_" + transform.name,
                    //    "from", fromAnchoredPosition,
                    //    "to", pos,
                    //    "time", 0.5f,
                    //    //delay?
                    //    "onupdatetarget", gameObject,
                    //    "onupdate", "OnTweenUpdatePosition",
                    //    "easetype", iTween.EaseType.easeOutQuad,
                    //    "ignoretimescale", true
                    //    )
                    //);

                    Debug.Log("player previous pos = " + rectTransform.anchoredPosition);
                    Debug.Log("player pos = " + pos);
                    rectTransform.anchoredPosition = pos;
                }
                else
                {
                    Debug.Log("elseeeeeeee  called");
                    rectTransform.anchoredPosition = pos;
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log("Exception in SetPosition");
            }
            
    }

    void OnTweenUpdatePosition(Vector2 newValue)
    {

        rectTransform.anchoredPosition = newValue;

    }

    //TODO reset animation before starting an other one just in case

    void OnClick()
    {
        print("PlayerPointerBehaviour::OnClick");
        if (clickDelegate != null)
        {
            clickDelegate();
        }
    }
}

}
