using Unity.Netcode;
using UnityEngine;



    public class InterfaceManager : MonoBehaviour
    {

        void Awake()
        {

        }

        // UI
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));//Basic automated UI
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                NetworkButtons();
            }
            GUILayout.EndArea();
        }



        static void NetworkButtons()
        {
		
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }
        

    }

