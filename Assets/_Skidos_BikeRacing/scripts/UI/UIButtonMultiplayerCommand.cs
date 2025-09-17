namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIButtonMultiplayerCommand : MonoBehaviour, IPointerClickHandler
{

    public enum MPCommands
    {
        FBLogin,
        FBInvite,
    }

    public MPCommands MPCommand;

    void OnClick()
    {

        switch (MPCommand)
        {

            case MPCommands.FBLogin:
                MultiplayerManager.FBConnect();
                break;

            case MPCommands.FBInvite:
                MultiplayerManager.ButtonFBInvite();
                break;

            default:

                break;

        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {

        //Debug.Log("Pointer Click");
        OnClick();

    }

}

}
