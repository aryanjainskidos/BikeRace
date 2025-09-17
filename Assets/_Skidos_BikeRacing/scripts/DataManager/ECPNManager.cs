namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Nestatisks menedźeris (JAVA libas déĺ) 
 * pieder ECPNManager game objektam (dzívo zem Main Camera game objekta)
 * izsauc Telemetrijas menedźeris
 * 
 * 
 * This class is responsible for polling device Token from either APSN (Apple) or Google Cloud Messaging Server and once it is received it is sent to the PHP server
 * and stored in the MySQL database
 * Users can later send messages to all registered devices
 * Only three methods should be called from outside the class:
 * 1) RequestDeviceToken() - Requests the current device Token from GCM or APSN and sends it to our server to be stored
 * 2) SendMessageToAll() - Sends a notification message to all server-registered devices
 * 3) RequestUnregisterDevice() - Request the current device Token to be removed from GCM or APSN and our own server
 * - (GetDevToken() is there for convenience of the sample scene)
 */
public class ECPNManager : MonoBehaviour
{


    private string devToken;

#if UNITY_ANDROID && !UNITY_EDITOR
	private string GoogleCloudMessageProjectID = "721488572623";
	private string packageName = "skidos.bikeracing"; // android liba ir sakompiléta lietojot śo ká package ID 
	private AndroidJavaObject playerActivityContext;
#endif


    // PUBLIC METHODS //

    /* Only works in android and iOS devices
	 * Android: It calls a static method in our GCMRegistration class which polls the device Token from Google Services
	 * iOS: Uses Unity NotificationServices class to poll deviceToken -which we have to poll until found
	 */
    public void RequestDeviceToken()
    {
        //print("skip RequestDeviceToken");
        //return;
#if UNITY_EDITOR
        //Debug.Log("You can only register iOS and android devices, not the editor!");
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
		// Obtain unity context
        if(playerActivityContext == null) {
			AndroidJavaClass actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        	playerActivityContext = actClass.GetStatic<AndroidJavaObject>("currentActivity");
		}
		AndroidJavaClass jc = new AndroidJavaClass(packageName + ".GCMRegistration");
		jc.CallStatic("RegisterDevice", playerActivityContext, GoogleCloudMessageProjectID);
#endif
#if UNITY_IPHONE
        // if (UnityEngine.iOS.NotificationServices.deviceToken == null)
        // {
        //     print("NotificationServices.deviceToken == null");
        //     pollIOSDeviceToken = true;
        //     UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert |
        //                             UnityEngine.iOS.NotificationType.Badge |
        //                             UnityEngine.iOS.NotificationType.Sound);
        // }
        // else
        // {
        //     print("NotificationServices.deviceToken != null");
        //     RegisterIOSDevice();
        // }
#endif
    }

    public void RequestUnregisterDevice()
    {
#if UNITY_EDITOR
        Debug.Log("You can only unregister iOS and android devices, not the editor!");
#endif
#if UNITY_IPHONE
        // UnityEngine.iOS.NotificationServices.UnregisterForRemoteNotifications();
        //StartCoroutine(DeleteDeviceFromServer(devToken));
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
		// Obtain unity context
        if(playerActivityContext == null) {
			AndroidJavaClass actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        	playerActivityContext = actClass.GetStatic<AndroidJavaObject>("currentActivity");
		}
		AndroidJavaClass jc = new AndroidJavaClass(packageName + ".GCMRegistration");
		jc.CallStatic("UnregisterDevice",playerActivityContext);
#endif
    }


    /*
	 * Get the current device Token, if known (does not request it)
	 */
    public string GetDevToken()
    {
        return devToken;
    }

    // UNDER THE HOOD METHODS //

#if UNITY_IPHONE
    private bool pollIOSDeviceToken = false;


    void Update()
    {
        // Unity does not tell us when the deviceToken is ready, so we have to keep polling after requesting it
        if (pollIOSDeviceToken) RegisterIOSDevice();
    }
#endif

    // Called from Java class once the deviceToken is ready -should not be called manually
    public void RegisterAndroidDevice(string rID)
    {
        //Debug.Log ("DeviceToken: " + rID);
        StartCoroutine(StoreDeviceID(rID, "android"));
    }
    // Called from Java class in response to Unregister event
    public void UnregisterDevice(string rID)
    {
        //Debug.Log ("Unregister DeviceToken: " + rID);
        //StartCoroutine(DeleteDeviceFromServer(rID));
    }

#if UNITY_IPHONE
    /*
	 * Poll NotificationServices for deviceToken for iOS device
	 * If found, send it to the server (StoreDeviceID)
	 */
    private void RegisterIOSDevice()
    {
        // if (UnityEngine.iOS.NotificationServices.registrationError != null) Debug.Log(UnityEngine.iOS.NotificationServices.registrationError);
        // if (UnityEngine.iOS.NotificationServices.deviceToken == null) return;
        // pollIOSDeviceToken = false;
        // string hexToken = System.BitConverter.ToString(UnityEngine.iOS.NotificationServices.deviceToken).Replace("-", string.Empty);
        // StartCoroutine(StoreDeviceID(hexToken, "ios"));
    }
#endif


    /*
	 * Sends store device Token request to server
	 */
    private IEnumerator StoreDeviceID(string rID, string os)
    {
        devToken = rID;
        PlayerPrefs.SetString("PushNotificationToken", devToken);
        //		print("TT StoreDeviceID:: token = " + devToken);

        //int errorCode;
        WWWForm form = new WWWForm();
        form.AddField("UID", SystemInfo.deviceUniqueIdentifier);
        //		form.AddField( "OS", os);
        form.AddField("token", devToken);
        WWW w = new WWW(MultiplayerManager.ServerUrlMP + "/telemetry/set_notification_token", form);
        yield return w;
        if (w.error != null)
        {
            //errorCode = -1;
            //print("TT StoreDeviceID--:" + w.error);
        }
        else
        {
            //print("TT StoreDeviceID++:" + w.text);
            //string formText = w.text; 
            //w.Dispose();
            //errorCode = int.Parse(formText);
        }
    }

}

}
