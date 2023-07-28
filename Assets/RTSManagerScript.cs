using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSManagerScript : MonoBehaviour
{
    /*
     * unité selectionné par le joueur
     */
    public UnitList units ;
    public List<GameObject> selectedUnitsList = new List<GameObject>();




    private static RTSManagerScript _instance;
    private static RTSManagerScript Instance { get { return _instance; } }


    private void Awake()
    {
        if(_instance != null && _instance!=this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;   
        }
        //grab la unit list
        units = GetComponentInChildren<UnitList>();
    }


}
