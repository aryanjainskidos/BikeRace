namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MultiplayerLeagueWonBehaviour : MonoBehaviour
{

    Image iconImage;

    Text coinText;
    Text cupText;

    void Awake()
    {

        iconImage = transform.Find("IconImage").GetComponent<Image>();

        coinText = transform.Find("InfoPanel/CoinText").GetComponent<Text>();
        coinText.text = "";

        cupText = transform.Find("InfoPanel/CupText").GetComponent<Text>();
        cupText.text = "";

    }



    public void SetTeam(int value)
    {
        iconImage.sprite = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/MP/MultiplayerTeams", "TeamIco" + value);
    }

    public void SetCoins(int value)
    {
        coinText.text = "+" + value;
    }

    public void SetCups(int value)
    {
        cupText.text = "+" + value;
    }

}

}
