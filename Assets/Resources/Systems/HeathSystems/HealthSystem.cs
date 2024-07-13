using Unity.Netcode;
using UnityEngine;

public class HealthSystem : SyncedBehaviour, IWaitForGameSync
{
    public HealthSystemAsset HSA;
    HealthBar HB;
    bool showLifeBar = true;

    NetworkVariable<int> pv;
    NetworkVariable<int> pvMax;

    void Awake()
    {
        pv = new NetworkVariable<int>();
        pvMax = new NetworkVariable<int>();
        HSA = ScriptableObject.Instantiate(HSA);
        HB = ScriptableObject.Instantiate(HSA.lifeBar);
    }

    public override void StartAfterGameSync()
    {
        if (showLifeBar) { HB.CreateHealthBar(gameObject); }

        if (IsServer)
        {
            pvMax.Value = HSA.hpPool;
            pv.Value = pvMax.Value;
        }
        if (IsClient)
        {
            OnPvMaxChanged(-1, pvMax.Value);
            OnPvChanged(-1, pv.Value);
        }

        pvMax.OnValueChanged += OnPvMaxChanged;
        pv.OnValueChanged += OnPvChanged;
    }

    public void OnPvMaxChanged(int previous, int current)
    {
        HB.SetHealth((float)current/pvMax.Value);
        if (IsOwner && (pvMax.Value <= 0)) { Die(); }
    }

    public void OnPvChanged(int previous, int current)
    {
        HB.SetHealth((float)current/pvMax.Value);
        if (IsOwner && (pv.Value <= 0)) { Die(); }
    }

    //public void Update()
    //{//a changer
    //    if(HB != null ) HB.LookAtPosition(Vector3.forward);
    //}

    public override void OnNetworkDespawn()
    {
        pvMax.OnValueChanged -= OnPvMaxChanged;
        pv.OnValueChanged -= OnPvChanged;
        HB.DestroyHealthBar();
    }



    void Die()
    {
        switch (gameObject.tag)
        {
            case  "Player":
                SpawnManager.DestroyPlayer(gameObject);
                SpawnManager.SpawnPlayer("FPSPlayer", Vector3.zero);
                break;

            default:
                SpawnManager.DestroyObject(gameObject);
                break;
        }
    }

    public void SetPv(int dmg) { LoosePvServerRPC(pv.Value - dmg); }
    public void LoosePv(int dmg) { LoosePvServerRPC(dmg); }

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

