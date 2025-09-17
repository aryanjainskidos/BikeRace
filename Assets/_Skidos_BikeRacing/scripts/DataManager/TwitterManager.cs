namespace vasundharabikeracing {
// using UnityEngine;
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;

// using Soomla;
// using Soomla.Profile;

// public class TwitterManager : MonoBehaviour {

//     public static Provider provider = Provider.TWITTER;
//     public static string sendMessage = "test 1234";
//     static Action<bool> sendCallback;

//     // Use this for initialization
//     public static void Init () {

//         ProfileEvents.OnLoginFinished += onLoginFinished;
//         ProfileEvents.OnLoginFailed += onLoginFailed;

//         ProfileEvents.OnSocialActionFinished += onSocialActionFinished;
//         ProfileEvents.OnSocialActionFailed += onSocialActionFailed;
//         ProfileEvents.OnSocialActionCancelled += onSocialActionCancelled;

//         SoomlaProfile.Initialize();
//     }

//     public static void onLoginFinished(UserProfile profile, string payload) {

//         if (sendCallback != null) {
//             Send(sendMessage, sendCallback);
//         }

//         SoomlaUtils.LogDebug("Test ", "login finished ");
//     }

//     public static void onLoginFailed(Provider provider, string errorMessage, string payload) {

//         if (sendCallback != null) {
//             sendCallback(false);
//         }

//         SoomlaUtils.LogDebug("Test ", "login failed: " + errorMessage);
//     }

//     public static void onSocialActionFinished(Provider provider, SocialActionType action, string payload) {
//         // provider is the social provider
//         // action is the social action (like, post status, etc..) that finished
//         // payload is an identification string that you can give when you initiate the social action operation and want to receive back upon its completion

//         // ... your game specific implementation here ...
//         if (action == SocialActionType.UPDATE_STATUS) {

//             if (sendCallback != null) {
//                 sendCallback(true);
//             }

//             SoomlaUtils.LogDebug("Test ", "give them some sugar");
//             Debug.Log("event UPDATE_STATUS " + payload);
//         } else {

//             SoomlaUtils.LogDebug("Test ", "some other event");
//             Debug.Log("event OTHER " + payload);
//         }
//     }

//     public static void onSocialActionFailed(Provider provider, SocialActionType action, string errorMessage, string payload)
//     {
//         if (action == SocialActionType.UPDATE_STATUS) {

//             if (sendCallback != null) {
//                 sendCallback(false);
//             }

//             SoomlaUtils.LogDebug("Test ", "DON't give them sugar");
//             Debug.Log("event UPDATE_STATUS failed " + payload);
//         } else {

//             SoomlaUtils.LogDebug("Test ", "some other event");
//             Debug.Log("event OTHER failed " + payload);
//         }
//     }

//     public static void onSocialActionCancelled(Provider provider, SocialActionType action, string payload)
//     {
//         if (action == SocialActionType.UPDATE_STATUS) {

//             if (sendCallback != null) {
//                 sendCallback(false);
//             }

//             SoomlaUtils.LogDebug("Test ", "DON't give them sugar");
//             Debug.Log("event UPDATE_STATUS failed " + payload);
//         } else {

//             SoomlaUtils.LogDebug("Test ", "some other event");
//             Debug.Log("event OTHER failed " + payload);
//         }
//     }

//     public static void Send(string message, Action<bool> callback = null) {

//         if (callback != null) {
//             sendCallback = callback;
//         }

//         if (SoomlaProfile.IsLoggedIn(provider)) {
//             SoomlaProfile.UpdateStatus(provider, message, "statusUpdate");
//         } else {
//             sendMessage = message;

//             SoomlaProfile.Login(provider);
//         }
//     }

// //    void OnGUI(){
// //        if (GUI.Button(new Rect(100,100,100,100), "Login")) {
// //            SoomlaProfile.Login(provider);
// //        }
// //        if (SoomlaProfile.IsLoggedIn(provider)) {
// //            if (GUI.Button(new Rect(100,300,100,50), "status")) {
// //                SoomlaProfile.UpdateStatus(provider, message + System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"), "statusUpdate");
// //            }
// //            if (GUI.Button(new Rect(100,600,100,50), "story")) {
// //                SoomlaProfile.UpdateStory(provider, message + System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"), "a", "b", "c", "d", "e");
// //            }
// //            if (GUI.Button(new Rect(300,100,100,50), "logout")) {
// //                SoomlaProfile.Logout(provider);
// //            }
// //        }
// //    }
// }
}
