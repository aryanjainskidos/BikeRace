namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupInviteFriendsBehaviour : MonoBehaviour
{




    void OnEnable()
    {

        int cumulate = 0;
        for (int i = 1; i <= 5; i++)
        {
            cumulate += MultiplayerManager.InvBonusAmmount[i];
            transform.Find("Panel" + (i) + "/CoinText").GetComponent<Text>().text = cumulate.ToString();//MultiplayerManager.InvBonusAmmount[i].ToString();
        }

    }
}
}
