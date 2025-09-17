namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewsListPanelBehaviour : MonoBehaviour
{

    public int Type; //inspektorá jánoráda 0=Trases chúska, 1=league, 2=friends

    Transform Insides; //jaunumu paneĺa iekśas (viss vizuálais), ko var slégt iekśá/árá netraucéjot Geimobjektam, kas satur śo skriptu
    Text MainText;
    GameObject ImageButtonOk; //redzamá podzinja, kas nestrádá
    GameObject ImageButtonCancel;
    //	UIButtonSwitchScreen ButtonScriptScreen; //neredzamá podzińa, kas strádá (un nosedz visu paneli)  - ekránpárslégśanas skripts
    UIButtonSwitchScreenTab ButtonScriptTab; // taba párslégśanas 
    GameObject[] Icons = { null, null, null, null, null, null }; //ikoninjas (indeksam jásakrít ar NewsListItemType)
    GameObject LevelsInfoPanel; //info panelis límenju ekrána, kam pa virsu atradísies śis zinju panelis



    void Awake()
    {
        Insides = transform.Find("Insides");

        LevelsInfoPanel = GameObject.Find("Canvas/Levels/InfoPanel");

        MainText = Insides.Find("MainText").GetComponent<Text>();
        ImageButtonOk = Insides.Find("ImageButtonOk").gameObject;
        ImageButtonCancel = Insides.Find("ImageButtonCancel").gameObject;
        //		ButtonScriptScreen = Insides.FindChild("Button").GetComponent<UIButtonSwitchScreen>();
        ButtonScriptTab = Insides.Find("Button").GetComponent<UIButtonSwitchScreenTab>();

        Icons[(int)NewsListItemType.achievement] = Insides.Find("IcoAchievements").gameObject;
        Icons[(int)NewsListItemType.mpFriends] = Insides.Find("IcoMP").gameObject;
        Icons[(int)NewsListItemType.mpLeague] = Insides.Find("IcoMP").gameObject;
        Icons[(int)NewsListItemType.prize] = Insides.Find("IcoCoins").gameObject;
        Icons[(int)NewsListItemType.promo] = Insides.Find("IcoGarage").gameObject;
        Icons[(int)NewsListItemType.boost] = Insides.Find("IcoBoosts").gameObject;


    }


    void OnEnable()
    {

        try
        {
            if (UIManager.currentScreenType == GameScreenType.Levels)
            {
                if (LevelsInfoPanel == null)
                {
                    LevelsInfoPanel = transform.parent.Find("InfoPanel").gameObject;
                }
                if (Type != 0)
                {
                    Debug.LogError("NewsListPanelBehaviour::A::Wrong Type");
                }
            }
            else if (UIManager.currentScreenType == GameScreenType.MultiplayerMenu)
            {
                if (Type != 1 && Type != 2)
                {
                    Debug.LogError("NewsListPanelBehaviour::B:Wrong Type");
                }
            }


            NewsListItem nItem = NewsListManager.Pop(Type); //panjem vienu zinju, ja ir
            if (nItem != null)
            {
                if (Type == 0)
                { //trases chúskas gadíjumá zińu paneli parádot, ir jápaslépj info panelis (kuram daźi px paliek redzami) 
                    LevelsInfoPanel.SetActive(false);
                }
                Insides.gameObject.SetActive(true);

                //izslédz visas ikonas un ieslédz zinjas tipam atbilstośo
                for (int i = 0; i < Icons.Length; i++)
                {
                    Icons[i].SetActive(false);
                }
                Icons[(int)nItem.Type].SetActive(true);

                MainText.text = nItem.Text;
                //			ButtonScriptScreen.screen = nItem.GotoScreen;  //patiesajai pogai nomaina uz kurieni vedís klikśkjis
                ButtonScriptTab.screen = nItem.GotoScreen;
                ButtonScriptTab.tab = nItem.GotoTab;
                ButtonScriptTab.subTab = nItem.GotoSubTab;

                if (nItem.GotoScreen == UIManager.currentScreenType)
                { //ja ekráns, uz ko párslégt klikśkja gadíjumá, ir śi pats, tad nomainít redzamo podzinju OK pret CANCEL
                    ImageButtonOk.SetActive(false);
                    ImageButtonCancel.SetActive(true);
                }
                else
                {
                    ImageButtonOk.SetActive(true);
                    ImageButtonCancel.SetActive(false);
                }

            }
            else
            {
                Insides.gameObject.SetActive(false);
                if (Type == 0)
                {
                    LevelsInfoPanel.SetActive(true); //jáieslédz atpakaĺ parastais info panelítis
                }
            }

        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }


}

}
