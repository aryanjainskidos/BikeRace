namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WorldLockPanelBehaviour : MonoBehaviour
{

    public string levelName;
    public int index = -1;

    Image image;
    Image starImage;
    Text text;
    Button button;
    LevelDataContainer levelData;

    bool paid = false;

    // Use this for initialization
    void Awake()
    {
        index = -1;

        image = GetComponent<Image>();
        starImage = transform.Find("Image").GetComponent<Image>();

        text = transform.Find("Text").GetComponent<Text>();
        button = GetComponent<Button>();
        // Try to load from LoadAddressable_Vasundhara first, fallback to Resources
        if (LoadAddressable_Vasundhara.Instance != null)
        {
            levelData = LoadAddressable_Vasundhara.Instance.GetScriptableObjLevelData_Resources("LevelData");
        }
        
        // Fallback to Resources if LoadAddressable_Vasundhara failed or returned null
        if (levelData == null)
        {
            levelData = Resources.Load<LevelDataContainer>("Data/LevelData");
        }
        
        // If still null, create a default LevelDataContainer to prevent errors
        if (levelData == null)
        {
            Debug.LogWarning("LevelData not found in LoadAddressable_Vasundhara or Resources. Creating default LevelDataContainer.");
            levelData = ScriptableObject.CreateInstance<LevelDataContainer>();
            levelData.levelDataEntries = new System.Collections.Generic.List<LevelDataEntry>();
        }
        
        Debug.Log("<color=green>LevelData Loaded Successfully: </color>" + (levelData != null ? levelData.name : "null"));

        //handle this scripts handler first (moved to actualize, because sometimes this awake gets executed before the ToggleScreen awake)
        //        button.onClick.RemoveAllListeners();
        //        button.onClick.AddListener(OnClick);
        //        button.onClick.AddListener(GetComponent<UIButtonToggleScreen>().OnClick);

        if (levelData != null && levelData.levelDataEntries != null)
        {
            for (int i = 0; i < levelData.levelDataEntries.Count; i++)
            {
                if (levelData.levelDataEntries[i].name == levelName)
                {
                    index = i;
                }
            }
        }

    }

    void OnClick()
    {
        print("WorldLockPanelBehaviour");
        BikeGameManager.SelectedLevelName = levelName;
        SoundManager.Play("Click");
    }

    void Actualize()
    {

        if (index != -1 && levelData != null && levelData.levelDataEntries != null && index < levelData.levelDataEntries.Count)
        {
            //            if (DataManager.Stars < levelData.levelDataEntries[index].starsToUnlock && !DataManager.PaidLevelUnlock) { // if this world is locked and player didn't pay
            //
            //                text.text = DataManager.Stars + " / " + levelData.levelDataEntries[index].starsToUnlock;
            //
            //                if (index > 0 && DataManager.Stars < levelData.levelDataEntries[index-1].starsToUnlock) {// if previous is not unlocked
            //                    Show(false);
            //                    previousIsLocked = true;
            //                } else {
            //                    Show(true);
            //                    previousIsLocked = false;
            //                }
            //            } else {
            //                Show(false);
            //            }

            if (BikeDataManager.Stars >= levelData.levelDataEntries[index].starsToUnlock ||
                BikeDataManager.PaidLevelUnlock
               )
            {//if star requirement reached or paid

                Show(false);//hide

            }
            else
            { //if star requirement NOT reached and DIDN'T pay

                text.text = BikeDataManager.Stars + " / " + levelData.levelDataEntries[index].starsToUnlock;//set the star count

                if (index == 0 || //show if first or 
                    (index > 0 &&
                     BikeDataManager.Stars >= levelData.levelDataEntries[index - 1].starsToUnlock) //if previous is unlocked
                   )
                {
                    Show(true);

                    //make sure to set this scripts handler first
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(OnClick);
                    button.onClick.AddListener(GetComponent<UIButtonToggleScreen>().OnClick);

                    BikeDataManager.StarsToUnlockNextWorld = levelData.levelDataEntries[index].starsToUnlock;
                }
                else
                { //if previous is locked and not first
                    Show(false);
                }

            }

        }

    }

    // Update is called once per frame
    void Update()
    {
        if (paid != BikeDataManager.PaidLevelUnlock)
        {
            Actualize();
            paid = true;
        }

        if (!gameObject.activeSelf && //not active
            index > 0 && //not first
            levelData != null && levelData.levelDataEntries != null && 
            index - 1 < levelData.levelDataEntries.Count &&
            BikeDataManager.Stars >= levelData.levelDataEntries[index - 1].starsToUnlock) //previous is unlocked
        {
            print("DataManager.Stars levelData.levelDataEntries[index-1].starsToUnlock" + BikeDataManager.Stars + "/" + levelData.levelDataEntries[index - 1].starsToUnlock);
            Actualize();
        }
    }

    void OnEnable()
    {
        Actualize();
    }

    void Show(bool show)
    {
        image.enabled = show;
        starImage.enabled = show;
        text.enabled = show;
    }

}

}
