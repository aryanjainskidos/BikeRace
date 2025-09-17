namespace vasundharabikeracing {
#if UNITY_EDITOR_OSX  
//uz ierícém nav UnityEditor, bet uz WIN podzińa [a] tiek kjerta arí rakstot nosaukumu inspektorá

using UnityEditor;
using UnityEngine;

public class MyShortcuts : Editor
{
    [MenuItem("GameObject/ActiveToggle _a")]
    static void ToggleActivationSelection()
    {

        /*
		var go = Selection.activeGameObject;
		if(go != null){
			go.SetActive(!go.activeSelf);
		}*/


        var gos = Selection.gameObjects;
        for (int i = 0; i < gos.Length; i++)
        {
            gos[i].SetActive(!gos[i].activeSelf);
        }

    }
}
#endif

}
