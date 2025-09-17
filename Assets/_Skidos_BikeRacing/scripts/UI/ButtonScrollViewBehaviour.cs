namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonScrollViewBehaviour : MonoBehaviour
{

    LayoutElement wheelButtonLayoutElement;
    // LayoutElement shareButtonLayoutElement;
    // LayoutElement videoButtonLayoutElement;
    // LayoutElement inviteButtonLayoutElement;

    WheelButtonBehaviour wheelButton;
    // ShareButtonBehaviour shareButton;
    // RewardedVideoBehaviour videoButton;
    // PushyInviteButtonBehaviour inviteButton;

    // Use this for initialization
    void Awake()
    {
        wheelButtonLayoutElement = transform.Find("Content/WheelButtonContainer").GetComponent<LayoutElement>();
        // shareButtonLayoutElement = transform.Find("Content/ShareButtonContainer").GetComponent<LayoutElement>();
        // videoButtonLayoutElement = transform.Find("Content/VideoButtonContainer").GetComponent<LayoutElement>();
        // inviteButtonLayoutElement = transform.Find("Content/InviteButtonContainer").GetComponent<LayoutElement>();

        wheelButton = transform.Find("Content/WheelButtonContainer/WheelButton").GetComponent<WheelButtonBehaviour>();
        // shareButton = transform.Find("Content/ShareButtonContainer/ShareButton").GetComponent<ShareButtonBehaviour>();
        // videoButton = transform.Find("Content/VideoButtonContainer/RewardedVideoButton").GetComponent<RewardedVideoBehaviour>();
        // inviteButton = transform.Find("Content/InviteButtonContainer/InviteButton").GetComponent<PushyInviteButtonBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        if (wheelButton.visible == wheelButtonLayoutElement.ignoreLayout)
        {
            wheelButtonLayoutElement.ignoreLayout = !wheelButton.visible;
        }
        // if (shareButton.visible == shareButtonLayoutElement.ignoreLayout) {
        //     shareButtonLayoutElement.ignoreLayout = !shareButton.visible;
        // }
        // if (videoButton.visible == videoButtonLayoutElement.ignoreLayout) {
        //     videoButtonLayoutElement.ignoreLayout = !videoButton.visible;
        // }
        // if (inviteButton.visible == inviteButtonLayoutElement.ignoreLayout) {
        //     inviteButtonLayoutElement.ignoreLayout = !inviteButton.visible;
        // }
    }

    public void ShowExtraButtons(bool b)
    {
        //        throw new System.NotImplementedException();
        // shareButtonLayoutElement.gameObject.SetActive(b);
        // videoButtonLayoutElement.gameObject.SetActive(b);
        // inviteButtonLayoutElement.gameObject.SetActive(b);
    }
}

}
