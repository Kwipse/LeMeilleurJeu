using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class BombScript : NetworkBehaviour
{
    Vector3 push = new Vector3(0, 0, 10000);
    Rigidbody rb;
    NetworkRigidbody networkRigidbody;
    ulong localId;
    SpawnManager SM;

    public void Start()
    {
        networkRigidbody = GetComponent<NetworkRigidbody>();
        localId = NetworkManager.Singleton.LocalClientId;
        SM = GetComponent<SpawnManager>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            addRelativeForceServerRpc(push); 
        }
        else
        {
            rb = GetComponent<Rigidbody>();
            addRelativeForceServerRpc(push); 
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void addRelativeForceServerRpc(Vector3 push)
    {
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        rigidBody.AddRelativeForce(push);
    }

    void OnCollisionEnter(Collision collision)
    {    
            DestroyCubeServerRpc();
            SM.Spawn("BigExplosion", transform.position, localId, transform.rotation);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyCubeServerRpc(float dureeOuiNon = 0)
    {
        //GetComponent<Netw>
        if (dureeOuiNon == 0)
        {
            Destroy(gameObject);
        }
        else
            Destroy(gameObject, 10);

    }
}
