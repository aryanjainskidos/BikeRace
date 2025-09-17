namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ColorReceiverUI : MonoBehaviour
{

    public string group = ""; //-1 - no group
    Image image;
    public Color32 color;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void ChangeColor(Color32 color)
    {
        this.color = color;
        if (image != null)
            image.color = color;
    }
}

}
