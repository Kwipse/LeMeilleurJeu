using MLAPI;
using UnityEngine;


namespace LeMeilleurJeu
{
    public class GameManager : MonoBehaviour
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
                SendInfoToConsole();
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

                if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkedClient))
                {
          
                    var playerCTRL = networkedClient.PlayerObject.GetComponent<PlayerController>();


                    if (playerCTRL)
                    {
                        playerCTRL.SwitchMode();
                    }
                }
            }
        }

        static void SendInfoToConsole()
        {
                GUILayout.Label("Host : " + NetworkManager.Singleton.IsHost);
                GUILayout.Label("Local Client ID : " + NetworkManager.Singleton.LocalClientId);
                GUILayout.Label("Camera : " + Camera.current);
               

        }

    }
    
}