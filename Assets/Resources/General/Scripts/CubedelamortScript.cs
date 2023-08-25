using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class CubedelamortScript : NetworkBehaviour
{
    Vector3 push = new Vector3(0,0,500);
    Rigidbody rb ;
    GameObject go;
    NetworkObject no;
    SpawnManager SM;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) {enabled=false;}
        else
        {
            SM = (SpawnManager) SpawnManager.spawner;
            go = gameObject;
            no = GetComponent<NetworkObject>();
            rb= GetComponent<Rigidbody>();

            rb.AddRelativeForce(push);
            //SM.DestroyObject(go,10);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Unit")
        {
            collision.collider.GetComponent<HealthSystem>().LoosePv(100);
            SM.DestroyObject(go);
        }

        if (collision.gameObject.tag == "Building")
        {
            collision.collider.GetComponent<HealthSystem>().LoosePv(100);
            SM.DestroyObject(go);
        }
        
        if(collision.gameObject.tag == "Player")
        {
            collision.collider.GetComponent<FPSPlayerHealth>().LoosePv(25);
            SM.DestroyObject(go);
        }
    }
}
    
