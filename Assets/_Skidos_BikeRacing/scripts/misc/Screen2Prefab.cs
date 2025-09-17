namespace vasundharabikeracing {
#if UNITY_EDITOR
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Screen2Prefab : MonoBehaviour
{



    [MenuItem("Window/BikeUp UI/Screens to Prefabs %#&b", false, 70)]
    public static void Screens2Prefabs_()
    {
        if (EditorUtility.DisplayDialog("Veidojam ?", "Visi esošie ekrānprefabi tiks izdzēst un to vietā izveidoti šajā scēnā zem Canvas atrodamie ekrāni", "OK, bliežam!", "Paga, nē!"))
        {
            Screens2Prefabs();
        }

    }



    static void Screens2Prefabs()
    {

        string folderPathAbs = Application.dataPath + "/Resources/prefabs/UI/Screens/";
        string folderPathRel = "Assets/Resources/Prefabs/UI/Screens/";

        System.IO.Directory.Delete(folderPathAbs, true); //likvidé iepriekśéjos prefabus
        System.IO.Directory.CreateDirectory(folderPathAbs);


        Transform canvas = GameObject.Find("Canvas_game").transform;
        int n = 0;
        foreach (Transform child in canvas)
        {
            child.gameObject.SetActive(true);
            print("done:" + child.name);
            PrefabUtility.CreatePrefab(folderPathRel + child.name + ".prefab", child.gameObject);
            child.gameObject.SetActive(false);
            n++;
        }

        Debug.Log(n + " prefabi izveidoti: " + folderPathRel);
        EditorUtility.DisplayDialog("Gatavs", n + " prefabi izveidoti: " + folderPathRel + "\n\nNeaizmirsti noseivot scēnu, citādi prefabu faili nebūs komitojami!", "Sarunāts");

    }


}
#endif

}
