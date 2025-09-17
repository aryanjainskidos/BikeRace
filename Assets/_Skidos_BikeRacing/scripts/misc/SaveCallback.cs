namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
public class SaveCallback : UnityEditor.AssetModificationProcessor
{
    /*
	public static string[] OnWillSaveAssets(string[] paths)
	{
		// Get the name of the scene to save.
		string scenePath = string.Empty;
		string sceneName = string.Empty;
		
		foreach (string path in paths)
		{
			if (path.Contains(".unity"))
			{
				scenePath = Path.GetDirectoryName(path);
				sceneName = Path.GetFileNameWithoutExtension(path);
			}
		}
		
		if (sceneName.Length == 0)
		{
			return paths;
		}
		//-----
		Debug.Log("SaveCallback");


		//-----
		return paths;
	}*/

}
#endif

}
