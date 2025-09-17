namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class QualitySettingsManager : MonoBehaviour
{


    /**
	 * Maina sistémas kvalitátes iestatíjumus (tajos gan nekas daudz nav definéts)
	 * Citur kodá atkaríbá vai DataManager.SettingsHD ir true/false slédz iekśá/árá daźádus efektus:
	 * izslédz mocha dúmus 					[√]
	 * neieládé shading belt				[√]  --  ieládéjot límeni
	 * nomaina prefabu: coins/coinsNonHD	[√]  --  ieládéjot límeni
	 * 
	 * 
	 * 
	 * 
	 * izsauc Startup skritpá péc DataManager inicializéśanas
	 */
    public static void Init()
    {

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        //vSync uz iOS vienmér ir ieslégts
        //@todo -- párbaudít vai androidam nevajag

        if (BikeDataManager.FirstTimer)
        {
            //pirmo reizi piestartéjot izlemj vai vajag izslégt HD (default = on)


            string model = SystemInfo.deviceModel.ToString();
            if (model == "iPhone3,1")
            { // 4. aifons
                BikeDataManager.SettingsHD = false;
            }

            if (Application.platform == RuntimePlatform.Android && SystemInfo.systemMemorySize < 700)
            { //weak androids
                BikeDataManager.SettingsHD = false;
            }

        }

        UpdateQualityLevel();
    }

    public static void UpdateQualityLevel()
    {
        int level;

        if (BikeDataManager.SettingsHD)
        {
            level = 1;
        }
        else
        {
            level = 0;
        }

        //nesetotjaunu QQ, ja sistémá jau ir uzsetots pieprasítais
        if (QualitySettings.GetQualityLevel() != level)
        {
            QualitySettings.SetQualityLevel(level, true);
        }

        //print("QQ=" + level);

    }


}

}
