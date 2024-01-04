using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;


public class RTSSelection : MonoBehaviour
{   
    [HideInInspector]
    public List<GameObject> currentSelection, mainSubSelection = new List<GameObject>();
    public int subSelectionCount;
    List<GameObject>[] controlGroup = new List<GameObject>[9]; 
    //public GameObject uIPanel;

    Vector3 selectionStartPosition;
    Vector3 selectionEndPosition;
    
    Camera cam;
    bool isAddingToSelection;

    void Start()
    {
        cam = gameObject.GetComponentInChildren<Camera>();
        isAddingToSelection = false;
    }

    void Update()
    {
        RTSSelectionInput();
    }

    void RTSSelectionInput() {
        if (Input.GetMouseButtonDown(0)) { InitializeBoxSelection(); }
        if (Input.GetMouseButton(0)) { UpdateBoxSelection(); }
        if (Input.GetMouseButtonUp(0)) { BoxSelection(); }

        if (Input.GetKeyDown(KeyCode.LeftShift)) isAddingToSelection = true ;
        if (Input.GetKeyUp(KeyCode.LeftShift)) isAddingToSelection = false ;
    }


    void InitializeBoxSelection() {
        selectionStartPosition = GetMouseGroundHitPosition();
        Debug.Log($"SelectionStarted"); }


    void UpdateBoxSelection() {
        selectionEndPosition = GetMouseGroundHitPosition();
        ColorManager.DrawBounds(GetBoxBoundsFromTwoPoints(selectionStartPosition, selectionEndPosition)); }


    void BoxSelection() {
        Bounds boundSelection = GetBoxBoundsFromTwoPoints(selectionStartPosition, selectionEndPosition);
        List<GameObject> objectsInBound = GetObjectsInBounds(boundSelection);
        List<GameObject> newSelection = GetOwnedObjectsInSelection(objectsInBound); //Remove ennemies & nonSelectables
        AddToCurrentSelection(newSelection);
        RTSUnitPanelScript uiPanel = transform.GetComponentInChildren<RTSUnitPanelScript>(); 
        uiPanel.UpdateIconCount();}
        //a refactoriser je pense que cest pas tres propre d'appeler la


    Bounds GetBoxBoundsFromTwoPoints(Vector3 p1, Vector3 p2) {
        Bounds b = new Bounds();
        b.SetMinMax(Vector3.Min(p1, p2), Vector3.Max(p1, p2));
        return b; }


    List<GameObject> GetObjectsInBounds(Bounds b) {
        List<GameObject> newSelection = new List<GameObject>();
        GameObject[] goUnits = GameObject.FindGameObjectsWithTag("Unit");
        GameObject[] goBuildings = GameObject.FindGameObjectsWithTag("Building");

        //Debug.Log($"Testing for {goUnits.Length} unit(s), and {goBuildings.Length} building(s)");
        foreach (GameObject go in goUnits)
            if (isObjectInBounds(go, b)) newSelection.Add(go); 
        foreach (GameObject go in goBuildings)
            if (isObjectInBounds(go, b)) newSelection.Add(go); 

        //Debug.Log($"Found {newSelection.Count} objects in bounds");
        return newSelection; }


    bool isObjectInBounds(GameObject go, Bounds b) {
        Vector3 pos = go.transform.position;
        if (pos.x < b.min.x) return false;
        if (pos.z < b.min.z) return false;
        if (pos.x > b.max.x) return false; 
        if (pos.z > b.max.z) return false;
        //Debug.Log($"{go.name} is in bounds");
        return true; }


    List<GameObject> GetOwnedObjectsInSelection(List<GameObject> selection) {
        List<GameObject> newSelection = new List<GameObject>();
        ulong playerId = NetworkManager.Singleton.LocalClientId;

        foreach(GameObject go in selection) {
            if (playerId == go.GetComponent<NetworkObject>()?.OwnerClientId) 
                newSelection.Add(go); }
        
        return newSelection; }


    void AddToCurrentSelection(List<GameObject> selection) {
        //si shift n'est pas appuyé on nettoie les list
        if (!isAddingToSelection) 
        {
            currentSelection.Clear();
            mainSubSelection.Clear();
        }
        //on ajoute la selection a la liste sauf les unités déja compté
        currentSelection.AddRange(selection.Except(currentSelection));
        // on trie la selection
        currentSelection = SortListByName(currentSelection);
        /*
        on adapte la sub selection
        avec shift on ajoute les unités du meme type 
        sans shift on prend le premier type d'unité
        */ 
         if (!isAddingToSelection) 
        {
           UpdateSubSelection();
        }
        else
        {
            UpdateSubSelection(currentSelection[0].name);
        }
        Debug.Log($"Current selection has {currentSelection.Count} objects"); }


    Vector3 GetMouseGroundHitPosition() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, 3000.0f, (1<<8));
        return hit.point; }

    public int CurrentSelectionLenght()
    {
        return currentSelection.Count;
    }
    public List<GameObject> MainSubSelection()
    {
        return mainSubSelection;
    }

    List<GameObject> SortListByName(List<GameObject> gameObjectList)
    {
        //List<GameObject> sortedList = gameObjectList.OrderBy(go=>go.name).ToList();   
        return gameObjectList.OrderBy(go=>go.name).ToList();   
    }

    void UpdateSubSelection(string name = null)
    {
        if(name != null)
        {
            //on recupere les gameobject de meme nom 
            // jai tout piqué
            mainSubSelection = currentSelection
    .Where(go => go.name.Contains(name))
    .ToList();

        }
        else
        {
            // on prend le nom du premier objet
            mainSubSelection = currentSelection
    .Where(go => go.name.Contains(currentSelection[0].name))
    .ToList();
        }
    }


}
