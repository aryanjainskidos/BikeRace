namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MultiplayerLeagueSelectionBehaviour : MonoBehaviour
{

    Transform redButtonPanel;
    Transform greenButtonPanel;
    Transform blueButtonPanel;
    Transform purpleButtonPanel;

    Transform closeButton;

    //RectTransform top;
    //RectTransform left;
    //RectTransform right;
    //RectTransform bottom;

    void Awake()
    {

        closeButton = transform.Find("MenuButton");
        closeButton.gameObject.SetActive(false);

        redButtonPanel = transform.Find("RedButtonPanel");
        greenButtonPanel = transform.Find("GreenButtonPanel");
        blueButtonPanel = transform.Find("BlueButtonPanel");
        purpleButtonPanel = transform.Find("PurpleButtonPanel");

        //top = purpleButtonPanel.GetComponent<RectTransform>();
        //left = redButtonPanel.GetComponent<RectTransform>();
        // = blueButtonPanel.GetComponent<RectTransform>();
        //bottom = greenButtonPanel.GetComponent<RectTransform>();

        redButtonPanel.Find("Button").GetComponent<UIClickIndexDelegate>().indexDelegate = OnTeamButtonClick;
        greenButtonPanel.Find("Button").GetComponent<UIClickIndexDelegate>().indexDelegate = OnTeamButtonClick;
        blueButtonPanel.Find("Button").GetComponent<UIClickIndexDelegate>().indexDelegate = OnTeamButtonClick;
        purpleButtonPanel.Find("Button").GetComponent<UIClickIndexDelegate>().indexDelegate = OnTeamButtonClick;
    }

    void OnEnable()
    {

        if (MultiplayerManager.PlayerTeamID == 0)
        {
            closeButton.gameObject.SetActive(false);
        }
        else
        {
            closeButton.gameObject.SetActive(true);
        }
    }


    void OnTeamButtonClick(int index)
    {
        MultiplayerManager.PlayerTeamID = index;
        //zińo serverim par nomainíto komandu

        MultiplayerManager.DataID++; //piespieźu apdeitot MP ekránu

        UIManager.SetScreenTimeout(GameScreenType.PopupMultiplayerLoading, 10);// w/ timeouting
        UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, true); //loading overlay

        MultiplayerManager.MPPostTeamSelection(delegate ()
        {
            //sagaidu, kad serveris atbild
            UIManager.SwitchScreen(GameScreenType.MultiplayerMenu);//atver multiplayer ekránu
            UIManager.SwitchScreenTab(GameScreenType.MultiplayerMenu, "League"); //multipleijera ekráná atver League tabu

            UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, false);
        });


    }

}

}
