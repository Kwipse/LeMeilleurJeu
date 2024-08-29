using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SelectionSystem : NetworkBehaviour
{
    //Selection
    /*[HideInInspector]*/ public List<GameObject> selection = new List<GameObject>();

    //Selection properties
    [HideInInspector] public bool isAddingToSelection = false;
    [HideInInspector] public bool canSelectOwnedObjects = true;
    [HideInInspector] public bool canSelectNotOwnedObjects = true;
    [HideInInspector] public bool canSelectUnit = true;
    [HideInInspector] public bool canSelectBuilding = true;
    [HideInInspector] public bool canSelectWeapon = true;

    //Event on selection added/removed 
    public delegate void SelectEvent(GameObject go);
    public event SelectEvent ObjectSelectedEvent;
    public event SelectEvent ObjectUnselectedEvent;

    // //misc
    // Vector3 boxSelectStartPoint;
    // Vector3 boxSelectEndPoint;


    public override void OnNetworkSpawn() {
        if (!IsOwner) {enabled = false;} }


    //Selecting
    public void SelectObject(GameObject toSelect, bool forceAddSelection = false) 
    {
        //Contraintes pour la selection
        if ((!canSelectOwnedObjects) && (toSelect.GetComponent<NetworkObject>().IsOwner)) return;
        if ((!canSelectNotOwnedObjects) && (!toSelect.GetComponent<NetworkObject>().IsOwner)) return;
        if ((!canSelectUnit) && (toSelect.tag == "Unit")) return;
        if ((!canSelectBuilding) && (toSelect.tag == "Building")) return;
        if ((!canSelectWeapon) && (toSelect.tag == "Arme")) return;
        if (selection.Contains(toSelect)) return;

        //Est-ce qu'on supprime la selection avant ?
        if ((!isAddingToSelection) && (!forceAddSelection))
            ClearSelection();

        //Add to main selection and fire event
        selection.Add(toSelect);
        //Debug.Log($"Added {toSelect.name} to selection, selection contains {selection.Count} objects");
        if (ObjectSelectedEvent != null)
            ObjectSelectedEvent(toSelect);
    }

    public void SelectList(List<GameObject> toSelectList) 
    {
        if (!isAddingToSelection) {
            ClearSelection(); }

        foreach(var toSelect in toSelectList) {
            SelectObject(toSelect, true); } 
    }

    public void SelectInBox(Vector3 p1, Vector3 p2) 
    {
        if (!isAddingToSelection) {
            ClearSelection(); }

        GameObject[] allUnits = GameObject.FindGameObjectsWithTag("Unit"); //erf
        GameObject[] allBuildings = GameObject.FindGameObjectsWithTag("Building"); //euarf
        GameObject[] allWeapons = GameObject.FindGameObjectsWithTag("Arme"); //euarfldskfjs

        foreach (GameObject unit in allUnits) {
            if (isObjectInBox(unit, p1, p2)) {
                SelectObject(unit, true); } }

        foreach (GameObject building in allBuildings) 
            if (isObjectInBox(building, p1, p2)) 
                SelectObject(building, true); 

        foreach (GameObject weapon in allWeapons) 
            if (isObjectInBox(weapon, p1, p2)) 
                SelectObject(weapon, true); 
    }



    //Unselect & Clear
    public void UnselectObject(GameObject go) {
        //Remove from selection and fire event
        selection.Remove(go);
        //Debug.Log($"Removed {go.name} from selection, selection contains {selection.Count} objects");
        if (ObjectUnselectedEvent != null)
            ObjectUnselectedEvent(go); }

    public void UnselectList(List<GameObject> goList) {
        foreach (var go in goList) {
            UnselectObject(go); } }

    public void UnselectByTag(string tag) {
        foreach(var go in selection) {
            if(go.tag == tag)
                UnselectObject(go); } }

    public void ClearSelection() {
        //On supprime les éléments un par un, sinon on perd les events
        int sCount = selection.Count;
        for (int i = 0; i < sCount; i++) {
            UnselectObject(selection[0]); } }



    //Selection Getters
    public int GetSelectionCount() {
        return selection.Count; }

    public List<GameObject> GetSelection() {
        return selection; }



    //misc
    bool isObjectInBox(GameObject go, Vector3 p1, Vector3 p2) 
    {

        Vector3 pos = go.transform.position;
        Vector3 pmin = Vector3.Min(p1, p2);
        Vector3 pmax = Vector3.Max(p1, p2);
        //Debug.Log($"Box testing for {go.name} @{pos}, between {pmin} & {pmax} ?");

        if (pos.x < pmin.x) return false;
        if (pos.z < pmin.z) return false;
        if (pos.x > pmax.x) return false; 
        if (pos.z > pmax.z) return false;

        //Debug.Log($"{go.name} is in the box");
        return true; 
    }
}
