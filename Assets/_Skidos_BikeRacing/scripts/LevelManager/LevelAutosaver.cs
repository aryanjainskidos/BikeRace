namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;


/**
 * autoseivotájs
 * reizi X minútés noseivo límeni (sák autosevot tikai péc 1. maunála seiva, ieládéjot nákamo límeni autoseivs atkal tiek atcelts)
 */
[ExecuteInEditMode]
public class LevelAutosaver : MonoBehaviour
{
#if UNITY_EDITOR


    private static string autosaveLevelName = ""; //automátiski noskaidros pédéjá saglabátá faila nosaukumu
    private static float autosaveInterval = 60 * 3; //sekundes starp autoseiviem
    private static float timeUntilSaving;
    private static bool gameWindowActive;


    public static void AutosaveStart(string levelName)
    {

        AutosaveStop();

        autosaveLevelName = levelName;
        timeUntilSaving = autosaveInterval;
        print("autoseivośana saakta");

        //Edit reźímá nedarbojas ko-rutínas un iEnumeratori, tápéc improvizéju:
        //kamerai pieśauju klát śo skriptu - Update tiks izsaukts ik kadru
        GameObject.Find("Main Camera").AddComponent<LevelAutosaver>();

    }

    public static void AutosaveStop()
    {

        LevelAutosaver previous = GameObject.Find("Main Camera").GetComponent<LevelAutosaver>();
        if (previous != null)
        {
            DestroyImmediate(previous);
        }

    }


    void Update()
    {

        if (gameWindowActive)
        { //ne-autoseivos un neskaitís laiku, ja aktívs ir spéles logs (nevis scéna vai hierarhija u.c.)
            return;
        }

        timeUntilSaving -= Time.unscaledDeltaTime;
        if (timeUntilSaving <= 0)
        {
            timeUntilSaving = autosaveInterval;

            string autoLevelName = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss--") + autosaveLevelName;
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/Level-autosaves/");
            string autoPath = Application.persistentDataPath + "/Level-autosaves/" + autoLevelName + ".bytes";
            LevelManager.SerializeScene(autoPath, true);


        }

    }


    void OnApplicationFocus(bool focusStatus)
    {
        gameWindowActive = focusStatus; //true, ja spéles panelis ir aktívs, false, ja jebkas cits
    }


#endif


}

}
