using Unity.Netcode;
using UnityEngine;
using managers;

namespace systems {

    public class HealthSystem : SyncedBehaviour, IWaitForGameSync
    {
        public int pvMax = 100;
        NetworkVariable<int> pv;
        public bool showLifeBar = true;
        public HealthBar lifeBar;

        GameObject go;

        void Awake()
        {
            pv = new NetworkVariable<int>();
            lifeBar = ScriptableObject.Instantiate(lifeBar);
        }

        public override void StartAfterGameSync()
        {
            Debug.Log($"{gameObject.name} : HealthSystem is online");
            pv.OnValueChanged += OnPvChanged;
            if (showLifeBar) { lifeBar.CreateHealthBar(gameObject); }

            if (IsServer) { pv.Value = pvMax; }
        }

        public override void OnNetworkDespawn()
        {
            pv.OnValueChanged -= OnPvChanged;
            lifeBar.DestroyHealthBar();
        }


        public void OnPvChanged(int previous, int current)
        {
            Debug.Log($"{gameObject.name} : Filling health bar to {(float)current/pvMax}");
            lifeBar.SetHealth((float)current/pvMax);

            if (!IsOwner && !(pv.Value <= 0))
            {
                //Ce que le client qui a touche doit faire
                //Debug.Log("Le " + gameObject.name + " n'a plus que " + pv.Value + "pv");
                return;
            }

            if (IsOwner && !(pv.Value <= 0))
            {
                //Ce que le client doit faire quand son objet est touche
                Debug.Log("Votre " + gameObject.name + " n'a plus que " + pv.Value + "pv");
                return;
            }
 
            if (IsOwner && (pv.Value <= 0))
            {
                //Ce que le client doit faire quand son objet est detruit
                //Debug.Log("Votre " + gameObject.name + " a été détruit");
                Die();
                return;
            }

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

}
