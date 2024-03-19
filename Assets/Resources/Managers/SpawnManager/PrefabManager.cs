using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PrefabManager : ScriptableObject
{
	// Declare le dictionnaire  ObjCode-Obj
	public static Dictionary<int, GameObject> PrefabList = new Dictionary<int, GameObject>();


	public static void LoadAllPrefabs()
	{
        PrefabList.Clear();

		// attention a ne pas load deux fois
		// appeler un dossier puis appeler un sous-dossier fait appeler deux fois
        
		LoadPrefabs(""); //Load all prefabs in folder 'Resources/...'
        //Debug.Log("PREFABS ARE LOADED");
	}
	
	public static void LoadPrefabs(string PrefabPath)
	{
        //Add to Object list
        Object[] ObjectArray = Resources.LoadAll(PrefabPath,typeof(GameObject));

        //Populate PrefabList
		foreach (Object o in ObjectArray) {
			if(!PrefabList.ContainsKey(o.name.GetHashCode())) {
				PrefabList.Add(o.name.GetHashCode(),(GameObject) o); 
                //Debug.Log($"{o.name} added to Prefab list");
            } }

        //Populate NetworkPrefab list
        foreach (GameObject go in PrefabList.Values) {
            if (go.GetComponent<NetworkObject>()) {
                NetworkManager.Singleton.AddNetworkPrefab(go); 
                //Debug.Log($"{go.name} added to NetworkPrefab list");
            } }

	}


	public static GameObject GetPrefab(string prefabName)
	{
		GameObject obj;

		if (PrefabList.TryGetValue(prefabName.GetHashCode(), out obj))
			return obj;
		else
		{
			Debug.Log(prefabName + " not found");
			return null;
		}
	}

}
