namespace vasundharabikeracing {
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class LoadAddressable_Vasundhara : MonoBehaviour
{
    public static LoadAddressable_Vasundhara Instance;

    private AsyncOperationHandle<SceneInstance> handle;

    public List<SceneInstance> previosScenes = new List<SceneInstance>();
    public SceneInstance newScene;

    public bool isNeedTOUnload;


    [Header("All resources Prefab"), SerializeField]
    List<GameObject> AllPrefab_Resources;

    [Header("All resources  Audio"), SerializeField]
    List<AudioClip> AllAudio_Resources;

    [Header("All resources  Material"), SerializeField]
    List<Material> AllMaterial_Resources;

    [Header("All resources  Texture"), SerializeField]
    List<Texture> AllTexture_Resources;

    [Header("All resources  Sprite"), SerializeField]
    List<Sprite> AllSprite_Resources;

    [Header("All resources  PhysicsMaterial2D"), SerializeField]
    List<PhysicsMaterial2D> AllPhysicsMaterial_Resources;

    [Header("All resources  Text Asset"), SerializeField]
    List<TextAsset> AllTextAsset_Resources;


    [Header("All resources  Scriptable Objects LevelData"), SerializeField]
    List<LevelDataContainer> AllScriptableObjLevelData_Resources;


    [Header("All resources  Scriptable Objects UpgradeDataContainer"), SerializeField]
    List<UpgradeDataContainer> AllScriptableObjUpgradeData_Resources;

    [Header("All resources  MotoShader"), SerializeField]
    List<Shader> AllMotoShader;

        private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }

        Instance = this;

        DontDestroyOnLoad(this);
    }

        private void OnEnable()
        {
            Physics2D.autoSyncTransforms = true;
        }
        private void OnDisable()
        {
            Physics2D.autoSyncTransforms = false;
        }




        void Start()
    {

    }


    public string sceneAddressableKey;

    public async void LoadScene(string key, bool isSingle, bool _isNeedToUnload)
    {
        isNeedTOUnload = _isNeedToUnload;

        if (isSingle)
        {
            AsyncOperationHandle<SceneInstance> _handle = Addressables.LoadSceneAsync(key, LoadSceneMode.Single);
            _handle.Completed += SceneLoadCompleted;
            //AddressableManager.Instances.previoiusScene = _handle;
            await _handle.Task;
            handle = _handle;
        }
        else
        {
            // Load scene using its addressable key
            AsyncOperationHandle<SceneInstance> _handle = Addressables.LoadSceneAsync(key, LoadSceneMode.Additive);
            _handle.Completed += SceneLoadCompleted;
            await _handle.Task;
            handle = _handle;
        }

        Debug.Log(" Instance Name : " + handle.DebugName);
    }

    private void SceneLoadCompleted(AsyncOperationHandle<SceneInstance> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            newScene = obj.Result;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                Debug.Log(" Active Scene  : " + scene.name);
            }

            try
            {
                Debug.Log(" Result name : " + obj.Result.Scene.name);
                //if (obj.Result.Scene.name != "mainScene")
                {
                    Debug.Log(" 3 ");
                    previosScenes.Add(obj.Result);
                }
                //else
                {
                    Debug.Log(" 4 ");
                }
            }
            catch (Exception ex)
            {
                Debug.Log(" Handle Scene Not Found " + ex);
            }
            //if (obj.Task.Result.Scene.name != "mainScene")
            //{
            //    Debug.Log(" 3 ");
            //    //previosScenes.Add(obj.Result);
            //}
            //else
            //{
            //    Debug.Log(" 4 ");
            //}

            if (isNeedTOUnload)
            {
                for (int i = 0; i < previosScenes.Count; i++)
                {
                    if (previosScenes[i].Scene != newScene.Scene)
                    {
                        //Addressables.UnloadSceneAsync(previosScenes[i]).Completed += UnLoadAddressable_Completed;
                        //previosScenes.Remove(previosScenes[i]);
                        Debug.Log(" List Count : " + previosScenes.Count);
                    }
                }

                //Scene newlyLoadedScene = obj.Result.Scene;

                //List<Scene> loadedScenes = new List<Scene>();

                //for (int i = 0; i < SceneManager.sceneCount; i++)
                //{
                //    Scene scene = SceneManager.GetSceneAt(i);
                //    if (scene != newlyLoadedScene)
                //    {
                //        if(scene.name != "mainScene")
                //            loadedScenes.Add(scene);
                //    }
                //}

                //// Unload all loaded scenes except the newly loaded one
                //foreach (var scene in loadedScenes)
                //{
                //    string key = "Assets/Scenes/" + scene.name + ".unity";

                //    SceneManager.UnloadSceneAsync(scene);
                //}

                //isNeedTOUnload = false;
                //Debug.Log("Scene unloaded successfully!");
            }
            Debug.Log("Scene loaded successfully!");
        }
        else
        {
            isNeedTOUnload = false;
            Debug.LogError("Failed to load scene: " + obj.OperationException);
        }
    }



    #region All method of loading addressable

    public GameObject GetPrefab_Resources(string _name)
    {
        if (string.IsNullOrEmpty(_name))
        {
            Debug.LogError("GetPrefab_Resources: _name is null or empty!");
            return null;
        }

        if (AllPrefab_Resources == null)
        {
            Debug.LogError("GetPrefab_Resources: AllPrefab_Resources list is null!");
            return null;
        }

        Debug.Log("GetPrefab_Resources: Looking for prefab: " + _name);
        Debug.Log("GetPrefab_Resources: Available prefabs count: " + AllPrefab_Resources.Count);
        
        foreach (GameObject x in AllPrefab_Resources)
        {
            if (x != null)
            {
                Debug.Log("GetPrefab_Resources: Checking prefab: " + x.name);
                if (x.name == _name)
                {
                    Debug.Log("GetPrefab_Resources: Found matching prefab: " + x.name);
                    return x.gameObject;
                }
            }
        }
        
        Debug.LogWarning("GetPrefab_Resources: Prefab not found: " + _name);
        return null;
    }

    public AudioClip GetAudio_Resources(string _name)
    {

        foreach (AudioClip x in AllAudio_Resources)
        {
                if (x != null)
                    if (x.name == _name)
            {
                return x;
            }
        }
        return null;
    }



    public Material GetMaterial_Resources(string _name)
    {

        foreach (Material x in AllMaterial_Resources)
        {
                if (x != null)
                    if (x.name == _name)
            {
                return x;
            }
        }
        return null;
    }
    public Texture GetTexture_Resources(string _name)
    {

        foreach (Texture x in AllTexture_Resources)
        {
                if (x != null)
                    if (x.name == _name)
            {
                return x;
            }
        }
        return null;
    }

    public LevelDataContainer GetScriptableObjLevelData_Resources(string _name)
    {
        if (string.IsNullOrEmpty(_name))
        {
            Debug.LogError("GetScriptableObjLevelData_Resources: _name is null or empty!");
            return null;
        }

        if (AllScriptableObjLevelData_Resources == null)
        {
            Debug.LogError("GetScriptableObjLevelData_Resources: AllScriptableObjLevelData_Resources list is null!");
            return null;
        }

        foreach (LevelDataContainer x in AllScriptableObjLevelData_Resources)
        {
            if (x != null && x.name == _name)
            {
                return x;
            }
        }
        
        Debug.LogWarning("GetScriptableObjLevelData_Resources: LevelDataContainer not found: " + _name);
        return null;
    }

    public UpgradeDataContainer GetScriptableObjUpgradeData_Resources(string _name)
    {

        foreach (UpgradeDataContainer x in AllScriptableObjUpgradeData_Resources)
        {
                if (x != null)
                    if (x.name == _name)
            {
                return x;
            }
        }
        return null;
    }
    public Shader GetMotoShader(string _name) {

            foreach (Shader x in AllMotoShader)
            {
                if (x != null)
                    if (x.name == _name)
                    {
                        return x;
                    }
            }
            return null;

        }

    public Sprite GetSprite_Resources(string _name)
    {
        if (string.IsNullOrEmpty(_name))
        {
            Debug.LogError("GetSprite_Resources: _name is null or empty!");
            return null;
        }

        if (AllSprite_Resources == null)
        {
            Debug.LogError("GetSprite_Resources: AllSprite_Resources list is null!");
            return null;
        }

        foreach (Sprite x in AllSprite_Resources)
        {
            if (x != null && x.name == _name)
            {
                return x;
            }
        }
        
        Debug.LogWarning("GetSprite_Resources: Sprite not found: " + _name);
        return null;
    }

    public PhysicsMaterial2D GetPhysicsMaterial_Resources(string _name)
    {

        foreach (PhysicsMaterial2D x in AllPhysicsMaterial_Resources)
        {
                if (x != null)
                    if (x.name == _name)
            {
                return x;
            }
        }
        return null;
    }

    public TextAsset GetTextAsset_Resources(string _name)
    {

        foreach (TextAsset x in AllTextAsset_Resources)
        {
                if (x != null)
                    if (x.name == _name)
            {
                return x;
            }
        }
        return null;
    }
    #endregion

}

}
