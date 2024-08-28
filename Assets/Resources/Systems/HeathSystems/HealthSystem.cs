using Unity.Netcode;
using UnityEngine;

public class HealthSystem : SyncedBehaviour, IWaitForGameSync
{
    public HealthSystemAsset HSA;
    HealthBar HB;
    public bool showLifeBar = true;
    bool setToDie = false;
    public GameObject FPSDeathUI ;


    Camera cam;

    [HideInInspector] public NetworkVariable<int> pv;
    [HideInInspector] public NetworkVariable<int> pvMax;

    void Awake()
    {
        pv = new NetworkVariable<int>();
        pvMax = new NetworkVariable<int>();
        HB = ScriptableObject.Instantiate(HSA.lifeBar);
    }

    public override void StartAfterGameSync()
    {

        if (IsServer) {
            pvMax.Value = HSA.hpPool;
            pv.Value = pvMax.Value; }

        if (IsClient) {
            OnPvMaxChanged(-1, pvMax.Value);
            OnPvChanged(-1, pv.Value); }

        pvMax.OnValueChanged += OnPvMaxChanged;
        pv.OnValueChanged += OnPvChanged;

        //Create Lifebar or not
        if (!showLifeBar) { return; }
        if (IsOwner && gameObject.tag == "Player") { return; } 
        HB.CreateHealthBar(gameObject);
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

    public override void OnNetworkDespawn()
    {
        pvMax.OnValueChanged -= OnPvMaxChanged;
        pv.OnValueChanged -= OnPvChanged;
        HB.DestroyHealthBar();
    }



    public void UpdateCurrentCamera()
    {
        cam = PlayerManager.GetPlayerCamera(NetworkManager.Singleton.LocalClientId);
    }

    void Update()
    {
        if (cam) { HB.LookAtPosition(cam.transform.position); }
        if (!cam) { UpdateCurrentCamera(); }
    }




    void Die()
    {
        setToDie = true;
        switch (gameObject.tag)
        {
            case  "Player":
                //SpawnManager.DestroyPlayer(gameObject);
               
                ModeManager _modeManager = NetworkManager.LocalClient.PlayerObject.GetComponent<ModeManager>();
                _modeManager.BecomeGhost();

                break;

            default:
                SpawnManager.DestroyObject(gameObject);
                break;
        }
    }

    public void SetPv(int dmg) {
        if (!setToDie) { LoosePvServerRPC(pv.Value - dmg); } }
    public void LoosePv(int dmg) {
        if (!setToDie) { LoosePvServerRPC(dmg); } }

    [ServerRpc(RequireOwnership = false)]
    void LoosePvServerRPC(int dmg, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        pv.Value -= dmg;
    }

}

