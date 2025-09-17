namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MenuLeaguePanelBehaviour : MonoBehaviour
{

    Image teamImage;
    //    Text teamRankText;
    Text yourTributeText;

    Text seasonNumberText;
    Text timeText;

    Text coinText;
    Text cupText;
    Text itemText;

    List<TeamSliderBehaviour> teamSliders;

    int lastDataID = 0; //te pieraksta MultiplayerManager.DataID nuuru, kad uzzímé datus ekráná - lai zin zímét atkal, kad menedźerí pamainíjies skaitlis


    void Awake()
    {
        teamImage = transform.Find("SecondaryInfoPanel/TeamImage").GetComponent<Image>();
        yourTributeText = transform.Find("SecondaryInfoPanel/YourTributeText").GetComponent<Text>();

        seasonNumberText = transform.Find("MainInfoPanel/NumberText").GetComponent<Text>();
        timeText = transform.Find("MainInfoPanel/TimeText").GetComponent<Text>();
        coinText = transform.Find("MainInfoPanel/CoinText").GetComponent<Text>();
        cupText = transform.Find("MainInfoPanel/CupText").GetComponent<Text>();

        teamSliders = new List<TeamSliderBehaviour>();
        teamSliders.Add(null);//nulltá komanda - "NAV_KOMANDAS" - to te neráda
        teamSliders.Add(transform.Find("RedSlider").GetComponent<TeamSliderBehaviour>());
        teamSliders.Add(transform.Find("GreenSlider").GetComponent<TeamSliderBehaviour>());
        teamSliders.Add(transform.Find("BlueSlider").GetComponent<TeamSliderBehaviour>());
        teamSliders.Add(transform.Find("PurpleSlider").GetComponent<TeamSliderBehaviour>());

    }

    void OnEnable()
    {
        StartCoroutine(SeasonCountdown());
    }
    void OnDisable()
    {
        StopCoroutine(SeasonCountdown());
    }


    void Update()
    {



        if (Startup.Initialized && MultiplayerManager.DataID != lastDataID)
        {
            print("MultiplayerMenuBehaviour::atjaunojam info");
            lastDataID = MultiplayerManager.DataID;


            //------------------------------komandu punktu stabinji--------------------------
            //jánormalizé punkti attélośani grafikos, 0f - 1f
            float min = 0;
            float max = 0;

            for (int i = 1; i <= 4; i++)
            {
                if (MultiplayerManager.TeamCups[i] < min)
                {
                    min = MultiplayerManager.TeamCups[i];
                }
                if (MultiplayerManager.TeamCups[i] > max)
                {
                    max = MultiplayerManager.TeamCups[i];
                }
            }

            max += -min; //visus pastums uz augśu, lai zemákais bútu nulle nevis mínusos
                         //konformé stabińus - mazákais bús 10%, lielákais 100% (nevis 0% un 100%)
            min -= max * 0.1f;
            max *= 1.1f;

            if (max == 0)
            {
                max = 1;
            }


            for (int i = 1; i <= 4; i++)
            {
                teamSliders[i].SetPoints(MultiplayerManager.TeamCups[i]);
                teamSliders[i].SetSliderValue((MultiplayerManager.TeamCups[i] - min) / max);
                teamSliders[i].SetDelta(0);
                teamSliders[i].ShowGlow(false);
            }
            if (MultiplayerManager.PlayerTeamID > 0)
            {
                teamSliders[MultiplayerManager.PlayerTeamID].ShowGlow(true);
            }

            //----------------------------SecondaryInfoPanel: mana komanda--------------------------


            if (MultiplayerManager.PlayerTeamID > 0)
            {
                teamImage.sprite = LevelManager.GetSprite("visuals/Sprites/GUI_sprites/MP/MultiplayerTeams", "TeamIco" + MultiplayerManager.PlayerTeamID);
                yourTributeText.text = Lang.Get("MP:YourTribute:") + " " + MultiplayerManager.PlayerLeagueContributionCups;
            }
            else
            {
                yourTributeText.text = Lang.Get("MP:YourTribute:") + " -";
            }


            //-------------------------------MainInfoPanel------------------------------------------
            seasonNumberText.text = "#" + MultiplayerManager.SeasonID;

            coinText.text = "";
            cupText.text = "";
            if (MultiplayerManager.SeasonPrizes != null)
            {
                if (MultiplayerManager.SeasonPrizes["coins"].AsInt > 0)
                {
                    coinText.text = "+" + MultiplayerManager.SeasonPrizes["coins"];
                }

                if (MultiplayerManager.SeasonPrizes["cups"].AsInt > 0)
                {
                    cupText.text = "+" + MultiplayerManager.SeasonPrizes["cups"];
                }
            }


        }
    }



    IEnumerator SeasonCountdown()
    {
        while (true)
        {

            System.TimeSpan t = new System.TimeSpan();
            if (MultiplayerManager.SeasonTTL != 0)
            { // ja nav sanemta info no servera, cikos beidzas sezona, tead neko nerádít
                t = MultiplayerManager.SeasonEndDate - System.DateTime.Now;
                if (t.Ticks < 0)
                { //sezona ir beigusies
                    t = new System.TimeSpan(); //noresetoju, lai nerádítu negatívu laiku, bet nulli
                    if (MultiplayerManager.SeasonTTL != 0)
                    {// lai tikai 1x veiktu pieprasíjumu serverim							
                        print("Forced login - league season just ended");
                        MultiplayerManager.MPConnect(); //veic loginu MP serverí un iegúst aktuáláko info par sezonu;	
                    }
                    MultiplayerManager.SeasonTTL = 0;
                }
            }

            timeText.text = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", t.Days, t.Hours, t.Minutes, t.Seconds);
            yield return new WaitForSeconds(0.33f);

        }

    }


}
}
