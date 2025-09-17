namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//statisks menedźeris, kas pieskata jaunumus, kas attélojami trases chúská
public class NewsListManager : MonoBehaviour
{


    public static int ActiveRides = 0;  //śo uzseto MP menedźerí CheckMPNews(), kad ienák jauni braucieni 
    public static int LastShownActiveRides = 0; //śis sańem ActiveRides vértíbu, kad spélétájs atver draugu tabu

    public static int LeagueGamesPlayed = -1; //śajá sezoná (-1 == dunno)

    private static List<NewsListItem>[] list = { new List<NewsListItem>(), new List<NewsListItem>(), new List<NewsListItem>(), new List<NewsListItem>(), new List<NewsListItem>(), new List<NewsListItem>() }; //masívs ar n sarakstiem - katram NewsListItemType tipam ir savs saraksts



    /**
	 * atgriezís svarígáko, vecáko zinju (vai null, ja nav neká)
	 * toBeDisplayedIn -- kurá ekráná rádís --  0: trases chúská;  1: MP draugu lapá; 2: MP lígas lapá
	 */
    public static NewsListItem Pop(int toBeDisplayedIn = 0)
    {

        if (toBeDisplayedIn == 1)
        {//prasa no MP draugu lapas - likvidé draugu un lígas zińas
            EmptyCategory(NewsListItemType.mpFriends);
            //EmptyCategory(NewsListItemType.mpLeague); kápéc arí lígas zińas ?
        }

        if (toBeDisplayedIn == 2)
        {//prasa no MP lígas lapas - likvidé lígas zińas
            EmptyCategory(NewsListItemType.mpLeague);
        }

        for (int i = list.Length - 1; i >= 0; i--)
        { //sáks ar svarígákajám zińám		
            if (list[i].Count != 0)
            {
                NewsListItem news = list[i][0]; //nulltá zińa ir visvecáká
                list[i].RemoveAt(0);
                return news;
            }
        }
        return null;
    }

    //izdzéś visas zińas pieprasítajá kategorijá
    public static void EmptyCategory(NewsListItemType type)
    {
        list[(int)type] = new List<NewsListItem>();
    }

    /**
	 * ievieto zińu sarakstá
	 * 
	 * text
	 * type     	0..4   ir tips reizé prioritáte
	 * gotoScreen   default=Levels (trases chúska, tátad nepárslégt uz citu ekránu)
	 */
    public static void Push(string text, NewsListItemType type, GameScreenType gotoScreen = GameScreenType.Levels, string gotoTab = "", string gotoSubTab = "")
    {

        if (type == NewsListItemType.mpFriends && list[(int)type].Count >= 3)
        { //atskás pieńemt MP draugu zińu, kad jau ir 3 gabali sarakstá
            return;
        }

        if (type == NewsListItemType.boost && list[(int)type].Count > 0)
        { //atsakás pieńemt bústinju zinju, ja jau viena ir ierindota
            return;
        }

        list[(int)type].Add(new NewsListItem(text, type, gotoScreen, gotoTab, gotoSubTab));
    }


}

//augśá svarígákás zińas
public enum NewsListItemType
{
    prize = 0,
    mpFriends = 1,
    mpLeague = 2,
    promo = 3,
    achievement = 4,
    boost = 5,
}



//viens ierindots zińojums
public class NewsListItem
{

    public NewsListItem(string text, NewsListItemType type, GameScreenType gotoScreen = GameScreenType.Levels, string gotoTab = "", string gotoSubTab = "")
    {
        Text = text;
        Type = type;
        GotoScreen = gotoScreen;
        GotoTab = gotoTab;
        GotoSubTab = gotoSubTab;
    }


    public string Text;
    public NewsListItemType Type;
    public GameScreenType GotoScreen;
    public string GotoTab;
    public string GotoSubTab;

}

}
