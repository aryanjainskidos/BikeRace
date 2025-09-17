namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class RuntimeImageLoader : MonoBehaviour
{

    public string spriteName;
    public string spritePath;
    public bool DeleteAfterClosing = true;
    public bool DontWaitForInitialize = false; //TRUE, ja ir járáda bilde pirmajá ekráná, pirms Startup.Initialized (citádi = false)

    void OnEnable()
    {

        bool editMode = false;
#if UNITY_EDITOR
        editMode = !EditorApplication.isPlaying;
        GetSpriteName();
        GetSpritePath();
#endif

        if (((Startup.Initialized || DontWaitForInitialize) || editMode) && spritePath != "")
        {

            if (spriteName.Length > 0)
            {
                Sprite sp = LevelManager.GetSprite(spritePath, spriteName);
                transform.GetComponent<Image>().sprite = sp;

            }
        }

    }


    void OnDisable()
    {

#if UNITY_EDITOR
        GetSpriteName();
        GetSpritePath();
#endif

        if (DeleteAfterClosing && spritePath != "")
        {
            transform.GetComponent<Image>().sprite = null;
        }


    }

#if UNITY_EDITOR
    void GetSpritePath()
    {
        if (transform.GetComponent<Image>().sprite != null)
        {

            spritePath = AssetDatabase.GetAssetPath(transform.GetComponent<Image>().sprite);
            //lazy removing unnecessary parts
            spritePath = spritePath.Replace(".jpg", "");
            spritePath = spritePath.Replace(".png", "");

        }

        //        print(spritePath);

        if (spritePath.Contains("Resources/"))
        {
            spritePath = spritePath.Replace("Assets/", "");
            spritePath = spritePath.Replace("Resources/", "");
        }
        else
        {
            if (spritePath.Contains("Assets/"))
                spritePath = "";
        }
    }

    void GetSpriteName()
    {
        Sprite origSprite = transform.GetComponent<Image>().sprite;

        if (origSprite != null)
        {
            spriteName = origSprite.name;
        }
    }
#endif

}

}
