using System.Collections;
using System.Collections.Generic;   //Required for Dictionary
using UnityEngine;


public class PrefabManager : ScriptableObject
{

	// Declare le dictionnaire  ObjCode-Obj
	public static Dictionary<int, GameObject> gameObjectList = new Dictionary<int, GameObject>();


	public static void LoadAllPrefabs()
	{
		// attention a ne pas load deux fois
		// appeler un dossier puis appeler un sous-dossier fait appeler deux fois
		LoadPrefabs("Player/FPS");
		LoadPrefabs("Player/RTS/ControllerAndCamera");
		LoadPrefabs("Player/RTS/Sbires");
		LoadPrefabs("Player/RTS/Batiments");
		LoadPrefabs("Player/RTS/MapProps");
		
		Debug.Log("PREFABS ARE LOADED");
	}
	


	public static void LoadPrefabs(string PrefabPath)
	{
		Object[] ObjectArray = Resources.LoadAll(PrefabPath,typeof(GameObject));
		
		foreach (Object o in ObjectArray)
		{
			if(!gameObjectList.ContainsKey(o.name.GetHashCode()))
			{
				gameObjectList.Add(o.name.GetHashCode(),(GameObject) o);
			}
			else
			{
				Debug.Log("redundant key :"+o.name);
			}
		}
	}


	public static GameObject GetPrefab(string objName)
	{
		GameObject obj;

		if (gameObjectList.TryGetValue(objName.GetHashCode(), out obj))
			return obj;
		else
		{
			Debug.Log(objName + " not found");
			return null;
		}
	}



}