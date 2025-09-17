namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Data_MainProject;

/**
 * tikai definé bústińus
 */
public class BoostLineupManager : MonoBehaviour
{

    public static OrderedList_BikeRace<string, BoostRecord> DefineBoosts()
    {
        OrderedList_BikeRace<string, BoostRecord> Boosts = new OrderedList_BikeRace<string, BoostRecord>();



        /**
		 * Saraksts ar visiem bústińiem:
		 * 
		 * kods (jásakrít ar prefabu nosaukumiem: "Boost_kods" un "Boost_kods_crate" )
		 * =>
		 * nosaukums
		 * apraksts
		 * cik maksá (monétas)
		 * 
		 */
        Boosts["ice"] = new BoostRecord(0, "Ice", 50);
        Boosts["magnet"] = new BoostRecord(1, "Magnet", 50);
        Boosts["invincibility"] = new BoostRecord(2, "Invincibility", 50);
        Boosts["fuel"] = new BoostRecord(3, "Super fuel", 50);

        return Boosts;
    }

    public static string[] BoostNames = { "ice", "magnet", "invincibility", "fuel" }; // lai var zinát, ka bústs Nr3 = "fuel"
}


/**
 * JSONá saglabájama datu struktúra
 */
public class BoostRecord
{

    public BoostRecord(int id, string name, int pricePerMinute)
    {
        ID = id;
        Name = Lang.Get("Boost:Name:" + name); //iztulkos péc izveidośanas
        Description = Lang.Get("Boost:Description:" + name);
        PricePerMinute = pricePerMinute;
        Active = false;
        Selected = false;
        Number = 0;
        Discovered = false;
        FarmingTimestamp = System.DateTime.MinValue;
        FarmingDuration = new System.TimeSpan(0, 0, 10);
        NumberPerFarming = 5;
    }

    public int ID; //numurs péc kártas
    public string Name; //for display purposes
    public string Description;
    public int PricePerMinute; //per minute
    public bool Active; // vai ir ieslégts
    public bool Selected;
    public int Number; //cik gabali pieder spélétájam  | tikai śis tiek seivots JSONá
    public int NumberPerFarming; //cik gabali pieder spélétájam  | tikai śis tiek seivots JSONá
    public bool Discovered; // vai spélétájs ir atklájis un var pirkt
    public System.DateTime FarmingTimestamp; //kad sáka audzét tagadéjo bústu
    public System.TimeSpan FarmingDuration;

    public int Price
    {
        get { return Mathf.RoundToInt((PricePerMinute / 60.0f) * (float)FarmingDuration.TotalSeconds); }
    }

}


}
