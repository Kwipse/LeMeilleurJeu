using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu]
public class LazerAsset : ScriptableObject
{ 
    public GameObject lazerPrefab;
    Lazer lazer;
    public float lenght,diameter;
    public float maxDuration;
    public int dmgUnit;
    public int dmgBat;
    public int force;


    public void LazerToPos(Vector3 pos,Vector3 directionPoint)
    {
        lazer = lazerPrefab.GetComponent<Lazer>();
        lazer.LazerDuration = maxDuration;
        lazer.damageToUnit = dmgUnit;
        lazer.damageToBuilding = dmgBat;
        lazer.outwardForce = force;

        Vector3 direction = directionPoint - pos;


        SpawnManager.SpawnObject(lazerPrefab, pos, Quaternion.FromToRotation(Vector3.forward, direction));
    }

}

