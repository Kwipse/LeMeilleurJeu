using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Saucisse : NetworkBehaviour
{
    public int Speed = 1000;

    public int DegatsAuxUnites = 100;
    public int DegatsAuxBatiments = 100;
    public int DegatsAuxJoueurs = 100;

    int dmg;

    public override void OnNetworkSpawn() 
    {
       ColorManager.SetObjectColors(gameObject);

       if (IsOwner)
       {
           Rigidbody rb = GetComponent<Rigidbody>();
           rb.AddForce(gameObject.transform.forward * Speed);
       }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!IsOwner) {return;}

        switch (collision.gameObject.tag)
        {
            case "Unit":
                dmg = DegatsAuxUnites;
                break;

            case "Building":
                dmg = DegatsAuxBatiments;
                break;

            case "Player":
                dmg = DegatsAuxJoueurs;
                break;

            default:
                break;
        }

        collision.collider.GetComponent<HealthSystem>()?.LoosePv(dmg);
        SpawnManager.DestroyObject(this.gameObject);
    }

}


    
    
