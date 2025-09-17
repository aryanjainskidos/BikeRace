namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIButtonSimpleDelegate : MonoBehaviour
{

    public delegate void SimpleButtonDelegate();
    public SimpleButtonDelegate buttonDelegate;

    //automatically add OnClick to unity ui button listeners
    void Awake()
    {
        Button btn = transform.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() => OnClick());
        }
    }

    void OnClick()
    {

        if (buttonDelegate != null)
            buttonDelegate();
    }

}
}
