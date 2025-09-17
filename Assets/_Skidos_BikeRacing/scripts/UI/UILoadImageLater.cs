namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

/**
 * Jápieliek pie UI.Image saturośiem GameObjektiem 
 * ieládés bildes spraitu tikai, kam geimobjekts ir redzams un dzésís, kad nav redzams ( arí redaktorá :D )
 * 
 * @note -- nestrádá ar bildeḿ subdirektorijás (maybe @todo)
 * @note -- nestrádá ar bildém no spraitśítiem (maybe @todo)
 * 
 */
[ExecuteInEditMode]
public class UILoadImageLater : MonoBehaviour
{


    public string spriteName;
    public bool DeleteAfterClosing = true;
    public bool DontWaitForInitialize = false; //TRUE, ja ir járáda bilde pirmajá ekráná, pirms Startup.Initialized (citádi = false)


    void OnEnable()
    {
        Debug.LogWarning("@deprecated - dzéśam árá visu ekránu, nevis tikai bildi!");

        bool editMode = false;
#if UNITY_EDITOR
        editMode = !EditorApplication.isPlaying;
#endif


        if (editMode)
        {
            //editorá jánoskaidro bildes spraita nosaukums
            Sprite origSprite = transform.GetComponent<Image>().sprite;

            if (origSprite != null)
            {
                spriteName = origSprite.name;
                //Debug.Log("pieseivojam spraita nosaukumu: " + spriteName);
            }
        }

        //editorá un, kad spéle pa ístam iesákta, jáieládé spraits
        if ((Startup.Initialized || DontWaitForInitialize) || editMode)
        {

            //jáiedod bilde śis spraits
            if (spriteName.Length > 0)
            {
                /*
				  if(Debug.isDebugBuild){
					Debug.Log("ielaadees spraitu: " + spriteName);
				}//*/
                Sprite sp = LevelManager.GetSprite("Visuals/Sprites/GUI_sprites/" + spriteName, spriteName);
                transform.GetComponent<Image>().sprite = sp;

            }
        }




    }


    void OnDisable()
    {

        bool editMode = false;
#if UNITY_EDITOR
        editMode = !EditorApplication.isPlaying;
#endif

        if (editMode)
        {
            //editorá jánoskaidro bildes spraita nosaukums
            Sprite origSprite = transform.GetComponent<Image>().sprite;

            if (origSprite != null)
            {
                spriteName = origSprite.name;
                //Debug.Log("pieseivojam spraita nosaukumu: " + spriteName);
            }
        }

        //likvidé bildei spraitu - bet nespiedís to izmest no atmińas (Unity ir liels bérns, pats izlems)
        if (DeleteAfterClosing)
        {

            /*
			if(Debug.isDebugBuild){
				if(transform.GetComponent<Image>().sprite != null){
					Debug.Log("Dzeeshu spraitu: " + spriteName);
				}
			}//*/

            transform.GetComponent<Image>().sprite = null;
        }


    }

}

}
