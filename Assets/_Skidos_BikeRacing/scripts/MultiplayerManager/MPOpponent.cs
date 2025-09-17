namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MPTypes
{
    //śádá secíbá notiek braucieni !
    league, //līgas spéle - pret nejauśu pretinieku, vienmér revanśs
    replay, //skatás repleju
    revanche, //atbild uz braucienu
    first,//brauc pirmais
}


/**
 * multipleijera pretinieku attélojośa datu struktúra
 */
public class MPOpponent
{

    public string FBID;
    public string Name;
    public string Picture; //pilns URLis
    public string Message; //ko śis pretinieks ir teicis spélétájam
    public bool Waiting; //ja true, tad Poke | vai spélétájs gaida uz pretienieku (pretinieka gájiens)
    public string NextTrack; //límeńa nosaukums | ko braukt péc revanśa  / vienatné / pirmais
    public int CupsNextTrackWin = 0; //cik kausińus dabús vienatné izbraucot un uzvarot, 		tikai informatíva nozíme (patiesá uzvaréśana notiks, kad bús izbraucis arí pretinieks)
    public int CupsNextTrackLose = 0; //   --------"------"--------            zaudéjot
    public int Balance = 0; //mani kausińi +/- pret śo pretinieku
    public int Cups = 0;
    public int CupsAfterRevanche = 0; //péc revanśa tiek zaudéti/iegúti kausińi, śí lokáli aprékjinátá starpíba ir jápieskaita iebraukśanas finiśa ekrána - citádi tur bús redami kausińi bez revanśá iegútajiem/zaudétajiem (śis ir aktuáli tikai pretieniekam, spélétája kausińi tiks apdeitoti no servera pirms nákamá finiśa ekrána) | @nelietojam
    public int TeamID = 0;
    public int LastPlayedAt = 0; //taimstamps | tikai serveris maina śo
    public int LastPokedAt = 0; //taimstamps | tikai serveris maina śo
    public int LeagueChallengeId = 0; //kurś `mp_ride_pool` ieraksts ir līgas pretinieks 

    public float Time; //pretinieka laiks
    public int[] Upgrades = { 0, 0, 0, 0, 0 }; //pretinieka apgriedi (tikai 4 svarígie MP apgreidi)
    public string Ride = ""; //faila nosaukums | pretinieka ghostińś
    public string Track; //límeńa nosaukums | ko braukt revanśá  / pret ghostińu
    public int CupsTrackWin = 0;//cik kausińus nopelnís/zaudés spélétájs un pretinieks, ja revanśá spélétájs uzvarés
    public int CupsTrackLose = 0; //     ---------------------------"-----------"----------------            zaudés

    public string ReplayMyRide = "";
    public string ReplayOppRide = "";
    public float ReplayMyTime = 0;
    public float ReplayOppTime = 0;
    public int[] ReplayMyUpgrades = { 0, 0, 0, 0, 0 };
    public int[] ReplayOppUpgrades = { 0, 0, 0, 0, 0 };
    public string ReplayTrack; //límeńa nosaukums | ko braukt rádít replejá  / nebraukt
    public int PowerRating = 0;


    public MPTypes MPType; //kurś etaps tagad jáspélé
    public bool Done = false; //var spélét, viss nepiecieśamie dati lejupieládéti

    public FriendGameEntryBehaviour UIEntry; //panelítis scéná, kam pieder śis skripts, to uzsit panelítis

    private bool poked = false;
    private bool achiCounted = false;



    //aizvác no iesákto spélju saraksta
    public void Remove()
    {
        MultiplayerManager.DeleteChallenge(FBID);
    }

    /**
	 * sáks spéli, ja var,
	 * poukos, ja nevar spélét un var poukot
	 * 
	 */
    public void Play()
    {

        if (!Waiting)
        { //serveris ĺauj spélét

            if (!Done)
            {
                //print("please wait");
                if (Debug.isDebugBuild) { Debug.Log("!done, please wait "); }
                return;
            }

            if (!achiCounted)
            {
                achiCounted = true; // lai neieskaitítu repleju, revanśu un jaunu spéli ká 3 multipleijera spéles, skaitís tikai pirmo 
                AchievementManager.AchievementProgress("mp_game", 1);
                AchievementManager.AchievementProgress("mp_game__2", 1);
                AchievementManager.AchievementProgress("mp_game__3", 1);
            }

            //print("Play with " + Done);

            if (Debug.isDebugBuild) { Debug.Log("start MP: " + MPType.ToString()); }

            GameScreenType switchToScreen = default(GameScreenType);
            string track = "";
            List<string> bikes = new List<string>();
            List<string> rideFiles = new List<string>();

            /*
			 if(MPType == MPTypes.replay ){ 
				Debug.Log("Ir járáda replejs, bet nav repleja failińu, SKIP IT"); 
				MPType++;
			} //*/

            switch (MPType)
            {
                case MPTypes.league:
                    track = Track;
                    switchToScreen = GameScreenType.MultiplayerPreGame;

                    //mans baiks
                    bikes.Add("player");
                    rideFiles.Add(null);

                    //pretinieka ghostińś
                    bikes.Add("opp");
                    rideFiles.Add(Ride);

                    for (int i = 0; i < 5; i++)
                    {
                        BikeDataManager.Bikes["MPGhost1"].Upgrades[i] = Upgrades[i];//TODO this is a ghost, so we don't care about temporary upgrades, can access and edit Upgrades directly
                    }

                    break;
                case MPTypes.replay:

                    track = ReplayTrack;
                    switchToScreen = GameScreenType.MultiplayerPreGameReplay; //GameScreenType.MultiplayerGameReplay; //TODO go to  GameScreenType.MultiplayerPreGameReplay
                                                                              //                    switchToScreen = GameScreenType.MultiplayerGameReplay; //TODO go to  GameScreenType.MultiplayerPreGameReplay


                    //bez spélétájkontrolétá baika
                    bikes.Add(null);
                    rideFiles.Add(null);

                    //mans repleja ghostińś
                    bikes.Add("opp");
                    rideFiles.Add(ReplayMyRide);
                    for (int i = 0; i < 5; i++)
                    {
                        BikeDataManager.Bikes["MPGhost1"].Upgrades[i] = ReplayMyUpgrades[i];//TODO this is a ghost, so we don't care about temporary upgrades, can access and edit Upgrades directly
                    }

                    //pretinieka repleja ghostińś
                    bikes.Add("opp");
                    rideFiles.Add(ReplayOppRide);
                    for (int i = 0; i < 5; i++)
                    {
                        BikeDataManager.Bikes["MPGhost2"].Upgrades[i] = ReplayOppUpgrades[i];//TODO this is a ghost, so we don't care about temporary upgrades, can access and edit Upgrades directly
                    }


                    break;
                case MPTypes.revanche:
                    track = Track;
                    switchToScreen = GameScreenType.MultiplayerPreGame;

                    //mans baiks
                    bikes.Add("player");
                    rideFiles.Add(null);

                    //pretinieka ghostińś
                    bikes.Add("opp");
                    rideFiles.Add(Ride);
                    for (int i = 0; i < 5; i++)
                    {
                        BikeDataManager.Bikes["MPGhost1"].Upgrades[i] = Upgrades[i]; //TODO this is a ghost, so we don't care about temporary upgrades, can access and edit Upgrades directly
                    }

                    break;
                case MPTypes.first:  //es iebraucu pret śo pretinieku;  ir tikai mans baiks, nav pretinieka ghostińa
                    track = NextTrack;
                    switchToScreen = GameScreenType.MultiplayerPreGame;

                    bikes.Add("player");
                    rideFiles.Add(null);
                    break;
            }


            if (track == null || track.Length == 0)
            {
                if (Debug.isDebugBuild) { Debug.LogError("nav trases, skipojam"); }
                return;
            }

            Debug.Log("LoadLevel 1 " + bikes.Count);
            LevelManager.LoadLevel("", track, false, bikes.ToArray(), rideFiles.ToArray());

            MultiplayerManager.CurrentOpponent = this;//ieliek śí skripta referenci menedźerí, lai visur , kur vajadzés pretinieka info, to varétu ńemt no śí skripta
            UIManager.SwitchScreen(switchToScreen); //ieládé ÍSTO SPÉLES logu (replejam ir cits)


        }
        else
        { //serveris neĺauj spélét 

        }


    }


    public void Poke()
    {
        /**
		 * @varbút -- skatíties vai serveris ĺauj poukot, 
		 * tagad pouko 1x múźá (párládéjot sarakstu varés vélreiz poukot)
		 * serveris patur tiesíbas nepoukot, ja ir párák bieźi poukots
		 */
        //Startup.LOG("Poke " + Name);

        if (!poked)
        {
            MultiplayerManager.SendPoke(FBID);
            poked = true;
        }
    }



}

}
