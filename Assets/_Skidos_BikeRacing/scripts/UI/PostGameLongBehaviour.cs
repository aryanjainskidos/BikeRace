namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PostGameLongBehaviour : MonoBehaviour
{

    List<CheckpointSliderBehaviour> sliderList;
    Queue<int> animationQueue;

    Text coinText;

    void Awake()
    {
        animationQueue = new Queue<int>();

        sliderList = new List<CheckpointSliderBehaviour>();

        for (int i = 1; i < 11; i++)
        {
            sliderList.Add(transform.Find("SliderPanel/CheckpointSlider" + i).GetComponent<CheckpointSliderBehaviour>());
        }

        coinText = transform.Find("InfoPanel/CoinText").GetComponent<Text>();
        //        
        //        //example
        //        foreach (var item in sliderList) {
        //            if(item.level < 3) {
        //                item.SetState(CheckpointSliderState.Unlocked);
        //            } else { 
        //                if (item.level > 4) {
        //                    item.SetState(CheckpointSliderState.Locked);
        //                } else {
        //                    item.SetState(CheckpointSliderState.Selected);
        //                }
        //            }
        //        }
        //        
    }

    void OnEnable()
    {

        if (Startup.Initialized)
        {

            Actualize();

            animationQueue.Clear();

            for (int i = 0; i < sliderList.Count; i++)
            {
                if (sliderList[i].state == CheckpointSliderState.Selected ||
                    sliderList[i].state == CheckpointSliderState.Unlocked)
                {

                    sliderList[i].Reset();
                    sliderList[i].StopAnimation();

                    if (i < checkpointsReached)
                    {
                        animationQueue.Enqueue(i);
                        //                    sliderList[i].Reset();
                        //                    sliderList[i].StopAnimation();
                    }
                }
            }

            if (animationQueue.Count > 0)
            {
                sliderList[animationQueue.Peek()].PlayAnimation();
            }
        }

    }

    public int bestCheckpoints;
    public int checkpointsReached;

    void Actualize()
    {

        if (BikeGameManager.initialized)
        {

            bestCheckpoints = BikeGameManager.bestCheckpoints;
            checkpointsReached = BikeGameManager.checkpointsReached;

            foreach (var item in sliderList)
            {
                if (item.level <= BikeGameManager.bestCheckpoints)
                {
                    item.SetState(CheckpointSliderState.Unlocked);
                }
                else
                {
                    if (item.level > BikeGameManager.checkpointsReached)
                    {
                        item.SetState(CheckpointSliderState.Locked);
                    }
                    else
                    {
                        item.SetState(CheckpointSliderState.Selected);
                    }
                }
            }


            coinText.text = BikeDataManager.CoinsInLastLevel.ToString();

        }

    }

    void Update()
    {
        if (animationQueue != null && animationQueue.Count > 0 && sliderList[animationQueue.Peek()].animationEnded)
        {
            animationQueue.Dequeue();
            if (animationQueue.Count > 0)
            {
                sliderList[animationQueue.Peek()].PlayAnimation();
            }
        }
    }

}

}
