namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

/**
 * Galvenajai kamerai piederośs skritps, kas pieskata spéles versiju, 
 * Versija tiek iegúta no Unity un pierakstíta publiská mainígajá tikai redaktorá,
 * bet śis publiskais mainígais bús pieejams nokompilétajá versijá
 */
[ExecuteInEditMode]
public class GameVersionManager : MonoBehaviour
{

    //ístá appas versija
    public static string V = ""; //nav redzams inspektorá, jo ir static; érti pieejams visiem skriptiem, jo static
    public string Version; //redzams inspektorá

    //MP-only versija - śo mainam, ja vajag, lai cilvéki nespélé MP ar vecu versiju
    public static string VMP = "";
    public string VersionMP;


    void Start()
    {
        V = Version; //atjauno statisko vértíbu no publiskás
        VMP = VersionMP;
    }


#if UNITY_EDITOR
    public void Awake()
    {
        EditorApplication.playmodeStateChanged += PlaymodeCallback;
    }

    public void PlaymodeCallback()
    {
        V = Version = PlayerSettings.bundleVersion; //atjauno publisko un statisko vértíbu	
    }
    public void OnValidate()
    {
        V = Version = PlayerSettings.bundleVersion;
    }
#endif



}

}
