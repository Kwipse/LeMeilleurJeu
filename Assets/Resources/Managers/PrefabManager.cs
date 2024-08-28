using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PrefabManager : ScriptableObject
{
	// Declare le dictionnaire  ObjCode-Obj
	public static Dictionary<int, GameObject> PrefabList = new Dictionary<int, GameObject>();

    static int amountToPool = 20;
    static List<GameObject> objectsToPool;
    static Dictionary<GameObject, List<GameObject>> pools;

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
		foreach (Object o in ObjectArray)   {
			if(!PrefabList.ContainsKey(o.name.GetHashCode()))   {
				PrefabList.Add(o.name.GetHashCode(),(GameObject) o); 
                //Debug.Log($"{o.name} added to Prefab list");
            } }

        //Populate NetworkPrefab list
        foreach (GameObject go in PrefabList.Values) {
            if (go.GetComponent<NetworkObject>() && (go.name != "PlayerController")) {
                NetworkManager.Singleton.AddNetworkPrefab(go); 
                //Debug.Log($"{go.name} added to NetworkPrefab list");
            } 
            else
            {
                //Debug.Log($"{go.name} has not been added to NetworkPrefab list");
            }
        }

        LoadPools();

	}

    static void LoadPools()
    {
        //Pooling
        objectsToPool = new List<GameObject>();
        pools = new Dictionary<GameObject, List<GameObject>>();

        foreach (GameObject go in PrefabList.Values) {
            if (go.tag == "Projectile") {
                objectsToPool.Add(go); } }

        foreach (GameObject go in objectsToPool)
        {
            GameObject tmpGo;
            List<GameObject> tmpPool = new List<GameObject>();

            for(int i = 0; i < amountToPool; i++)
            {
                tmpGo = Instantiate(go);
                tmpGo.GetComponent<NetworkObject>().Spawn();
                int id = ObjectManager.AddObjectToList(tmpGo);
                tmpGo.GetComponent<NetworkObject>().Despawn(false);
                tmpGo.SetActive(false);
                tmpPool.Add(tmpGo);
            }
            pools.Add(go,tmpPool);
        }
        Debug.Log($"POOLS ARE READY, {pools.Count} POOLS !!");
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

    public static GameObject GetPooledObject(GameObject prefab)
    {
        List<GameObject> pool = pools[prefab];

        for (int i=0 ; i < amountToPool ; i++) {
            if (!pool[i].activeInHierarchy) {
                return pool[i]; } }

        return null;
    }
}
