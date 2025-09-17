namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MultiplayerLevelInfoBehaviour : MonoBehaviour
{

    List<LevelSliderBehaviour> sliderList;

    Text infoText;
    Text coinText;

    bool initialized = false;

    // Use this for initialization
    void Awake()
    {

        sliderList = new List<LevelSliderBehaviour>();

        for (int i = 1; i < 11; i++)
        {
            sliderList.Add(transform.Find("SliderPanel/LevelSlider" + i).GetComponent<LevelSliderBehaviour>());
        }

        infoText = transform.Find("InfoPanel/InfoText").GetComponent<Text>();
        coinText = transform.Find("InfoPanel/CoinText").GetComponent<Text>();

        //example
        //        foreach (var item in sliderList) {
        //            if(item.level < 5) {
        //                item.SetState(LevelSliderState.Unlocked);
        //            } else { 
        //                if (item.level > 5) {
        //                    item.SetState(LevelSliderState.Locked);
        //                } else {
        //                    item.SetState(LevelSliderState.Selected);
        //                }
        //            }
        //        }
        initialized = true;
    }


    void OnEnable()
    {
        if (Startup.Initialized && initialized)
        {
            foreach (var mpLevel in BikeDataManager.PlayerMultiplayerLevels)
            {
                if (MultiplayerManager.Cups >= mpLevel.Value.Cups)
                {

                    if (!BikeDataManager.PlayerMultiplayerLevels.ContainsKey(mpLevel.Key + 1) ||
                        (BikeDataManager.PlayerMultiplayerLevels.ContainsKey(mpLevel.Key + 1) && BikeDataManager.PlayerMultiplayerLevels[mpLevel.Key + 1].Cups > MultiplayerManager.Cups))
                    {
                        sliderList[mpLevel.Key].SetState(LevelSliderState.Selected);

                        infoText.text = Lang.Get("MP:Levels:CoinsPerWin:");
                        coinText.text = mpLevel.Value.CoinsPerWin.ToString();
                    }
                    else
                    {
                        sliderList[mpLevel.Key].SetState(LevelSliderState.Unlocked);
                    }

                }
                else
                {
                    sliderList[mpLevel.Key].SetState(LevelSliderState.Locked);
                }

                sliderList[mpLevel.Key].SetCups(mpLevel.Value.Cups);
            }
        }
    }

}

}
