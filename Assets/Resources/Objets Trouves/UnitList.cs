using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Tools.NetStats;
using UnityEngine;

public class UnitList : MonoBehaviour
{
    public List<GameObject> unitsList,sbire,mob = new List<GameObject>();

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
		if(unit.GetComponent<Unit>().type == "sbire") sbire.Add(unit);
		if(unit.GetComponent<Unit>().type == "mob") mob.Add(unit);
    }
    public void DelUnit(GameObject unit)
    {
        unitsList.Remove(unit);
		if(unit.GetComponent<Unit>().type == "sbire") sbire.Remove(unit);
		if(unit.GetComponent<Unit>().type == "mob") mob.Remove(unit);

    }
}

