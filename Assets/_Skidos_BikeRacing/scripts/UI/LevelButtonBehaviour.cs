namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum LevelButtonState
{
    Unlocked, //can play
    Locked, //can not play - must pay to unlock    
}

public enum LevelButtonType
{
    Regular,
    Bonus,
    Long,
}

public class LevelButtonBehaviour : MonoBehaviour
{

    public string levelName = "";
    public float flagAngle = 0;
    public LevelButtonType type;
    public LevelButtonState state;

    //UIButtonLevelCommand buttonLevelCommand;

    Image lockedImage;
    Image unlockedImage;
    Image regularImage;
    Image bonusImage;
    Image longImage;

    Text numberText;

    Image star1Image;
    Image star2Image;
    Image star3Image;

    Image greyStar1Image;
    Image greyStar2Image;
    Image greyStar3Image;

    void Awake()
    {
        //buttonLevelCommand = GetComponent<UIButtonLevelCommand>();

        //some children are disabed, so enable them to be able to set the variables
        foreach (Transform item in transform)
        {
            if (item.name == "Locked" || item.name == "Unlocked" || item.name == "BUnlocked" || item.name == "BLocked" || item.name.Contains("Star"))
            {
                item.gameObject.SetActive(true);
            }
        }

        lockedImage = transform.Find("Locked").GetComponent<Image>();
        regularImage = transform.Find("Unlocked").GetComponent<Image>();
        bonusImage = transform.Find("BUnlocked").GetComponent<Image>();
        longImage = transform.Find("BLocked").GetComponent<Image>();

        numberText = transform.Find("Text").GetComponent<Text>();//.gameObject.SetActive(looksUnlocked);

        star1Image = transform.Find("Star1").GetComponent<Image>();
        star2Image = transform.Find("Star2").GetComponent<Image>();
        star3Image = transform.Find("Star3").GetComponent<Image>();
        greyStar1Image = transform.Find("GreyStar1").GetComponent<Image>();
        greyStar2Image = transform.Find("GreyStar2").GetComponent<Image>();
        greyStar3Image = transform.Find("GreyStar3").GetComponent<Image>();

        state = LevelButtonState.Locked;

        SetTypeByName();
    }

    public void SetTypeByName()
    {
        if (levelName.ToLower().Contains("bonuss"))
        {
            type = LevelButtonType.Bonus;
        }
        else if (levelName.ToLower().Contains("long"))
        {
            type = LevelButtonType.Long;
        }
        else
        {
            type = LevelButtonType.Regular;
        }

        switch (type)
        {
            case LevelButtonType.Regular:
                unlockedImage = regularImage;
                bonusImage.enabled = false;
                longImage.enabled = false;
                break;
            case LevelButtonType.Bonus:
                unlockedImage = bonusImage;
                regularImage.enabled = false;
                longImage.enabled = false;
                break;
            case LevelButtonType.Long:
                unlockedImage = longImage;
                regularImage.enabled = false;
                bonusImage.enabled = false;

                star1Image.enabled = false;
                star2Image.enabled = false;
                star3Image.enabled = false;

                greyStar1Image.enabled = false;
                greyStar2Image.enabled = false;
                greyStar3Image.enabled = false;

                break;
            default:
                break;
        }
    }

    public void SetState(LevelButtonState newState)
    {
        try
        {
            state = newState;
            switch (state)
            {
                case LevelButtonState.Locked:
                    lockedImage.enabled = true;
                    unlockedImage.enabled = false;
                    numberText.enabled = false;
                    //TODO set script states
                    break;
                case LevelButtonState.Unlocked:
                    lockedImage.enabled = false;
                    unlockedImage.enabled = true;
                    numberText.enabled = true;
                    //TODO set script states
                    break;
                default:
                    break;
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Exception in SetState: " + ex);
        }


    }

    public void SetStars(int stars, bool tried)
    {
        try
        {
            if (type != LevelButtonType.Long)
            {
                star1Image.enabled = (stars > 0) ? true : false; // "? true : false" is for readability
                greyStar1Image.enabled = !star1Image.enabled;

                star2Image.enabled = (stars > 1) ? true : false;
                greyStar2Image.enabled = !star2Image.enabled;

                star3Image.enabled = (stars > 2) ? true : false;
                greyStar3Image.enabled = !star3Image.enabled;

                if (!tried)
                { //hide grey stars on locked levels  || also on unvisited levels
                    greyStar1Image.enabled = false;
                    greyStar2Image.enabled = false;
                    greyStar3Image.enabled = false;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Exception in SetStars:" + ex);
        }

    }

}

}
