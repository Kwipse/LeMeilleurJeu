
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace LeMeilleurJeu
{
    public class JoueurDuMeilleurJeu : NetworkBehaviour
    {
        public NetworkVariableVector3 Position = new NetworkVariableVector3(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.Everyone,
            ReadPermission = NetworkVariablePermission.Everyone
        });

        public Camera cam;

        public override void NetworkStart()
        {
            //Donne sa camera à chaque joueur
            cam.enabled = false;
            if (IsLocalPlayer) { cam.enabled = true; }
    
  

            Debug.Log("Ohé du bateau"); 
        }

        void Update()
        {
            transform.position = Position.Value;
        }

        //*/
        public void Move()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                var randomPosition = GetRandomPositionOnPlane();
                transform.position = randomPosition;
                Position.Value = randomPosition;
            }
            else
            {
                SubmitPositionRequestServerRpc();
            }
        }

        
        [ServerRpc]
        void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
        {
            Position.Value = GetRandomPositionOnPlane();
        }
        
        
        static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }
        //*/




    }
}