using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PrefabManager : ScriptableObject
{
	// Declare le dictionnaire  ObjCode-Obj
	public static Dictionary<int, GameObject> gameObjectList = new Dictionary<int, GameObject>();


	public static void LoadAllPrefabs()
	{
        gameObjectList.Clear();

		// attention a ne pas load deux fois
		// appeler un dossier puis appeler un sous-dossier fait appeler deux fois
        
		LoadPrefabs(""); //Load all prefabs in folder 'Resources/...'

        //LoadPrefabs("Player"); 
		//LoadPrefabs("Prefabs");
		//LoadPrefabs("Objets Trouves");
        //Debug.Log("PREFABS ARE LOADED");
	}
	
	public static void LoadPrefabs(string PrefabPath)
	{
        //Add to Object list
        Object[] ObjectArray = Resources.LoadAll(PrefabPath,typeof(GameObject));

        //Populate gameObjectList
		foreach (Object o in ObjectArray) {
			if(!gameObjectList.ContainsKey(o.name.GetHashCode())) {
				gameObjectList.Add(o.name.GetHashCode(),(GameObject) o); 
                Debug.Log($"{o.name} added to Prefab list"); } }

        //Populate NetworkPrefab list
        foreach (GameObject go in gameObjectList.Values) {
            if (go.GetComponent<NetworkObject>()) {
                NetworkManager.Singleton.AddNetworkPrefab(go); 
                Debug.Log($"{go.name} added to NetworkPrefab list"); } }

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
