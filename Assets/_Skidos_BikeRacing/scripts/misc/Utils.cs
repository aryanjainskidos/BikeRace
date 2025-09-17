namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Utils
{

    public static void ShowChildrenGraphics(GameObject uiObject, bool show)
    {
        Graphic[] graphics = uiObject.GetComponentsInChildren<Graphic>();
        for (int i = 0; i < graphics.Length; i++)
        {
            graphics[i].enabled = show;
        }
    }

    public static void EnableChildrenWithGraphics(GameObject uiObject, bool enable)
    {
        Graphic[] graphics = uiObject.GetComponentsInChildren<Graphic>();
        for (int i = 0; i < graphics.Length; i++)
        {
            graphics[i].gameObject.SetActive(enable);
        }
    }
}

}
