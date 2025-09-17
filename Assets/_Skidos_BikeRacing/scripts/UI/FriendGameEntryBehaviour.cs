namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FriendGameEntryBehaviour : MonoBehaviour
{

    MPOpponent opponent;

    public bool AutoDownload = true; //vai automátiski lejupieládét visu info (lígas spélém śo izslédz, lai nelejupieládétu kandidátu datus, kas nebús vajadzígi 66% gadíjumu)
    public bool AutoPlay = false; //lieto lígas braucieniem - ja true, tad sáks spéli tiklídz bús lejupieládéti visi dati
    public int DownloadsInProgress = 0;

    Button playButton;
    Button pokeButton;
    Button removeButton;

    Image pictureImage;
    Image teamIconImage;
    Text rankText;
    Text nameText;
    Text scoreText;
    Text messageText;

    Color32 positiveColor = new Color32(0, 220, 7, 255);
    Color32 negativeColor = new Color32(229, 64, 48, 255);

    // Use this for initialization
    void Awake()
    {
        playButton = transform.Find("PlayButton").GetComponent<Button>();
        pokeButton = transform.Find("PokeButton").GetComponent<Button>();
        removeButton = transform.Find("RemoveButton").GetComponent<Button>(); //@todo -- izveidot pogu

        playButton.GetComponent<UIButtonSimpleDelegate>().buttonDelegate = OnPlayClick;
        pokeButton.GetComponent<UIButtonSimpleDelegate>().buttonDelegate = OnPokeClick;
        removeButton.GetComponent<UIButtonSimpleDelegate>().buttonDelegate = OnRemoveClick; //@todo -- ieslégt


        pictureImage = transform.Find("PicturePanel/Picture").GetComponent<Image>();
        teamIconImage = transform.Find("TeamIcon").GetComponent<Image>();
        rankText = transform.Find("RankText").GetComponent<Text>();
        nameText = transform.Find("NameText").GetComponent<Text>();
        scoreText = transform.Find("ScoreText").GetComponent<Text>();
        messageText = transform.Find("MessageText").GetComponent<Text>();
    }

    public void SetData(MPOpponent thisGuy)
    {
        opponent = thisGuy;
        opponent.UIEntry = this;

        rankText.text = opponent.PowerRating.ToString(); //power rating, nevis rank !
        nameText.text = opponent.Name;
        messageText.text = opponent.Message;


        int score = opponent.Balance;
        if (score >= 0)
        {
            scoreText.text = (score > 0 ? "+" : "") + score;
            scoreText.color = positiveColor;
        }
        else
        {
            scoreText.text = score.ToString();
            scoreText.color = negativeColor;
        }
        if (opponent.TeamID > 0)
        {
            teamIconImage.sprite = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/MP/MultiplayerTeams", "TeamIco" + opponent.TeamID);
        }
        else
        {
            teamIconImage.gameObject.SetActive(false);
        }


        if (opponent.Waiting)
        {// 2poke||!2poke
            pokeButton.gameObject.SetActive(true);
        }
        else
        {
            pokeButton.gameObject.SetActive(false);
        }
        //print("opponent.Waiting = " + opponent.Waiting);

        DownloadsInProgress++; //@haxx -- sákot lejupieládes aizváks śo lieko inkrementu - bet tas palídz nesákt AUTOPLAY, jo bél nav notikusi lejupieláde
        if (AutoDownload)
        {
            DownloadData();
        }

        EnablPlayIfEverythingIsGood();
    }

    void OnPlayClick()
    {
        //print("OnPlayClick");
        opponent.Play();
    }

    void OnPokeClick()
    {
        //print("OnPPokeClick");
        opponent.Poke();
        pokeButton.gameObject.SetActive(false);
    }

    void OnRemoveClick()
    {
        //print("OnRemoveClick");
        Destroy(gameObject);
        opponent.Remove();
    }

    void Update()
    {
        if (opponent != null && opponent.Done && AutoPlay)
        {
            //print("Auto PLay ++");
            AutoPlay = false;
            UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, false); //aizveru loading ekránu
            opponent.Play();
        }
        else
        {
            //print("Auto PLay NOT YET");
        }

    }

    public void DownloadData()
    {
        //Debug.Log("lol, lejupielaadeejam");
        DownloadsInProgress--;
        MultiplayerManager.GetPicture(opponent.Picture, opponent.FBID, pictureImage); //asinhroni ieládés júzerpikchu
        MultiplayerManager.GetRide(this, opponent.Ride); //káds [vai visi]  no śiem raidiem var nebút definéti
        MultiplayerManager.GetRide(this, opponent.ReplayMyRide);
        MultiplayerManager.GetRide(this, opponent.ReplayOppRide);

    }

    public void EnablPlayIfEverythingIsGood()
    {

        if (DownloadsInProgress > 0)
        {
            return; //nav vél beiguśás visas iesáktás lejupieládes
        }

        opponent.Done = true;

        if (!opponent.Waiting)
        {
            playButton.gameObject.SetActive(true);
        }
        else
        {
            playButton.gameObject.SetActive(false);
        }

    }


}

}
