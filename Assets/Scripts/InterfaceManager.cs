using MLAPI;
using UnityEngine;


namespace LeMeilleurJeu
{
    public class InterfaceManager : MonoBehaviour
    {

        // UI
        void OnGUI()
        {

            GUILayout.BeginArea(new Rect(10, 10, 300, 300));//Basic automated UI
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                NetworkButtons();
            }
            else
            {
                ChooseMode();
                SendInfoToHud();
            }
            GUILayout.EndArea();
        }



        static void NetworkButtons()
        {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }

        static void ChooseMode()
        {

            if (GUILayout.Button("Switch Mode"))
            {
                var localId = NetworkManager.Singleton.LocalClientId;
                Debug.Log("Client " + localId + " hit the switch button");

                if (NetworkManager.Singleton.ConnectedClients.TryGetValue(localId, out var localClient))
                {
                    Debug.Log("LocalClient : " + localClient);

                    var localPlayerController = localClient.PlayerObject;
                    Debug.Log("LocalPlayerController : " + localPlayerController);

                    if (localPlayerController)
                    {
                        localPlayerController.GetComponent<PlayerController>().SwitchMode();
                    }
                }


            }
        }

        static void SendInfoToHud()
        {
            GUILayout.Label("Host : " + NetworkManager.Singleton.IsHost);
            GUILayout.Label("Local Client ID : " + NetworkManager.Singleton.LocalClientId);
            GUILayout.Label("Camera : " + Camera.current);
           
        }

    }
    
}