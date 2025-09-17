namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupUnlockLevelsBehaviour : MonoBehaviour
{


    GameObject PlayMoreGO;
    GameObject GetStarsGO;
    Text NumStars;
    Text NumCoins;

    LevelDataContainer levelData;

    void Awake()
    {
        PlayMoreGO = transform.Find("PlayMore").gameObject;
        GetStarsGO = transform.Find("GetStars").gameObject;
        NumStars = GetStarsGO.transform.Find("NumText").GetComponent<Text>();
        NumCoins = transform.Find("UpgradeButton/CoinText").gameObject.GetComponent<Text>();

        NumCoins.text = BikeDataManager.PriceUnlockAll.ToString();

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
    }


    void OnEnable()
    {
        if (Startup.Initialized)
        {
            NumCoins.text = BikeDataManager.PriceUnlockAll.ToString();

            //fixed this
            int stars = 0;
            print("GameManager.SelectedLevelName" + BikeGameManager.SelectedLevelName);
            foreach (var item in levelData.levelDataEntries)
            {
                if (item.name == BikeGameManager.SelectedLevelName)
                {
                    stars = item.starsToUnlock;
                }
            }

            //            if(GameManager.SelectedLevelName == )
            if (stars > 0)
            { // number of stars needed to unlock
              //				//tell about stars n stuff
                PlayMoreGO.SetActive(false);
                GetStarsGO.SetActive(true);
                NumStars.text = (stars - BikeDataManager.Stars).ToString();
            }
            else
            {
                //				//tell about playing more levels
                PlayMoreGO.SetActive(true);
                GetStarsGO.SetActive(false);
            }
        }
    }


}

}
