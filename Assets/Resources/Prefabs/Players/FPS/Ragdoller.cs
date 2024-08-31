using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class Ragdoller : MonoBehaviour
{
    Rigidbody rb;
    BoxCollider col;
    CharacterJoint joint;

    public float ragdollTime = 5;

    public void TurnRagdoll()
    {
        Invoke("SelfDestroy", ragdollTime);

        gameObject.GetComponent<HealthSystem>().GetHealthBar().DestroyHealthBar();

        List<GameObject> currentObjP1 = new List<GameObject>();
        List<GameObject> currentObjP2 = new List<GameObject>();
        List<GameObject> currentObjP3 = new List<GameObject>();

        Debug.Log($"Ragdolling");


        if ( gameObject.GetComponent<Animator>()) { gameObject.GetComponent<Animator>().enabled = false; }
        if ( gameObject.GetComponent<Collider>()) { gameObject.GetComponent<Collider>().enabled = false; }
        if ( gameObject.GetComponent<Unit>()) { gameObject.GetComponent<Unit>().enabled = false; }
        if ( gameObject.GetComponent<NavMeshAgent>()) { gameObject.GetComponent<NavMeshAgent>().enabled = false; }
        if ( gameObject.GetComponent<RigBuilder>()) { gameObject.GetComponent<RigBuilder>().enabled = false; }
        if ( gameObject.GetComponent<WeaponSystem>()) { gameObject.GetComponent<WeaponSystem>().UnequipWeapon(); }


        GameObject armature = gameObject.transform.Find("Armature").gameObject;
        armature.AddComponent<Rigidbody>();
        armature.transform.parent.GetComponent<Rigidbody>().isKinematic = true;
        



        for (int i = 0; i < armature.transform.childCount; i++) {
            currentObjP1.Add(armature.transform.GetChild(i).gameObject); }

        foreach (var go in currentObjP1) {
            for (int i = 0; i < go.transform.childCount; i++) {
                currentObjP2.Add(go.transform.GetChild(i).gameObject); } }

        foreach (var go in currentObjP2) {
            for (int i = 0; i < go.transform.childCount; i++) {
                currentObjP3.Add(go.transform.GetChild(i).gameObject); } }
        


        foreach (var go in currentObjP1) { setupRagdollComponents(go, 0.2f); }
        foreach (var go in currentObjP2) { setupRagdollComponents(go, 0.2f); }
        foreach (var go in currentObjP3) { setupRagdollComponents(go, 0.2f); }
    }

    void setupRagdollComponents(GameObject go, float size)
    {
        Rigidbody rb = go.AddComponent<Rigidbody>();
        CapsuleCollider col = go.AddComponent<CapsuleCollider>();
        CharacterJoint joint = go.AddComponent<CharacterJoint>();
        col.radius = size;
        joint.connectedBody = go.transform.parent.GetComponent<Rigidbody>();
    }

    void SelfDestroy()
    {
        SpawnManager.DestroyObject(gameObject);
    }
}
