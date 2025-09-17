namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MultiplayerInfoPanelBubblesBehaviour : MonoBehaviour
{


    Text playerNameText;
    Image playerPicture;
    Image playerTeamIcon;
    Text playerPowerRatingText;

    Transform opponentPanel;

    Text opponentNameText;
    Image opponentPicture;
    Image opponentTeamIcon;
    Text opponentPowerRatingText;




    GameObject vsText;

    void Awake()
    {


        playerNameText = transform.Find("InfoPanelBubblePlayer/NameText").GetComponent<Text>();
        playerPicture = transform.Find("InfoPanelBubblePlayer/Picture").GetComponent<Image>();
        playerTeamIcon = transform.Find("InfoPanelBubblePlayer/TeamIcon").GetComponent<Image>();
        playerPowerRatingText = transform.Find("InfoPanelBubblePlayer/TimeText").GetComponent<Text>();

        opponentPanel = transform.Find("InfoPanelBubbleOpponent");

        opponentNameText = opponentPanel.Find("NameText").GetComponent<Text>();
        opponentPicture = opponentPanel.Find("Picture").GetComponent<Image>();
        opponentTeamIcon = opponentPanel.Find("TeamIcon").GetComponent<Image>();
        opponentPowerRatingText = opponentPanel.Find("TimeText").GetComponent<Text>();


        vsText = transform.Find("VsText").gameObject;

        //MultiplayerManager.GetPicture(MultiplayerManager.CurrentOpponent.Picture, MultiplayerManager.CurrentOpponent.FBID, opponentPicture);

    }

    void OnEnable()
    {

        if (Startup.Initialized)
        {

            playerNameText.text = MultiplayerManager.PlayerName;
            MultiplayerManager.GetPicture(MultiplayerManager.PlayerPicture, MultiplayerManager.FBID, playerPicture);
            playerPowerRatingText.text = MultiplayerManager.PowerRating.ToString();
            if (MultiplayerManager.PlayerTeamID > 0)
            {
                playerTeamIcon.sprite = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/MP/MultiplayerTeams", "TeamIco" + MultiplayerManager.PlayerTeamID);
                playerTeamIcon.gameObject.SetActive(true);
            }
            else
            {
                playerTeamIcon.gameObject.SetActive(false);
            }

            if (MultiplayerManager.CurrentOpponent.MPType == MPTypes.first)
            {
                opponentPanel.gameObject.SetActive(false); //iebraucot  MP braucienu [braucot vienatné] neráda pretinieka pláksníti
                vsText.SetActive(false);

            }
            else
            {
                opponentPanel.gameObject.SetActive(true);
                vsText.SetActive(true);

                opponentNameText.text = MultiplayerManager.CurrentOpponent.Name;
                MultiplayerManager.GetPicture(MultiplayerManager.CurrentOpponent.Picture, MultiplayerManager.CurrentOpponent.FBID, opponentPicture);
                opponentPowerRatingText.text = MultiplayerManager.CurrentOpponent.PowerRating.ToString();
                if (MultiplayerManager.CurrentOpponent.TeamID > 0)
                {
                    opponentTeamIcon.sprite = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/MP/MultiplayerTeams", "TeamIco" + MultiplayerManager.CurrentOpponent.TeamID);
                    opponentTeamIcon.gameObject.SetActive(true);
                }
                else
                {
                    opponentTeamIcon.gameObject.SetActive(false);
                }
            }
        }
    }


}

}
