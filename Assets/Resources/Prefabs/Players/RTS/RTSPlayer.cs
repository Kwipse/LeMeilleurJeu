using Unity.Netcode;
using UnityEngine;
using systems;
using scriptablesobjects;

namespace classes {

    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(RTSCamera))]
    [RequireComponent(typeof(RTSSelection))]

    public class RTSPlayer : NetworkBehaviour
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

        public override void OnNetworkSpawn() {
            if (!IsOwner) { enabled = false; } }

        void Awake()
        {
            selection = GetComponent<RTSSelection>();
            cam = GetComponent<RTSCamera>();
        }

        void Start()
        {
            UI.SetUI(gameObject);

            //Subscribe to selection mode events
            selection.RTSSwitchedSelectionModeEvent += OnSwitchedSelectionMode;
            SwitchRtsMode("Selection");
        }

        //On switch selection mode event
        void OnSwitchedSelectionMode(string mode) 
        {
            selectionMode = mode; 
        }


        void Update()
        {
            ModeInputs();

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

        void SelectionInputs()
        {
            if (Input.GetMouseButtonDown(0)) { selection.StartBoxSelection(GetMousePos()); }
            if (Input.GetMouseButton(0)) { selection.UpdateBoxSelection(GetMousePos()); }
            if (Input.GetMouseButtonUp(0)) { selection.EndBoxSelection(GetMousePos()); }
            if (Input.GetKeyDown(KeyCode.LeftShift)) selection.isAddingToSelection = true ;
            if (Input.GetKeyUp(KeyCode.LeftShift)) selection.isAddingToSelection = false ;
        }

        void SelectionVideInputs()
        {

        }

        void SelectionArmeeInputs()
        {
            //SelectionArmee inputs
            if (Input.GetMouseButtonDown(1)) { selection.OrderSelectedUnitsToMove(GetMousePos()); }
        }

        void SelectionBatimentInputs()
        {
            //SelectionBatiment inputs
            if (Input.GetMouseButtonDown(1)) { selection.OrderSelectedBuildingsToMoveRallyPoint(GetMousePos()); } 
            if (Input.GetKeyDown(KeyCode.W)) { selection.OrderSelectedSpawnersToCreateUnit(0); }
            if (Input.GetKeyDown(KeyCode.X)) { selection.OrderSelectedSpawnersToCreateUnit(1); }
        }

        void ConstructionInputs()
        {
            //Back to Selection Mode
            if (Input.GetMouseButtonDown(1)) { SwitchRtsMode("Selection"); }

            //Blueprint Inputs 
            if (Input.GetKeyDown(KeyCode.W)) builder.SelectBlueprint(0);
            if (Input.GetKeyDown(KeyCode.X)) builder.SelectBlueprint(1);
            if (Input.mouseScrollDelta.y < 0) builder.SelectNextBlueprint();
            if (Input.mouseScrollDelta.y > 0) builder.SelectPreviousBlueprint();

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
                builder.MoveBlueprintToPosition(GetMousePos()); } }

        void OnConstructionEnd() {
            builder.ClearBlueprint();
            cam.isZoomActive = true; }



        //Misc
        Vector3 GetMousePos() { 
            return cam.GetMouseGroundHitPosition(); }
    }
}

