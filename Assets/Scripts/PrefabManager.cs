using UnityEngine;
using System.Collections;
using System.Collections.Generic;   //Required for Dictionary

public class PrefabManager : ScriptableObject
{
	public static Dictionary<int, Object> objectList = new Dictionary<int, Object>();

	//...

	public static void LoadAllPrefabs(string PrefabPath)
	{
		Object[] ObjectArray = Resources.LoadAll(PrefabPath);

		foreach (Object o in ObjectArray)
			objectList.Add(o.name.GetHashCode(), (Object)o);
	}


	public static Object GetPrefab(string objName)
	{
		Object obj;

		if (objectList.TryGetValue(objName.GetHashCode(), out obj))
			return obj;
		else
		{
			Debug.Log("Object not found");
			return null;
		}
	}
}