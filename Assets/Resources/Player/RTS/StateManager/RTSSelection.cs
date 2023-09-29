using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;


public class SelectionState : StateMachineBehaviour
{   
    List<GameObject> currentSelection = new List<GameObject>();

    GameObject[] selectedUnits;
    GameObject[] selectedBuildings;

    Vector3 selectionStartPosition;
    Vector3 selectionEndPosition;
    
    bool isAddingToSelection;
    Animator anim;
    Camera cam;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        anim = animator;
        cam = animator.gameObject.GetComponentInChildren<Camera>();
        isAddingToSelection = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RTSStateInputs();
        SelectionInput();
    }


    void InitializeBoxSelection()
    {
        selectionStartPosition = GetMouseGroundHitPosition();
        Debug.Log($"SelectionStarted");
    }

    void UpdateBoxSelection()
    {
        selectionEndPosition = GetMouseGroundHitPosition();
        ColorManager.DrawBounds(GetBoxBoundsFromTwoPoints(selectionStartPosition, selectionEndPosition));
    }

    void BoxSelection()
    {
        Bounds boundSelection = GetBoxBoundsFromTwoPoints(selectionStartPosition, selectionEndPosition);
        List<GameObject> objectsInBound = GetObjectsInBounds(boundSelection);
        List<GameObject> newSelection = GetOwnedObjectsInSelection(objectsInBound); //Remove ennemies & nonSelectables

        AddToCurrentSelection(newSelection);
    }

    Bounds GetBoxBoundsFromTwoPoints(Vector3 p1, Vector3 p2)
    {
        Bounds b = new Bounds();
        b.SetMinMax(Vector3.Min(p1, p2), Vector3.Max(p1, p2));
        return b;
    }

    List<GameObject> GetObjectsInBounds(Bounds b)
    {
        List<GameObject> newSelection = new List<GameObject>();

        GameObject[] goUnits = GameObject.FindGameObjectsWithTag("Unit");
        GameObject[] goBuildings = GameObject.FindGameObjectsWithTag("Building");

        //Debug.Log($"Testing for {goUnits.Length} unit(s), and {goBuildings.Length} building(s)");
        foreach (GameObject go in goUnits)
            if (isObjectInBounds(go, b)) newSelection.Add(go); 
        foreach (GameObject go in goBuildings)
            if (isObjectInBounds(go, b)) newSelection.Add(go); 

        //Debug.Log($"Found {newSelection.Count} objects in bounds");
        return newSelection;

    }

    bool isObjectInBounds(GameObject go, Bounds b)
    {
        Vector3 pos = go.transform.position;

        if (pos.x < b.min.x) return false;
        if (pos.z < b.min.z) return false;
        if (pos.x > b.max.x) return false; 
        if (pos.z > b.max.z) return false;

        //Debug.Log($"{go.name} is in bounds");
        return true;
    }

    List<GameObject> GetOwnedObjectsInSelection(List<GameObject> selection)
    {
        List<GameObject> newSelection = new List<GameObject>();
        ulong playerId = NetworkManager.Singleton.LocalClientId;

        foreach(GameObject go in selection) {
            if (playerId == go.GetComponent<NetworkObject>()?.OwnerClientId) 
                newSelection.Add(go); }
        
        return newSelection;
    }

    void AddToCurrentSelection(List<GameObject> selection)
    {
            if (!isAddingToSelection)
                currentSelection.Clear();

            currentSelection.AddRange(selection.Except(currentSelection)); 

            Debug.Log($"Current selection has {currentSelection.Count} objects");
    }

    Vector3 GetMouseGroundHitPosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, 3000.0f, (1<<8));
        return hit.point; 
    }

    void RTSStateInputs()
    {
        if (Input.GetKeyDown(KeyCode.B)) { anim.SetBool("ConstructionMode", true);}
        if (Input.GetKeyDown(KeyCode.C)) { anim.SetBool("sbireCreation", true); }
    }
    void SelectionInput()
    {
        if (Input.GetMouseButtonDown(0)) { InitializeBoxSelection(); }
        if (Input.GetMouseButton(0)) { UpdateBoxSelection(); }
        if (Input.GetMouseButtonUp(0)) { BoxSelection(); }
        if (Input.GetKeyDown(KeyCode.LeftShift)) isAddingToSelection = true ;
        if (Input.GetKeyUp(KeyCode.LeftShift)) isAddingToSelection = false ;
    }

}
