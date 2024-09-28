using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BuilderSystem : ScriptableObject
{
    //Définir les batiments par defaut dans l'éditeur
    public List<GameObject> AvailableBuildings;

    [HideInInspector] public GameObject currentBlueprint;
    [HideInInspector] public bool isBlueprintAllowed = true;
    [HideInInspector] public bool keepBlueprintOnConstruction = false;

    int currentBlueprintNumber;
    Building currentBuildingScript;

    List<Collider> currentBlueprintTriggers = new List<Collider>();

    float rotationSpeed = 1;



    //Availables Buildings
    public void AddAvailableBuilding(GameObject buildingPrefab) {
        AvailableBuildings.Add(buildingPrefab); }

    public void RemoveAvailableBuilding(GameObject buildingPrefab) {
        AvailableBuildings.Remove(buildingPrefab); }



    //Blueprint selection
    void InitBlueprint() 
    {
        //Subscribe to blueprint trigger events
        currentBuildingScript = currentBlueprint.GetComponent<Building>();
        currentBuildingScript.TriggerEnterEvent += OnBlueprintTriggerEnter;
        currentBuildingScript.TriggerExitEvent += OnBlueprintTriggerExit;

        ColorBlueprint();
    }

    public void SelectBlueprint(int blueprintPrefabNumber = 0) //default to the first available building
    {
        //Hide current Blueprint
        if (currentBlueprint) ClearBlueprint();

        //InitNewBlueprint
        currentBlueprintNumber = blueprintPrefabNumber;
        currentBlueprint = Instantiate(AvailableBuildings[blueprintPrefabNumber]);
        InitBlueprint();
    }

    public void SelectNextBlueprint() 
    {
        int totalBp = AvailableBuildings.Count;
        int currBp = currentBlueprintNumber;
        int nextBlueprintNumber = ((currBp + 1) + totalBp) % totalBp ; //Goes back to 0 when over the totalBp
        SelectBlueprint(nextBlueprintNumber);
    }

    public void SelectPreviousBlueprint() 
    {
        int totalBp = AvailableBuildings.Count;
        int currBp = currentBlueprintNumber;
        int previousBlueprintNumber = ((currBp - 1) + totalBp) % totalBp ; //Goes back to totalBp when below 0
        SelectBlueprint(previousBlueprintNumber); 
    }

    public void ClearBlueprint() 
    {
        //Unsubscribe to blueprint trigger events
        currentBuildingScript.TriggerEnterEvent -= OnBlueprintTriggerEnter;
        currentBuildingScript.TriggerExitEvent -= OnBlueprintTriggerExit;


        //Destroy blueprint and clear trigger list
        Destroy(currentBlueprint);
        currentBlueprintTriggers.Clear();
    }



    //Blueprint Movement
    public void MoveBlueprintToHit(RaycastHit hit) {
        currentBlueprint.transform.position = hit.point;
        currentBlueprint.transform.rotation *= Quaternion.FromToRotation(currentBlueprint.transform.up, hit.normal);
    }
    public void MoveBlueprintToPosition(Vector3 position) { currentBlueprint.transform.position = position; }
    public void RotateBlueprintToQuaternion(Quaternion rotation) { currentBlueprint.transform.rotation = rotation; }
    public void RotateBlueprint(bool rotateClockwise) { currentBlueprint.transform.Rotate(Vector3.up * rotationSpeed/3 * (rotateClockwise?1:-1), Space.Self); }



    //Blueprint triggers 
    void OnBlueprintTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "ground") {return;} 
        //Debug.Log($"Builder : blueprint triggered by {col.name}");
        currentBlueprintTriggers.Add(col);
        if (currentBlueprintTriggers.Count == 1) {
            isBlueprintAllowed = false;
            ColorBlueprint(); }
    }

    void OnBlueprintTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "ground") {return;} 
        //Debug.Log($"Builder : blueprint not triggered anymore by {col.name}");
        currentBlueprintTriggers.Remove(col);
        if (currentBlueprintTriggers.Count == 0) {
            isBlueprintAllowed = true;
            ColorBlueprint(); }
    }


    //Blueprint Misc
    void ColorBlueprint() {
        ColorManager.SetBlueprintColor(currentBlueprint, isBlueprintAllowed); }



    //Construction
    public void ConstructCurrentBlueprint() {
        ConstructBuilding(currentBlueprintNumber, currentBlueprint.transform.position, currentBlueprint.transform.rotation); }

    public void ConstructBuilding(int buildingNumber, Vector3 position, Quaternion rotation) 
    {
        if (!isBlueprintAllowed) return;
        SpawnManager.SpawnObject(AvailableBuildings[buildingNumber], position, rotation); 
        if (!keepBlueprintOnConstruction) ClearBlueprint();
    }


}
