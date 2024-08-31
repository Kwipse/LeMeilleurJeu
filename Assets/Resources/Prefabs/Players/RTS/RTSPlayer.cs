using UnityEngine;

[RequireComponent(typeof(RTSCamera))]
[RequireComponent(typeof(RTSSelection))]

public class RTSPlayer : SyncedBehaviour, IWaitForGameSync
{
    public BuilderSystem builder;
    public GoldSystem gold;
    public RTSUI UI;

    string rtsMode;
    string selectionMode;

    RTSSelection selection;
    RTSCamera cam;

    public delegate void RTSModeEvent(string mode); 
    public event RTSModeEvent RTSSwitchedModeEvent;

    void Awake()
    {
        gold = ScriptableObject.Instantiate(gold);
        builder = ScriptableObject.Instantiate(builder);
        UI = ScriptableObject.Instantiate(UI);
        selection = GetComponent<RTSSelection>();
        cam = GetComponent<RTSCamera>();
    }

    public override void StartAfterGameSync()
    {
        if (IsOwner)
        {
            //gold.SetGold();
            UI.SetUI(gameObject);
            selection.RTSSwitchedSelectionModeEvent += OnSwitchedSelectionMode;
            SwitchRtsMode("Selection");
        }

        if (!IsOwner)
        {
            enabled = false;
        }
    }

    //On switch selection mode event
    void OnSwitchedSelectionMode(string mode) 
    {
        selectionMode = mode; 
    }


    void Update()
    {
        ModeInputs();
        GroupControlInputs();

        if (rtsMode == "Selection") {
            SelectionInputs();
            if (selectionMode == "SelectionVide") { SelectionVideInputs(); }
            if (selectionMode == "SelectionArmee") { SelectionArmeeInputs(); }
            if (selectionMode == "SelectionBatiment") { SelectionBatimentInputs(); } }

        if (rtsMode == "Construction") {
            ConstructionInputs(); 
            ConstructionUpdate(); }
    }

    public override void OnDestroy()
    {
        //Unsubscribe to selection mode events
        selection.RTSSwitchedSelectionModeEvent -= OnSwitchedSelectionMode;

        base.OnDestroy();
    }



    //Inputs
    void ModeInputs()
    {
        if (Input.GetKeyDown(KeyCode.B)) {SwitchRtsMode("Construction"); }
        if (Input.GetKeyDown(KeyCode.Escape)) { SwitchRtsMode("Selection"); } 
    }

    void GroupControlInputs()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1   )   )    
        {
            Debug.Log($"cg_fillgroupcontrol");
            selection.FillGroupControl();

        }
        if(Input.GetKeyDown(KeyCode.Alpha2   )  ) 
        {
            Debug.Log($"cg_loadgroupcontrol");
            selection.LoadGroupControl();
        }
    }

    void SelectionVideInputs()
    {

    }

    void SelectionInputs()
    {
        if (Input.GetMouseButtonDown(0)) { selection.StartBoxSelection(cam.GetMouseGroundHitPosition()); }
        if (Input.GetMouseButton(0)) { selection.UpdateBoxSelection(cam.GetMouseGroundHitPosition()); }
        if (Input.GetMouseButtonUp(0)) { selection.EndBoxSelection(cam.GetMouseGroundHitPosition()); }
        if (Input.GetKeyDown(KeyCode.LeftShift)) { selection.isAddingToSelection = true ; }
        if (Input.GetKeyUp(KeyCode.LeftShift)) { selection.isAddingToSelection = false ; }
    }

    void SelectionArmeeInputs()
    {
        //SelectionArmee inputs
        if (Input.GetMouseButtonDown(1)) { selection.OrderSelectedUnitsToMove(cam.GetMouseNavmeshPosition()); }
        if (Input.GetKeyDown(KeyCode.LeftShift)) { selection.isAddingToWaypoints = true ; }
        if (Input.GetKeyUp(KeyCode.LeftShift)) { selection.isAddingToWaypoints = false ; }
        if (Input.GetKeyUp(KeyCode.S)) { selection.OrderSelectedUnitsToSkill(cam.GetMouseNavmeshPosition()); }
    }

    void SelectionBatimentInputs()
    {
        //SelectionBatiment inputs
        if (Input.GetMouseButtonDown(1)) { selection.OrderSelectedBuildingsToMoveRallyPoint(cam.GetMouseGroundHitPosition()); } 
        if (Input.GetKeyDown(KeyCode.W)) { selection.OrderSelectedSpawnersToCreateUnit(0); }
        if (Input.GetKeyDown(KeyCode.X)) { selection.OrderSelectedSpawnersToCreateUnit(1); }
    }

    void ConstructionInputs()
    {
        //Back to Selection Mode
        if (Input.GetMouseButtonDown(1)) { SwitchRtsMode("Selection"); }

        //Blueprint Inputs 
        if (Input.GetKeyDown(KeyCode.W)) { builder.SelectBlueprint(0); }
        if (Input.GetKeyDown(KeyCode.X)) { builder.SelectBlueprint(1); }
        if (Input.GetKey(KeyCode.A)) { builder.RotateBlueprint(true); }
        if (Input.GetKey(KeyCode.E)) { builder.RotateBlueprint(false); }
        if (Input.mouseScrollDelta.y < 0) { builder.SelectNextBlueprint(); }
        if (Input.mouseScrollDelta.y > 0) { builder.SelectPreviousBlueprint(); }

        //Construction Input
        if (Input.GetKeyDown(KeyCode.LeftShift))  builder.keepBlueprintOnConstruction = true; 
        if (Input.GetKeyUp(KeyCode.LeftShift)) builder.keepBlueprintOnConstruction = false;
        if (Input.GetMouseButtonUp(0)) 
        {
            if (!builder.isBlueprintAllowed)  { return; }
            builder.ConstructCurrentBlueprint(); 
            if (!builder.keepBlueprintOnConstruction) { 
                SwitchRtsMode("Selection");
            }
        }
    }



    // Mode switchers
    void SwitchRtsMode(string mode)
    {
        //End current mode
        if (rtsMode == "Selection") {/*OnSelectionEnd();*/}
        if (rtsMode == "Construction") { OnConstructionEnd(); }

        //Start new mode
        if (mode == "Selection") {/*OnSelectionStart();*/}
        if (mode == "Construction") { OnConstructionStart(); }

        //Update mode 
        rtsMode = mode;
        //Debug.Log($"Entered {mode} mode");

        //Fire switch mode event
        if (RTSSwitchedModeEvent != null)
            RTSSwitchedModeEvent(rtsMode);
    }


    //Selection state
    void OnSelectionStart() { }
    void SelectionUpdate() { }
    void OnSelectionEnd() { }



    //Construction state 
    void OnConstructionStart() {
        cam.isZoomActive = false; }

    void ConstructionUpdate() {
        if (builder.currentBlueprint) {
            builder.MoveBlueprintToHit(cam.GetMouseGroundHit()); } }

    void OnConstructionEnd() {
        builder.ClearBlueprint();
        cam.isZoomActive = true; }



    //Misc
    public RTSCamera GetCamera() { return cam; }
}

