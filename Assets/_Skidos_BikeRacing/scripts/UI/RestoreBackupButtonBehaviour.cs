namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class RestoreBackupButtonBehaviour : MonoBehaviour
{

    GameObject visual;
    bool popupShown = false;

    void Awake()
    {
        visual = transform.Find("Visual").gameObject;
        //transform.FindChild("Visual/Button").GetComponent<UIButtonSimpleDelegate>().buttonDelegate = Click;
    }

    void OnEnable()
    {

        if (!DataBackupManager.HasCheckedForBackup)
        { //still checking, vélák jáapskatás vai rádít/nerádít pogu
            StartCoroutine(TurnOnIfGotBackup());
        }
        ShowHideUIStuff();
    }



    IEnumerator TurnOnIfGotBackup()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (DataBackupManager.HasCheckedForBackup)
            { //esam beiguśi chekot
                ShowHideUIStuff();
                yield break;
            }

        }

    }


    //rádít/nerádít pogu
    void ShowHideUIStuff()
    {
        if (DataBackupManager.BackupAwailable && Time.realtimeSinceStartup < 180)
        {
            visual.SetActive(true);
            if (!popupShown)
            {
                popupShown = true;
                UIManager.ToggleScreen(GameScreenType.PopupRestoreBackup, true);
            }
        }
        else
        {
            visual.SetActive(false);
        }
    }

}

}
