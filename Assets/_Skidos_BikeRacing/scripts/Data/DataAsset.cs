namespace vasundharabikeracing {
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

public class DataAsset : MonoBehaviour
{

#if UNITY_EDITOR
    [MenuItem("Assets/Create/LevelDataContainer")]
    public static void CreateLevelDataAsset()
    {
        CustomAssetUtility.CreateAsset<LevelDataContainer>();
    }


    [MenuItem("Assets/Create/UpgradeDataContainer")]
    public static void CreateUpgradeDataAsset()
    {
        CustomAssetUtility.CreateAsset<UpgradeDataContainer>();
    }
#endif
}

}
