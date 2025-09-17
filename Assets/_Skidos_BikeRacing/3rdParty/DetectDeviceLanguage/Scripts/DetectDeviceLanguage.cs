using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.Threading;
using System.Runtime.InteropServices;
//using Skidos;

public class DetectDeviceLanguage : MonoBehaviour
{
    public static DetectDeviceLanguage instance;
    public Text langNdcountryText;
    public DeviceLanguage deviceCurrLanguage;
    private string currntLanguageStr = string.Empty;
    public string testString = "EN-US-KL";
    public string deviceLangWithoutFallback = string.Empty;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        StartMethod();
    }


    private void StartMethod()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("java.util.Locale");
		AndroidJavaObject locale = androidJavaClass.CallStatic<AndroidJavaObject>("getDefault");
		string deviceLanguage = locale.Call<string>("getLanguage");
		string countryName = locale.Call<string>("getCountry");
		string str = deviceLanguage + "-" + countryName;
		Debug.Log("UNITY WAYNE wants to know device language  " + deviceLanguage);
		Debug.Log("UNITY WAYNE wants to know device country name  " + countryName);
		SetDeviceLanguageName(str);
        GetLocale = str;
        //langNdcountryText.text = GetDeviceCurrLanguage();
#endif

#if UNITY_IOS && !UNITY_EDITOR
        GetLocale = GetDeviceLanguage();

        SetDeviceLanguageName(GetDeviceLanguage());
		//langNdcountryText.text = GetDeviceCurrLanguage();
#endif

#if UNITY_EDITOR
        GetLocale = deviceCurrLanguage.ToString();
        SetDeviceLanguageName(deviceCurrLanguage.ToString());
        //SetDeviceLanguageName(testString);
#endif
    }

#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern string GetIOSDeviceLanguage();
#endif


#if UNITY_IOS && !UNITY_EDITOR
	private static string GetDeviceLanguage()
	{
	if(Application.platform == RuntimePlatform.IPhonePlayer)
	{
	return GetIOSDeviceLanguage();
	}
	Debug.Log("WRONG PLATFORM");
	return string.Empty;
	}
#endif

    private string locale = "";
    public string GetLocale
    {
        get { return locale; }
        set { locale = value; }

    }

    private void SetDeviceLanguageName(string languageName)
    {
        deviceLangWithoutFallback = languageName.ToUpper();
        //SkidosConstants.LOCALE_WITHOUT_FALLBACK = deviceLangWithoutFallback;
        languageName = languageName.ToUpper();

        //Split string here
        //string tempLanguage = languageName;
        //tempLanguage = tempLanguage.Replace("_", "-");

        //string[] strArray = tempLanguage.Split('-');

        //string compareStr = CompareLocaleString(strArray);

        if (languageName == DeviceLanguageNames.EN_US)
        {
            deviceCurrLanguage = DeviceLanguage.EN_US;

        }
        else if (languageName == DeviceLanguageNames.EN_GB)
        {
            deviceCurrLanguage = DeviceLanguage.EN_GB;
        }
        else if (languageName == DeviceLanguageNames.DA)
        {
            deviceCurrLanguage = DeviceLanguage.DA;
        }
        else if (languageName == DeviceLanguageNames.SV)
        {
            deviceCurrLanguage = DeviceLanguage.SV;
        }
        else if (languageName == DeviceLanguageNames.NB)
        {
            deviceCurrLanguage = DeviceLanguage.NB;
        }

        else if (languageName == DeviceLanguageNames.AU)
        {
            deviceCurrLanguage = DeviceLanguage.EN_GB;
        }

        else if (languageName == DeviceLanguageNames.ES)
        {
            deviceCurrLanguage = DeviceLanguage.ES;
        }

        else if (languageName == DeviceLanguageNames.NL)
        {
            deviceCurrLanguage = DeviceLanguage.NL;
        }

        else if (languageName == DeviceLanguageNames.ES_MX)
        {
            deviceCurrLanguage = DeviceLanguage.ES_MX;
        }

        else if (languageName == DeviceLanguageNames.PT_BR)
        {
            deviceCurrLanguage = DeviceLanguage.PT_BR;
        }
        else if (languageName == DeviceLanguageNames.EN_AU)
        {
            deviceCurrLanguage = DeviceLanguage.EN_AU;
        }
        else if (languageName == DeviceLanguageNames.UK_UA)
        {
            deviceCurrLanguage = DeviceLanguage.UK_UA;
        }

        else if (languageName == DeviceLanguageNames.DE)
        {
            deviceCurrLanguage = DeviceLanguage.DE;
        }

        else
        {
            deviceCurrLanguage = DeviceLanguage.EN_US;
        }
    }


    public string GetDeviceCurrLanguage()
    {
        currntLanguageStr = deviceCurrLanguage.ToString();
        return currntLanguageStr;
    }


    public DeviceLanguage GetDeviceCurrLanguageENUM()
    {

        return deviceCurrLanguage;
    }

    private string CompareLocaleString(string[] charcters)
    {
        string stringPriorityONE = string.Empty;
        string stringPriorityTWO = string.Empty;

        string finalString = string.Empty;


        switch (charcters.Length)
        {
            case 1:
                stringPriorityONE = charcters[0];
                break;

            case 2:
                stringPriorityONE = charcters[0];
                stringPriorityTWO = charcters[1];
                break;

            default:
                stringPriorityONE = charcters[0];
                stringPriorityTWO = charcters[1];
                break;
        }

        if (!string.IsNullOrEmpty(stringPriorityONE) && string.IsNullOrEmpty(stringPriorityTWO))
        {
            return stringPriorityONE;
        }

        if (!string.IsNullOrEmpty(stringPriorityONE) && !string.IsNullOrEmpty(stringPriorityTWO))
        {
            if (stringPriorityONE.Contains(DeviceLanguageNames.EN))
            {
                if (stringPriorityTWO.Contains(DeviceLanguageNames.US))
                {
                    finalString = DeviceLanguageNames.US;

                }
                else if (stringPriorityTWO.Contains(DeviceLanguageNames.GB))
                {
                    finalString = DeviceLanguageNames.GB;

                }
                else if (stringPriorityTWO.Contains(DeviceLanguageNames.AU))
                {
                    finalString = DeviceLanguageNames.GB;

                }
                else
                {
                    finalString = DeviceLanguageNames.US;
                }
            }

            else if (stringPriorityONE.Contains(DeviceLanguageNames.SV))
            {
                finalString = DeviceLanguageNames.SV;
            }

            else if (stringPriorityONE.Contains(DeviceLanguageNames.NB))
            {
                finalString = DeviceLanguageNames.NB;
            }

            else if (stringPriorityONE.Contains(DeviceLanguageNames.DA))
            {
                finalString = DeviceLanguageNames.DA;
            }

            else if (stringPriorityONE.Contains(DeviceLanguageNames.AU))
            {
                finalString = DeviceLanguageNames.GB;
            }

            else if (stringPriorityONE.Contains(DeviceLanguageNames.ES))
            {
                finalString = DeviceLanguageNames.ES;
            }

            else if (stringPriorityONE.Contains(DeviceLanguageNames.PT_BR))
            {
                finalString = DeviceLanguageNames.PT_BR;
            }
            else if (stringPriorityONE.Contains(DeviceLanguageNames.EN_AU))
            {
                finalString = DeviceLanguageNames.EN_AU;
            }

            else if (stringPriorityONE.Contains(DeviceLanguageNames.DE))
            {
                finalString = DeviceLanguageNames.DE;
            }

            else if (stringPriorityONE.Contains(DeviceLanguageNames.UK_UA))
            {
                finalString = DeviceLanguageNames.UK_UA;
            }
            else
            {
                finalString = DeviceLanguageNames.US;
            }
        }
        return finalString;
    }
}

[System.Serializable]
public enum DeviceLanguage
{
    EN_US,
    EN_GB,
    SV,
    NB,
    DA,
    ES,
    NL,
    IT,
    FR,
    ES_MX,
    DE,
    PT_BR,
    EN_AU,
    UK_UA,
    DEFAULT
}

public static class DeviceLanguageNames
{
    public static string US = "US";
    public static string EN_US = "EN_US";
    public static string EN_GB = "EN_GB";
    public static string GB = "GB";
    public static string DA = "DA";
    public static string SV = "SV";
    public static string NB = "NB";
    public static string AU = "AU";
    public static string EN = "EN";
    public static string ES = "ES";
    public static string NL = "NL";
    public static string ES_MX = "ES_MX";
    public static string DE = "DE";
    public static string PT_BR = "PT_BR";
    public static string EN_AU = "EN_AU";
    public static string UK_UA = "UK_UA";
}