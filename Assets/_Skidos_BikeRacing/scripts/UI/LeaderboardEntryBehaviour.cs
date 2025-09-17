namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LeaderboardEntryBehaviour : MonoBehaviour
{

    string PictureUrl = "";// URL
    string FBID = "";// for cache naming purposes only

    Image highlightImage;
    Text positionText;
    Image pictureImage;
    Text nameText;
    Image teamIconImage;
    Text rankText;
    Text cupText;

    void Awake()
    {
        highlightImage = transform.Find("Highlight").GetComponent<Image>();
        pictureImage = transform.Find("PicturePanel/Picture").GetComponent<Image>();
        teamIconImage = transform.Find("TeamIcon").GetComponent<Image>();
        positionText = transform.Find("PositionText").GetComponent<Text>();
        rankText = transform.Find("RankText").GetComponent<Text>();
        nameText = transform.Find("NameText").GetComponent<Text>();
        cupText = transform.Find("CupText").GetComponent<Text>();
    }

    public void ShowHighlight(bool show)
    {
        highlightImage.enabled = show;
    }

    public void SetPosition(int position)
    {
        positionText.text = position.ToString();
    }

    public void SetRank(int rank)
    {
        rankText.text = rank.ToString();
    }

    public void SetCups(int cups)
    {
        cupText.text = cups.ToString();
    }

    public void SetName(string name)
    {
        nameText.text = name;
    }

    public void SetPicture(string url, string fbid)
    {
        PictureUrl = url;
        FBID = fbid;

        if (PictureUrl != null && PictureUrl.Length > 0)
        {
            //print("get pic for " + nameText.text + " (" + PictureUrl +")" + " (" + FBID+")");
            MultiplayerManager.GetPicture(PictureUrl, FBID, pictureImage);
        }
    }



    public void SetTeam(int teamID)
    {

        if (teamID > 0)
        {
            teamIconImage.sprite = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/MP/MultiplayerTeams", "TeamIco" + teamID);
        }
        else
        {
            teamIconImage.gameObject.SetActive(false);
        }

    }

}



}
