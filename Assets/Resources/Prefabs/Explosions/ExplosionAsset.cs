using UnityEngine;

[CreateAssetMenu]
public class ExplosionAsset : ScriptableObject
{ 
    public GameObject explosionPrefab;
    Explosion explosion;
    public bool resizable=true;
    public float size;
    public float duration;
    public int defaultDmg = 0;
    public int dmgUnit = 1;
    public int dmgBat = 1;
    public int force;


    public void ExplodeAtPos(Vector3 pos)
    {
        if(resizable)explosionPrefab.transform.localScale = new Vector3 (size, size, size);
        explosion = explosionPrefab.GetComponent<Explosion>();
        explosion.ExplosionDuration = duration;
        explosion.defaultDamage = defaultDmg;
        explosion.damageToUnit = dmgUnit;
        explosion.damageToBuilding = dmgBat;
        explosion.outwardForce = force;

        SpawnManager.SpawnObject(explosionPrefab, pos, Quaternion.identity);
    }
}
