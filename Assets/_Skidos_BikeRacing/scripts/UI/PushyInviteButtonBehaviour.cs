namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Globalization;

public class PushyInviteButtonBehaviour : MonoBehaviour
{

    const float MINUTES_TO_WAIT_TILL_NEXT_INVITE_OFFER = 90;
    //    const float MINUTES_TO_WAIT_TILL_NEXT_INVITE_OFFER_DID_NOT_INVITE = 10;

    const string LAST_INVITE_BUTTON_CLICK_TIMESTAMP_KEY = "LastInviteButtonClickTimestamp";

    public bool visible = true;
    Button button;

    Image image;

    //could probably make an interface for this or sth, since there are a couple of buttons behaving similarly

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        image = GetComponent<Image>();
    }

    void SetVisibility(bool value)
    {
        visible = value;
        image.enabled = value;
        button.enabled = value;

        Utils.ShowChildrenGraphics(gameObject, value);
    }

    void OnClick()
    {
        //        PlayerPrefs.SetString(LAST_INVITE_BUTTON_CLICK_TIMESTAMP_KEY, DateTime.Now.ToString());//DONE write down a timestamp of last invite click
        //        PlayerPrefs.Save();
    }

    void OnEnable()
    {
        if (BikeDataManager.Levels["a___007"].Tried
            && PlayerPrefs.GetInt("HasInvitedEveryone", 0) == 0 // if hasn't invited all 5 already, and they all approved
            && TimestampIsOldEnough() //TODO && some timestamp is far enough in the past
            )
        {
            SetVisibility(true);
        }
        else
        {
            SetVisibility(false);
        }
    }



    bool TimestampIsOldEnough()
    {

        bool timestampIsOldEnough = true;

        DateTime inviteTimestamp;
        //        DateTime inviteClickTimestamp;

        bool invitedBefore = GetTimestamp(MultiplayerManager.LAST_INVITE_TIMESTAMP_KEY, out inviteTimestamp);
        //        bool clickedInviteBefore = GetTimestamp(LAST_INVITE_BUTTON_CLICK_TIMESTAMP_KEY, out inviteClickTimestamp);

        //this seems complicated, but all it does is check if button was clicked after the invite or before
        //if LAST_INVITE_BUTTON_CLICK_TIMESTAMP_KEY is older than the LAST_INVITE_TIMESTAMP_KEY, show the button after MINUTES_TO_WAIT_TILL_NEXT_INVITE_OFFER
        //if it's not assume that the player did't invite anyone after clicking the button and show it after MINUTES_TO_WAIT_TILL_NEXT_INVITE_OFFER_DID_NOT_INVITE
        //        if (clickedInviteBefore) { //clicked this button before
        //
        //            bool invitedBeforeClick = (invitedBefore && DateTime.Compare(inviteTimestamp, inviteClickTimestamp) < 0); //if invite timestamp is earlier than inviteClick timestamp, player didn't invite anyone after clicking the button
        //
        //            if (!invitedBefore || invitedBeforeClick ) { //didn't invite friends before or invited before clicking again
        //
        //                TimeSpan delta = DateTime.Now - inviteClickTimestamp;
        //
        //                if (delta.TotalMinutes < MINUTES_TO_WAIT_TILL_NEXT_INVITE_OFFER_DID_NOT_INVITE) {
        //                    timestampIsOldEnough = false;
        //                }
        //
        //            } //else invited friends before and it was after click
        //
        //        }//else didn't click the button or the value is messed up

        if (invitedBefore)
        {
            TimeSpan delta = DateTime.Now - inviteTimestamp;

            if (delta.TotalMinutes < MINUTES_TO_WAIT_TILL_NEXT_INVITE_OFFER)
            {
                timestampIsOldEnough = false;
            }

        }

        //assume that a bad timestamp is an old timestamp
        //assume that no timestamp is an old timestamp

        return timestampIsOldEnough;
    }

    bool timestampIsOld;

    void Update()
    {

        //DONE check if a friend was invited within the last 90 minutes 
        timestampIsOld = TimestampIsOldEnough();

        //button can only be visible if timestamp is old or doesn't exist, so both visible and tiestampIsOld should be equal
        if (visible != timestampIsOld)
        {//could do this with a coroutine, or an event or sth, but couroutine would produce a lag unless called every frame
            //SetVisibility(!visible);
            OnEnable();
        }

    }


    bool GetTimestamp(string timestampKey, out DateTime timestampDate)
    { //TODO this could be a static utility function

        bool validTimestamp = false;
        timestampDate = DateTime.MinValue;

        if (PlayerPrefs.HasKey(timestampKey))
        {

            string timestamp = PlayerPrefs.GetString(timestampKey);

            validTimestamp = DateTime.TryParseExact(timestamp, "G", CultureInfo.InvariantCulture, DateTimeStyles.None, out timestampDate);
        }

        return validTimestamp;
    }
}

}
