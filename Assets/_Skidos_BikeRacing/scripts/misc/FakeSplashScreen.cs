namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class FakeSplashScreen : MonoBehaviour
{

    int timeToWasteWithFakeSplashScreen = 1;
    bool go = false;
    string nextScene = "MainOneAtATime";
    string nextSceneAlternative = "Main";

    /*
	#if UNITY_ANDROID
	string expPath;
	string logtxt;
	bool alreadyLogged = false;
	bool downloadStarted;
	#endif
	*/

    /**
	 * páráda feiko splash ekránu un ieládé ísto scénu, uz Androída, pirms tam apskatás vai nevajag lejupieládét OBB failu
	 * 
	 * @note -- .obb jásaucás: main.[BUNDLE_VESION_CODE].[BUNDLE_ID].obb
	 * main.2.com.FunGenerationLab.BikeUp.obb
	 * 
	 * @todo -- skripts jápadara iOS drośs
	 * @todo -- jápaméǵina ieládét scénu - ja neizdodas - skatíties OBB - lai strádátu arí nesplitotám appám
	 */
    void Update()
    {

        if (Time.frameCount > timeToWasteWithFakeSplashScreen && !go)
        { //péc X kadriem ieládés ísto scénu
            go = true;

            //vnk piestarté; ja vajag splitot - maini kodu >:)

            Application.LoadLevel(nextScene);
            if (Debug.isDebugBuild)
            {
                Application.LoadLevel(nextSceneAlternative); //ja nav uzbúvéjis ar "MainOneAtATime", tad bús fallback uz "Main" - tikai debug vajadzíbám
            }
            return;
            /*
                        #if !UNITY_ANDROID
                        //ja nav andrítis, neko nenjemamies - vnk piestartéjam ísto scénu
                        Application.LoadLevel(nextScene);
                        return;

                        #else

                        if(!GooglePlayDownloader.RunningOnAndroid()) {
                            print("Use GooglePlayDownloader only on Android device!");
                            return;
                        }


                        expPath = GooglePlayDownloader.GetExpansionFilePath();
                        if(expPath == null){
                            print("External storage is not available!");
                            return;
                        }


                        string mainPath = GooglePlayDownloader.GetMainOBBPath(expPath);
                        //string patchPath = GooglePlayDownloader.GetPatchOBBPath(expPath);
                        if( alreadyLogged == false )
                        {
                            alreadyLogged = true;
                            print("expPath = "  + expPath );
                            print("Main = "  + mainPath );
                            //print("Main = " + mainPath.Substring(expPath.Length));

                            if (mainPath != null){
                                StartCoroutine(loadLevelWithObb());
                            } else {
                                print("saakam lejupielaadi");
                                GooglePlayDownloader.FetchOBB();
                                StartCoroutine(loadLevelWithObb());
                            }				
                        }
                        #endif
            */
        }


    }
    /*
	#if UNITY_ANDROID
	protected IEnumerator loadLevelWithObb() 
	{ 
		string mainPath;
		do
		{
			yield return new WaitForSeconds(0.2f);
			mainPath = GooglePlayDownloader.GetMainOBBPath(expPath);	
			print("waiting mainPath "+mainPath);
		}
		while( mainPath == null);
		
		if( downloadStarted == false )
		{
			downloadStarted = true;
			
			string uri = "file://" + mainPath;
			print("downloading " + uri);
			WWW www = WWW.LoadFromCacheOrDownload(uri , 0);		
			
			// Wait for download to complete
			yield return www;
			
			if (www.error != null)
			{
				print("wwww error " + www.error);
			}
			else
			{
				Application.LoadLevel(nextScene);
			}
		}
	}
	#endif
*/

}
}
