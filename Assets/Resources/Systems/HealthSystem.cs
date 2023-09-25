using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealthSystem : NetworkBehaviour
{
    GameObject go;

	public NetworkVariable<int> pv = new NetworkVariable<int>(100);

    public override void OnNetworkSpawn()
    {
        go = this.gameObject;
        pv.OnValueChanged += OnPvChanged;
    }

    public override void OnNetworkDespawn()
    {
        pv.OnValueChanged -= OnPvChanged;
    }


    public void OnPvChanged(int previous, int current)
    {
        if (!IsOwner && !(pv.Value <= 0))
        {
            //Ce que le client qui a touche doit faire
            Debug.Log("Le " + go.name + " n'a plus que " + pv.Value + "pv");
            return;
        }

        if (IsOwner && !(pv.Value <= 0))
        {
            //Ce que le client doit faire quand son objet est touche
            Debug.Log("Votre " + go.name + " n'a plus que " + pv.Value + "pv");
            return;
        }

        if (IsOwner && (pv.Value <= 0))
        {
            //Ce que le client doit faire quand son objet est detruit
            Debug.Log("Votre " + go.name + " a été détruit");
            Die();
            return;
        }

    }

        
    void Die()
    {
        switch (go.tag)
        {
            case  "Player":
                SpawnManager.DestroyPlayer(go);
                SpawnManager.SpawnPlayer("FPSPlayer", Vector3.zero);
                break;

            default:
                SpawnManager.DestroyObject(go);
                break;
        }
    }
    

    public void LoosePv(int dmg)
        { LoosePvServerRPC(dmg); }
    [ServerRpc(RequireOwnership = false)]
    void LoosePvServerRPC(int dmg, ServerRpcParams serverRpcParams = default)
    {
		var clientId = serverRpcParams.Receive.SenderClientId;
        pv.Value -= dmg;

        if (pv.Value <= 0)
        {
            //Ce que le serveur doit faire en cas de mort
        }
    }

}
