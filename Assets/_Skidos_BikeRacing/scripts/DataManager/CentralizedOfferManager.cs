namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

//pieskata offerus, ko ieslédz uz servera
public class CentralizedOfferManager : MonoBehaviour
{


    public static int DoubleCoinWeekendLength = 60;  //2 dienas + nedaudz
    private static System.DateTime DoubleCoinWeekendStart;



    public static bool IsDoubleCoinWeekendOn()
    {

        if (DoubleCoinWeekendStart != System.DateTime.MinValue)
        { //ja ir jebkáds datums uzzináts
            System.TimeSpan timeTilNextWeekend = DoubleCoinWeekendStart - System.DateTime.Now;
            if (timeTilNextWeekend.TotalHours < 0 && timeTilNextWeekend.TotalHours > -DoubleCoinWeekendLength)
            { //ir sácies un nav pagájuśas viarák ká 48h kopś sákuma
                return true;
            }
        }
        return false;
    }


    public static System.TimeSpan GetDoubleCoinWeekendEndTime()
    {
        return DoubleCoinWeekendStart - System.DateTime.Now + System.TimeSpan.FromHours(DoubleCoinWeekendLength);
    }


    public static void Init()
    {
        IsDoubleCoinWeekendOn();
        GameObject.Find("Main Camera").GetComponent<MonoBehaviour>().StartCoroutine(PeriodiclyCheckOffer());
    }


    public static IEnumerator PeriodiclyCheckOffer()
    {

        while (true)
        {

            if (UIManager.currentScreenType == GameScreenType.Menu || //nechekos menjucí  
               BikeGameManager.initialized || //vai brauciena laiká			  
               UIManager.currentScreenType == GameScreenType.PopupPreGameLoading //vai ieládéjot límeni
               )
            {
                yield return new WaitForSeconds(1);
                continue;
            }

            GameObject.Find("Main Camera").GetComponent<MonoBehaviour>().StartCoroutine(AskServer());
            yield return new WaitForSeconds(3600); //1x stundá atkártos

        }
    }




    public static IEnumerator AskServer()
    {

        string url = MultiplayerManager.ServerUrlMP + "/centralized_offer.txt";
        WWW www = new WWW(url);
        yield return www;

        if (www.error == null)
        {

            //print("CentralizedOfferManager::" + url+ "  "  + www.text);
            string[] parts = www.text.Split(' ');//pagaidám ir tikai viena rindińa śajá failá: "taimstamps heśs"
            if (parts != null && parts.Length > 1)
            {

                if (Sha1.Sha1Sum(parts[0] + "offer") == parts[1])
                { //heśs ir sha1 "{$taimstamps}offer"

                    long unixTimeStamp = long.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
                    DoubleCoinWeekendStart = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    DoubleCoinWeekendStart = DoubleCoinWeekendStart.AddSeconds(unixTimeStamp).ToLocalTime();
                    //DoubleCoinWeekendStart = DoubleCoinWeekendStart.AddHours(8).ToLocalTime(); //for testing
                    //print("CentralizedOfferManager::party start @ " + DoubleCoinWeekendStart);

                    if (IsDoubleCoinWeekendOn())
                    {
                        MaybeShowPopup();
                    }

                }
                else
                {
                    //if(Debug.isDebugBuild){Debug.LogError("CentralizedOfferManager::paraksts nesakriit");}		
                }


            }
            else
            {
                //if(Debug.isDebugBuild){Debug.LogError("CentralizedOfferManager::nav datu");}		
            }


        }
        else
        {
            //hmm
            //if(Debug.isDebugBuild){Debug.LogError("CentralizedOfferManager::no centralized_offer: " + www.error.ToString());}		
        }

    }


    private static void MaybeShowPopup()
    {

        System.DateTime doubleCoinWeekendLastTimePopupShown;

        if (!System.DateTime.TryParse(PlayerPrefs.GetString("dcwltps", ""), out doubleCoinWeekendLastTimePopupShown))
        {
            doubleCoinWeekendLastTimePopupShown = new System.DateTime();
        }

        System.TimeSpan timeSinceLastPopup = System.DateTime.Now - doubleCoinWeekendLastTimePopupShown;
        //print("CentralizedOfferManager::MaybeShowPopup::timeSinceLastPopup=" + timeSinceLastPopup.TotalHours);

        if (timeSinceLastPopup.TotalHours > 3)
        { //ne bieźák ká reizi 3 stundás parádís popupu		
            doubleCoinWeekendLastTimePopupShown = System.DateTime.Now;
            PlayerPrefs.SetString("dcwltps", doubleCoinWeekendLastTimePopupShown.ToString());
            //print("CentralizedOfferManager::MaybeShowPopup::YES!");
            //			UIManager.ToggleScreen(GameScreenType.PopupDoubleCoinWeekend,true);
        }
        else
        {
            print("CentralizedOfferManager::MaybeShowPopup::too soon; time since last popup" + timeSinceLastPopup.ToString());
        }


    }




}

}
