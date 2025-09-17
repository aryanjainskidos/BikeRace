namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerButtonBehaviour : MonoBehaviour
{

    public bool visible = true;
    Button button;

    Image image;

    void Awake()
    {
        button = GetComponent<Button>();

        image = GetComponent<Image>();
    }

    void SetVisibility(bool value)
    {
        visible = value;
        image.enabled = value;
        button.enabled = value;

        Utils.ShowChildrenGraphics(gameObject, value);
    }

    void OnEnable()
    {
        if (BikeDataManager.PowerBoostEnabled || MultiplayerManager.PermanentPowerRating == 500 || MultiplayerManager.NumGames < 2)
        {//TODO would be nice to unhardcode
            SetVisibility(false);
        }
        else
        {
            SetVisibility(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (visible == BikeDataManager.PowerBoostEnabled)
        {
            OnEnable();
        }
    }
}

}
