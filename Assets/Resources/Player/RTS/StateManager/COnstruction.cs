using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionMode : StateMachineBehaviour
{
    public GameObject Barracks;
    public GameObject GoldMine;

    public Material mBlueprintAllowed;
    public Material mBlueprintNotAllowed;

    GameObject currentBlueprintPrefab;
    GameObject selectedBlueprintPrefab;

    GameObject Blueprint;

    public bool isBlueprintAllowed;
    bool setToDestroy;

    Animator anim;
    GameObject go;
    Camera cam;
    Ray ray;
    RaycastHit hit;



    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        anim = animator;
        cam = animator.gameObject.GetComponentInChildren<Camera>();
        Debug.Log("Construction Mode");

        currentBlueprintPrefab = null;
        selectedBlueprintPrefab = null;
        setToDestroy = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerInput();
        UpdateBlueprint();
    }


    void UpdateBlueprint()
    {
        if (!selectedBlueprintPrefab) { 
            return;} 

        if (currentBlueprintPrefab != selectedBlueprintPrefab) { 
            CreateBlueprint(); }

        if (Blueprint){
            Blueprint.transform.position = GetMouseGroundHit().point; 
        }
    }


    void CreateBlueprint()
    {
        if (Blueprint) {Destroy(Blueprint);}
        currentBlueprintPrefab = selectedBlueprintPrefab;
        Blueprint = Instantiate(currentBlueprintPrefab);
    }


    void ConstructBuilding()
    {
        if (!Blueprint.GetComponent<BatimentSystem>().isBlueprintAllowed) { return; }

        Vector3 pos = Blueprint.transform.position;
        Quaternion rot = Blueprint.transform.rotation;

        SpawnManager.SpawnObject(currentBlueprintPrefab, pos, rot);

        QuitConstructionMode();

    }

    RaycastHit GetMouseGroundHit()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, 3000.0f, (1<<8));
        return hit; 
    }


    void QuitConstructionMode()
    {
        currentBlueprintPrefab = null;
        selectedBlueprintPrefab = null;

        if (Blueprint && !setToDestroy) {
            setToDestroy = true;
            Destroy(Blueprint);}

        anim.SetBool("ConstructionMode", false);
    }


    void PlayerInput()
    {
        if (Input.GetMouseButtonDown(0)) { ConstructBuilding(); }
        if (Input.GetKeyDown(KeyCode.Escape)) { QuitConstructionMode(); }
        if (Input.GetKeyDown(KeyCode.W)) { selectedBlueprintPrefab = Barracks; }
        if (Input.GetKeyDown(KeyCode.X)) { selectedBlueprintPrefab = GoldMine; }
        if (Input.GetKeyDown(KeyCode.P)) { Debug.Log("Prout!"); }

    }
}
