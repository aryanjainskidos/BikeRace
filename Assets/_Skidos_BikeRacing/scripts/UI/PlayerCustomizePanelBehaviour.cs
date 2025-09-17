namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
// using GameAnalyticsSDK;

public class PlayerCustomizePanelBehaviour : MonoBehaviour
{

    public Dictionary<string, ColorPresetPanelBehaviour> map;
    public StylePanelBehaviour stylePanelBehaviour;

    Transform confirmPanel;
    public Button clearButton;
    public Button confirmButton;
    public Button unlockButton;
    public Button clearUnlockButton;
    Text unlockConditionText;

    Text unlockButtonText;

    public string bikeRecordName;
    public Dictionary<string, int> groupPresetIDs = new Dictionary<string, int>();

    int styleID;

    Text infoText;

    void Awake()
    {

        map = new Dictionary<string, ColorPresetPanelBehaviour>();
        map["helmet"] = transform.Find("ColorPresetPanelH").GetComponent<ColorPresetPanelBehaviour>();
        map["body"] = transform.Find("ColorPresetPanelB").GetComponent<ColorPresetPanelBehaviour>();
        map["bike"] = transform.Find("ColorPresetPanelK").GetComponent<ColorPresetPanelBehaviour>();
        map["rims"] = transform.Find("ColorPresetPanelT").GetComponent<ColorPresetPanelBehaviour>();
        map["rims"].maxPresetCount = 9;

        stylePanelBehaviour = transform.Find("StylePanel").GetComponent<StylePanelBehaviour>();
        //
        foreach (var item in map)
        {
            item.Value.key = item.Key;
        }

        confirmPanel = transform.Find("ConfirmPanel");

        infoText = transform.Find("ConfirmPanel/Text").GetComponent<Text>();

        clearButton = transform.Find("ConfirmPanel/ClearButton").GetComponent<Button>();
        confirmButton = transform.Find("ConfirmPanel/ConfirmButton").GetComponent<Button>();
        unlockButton = transform.Find("ConfirmPanel/UnlockButton").GetComponent<Button>();
        unlockConditionText = transform.Find("ConfirmPanel/ConditionText").GetComponent<Text>();
        unlockButtonText = unlockButton.transform.Find("CoinText").GetComponent<Text>();
        clearUnlockButton = transform.Find("ConfirmPanel/ClearUnlockButton").GetComponent<Button>();
        unlockButton.onClick.AddListener(UnlockStyle);

        unlockButton.gameObject.SetActive(false);
        clearUnlockButton.gameObject.SetActive(false);
        unlockConditionText.gameObject.SetActive(false);
    }

    public void Init()
    {
        foreach (var item in map)
        {
            item.Value.Init();

            item.Value.panelDelegate = null;
            item.Value.panelDelegate += ColorPartGroup;
        }

        stylePanelBehaviour.Init();
        stylePanelBehaviour.panelDelegate = null;
        stylePanelBehaviour.panelDelegate += SetRider;

        Load();
    }

    void OnEnable()
    {
        confirmPanel.gameObject.SetActive(false);

        //there must be a better place for this
        if (MultiplayerManager.NumGames >= 200 && BikeDataManager.Styles[8].Locked)
        {
            BikeDataManager.Styles[8].Locked = false;
        }
    }

    public void Load()
    {

        //clear current values
        groupPresetIDs.Clear();

        //load data
        foreach (var item in BikeDataManager.Bikes[bikeRecordName].GroupPresetIDs)
        {
            groupPresetIDs[item.Key] = item.Value;
        }

        //set the checkboxes after loading the data (update the view)
        foreach (var ppb in map)
        {
            if (groupPresetIDs.ContainsKey(ppb.Key))
            {
                ppb.Value.SetCheckboxByIndex(groupPresetIDs[ppb.Key]);
                //                print("LoadGroupPresetIDs " + ppb.Key + " " + GroupPresetIDs[ppb.Key] + " " + success);
            }
            else
            {
                print("GroupPresetIDs.ContainsKey(ppb.key): " + ppb.Key + " " + groupPresetIDs.ContainsKey(ppb.Key));
            }
        }

        //load data
        styleID = BikeDataManager.Bikes[bikeRecordName].StyleID;
        stylePanelBehaviour.SetToggleByIndex(styleID);

        foreach (var item in map)
        {
            item.Value.SetVisiblePresets(BikeDataManager.Styles[styleID].GroupPresetIDs[item.Key]);
        }

        confirmPanel.gameObject.SetActive(false);
    }

    public void Save()
    {

        if (BikeDataManager.Bikes.ContainsKey(bikeRecordName))
        {

            BikeDataManager.Bikes[bikeRecordName].StyleID = styleID; //first set the rider id, otherwise it will save the color to the wrong rider

            foreach (var item in groupPresetIDs)
            {
                BikeDataManager.Bikes[bikeRecordName].GroupPresetIDs[item.Key] = item.Value;
            }

        }

        confirmPanel.gameObject.SetActive(false);
    }

    void ColorPartGroup(string groupName, int presetIndex)
    {
        //        print("PlayerCustomizePanelBehaviour::ColorPartGroup " + bikeRecordName + " " + groupName + " " + presetIndex);
        //        Color32[] presetColors = DataManager.Presets[presetIndex].Colors;
        groupPresetIDs[groupName] = presetIndex;

        if (unlockButton.gameObject.activeSelf && !BikeDataManager.Styles[styleID].Locked)
        {

            infoText.text = Lang.Get("UI:Garage:ConfirmStyle");
            unlockButton.gameObject.SetActive(false);
            clearUnlockButton.gameObject.SetActive(false);
            clearButton.gameObject.SetActive(true);
            confirmButton.gameObject.SetActive(true);

            unlockConditionText.gameObject.SetActive(false);
        }
        confirmPanel.gameObject.SetActive(true);
    }

    void SetRider(int index)
    {

        styleID = index;

        foreach (var item in map)
        {
            item.Value.SetVisiblePresets(BikeDataManager.Styles[styleID].GroupPresetIDs[item.Key]);
        }


        //////////////
        //clear current values
        groupPresetIDs.Clear();

        //load data //can't use Bikes[bikeRecordName].GroupPresetIDs because riderID hasn't been confirmed yet
        foreach (var item in BikeDataManager.Bikes[bikeRecordName].StyleGroupPresetIDs[styleID])
        {
            groupPresetIDs[item.Key] = item.Value;
        }

        //set the checkboxes after loading the data (update the view)
        foreach (var ppb in map)
        {
            if (groupPresetIDs.ContainsKey(ppb.Key))
            {
                /*var success = */
                ppb.Value.SetCheckboxByIndex(groupPresetIDs[ppb.Key]);
            }
            else
            {
                print("GroupPresetIDs.ContainsKey(ppb.key): " + ppb.Key + " " + groupPresetIDs.ContainsKey(ppb.Key));
            }
        }
        ///////////

        if (BikeDataManager.Bikes[bikeRecordName].StyleID == styleID)
        {
            confirmPanel.gameObject.SetActive(false); // selecting the already selected style doesn't activate the confirm panel
        }
        else
        {
            confirmPanel.gameObject.SetActive(true);
        }

        unlockConditionText.gameObject.SetActive(false);
        if (BikeDataManager.Styles[styleID].Locked)
        {
            //change text & hide buttons
            SoundManager.Play("StyleLocked");

            infoText.text = Lang.Get("Garage:UnlockStyle");
            unlockButton.gameObject.SetActive(true);
            unlockButtonText.text = BikeDataManager.Styles[styleID].Price.ToString();
            clearUnlockButton.gameObject.SetActive(true);
            clearButton.gameObject.SetActive(false);
            confirmButton.gameObject.SetActive(false);

            unlockConditionText.gameObject.SetActive(true);
            SetUnlockConditionText(styleID);

        }
        else
        {
            if (unlockButton.gameObject.activeSelf)
            {

                infoText.text = Lang.Get("UI:Garage:ConfirmStyle");
                unlockButton.gameObject.SetActive(false);
                clearUnlockButton.gameObject.SetActive(false);
                clearButton.gameObject.SetActive(true);
                confirmButton.gameObject.SetActive(true);

                unlockConditionText.gameObject.SetActive(false);
            }
        }
    }

    void SetUnlockConditionText(int styleID)
    {
        //        int index = -1;
        bool getsGifted = false;
        string levelDisplayName = "";

        //TODO edit this
        //        for (int i = 0; i < DataManager.GiftStyleIndices.Length; i++) {
        //            if (DataManager.GiftStyleIndices[i] == styleID) {
        //                getsGifted = true;
        //                index = i;
        //            }
        //        }

        foreach (var item in BikeDataManager.LevelGifts)
        {
            if (item.Value.StyleID == styleID)
            {
                getsGifted = true;
                levelDisplayName = item.Value.LevelDisplayName;
            }
        }


        if (getsGifted)
        {
            unlockConditionText.text = Lang.Get("Garage:Unlock:unlocked at level").Replace("|param|", levelDisplayName);//(index + 1) * 10;
        }
        else
        {
            unlockConditionText.text = "";
        }

        if (styleID == 888)
        {
            unlockConditionText.text = Lang.Get("Garage:Unlock:play multiplayer games to unlock").Replace("|param|", " 200 ") + " (" + MultiplayerManager.NumGames + "/200)";
        }
    }

    void UnlockStyle()
    {
        if (PurchaseManager.CoinPurchase(BikeDataManager.Styles[styleID].Price))
        {
            // Debug.Log("unlocked styleID= " + styleID + " '" + DataManager.Styles[styleID].Name.Remove(DataManager.Styles[styleID].Name.Length - 1, 1) +"'");
            BikeDataManager.Styles[styleID].Locked = false;
            stylePanelBehaviour.UnlockCurrentToggle();

            infoText.text = Lang.Get("UI:Garage:ConfirmStyle");
            unlockButton.gameObject.SetActive(false);
            clearUnlockButton.gameObject.SetActive(false);
            clearButton.gameObject.SetActive(true);
            confirmButton.gameObject.SetActive(true);

            // GameAnalyticsSDK.Wrapper.GA_Wrapper.AddDesignEvent(DataManager.Styles[styleID].Name.Remove(DataManager.Styles[styleID].Name.Length - 1, 1));

            //autoconfirm, click on the confirm button for the user, other way would be to execute GarageBehaviours SaveColorConfiguration method, this is simpler but a bit hacky, though easier to change if decided to go back to previous behaviour
            ExecuteEvents.Execute(
                confirmButton.gameObject,
                new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerClickHandler);

            SoundManager.Play("StyleUnlock");

            BikeDataManager.Flush(); //save to disk

        }
    }
}

}
