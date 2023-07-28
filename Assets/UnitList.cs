using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Tools.NetStats;
using UnityEngine;

public class UnitList : MonoBehaviour
{
    public List<GameObject> unitsList = new List<GameObject>();

    private static UnitList _instance;
    private static UnitList Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void AddUnit(GameObject unit)
    {
        unitsList.Add(unit);
    }
    public void DelUnit(GameObject unit)
    {
        unitsList.Remove(unit);
    }
}

