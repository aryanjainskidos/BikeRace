namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class UIButtonSwitchDelegate : MonoBehaviour
{

    public string nameParam = "";
    public delegate void ButtonDelegate(string nameParam);
    public ButtonDelegate buttonDelegate;

    // Use this for initialization
    void OnClick()
    {

        if (buttonDelegate != null)
            buttonDelegate(nameParam);
    }

}

}
