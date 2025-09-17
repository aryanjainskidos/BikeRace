namespace vasundharabikeracing {
#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MissingReferencesFinder : MonoBehaviour
{
    [MenuItem("Window/Missing Stuff/Show Missing Object References in scene", false, 50)]
    public static void FindMissingReferencesInCurrentScene()
    {
        var objects = GetSceneObjects();
        FindMissingReferences(EditorApplication.currentScene, objects);
    }

    [MenuItem("Window/Missing Stuff/Show Missing Object References in all scenes", false, 51)]
    public static void MissingSpritesInAllScenes()
    {
        foreach (var scene in EditorBuildSettings.scenes)
        {
            EditorApplication.OpenScene(scene.path);
            FindMissingReferences(scene.path, Resources.FindObjectsOfTypeAll<GameObject>());
        }
    }

    [MenuItem("Window/Missing Stuff/Show Missing Object References in assets", false, 52)]
    public static void MissingSpritesInAssets()
    {
        var allAssets = AssetDatabase.GetAllAssetPaths();
        var objs = allAssets.Select(a => AssetDatabase.LoadAssetAtPath(a, typeof(GameObject)) as GameObject).Where(a => a != null).ToArray();

        FindMissingReferences("Project", objs);
    }
    [MenuItem("Window/Missing Stuff/Show Missing(None) Images in scene (not buttons)", false, 60)]
    public static void FindMissingImageReferencesInCurrentScene()
    {
        var objects = GetSceneObjects();
        FindMissingReferences(EditorApplication.currentScene, objects, true);
    }
    [MenuItem("Window/Missing Stuff/Show None Sprites", false, 60)]
    public static void FindMissingSpriteReferencesInCurrentScene()
    {
        var objects = GetSceneObjects();
        FindMissingReferences(EditorApplication.currentScene, objects, false, true);
    }
    [MenuItem("Window/Missing Stuff/All Textures", false, 60)]
    public static void AllTextures_()
    {
        AllTextures();
    }

    private static void FindMissingReferences(string context, GameObject[] objects, bool imagesOnly = false, bool spritesOnly = false)
    {
        foreach (var go in objects)
        {
            Component[] components;
            if (imagesOnly)
            {
                components = go.GetComponents<Image>();
            }
            else if (spritesOnly)
            {
                components = go.GetComponents<SpriteRenderer>();
            }
            else
            {
                components = go.GetComponents<Component>();
            }

            foreach (Component c in components)
            {

                if (imagesOnly)
                {
                    if (!((Image)c).enabled)
                    {
                        //image is manualy(!) disabled, skiping
                        continue;
                    }

                    Button maybeButton = go.GetComponent<Button>();
                    if (maybeButton)
                    {
                        continue; //image has sibling component - Button, can't mess with those, skiping
                    }
                    Mask maybeMask = go.GetComponent<Mask>();
                    if (maybeMask)
                    {
                        continue;
                    }


                    if (((Image)c).color == new Color32(0, 0, 0, 230) || ((Image)c).color == new Color32(255, 255, 255, 230))
                    {
                        continue; //hardly-transparent overlay - we need em, skiping
                    }

                    if (((Image)c).sprite == null)
                    { //NONE sprite
                        Debug.LogError("None Sprite in GO: " + FullPath(go), go);
                    }

                }

                if (spritesOnly)
                {
                    if (((SpriteRenderer)c).sprite == null)
                    { //NONE sprite
                        Debug.LogError("None Sprite in GO: " + FullPath(go), go);

                    }
                }


                if (!c)
                {
                    Debug.LogError("Missing Component in GO: " + FullPath(go), go);
                    continue;
                }

                SerializedObject so = new SerializedObject(c);
                var sp = so.GetIterator();

                while (sp.NextVisible(true))
                {
                    if (sp.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if (sp.objectReferenceValue == null
                            && sp.objectReferenceInstanceIDValue != 0)
                        {
                            ShowError(context, go, c.GetType().Name, ObjectNames.NicifyVariableName(sp.name));
                        }
                    }
                }
            }
        }
    }

    private static GameObject[] GetSceneObjects()
    {
        return Resources.FindObjectsOfTypeAll<GameObject>()
            .Where(go => string.IsNullOrEmpty(AssetDatabase.GetAssetPath(go))
                   && go.hideFlags == HideFlags.None).ToArray();
    }

    private const string err = "Missing Ref in: [{3}]{0}. Component: {1}, Property: {2}";

    private static void ShowError(string context, GameObject go, string c, string property)
    {
        Debug.LogError(string.Format(err, FullPath(go), c, property, context), go);
    }

    private static string FullPath(GameObject go)
    {
        return go.transform.parent == null
            ? go.name
                : FullPath(go.transform.parent.gameObject) + "/" + go.name;
    }

    private static void AllTextures()
    {

        Texture[] textures = (Texture[])Resources.FindObjectsOfTypeAll(typeof(Texture));
        for (int i = 0; i < textures.Length; i++)
        {
            Texture t = textures[i];
            Debug.Log(t.name + " " + t.width + "x" + t.height + " ");
        }



    }
}
#endif

}
