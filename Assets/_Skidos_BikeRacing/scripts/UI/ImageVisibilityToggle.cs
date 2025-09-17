namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageVisibilityToggle : MonoBehaviour
{

    Image image;
    // Use this for initialization
    void Awake()
    {
        image = transform.GetComponent<Image>();
    }

    public void Hide(bool hide)
    {
        image.enabled = !hide;
    }

    public void Show(bool show)
    {
        image.enabled = show;
    }
}

}
