namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelLongButtonBehaviour : MonoBehaviour
{

    public string levelName = "";
    public float flagAngle = 0;
    public LevelButtonType type;
    public LevelButtonState state;

    Image lockedImage;
    Image unlockedImage;
    Image poleLockedImage;
    Image poleUnlockedImage;
    Image panelImage;

    Text numberText;


    void Awake()
    {
        //some children are disabed, so enable them to be able to set the variables
        foreach (Transform item in transform)
        {
            if (item.name.Contains("Locked") || item.name == "Unlocked" || item.name == "Panel")
            {
                item.gameObject.SetActive(true);
            }
        }

        lockedImage = transform.Find("Locked").GetComponent<Image>();
        unlockedImage = transform.Find("Unlocked").GetComponent<Image>();
        poleLockedImage = transform.Find("PoleLocked").GetComponent<Image>();
        poleUnlockedImage = transform.Find("PoleUnlocked").GetComponent<Image>();
        panelImage = transform.Find("Panel").GetComponent<Image>();

        numberText = transform.Find("Panel/Text").GetComponent<Text>();

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
    }

    public void SetState(LevelButtonState newState)
    {
        state = newState;
        switch (state)
        {
            case LevelButtonState.Locked:
                lockedImage.enabled = true;
                unlockedImage.enabled = false;
                poleLockedImage.enabled = true;
                poleUnlockedImage.enabled = false;
                panelImage.gameObject.SetActive(false);
                //                numberText.enabled = false;
                //TODO set script states
                break;
            case LevelButtonState.Unlocked:
                lockedImage.enabled = false;
                unlockedImage.enabled = true;
                poleLockedImage.enabled = false;
                poleUnlockedImage.enabled = true;
                panelImage.gameObject.SetActive(true);
                //                numberText.enabled = true;
                //TODO set script states
                break;
            default:
                break;
        }
    }

    public void SetCheckpoints(int checkpoints)
    {
        numberText.text = checkpoints + "/10";
    }

}


}
