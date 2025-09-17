namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StyleEntryBehaviour : MonoBehaviour
{

    bool initialized = false;
    bool locked;
    int styleID;

    public int StyleID
    {
        get
        {
            return styleID;
        }
        set
        {
            styleID = value;
            Actualize();
        }
    }

    Image iconImage;
    Image lockImage;
    public UIClickIndexDelegate clickIndexDelegate;

    // Use this for initialization
    void Awake()
    {
        iconImage = transform.Find("Image").GetComponent<Image>();
        lockImage = transform.Find("Image/Lock").GetComponent<Image>();
        clickIndexDelegate = transform.GetComponent<UIClickIndexDelegate>();
    }

    // Update is called once per frame
    void Actualize()
    {
        clickIndexDelegate.index = styleID;
        SetLockState(BikeDataManager.Styles[styleID].Locked);

        if (BikeDataManager.Styles[styleID].Icon != null)
            iconImage.sprite = BikeDataManager.Styles[styleID].Icon;

        initialized = true;
    }

    void Update()
    {
        if (initialized && locked != BikeDataManager.Styles[styleID].Locked)
        {
            UpdateLockState();
        }
    }

    public void SetLockState(bool locked)
    {
        this.locked = locked;
        lockImage.enabled = locked;
    }

    public void UpdateLockState()
    {
        SetLockState(BikeDataManager.Styles[styleID].Locked);
    }

}

}
