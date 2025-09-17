namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum LevelButtonMode
{
    play = 0, //can play
    locked = 1, //can not play - must pay to unlock
    lockedWorld = 2, //can not play - must get X stars to unlock or pay to unlock
    lockedShare = 3, //can not play - must share to unlock
    popupLong = 4, //can play, but first - look at this popup (for LONG only)

}

public class UIButtonLevelCommand : MonoBehaviour
{

    public string levelName;
    public LevelButtonMode mode;
    public int stars = 0; //for popup info


    //automatically add OnClick to unity ui button listeners
    void Awake()
    {
        Button btn = transform.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() => OnClick());
        }
    }


    public void OnClick()
    {

        if (enabled)
        {

            switch (mode)
            {
                case LevelButtonMode.play:
                    Play(levelName);
                    break;

                case LevelButtonMode.locked:
                    BikeGameManager.SelectedLevelName = "";
                    UIManager.ToggleScreen(GameScreenType.PopupUnlockLevels);
                    //print("show popup: play prev levels || pay to unlock");
                    break;

                case LevelButtonMode.lockedWorld://TODO this is also handled by world lock panel
                                                 //				GameManager.SelectedLevel = stars.ToString(); //popup will be using this info, -no it won't
                    BikeGameManager.SelectedLevelName = levelName;
                    //@todo -- when MP wins wil be necessary to unock a level - pass them too
                    UIManager.ToggleScreen(GameScreenType.PopupUnlockLevels);
                    //print("show popup: get more stars || pay to unlock");
                    break;

                case LevelButtonMode.lockedShare:

                    //if(BikeDataManager.Levels[levelName].Shared) // if just now shared/tweeted and levelscreen has not been refreshed
                    //{
                    //	Play(levelName);
                    //}
                    //else
                    //{					
                    //	BikeGameManager.SelectedLevelName = levelName; //pass info to gamecommand (klick on share button in popup will trigger this gamecommand)
                    // UIManager.ToggleScreen(GameScreenType.PopupUnlockBonusLevels);
                    BikeDataManager.Levels[levelName].Shared = true;
                    Play(levelName);


                    //SkidosRewardSystem.RewardEvent -= new SkidosRewardSystem.RewardHandler(AnsweredForBonus);
                    //	SkidosRewardSystem.RewardEvent += new SkidosRewardSystem.RewardHandler(AnsweredForBonus);
                    //	SkidosRewardSystem.INSTANCE.StartCompulsoryQuestions("compulsory",1,500);// StartWithSingleQuestions("BonusLevel", 500, 2, 0, false, false);
                    //SkidosRewardSystem.RewardEvent += new SkidosRewardSystem.RewardHandler(AnsweredForBonus);
                    //}
                    break;

                case LevelButtonMode.popupLong:
                    BikeGameManager.SelectedLevelName = levelName; //pass info to gamecommand
                    UIManager.ToggleScreen(GameScreenType.PopupLongLevel);
                    break;
            }


        }

    }

    private void AnsweredForBonus(string rewardID, float rewardAmount)
    {
        Debug.Log("pievieno naudu tagad + " + rewardAmount);
        if (rewardID == "compulsory")
        {
            BikeDataManager.Levels[levelName].Shared = true;
            Play(levelName);
        }
        //SkidosRewardSystem.RewardEvent -= new SkidosRewardSystem.RewardHandler(AnsweredForBonus);
    }


    //static - because also used in GameManager
    void Play(string name)
    {
        UIManager.SwitchScreen(GameScreenType.PreGame); //TODO there's a separate script for this
        LevelManager.LoadLevel("", name);
        SoundManager.Play("Play"); //play sound, because LevelButton is a prefab - i can't use its OnClick to do it :\
    }
}

}
