namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TeamSliderBehaviour : MonoBehaviour
{

    Color32 positiveChangeColor = new Color32(252, 252, 252, 255);
    Color32 negativeChangeColor = new Color32(130, 130, 130, 255);

    Image glowImage;
    Text pointText;
    Text deltaText;
    Slider slider;

    void Awake()
    {
        pointText = transform.Find("PointText").GetComponent<Text>();
        deltaText = transform.Find("Fill Area/Fill/DeltaText").GetComponent<Text>();
        glowImage = transform.Find("Fill Area/Fill/GlowImage").GetComponent<Image>();
        slider = transform.GetComponent<Slider>();
        ShowGlow(false);
    }

    public void SetPoints(int points)
    {
        pointText.text = points.ToString();
    }

    /**
	 * recent changes - number next to bar
	 */
    public void SetDelta(int delta)
    {
        if (delta == 0)
        {
            deltaText.enabled = false;
        }
        else
        {
            deltaText.enabled = true;
            if (delta > 0)
            {
                deltaText.text = "+" + delta;
                deltaText.color = positiveChangeColor;
            }
            else
            {
                deltaText.text = delta.ToString();
                deltaText.color = negativeChangeColor;
            }
        }
    }

    public void SetSliderValue(float normalizedValue)
    {
        slider.value = normalizedValue;
    }

    public void ShowGlow(bool show)
    {
        glowImage.enabled = show;
    }

}

}
