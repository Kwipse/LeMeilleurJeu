using UnityEngine;
using System.Collections.Generic; 

public class RTSSelection : SelectionSystem
{
    [HideInInspector] public bool isAddingToWaypoints;

    public List<GameObject> groupControl;
    string selectionMode;

    Vector3 boxSelectStartPoint;
    Vector3 boxSelectEndPoint;

    public delegate void RTSModeEvent(string mode); 
    public event RTSModeEvent RTSSwitchedSelectionModeEvent;


    void Start()
    {
        //Init Selection options
        canSelectOwnedObjects = true; 
        canSelectUnit = true;
        canSelectBuilding = true;
        canSelectNotOwnedObjects = false;
        canSelectWeapon = false;
        isAddingToSelection = false;
        isAddingToWaypoints = false;

        //Subscribe to selection events
        ObjectSelectedEvent += OnObjectSelected;
        ObjectUnselectedEvent += OnObjectUnselected;

        SwitchSelectionMode("SelectionVide");
    }

    void Update()
    {
        if (selectionMode == "SelectionVide") { SelectionVideUpdate(); }
        if (selectionMode == "SelectionArmee") { SelectionArmeeUpdate(); }
        if (selectionMode == "SelectionBatiment") { SelectionBatimentUpdate(); }
    }



    //On selection events
    void OnObjectUnselected(GameObject go)
    {
        if (GetSelectionCount() == 0) { SwitchSelectionMode("SelectionVide"); }
    }

    void OnObjectSelected(GameObject go)
    {
        if (selectionMode == "SelectionVide") {
            if (go.tag == "Unit") { SwitchSelectionMode("SelectionArmee"); }
            if (go.tag == "Building") { SwitchSelectionMode("SelectionBatiment"); } }

        if (selectionMode == "SelectionBatiment") {
            if (go.tag == "Unit") { SwitchSelectionMode("SelectionArmee"); } }

        if (selectionMode == "SelectionArmee") { }
    }

    public void FillGroupControl()
    {
        groupControl = new List<GameObject>(GetSelection());
    }

    public void LoadGroupControl()
    {
        if(groupControl != null) 
        {
            SelectList(groupControl);
        }
        else
        {
            Debug.Log($"groupcontrolempty");
        }
    }

    //Switch selection mode
    void SwitchSelectionMode(string mode)
    {
        //End current mode
        if (selectionMode == "SelectionVide") { OnSelectionVideEnd(); }
        if (selectionMode == "SelectionArmee") { OnSelectionArmeeEnd(); }
        if (selectionMode == "SelectionBatiment") { OnSelectionBatimentEnd(); }

        //Start new mode
        if (mode == "SelectionVide") { OnSelectionVideStart(); }
        if (mode == "SelectionArmee") { OnSelectionArmeeStart(); }
        if (mode == "SelectionBatiment") { OnSelectionBatimentStart(); }

        //Update mode 
        selectionMode = mode;
        //Debug.Log($"Entered {mode} mode");

        //Fire switch selection mode event
        if (RTSSwitchedSelectionModeEvent != null)
            RTSSwitchedSelectionModeEvent(selectionMode);
    }



    //States
    void OnSelectionVideStart() { }
    void SelectionVideUpdate() { }
    void OnSelectionVideEnd() { }

    void OnSelectionArmeeStart() {
        UnselectByTag("Building");
        canSelectBuilding = false; }
    void SelectionArmeeUpdate() { }
    void OnSelectionArmeeEnd() {
        canSelectBuilding = true; }

    void OnSelectionBatimentStart() { }
    void SelectionBatimentUpdate() { }
    void OnSelectionBatimentEnd() { }



    //Box Selection
    public void StartBoxSelection(Vector3 startPoint) {
        boxSelectStartPoint = startPoint;
        boxSelectEndPoint = startPoint; }

    public void UpdateBoxSelection(Vector3 updatePoint) {
        boxSelectEndPoint = updatePoint;
        ColorManager.DrawBox(boxSelectStartPoint, boxSelectEndPoint); }

    public void EndBoxSelection(Vector3 endPoint) {
        boxSelectEndPoint = endPoint;
        SelectInBox(boxSelectStartPoint, boxSelectEndPoint); }



    //Basic Orders
    public void OrderSelectedUnitsToMove(Vector3 pos) 
    {
        foreach (GameObject go in selection) {
            go.GetComponent<Unit>()?.MoveOrder(pos, isAddingToWaypoints); } 
    }

    public void OrderSelectedBuildingsToMoveRallyPoint(Vector3 pos) 
    {
        foreach (GameObject go in selection)
            go.GetComponent<UnitSpawnerSystem>()?.MoveRallyPoint(pos); 
    }

    public void OrderSelectedSpawnersToCreateUnit(int unitIndex)
    {
        foreach(GameObject go in selection) 
            go.GetComponent<UnitSpawnerSystem>()?.SpawnUnitByIndex(unitIndex);
    }

    public void OrderSelectedUnitsToSkill(Vector3 pos)
    {
        foreach(GameObject go in selection) 
            go.GetComponent<Unit>()?.SkillAction(pos);
    }



    public override void OnDestroy()
    {
        //Unsubscribe to selection events
        ObjectSelectedEvent -= OnObjectSelected;
        ObjectUnselectedEvent -= OnObjectUnselected;

        base.OnDestroy();
    }
}
