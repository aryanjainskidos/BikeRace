namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupRestoreBackupBehaviour : MonoBehaviour
{
    Text infoText;

    void Awake()
    {
        infoText = transform.Find("InfoText").GetComponent<Text>();
        transform.Find("RestoreButton").GetComponent<UIButtonSimpleDelegate>().buttonDelegate = ClickRestore;

    }


    void OnEnable()
    {
        string savedGameLevel = DataBackupManager.BackupLevels.ToString();
        string savedGameDate = DataBackupManager.BackupTime.ToString("d"); // [ceru, ka] no valodas atkarígs datuma formáts

        infoText.text = Lang.Get("UI:PopupRestoreSave:InfoText").Replace("|param1|", savedGameLevel).Replace("|param2|", savedGameDate);

        //UI:PopupRestoreSave:A level |param1| game was saved to our web server on |param2|. Would you like to continue with the level 1 game on your device or restore the save game?
    }


    void ClickRestore()
    {
        //restorét
        //párládét levels ekránu (aizmest uz STARTA ekránu?)

        DataBackupManager.Restore(); //nomaina failu
        BikeDataManager.LoadData(true); //datu menedźeris sák lietot jaunos datus

        //refreśo ekránu
        UIManager.ToggleScreen(GameScreenType.Levels, false);
        UIManager.ToggleScreen(GameScreenType.Levels, true);

        TelemetryManager.EventRestoredGamedata(DataBackupManager.BackupLevels);

    }


}

}
