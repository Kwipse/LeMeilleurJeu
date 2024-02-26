using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using systems;
using classes;
using scriptablesobjects;


[RequireComponent(typeof(RTSSelection))]

[CreateAssetMenu]
public class RTSUI : UI 
{
    RTSPlayer RP;
    RTSSelection SS;
    GoldSystem RTSGold;

    List<GameObject> UISelection;

    public override void OnSetUI(GameObject RTSPlayer) 
    {
        RP = RTSPlayer.GetComponent<RTSPlayer>();
        SS = RTSPlayer.GetComponent<RTSSelection>();
        RTSGold = RP.gold;

        UISelection = new List<GameObject>();

        //Subscribe to rts mode events
        RP.RTSSwitchedModeEvent += OnRTSSwitchedMode;
        SS.RTSSwitchedSelectionModeEvent += OnRTSSwitchedMode;

        //Subscribe to selection events
        SS.ObjectSelectedEvent += OnObjectSelected;
        SS.ObjectUnselectedEvent += OnObjectUnselected;

        //Subscribe to gold events
        RTSGold.goldRes.ChangeEvent += OnGoldChanged;
        OnGoldChanged(RTSGold.GetCurrentGold()); //Init Gold text
    }



    //Events
    void OnRTSSwitchedMode(string mode)
    {
        SetUIText("ModeText", mode);

        if (mode == "Construction") { }
        if (mode == "SelectionVide") { }
        if (mode == "SelectionBatiment") { }
        if (mode == "SelectionArmee") { }
    }

    void OnObjectSelected(GameObject go) 
    {
        SortListByName(SS.GetSelection());
        UpdateSubSelection(); 
        RefreshSubSelection();
    }

    void OnObjectUnselected(GameObject go) 
    {
        UpdateSubSelection(); 
    }


    void OnGoldChanged(int newGoldAmount)
    {
        SetUIText("ResourceText", $"Gold : {newGoldAmount.ToString()}");
    }



    //Trucs de Jakes
    void UpdateSubSelection(string name = null)
    {
        if(name != null) {
            //on recupere les gameobject de meme nom 
            // jai tout piqué
            UISelection = SS.GetSelection() 
                .Where(go => go.name.Contains(name))
                .ToList(); }
        else {
            // on prend le nom du premier objet
            UISelection = SS.GetSelection()
                .Where(go => go.name.Contains(SS.GetSelection()[0].name))
                .ToList(); }
    }

    void RefreshSubSelection()
    {
        //on affiche le nom de la subselection
        //on met a jour les ordres possibles 
        if(SS.GetSelectionCount()>0)
            SetUIText("SelectionText", UISelection[0].name);
        else
            SetUIText("SelectionText", "Aucune Selection");
    }

    List<GameObject> SortListByName(List<GameObject> gameObjectList)
    {
        //List<GameObject> sortedList = gameObjectList.OrderBy(go=>go.name).ToList();   
        return gameObjectList.OrderBy(go=>go.name).ToList();   
    }

    public void UpdateCommands()
    {

    }
}
