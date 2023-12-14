using UnityEngine;
using systems;

public class RTSUnitCreationState : StateMachineBehaviour
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

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RTSStateInputs();
        RTSUnitCreationInputs();
    }
    
    void RTSStateInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { QuitUnitCreationMode(); }

        if (Input.GetMouseButtonDown(0)) { QuitUnitCreationMode(); }
        if (Input.GetMouseButtonDown(1)) { QuitUnitCreationMode(); }
    }

    void RTSUnitCreationInputs()
    {
        if (Input.GetKeyDown(KeyCode.W)) { CreateUnit(0); }
        if (Input.GetKeyDown(KeyCode.X)) { CreateUnit(1); }
    }

    void CreateUnit(int unitIndex)
    {
        foreach(GameObject go in selector.currentSelection) 
            go.GetComponent<UnitSpawnerSystem>()?.SpawnUnit(unitIndex);

    }

    void QuitUnitCreationMode()
    {
        anim.SetBool("UnitCreationMode", false);
    }
}
