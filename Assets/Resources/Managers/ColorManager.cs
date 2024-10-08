using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

    public class ColorManager : SyncedBehaviour, ISyncBeforeGame
    {
        public Material m_Jaune;
        public Material m_Rouge;
        public Material m_Bleu;
        public Material m_Vert;
        public Material m_Rose;
        public Material m_Violet;
        public Material m_Orange;
        public Material m_Marron;
        public Material m_Noir;
        public Material m_Blanc;

        public Material m_BlueprintAllowed;
        public Material m_BlueprintNotAllowed;

        static NetworkList<int> PlayerMaterial;
        static NetworkList<int> TeamMaterial;
        static Material[] Materials;

        Renderer[] renderers;

        int matNumber;
        Material mat;
        GameObject go;
        NetworkObject no;

        static ColorManager CM;

        void Awake() 
        { 
            CM = this;
            PlayerMaterial = new NetworkList<int>();
            TeamMaterial = new NetworkList<int>(); 
            //Debug.Log($"{this.GetType().Name} : J'existe !");
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void InitializeBeforeSync()
        {
            Materials = new Material[10];
            InitializeMaterials();

            TeamMaterial.OnListChanged += OnTeamMaterialChanged;
            PlayerMaterial.OnListChanged += OnPlayerMaterialChanged;

            var clientId = NetworkManager.Singleton.LocalClientId;

            if (IsServer) 
            {
                InitializePlayerMaterials();
                InitializeTeamMaterials();
            }

            if (IsClient)
            {
                //Debug.Log($"ColorManager : TeamColor set to {TeamMaterial[(int)clientId]}, PlayerColor set to {PlayerMaterial[(int)clientId]}");
            }

        }




        void OnPlayerMaterialChanged(NetworkListEvent<int> ListEvent) {
            SetPlayerColors((ulong) ListEvent.Index); }

        void OnTeamMaterialChanged(NetworkListEvent<int> ListEvent) {
            SetPlayerColors((ulong) ListEvent.Index); }


        static Material GetPlayerMaterial(int playerId) {
            return Materials[PlayerMaterial[playerId]]; }

        static Material GetTeamMaterial(int playerId) { 
            return Materials[TeamMaterial[playerId]]; }


        public static void SetPlayerMaterial(int playerId, int matId) 
        { CM.SetPlayerMaterialServerRPC(playerId, matId); }
        [ServerRpc(RequireOwnership = false)]
        void SetPlayerMaterialServerRPC(int playerId, int matId) 
        {
            PlayerMaterial[playerId] = matId; 
            //Debug.Log($"ColorManager : Player {playerId} material set to {matId}"); 
        }


        public static void SetTeamMaterial(int teamId, int matId) 
        { CM.SetTeamMaterialServerRPC(teamId, matId); }
        [ServerRpc(RequireOwnership = false)]
        void SetTeamMaterialServerRPC(int teamId, int matId)
        {
            TeamMaterial[teamId] = matId; 
            //Debug.Log($"ColorManager : Team {teamId} material set to {matId}"); 
        }


        public static void SetObjectColors(GameObject objectToColor)
        {
            NetworkObject no = objectToColor.GetComponent<NetworkObject>();
            ulong clientId = no.OwnerClientId;
            int playerId = (int) clientId;
            int teamId = TeamManager.GetTeam(clientId);

            Material playerMat = GetPlayerMaterial(playerId);
            Material teamMat = GetTeamMaterial(teamId);

            foreach(Renderer r in objectToColor.GetComponentsInChildren<Renderer>()) 
            {
                string tag = r.gameObject.tag;
                switch (tag) 
                {
                    case "PlayerColor":
                        r.material = playerMat; 
                        break;

                    case "TeamColor":
                        r.material = teamMat; 
                        break;

                    default:
                        break; 
                } 
            }
        }

        public static void SetBlueprintColor(GameObject go, bool Allowed)
        {
            Material mBlueprint;
            if (Allowed)
                mBlueprint = CM.m_BlueprintAllowed;
            else
                mBlueprint = CM.m_BlueprintNotAllowed;

            Renderer[] renderers = go.GetComponentsInChildren<Renderer>(); 
            foreach (Renderer r in renderers) 
                r.material = mBlueprint;
        } 


        public static void SetPlayerColors(ulong clientId)
        {


        }


        void InitializeMaterials()
        {
            Materials[0] = m_Jaune;
            Materials[1] = m_Rouge;
            Materials[2] = m_Bleu;
            Materials[3] = m_Vert;
            Materials[4] = m_Rose;
            Materials[5] = m_Violet;
            Materials[6] = m_Orange;
            Materials[7] = m_Marron;
            Materials[8] = m_Noir;
            Materials[9] = m_Blanc;
        }


        void InitializePlayerMaterials()
        {
            PlayerMaterial.Add(4);
            PlayerMaterial.Add(5);
            PlayerMaterial.Add(6);
            PlayerMaterial.Add(7);
            PlayerMaterial.Add(8);
            PlayerMaterial.Add(9);
        }


        void InitializeTeamMaterials()
        {
            TeamMaterial.Add(0);
            TeamMaterial.Add(1);
            TeamMaterial.Add(2);
            TeamMaterial.Add(3);
        }

        public static void DrawBox(Vector3 p1, Vector3 p2)
        {
            Bounds b = new Bounds();
            b.SetMinMax(Vector3.Min(p1, p2), Vector3.Max(p1, p2));
            DrawBounds(b);
        }

        public static void DrawBounds(Bounds b)
        {
            var p1 = new Vector3(b.min.x, 1, b.min.z);
            var p2 = new Vector3(b.max.x, 1, b.min.z);
            var p3 = new Vector3(b.max.x, 1, b.max.z);
            var p4 = new Vector3(b.min.x, 1, b.max.z);

            Debug.DrawLine(p1, p2, Color.blue);
            Debug.DrawLine(p2, p3, Color.red);
            Debug.DrawLine(p3, p4, Color.yellow);
            Debug.DrawLine(p4, p1, Color.magenta);
        }
    }



