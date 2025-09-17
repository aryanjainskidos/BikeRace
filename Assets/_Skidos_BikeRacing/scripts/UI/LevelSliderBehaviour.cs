namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum LevelSliderState
{
    Locked,
    Unlocked,
    Selected,
}

public class LevelSliderBehaviour : MonoBehaviour
{

    public int level;

    public LevelSliderState state = LevelSliderState.Locked;

    Color32 lockedTextColor = new Color32(169, 169, 169, 255);
    Color32 lockedBorderColor = new Color32(84, 84, 84, 255);
    Color32 lockedGradientColor = new Color32(57, 57, 57, 255);

    Color32 unlockedTextColor = new Color32(252, 252, 252, 255);
    Color32 unlockedBorderColor = new Color32(182, 204, 224, 255);
    Color32 unlockedGradientColor = new Color32(145, 187, 232, 255);

    Slider slider;

    Text cupText;
    Image cupImage;
    Image cupGrayImage;

    Image highlightImage;
    Image highlightTopImage;

    Image borderImage;
    Image gradientImage;

    Image iconImage;
    Image numberImage;

    bool initialized = false;

    void Awake()
    {

        iconImage = transform.Find("Fill Area/Fill/IconImage").GetComponent<Image>();
        numberImage = transform.Find("Fill Area/Fill/NumberImage").GetComponent<Image>();

        highlightImage = transform.Find("Fill Area/Fill/HighlightImage").GetComponent<Image>();
        highlightTopImage = transform.Find("Fill Area/Fill/HighlightTopImage").GetComponent<Image>();

        cupText = transform.Find("Fill Area/Fill/CupText").GetComponent<Text>();
        cupImage = transform.Find("Fill Area/Fill/CupImage").GetComponent<Image>();
        cupGrayImage = transform.Find("Fill Area/Fill/CupGrayImage").GetComponent<Image>();

        borderImage = transform.Find("Fill Area/Fill/BorderImage").GetComponent<Image>();
        gradientImage = transform.Find("Fill Area/Fill/GradientImage").GetComponent<Image>();

        slider = transform.GetComponent<Slider>();
        ShowHighlight(false);

        initialized = true;

    }

    public void SetLevel(int value)
    {
        bool locked = state == LevelSliderState.Locked;

        iconImage.sprite = UIManager.GetLevelBadgeSprite(locked ? 0 : value);
        numberImage.sprite = UIManager.GetLevelNumberSprite(value, locked);
    }

    public void SetCups(int value)
    {
        cupText.text = value + "+";
    }

    public void SetSliderValue(float normalizedValue)
    {
        slider.value = normalizedValue;
    }

    public void ShowHighlight(bool show)
    {
        highlightImage.enabled = show;
        highlightTopImage.enabled = show;
    }

    public void SetState(LevelSliderState value)
    {

        if (!initialized)
        {
            Awake();
        }

        switch (value)
        {
            case LevelSliderState.Locked:
                Lock();
                ShowHighlight(false);
                break;

            case LevelSliderState.Unlocked:
                Unlock();
                ShowHighlight(false);
                break;

            case LevelSliderState.Selected:
                Unlock();
                ShowHighlight(true);
                break;
            default:
                break;
        }
        state = value;
        SetLevel(level);
    }

    void Lock()
    {

        cupImage.enabled = false;
        cupGrayImage.enabled = true;

        cupText.color = lockedTextColor;
        borderImage.color = lockedBorderColor;
        gradientImage.color = lockedGradientColor;
    }

    void Unlock()
    {

        cupImage.enabled = true;
        cupGrayImage.enabled = false;

        cupText.color = unlockedTextColor;
        borderImage.color = unlockedBorderColor;
        gradientImage.color = unlockedGradientColor;
    }

}

}
