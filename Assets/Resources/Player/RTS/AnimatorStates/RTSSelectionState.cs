using System.Collections.Generic;
using UnityEngine;
using AbstractClasses;


public class RTSSelectionState : StateMachineBehaviour
{   
    Camera cam;
    Animator anim;
    RTSSelection selector;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        anim = animator;
        cam = animator.gameObject.GetComponentInChildren<Camera>();
        selector =  animator.gameObject.GetComponent<RTSSelection>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RTSStateInputs();
        RTSMoveInput();
    }


    void RTSStateInputs() {
        if (Input.GetKeyDown(KeyCode.B)) { anim.SetBool("ConstructionMode", true); }
        if (Input.GetKeyDown(KeyCode.C)) { anim.SetBool("UnitCreationMode", true); } 
    }
    void RTSMoveInput() {
        if (Input.GetMouseButtonDown(1)) { OnRightClick(); }
    }


    void OnRightClick() {
        if (selectionContainsUnits())
            MoveSelectedUnits();
        else
            MoveSelectedBuildingsRallyPoint(); }
        

    void MoveSelectedUnits() {
        Debug.Log("Ordering units to move");
        Vector3 pos = GetMouseGroundHitPosition();
        foreach (GameObject go in selector.currentSelection) {
            go.GetComponent<UnitSystem>()?.MoveUnitToPos(pos, false); 
            go.GetComponent<RTSUnit>()?.MoveUnitToPos(pos, false); } }


    void MoveSelectedBuildingsRallyPoint() {
        Debug.Log("Ordering buildings to move their rally point");
        Vector3 pos = GetMouseGroundHitPosition();
        foreach (GameObject go in selector.currentSelection)
            go.GetComponent<UnitSpawnerSystem>()?.MoveRallyPoint(pos); }


    Vector3 GetMouseGroundHitPosition() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, 3000.0f, (1<<8));
        return hit.point; }


    bool selectionContainsUnits() {
        foreach (GameObject go in selector.currentSelection)
            if (go.tag == "Unit")
                return true;
        return false; }
}
