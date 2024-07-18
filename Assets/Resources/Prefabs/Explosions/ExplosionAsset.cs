using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using managers;

namespace scriptablesobjects
{
    [CreateAssetMenu]
    public class ExplosionAsset : ScriptableObject
    { 
        public GameObject explosionPrefab;
        Explosion explosion;
        public bool resizable=true;
        public float size;
        public float duration;
        public int dmgUnit;
        public int dmgBat;
        public int force;


        public void ExplodeAtPos(Vector3 pos)
        {
            if(resizable)explosionPrefab.transform.localScale = new Vector3 (size, size, size);
            explosion = explosionPrefab.GetComponent<Explosion>();
            explosion.ExplosionDuration = duration;
            explosion.damageToUnit = dmgUnit;
            explosion.damageToBuilding = dmgBat;
            explosion.outwardForce = force;

            SpawnManager.SpawnObject(explosionPrefab, pos, Quaternion.identity);
        }

    }
}
