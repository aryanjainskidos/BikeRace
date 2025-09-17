namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Data_MainProject;

/**
 * define gifts for finishing a level, for now only style gifts
 */
public class LevelGiftLineupManager : MonoBehaviour
{

    public static OrderedList_BikeRace<int, LevelGiftRecord> DefineLevelGifts()
    {
        OrderedList_BikeRace<int, LevelGiftRecord> Gifts = new OrderedList_BikeRace<int, LevelGiftRecord>();

        //{1,5,3,4,2,6,9}
        //        Gifts[0] = new LevelGiftRecord("a___010BONUSS", "10", 1);//beach
        //        Gifts[1] = new LevelGiftRecord("a___020BONUSS", "20", 5);//racer
        //        Gifts[2] = new LevelGiftRecord("a___030BONUSS", "30", 3);//tourist
        //        Gifts[3] = new LevelGiftRecord("a___050BONUSS", "50", 4);//hipster
        //        Gifts[4] = new LevelGiftRecord("a___070BONUSS", "70", 6);//space
        ////        Gifts[5] = new LevelGiftRecord("a___070", "70", 2);//agent
        ////        Gifts[6] = new LevelGiftRecord("a___070", "70", 9);//gold
        //        
        //        Gifts[0].SmallSpriteName = "";//beach is the first, so it'll never be next
        //        Gifts[0].SpriteName = "style_Beach";
        //        
        //        Gifts[1].SmallSpriteName = "style_Racer_next";
        //        Gifts[1].SpriteName = "style_Racer";
        //        
        //        Gifts[2].SmallSpriteName = "style_Tourist_next";
        //        Gifts[2].SpriteName = "style_Tourist";
        //        
        //        Gifts[3].SmallSpriteName = "style_Hipster_next";
        //        Gifts[3].SpriteName = "style_Hipster";
        //        
        //        Gifts[4].SmallSpriteName = "style_Astro_next";
        //        Gifts[4].SpriteName = "style_Astro";
        //        
        ////        Gifts[5].SmallSpriteName = "style_Agent_next";
        ////        Gifts[5].SpriteName = "style_Agent";
        //        
        ////        Gifts[6].SmallSpriteName = "style_Gold_next";
        ////        Gifts[6].SpriteName = "style_Gold";

        return Gifts;
    }

    public static int GetLevelGiftID(string levelName, OrderedList_BikeRace<int, LevelGiftRecord> Gifts)
    {

        int giftID = -1;
        foreach (var item in Gifts)
        {
            if (item.Value.LevelName == levelName)
            {
                giftID = item.Key;
            }
        }

        return giftID;
    }
}

public class LevelGiftRecord
{


    public LevelGiftRecord(string levelName, string levelDisplayName, int styleID)
    {
        LevelDisplayName = levelDisplayName;
        StyleID = styleID;
        LevelName = levelName;
        Gifted = false;
    }

    public int StyleID; //numurs péc kártas
    public string LevelName; //numurs péc kártas
    public string LevelDisplayName; //numurs péc kártas
    public bool Gifted; //numurs péc kártas
                        //TODO possibly add coins

    public string SpriteName = "";
    public string SmallSpriteName = "";

}


}
