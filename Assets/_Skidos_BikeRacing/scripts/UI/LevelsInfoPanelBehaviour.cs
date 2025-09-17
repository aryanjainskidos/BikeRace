namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelsInfoPanelBehaviour : MonoBehaviour
{

    int stars = -1;
    string title = "";

    Text titleText;
    Text starProgressText;


    void Awake()
    {
        titleText = transform.Find("TitleText").GetComponent<Text>();
        starProgressText = transform.Find("ProgressText").GetComponent<Text>();
    }


    void Update()
    {
        if (BikeDataManager.Stars != stars)
        {
            stars = BikeDataManager.Stars;
            starProgressText.text = stars + "/" + BikeDataManager.StarsTotal;
        }

        //print("DataManager.PlayerXPLevel:" + DataManager.PlayerXPLevel);
        string tmpTitle = BikeDataManager.PlayerXPLevels[BikeDataManager.PlayerXPLevel].Title;
        if (tmpTitle != title)
        {
            title = tmpTitle;
            titleText.text = "'" + title + "'";
        }
    }
}

}
