namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ColorPresetToggleBehaviour : MonoBehaviour
{
    Toggle toggle;

    Image imageDark;

    // Use this for initialization
    void Awake()
    {
        toggle = GetComponent<Toggle>();
        imageDark = transform.Find("ColorPreset/ImageDark").GetComponent<Image>();

        toggle.onValueChanged.AddListener(OnValueChanged);
    }

    void OnValueChanged(bool arg0)
    {
        imageDark.enabled = !arg0;
    }
}

}
