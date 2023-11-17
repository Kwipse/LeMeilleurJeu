using System.Collections.Generic;
using UnityEngine;

public class RTSConstructionState : StateMachineBehaviour
{
    [HideInInspector]
    public bool isBlueprintAllowed;
    bool keepBlueprintOnConstruct;
    bool setToDestroy;

    List<GameObject> Batiments;
    GameObject Blueprint;
    GameObject currentBlueprintPrefab;
    GameObject selectedBlueprintPrefab;

    Animator anim;
    GameObject rts;
    RTSCamera cam;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Construction Mode");
       
        anim = animator;
        rts = anim.gameObject;
        cam = rts.GetComponent<RTSCamera>();
        Batiments = rts.GetComponent<RTSPlayer>().AvailableBuildings; 

        currentBlueprintPrefab = null;
        selectedBlueprintPrefab = null;
        keepBlueprintOnConstruct = false;
        setToDestroy = false;

        cam.isZoomActive = false;

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerInput();
        UpdateBlueprint();
    }


    void UpdateBlueprint()
    {
        if (!selectedBlueprintPrefab)  
            return;

        if (currentBlueprintPrefab != selectedBlueprintPrefab)  
            CreateBlueprint(); 

        if (Blueprint)
            Blueprint.transform.position = cam.GetMouseGroundHit().point; 
    }

    void CreateBlueprint()
    {
        if (Blueprint)
            Destroy(Blueprint);

        currentBlueprintPrefab = selectedBlueprintPrefab;
        Blueprint = Instantiate(currentBlueprintPrefab);
    }


    void ConstructBuilding()
    {
        if (!Blueprint)
            return;
        if (!Blueprint.GetComponent<BlueprintSystem>().isBlueprintAllowed)
            return; 

        Vector3 pos = Blueprint.transform.position;
        Quaternion rot = Blueprint.transform.rotation;
        SpawnManager.SpawnObject(currentBlueprintPrefab, pos, rot);

        if (!keepBlueprintOnConstruct) 
            QuitConstructionMode();

    }


    void QuitConstructionMode()
    {
        currentBlueprintPrefab = null;
        selectedBlueprintPrefab = null;

        if (Blueprint && !setToDestroy) 
            setToDestroy = true;
            Destroy(Blueprint);

        cam.isZoomActive = true;
        anim.SetBool("ConstructionMode", false);
    }

    void PlayerInput()
    {
        //Selection Input 
        if (Input.GetKeyDown(KeyCode.W))
            selectedBlueprintPrefab = Batiments[0];

        if (Input.GetKeyDown(KeyCode.X))
            selectedBlueprintPrefab = Batiments[1];

        if (Input.mouseScrollDelta.y < 0) //MouseWheel Down
        {
            var currentIndex = Batiments.IndexOf(selectedBlueprintPrefab);
            if (currentIndex < Batiments.Count - 1)
                selectedBlueprintPrefab = Batiments[currentIndex + 1];
        }
       
        if (Input.mouseScrollDelta.y > 0) //MouseWheel Up
        {
            var currentIndex = Batiments.IndexOf(selectedBlueprintPrefab);
            if (currentIndex > 0)
                selectedBlueprintPrefab = Batiments[currentIndex - 1];
        }


        //Construction Input
        if (Input.GetMouseButtonDown(0))  ConstructBuilding(); 
        if (Input.GetKeyDown(KeyCode.LeftShift))  keepBlueprintOnConstruct = true; 
        if (Input.GetKeyUp(KeyCode.LeftShift))  keepBlueprintOnConstruct = false; 

        //Misc
        if (Input.GetKeyDown(KeyCode.Escape))  QuitConstructionMode(); 
        if (Input.GetKeyDown(KeyCode.P))  Debug.Log("Prout!"); 
    }
}
