using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : NetworkBehaviour
{
    public static Dictionary<int, GameObject> ObjectIdList = new Dictionary<int, GameObject>();
    public static Dictionary<GameObject, int> ReverseObjectIdList = new Dictionary<GameObject, int>();
    static int nextId;

    static ObjectManager OM;

    //Sync bool
    public static bool isSynced;
   
    //events
    public delegate void ObjectManagerEvent();
    public static event ObjectManagerEvent ObjectManagerSynchronizedEvent;

    void Awake()
    { 
        OM = this; 
        isSynced = false;
        ClearObjectList();
        //Debug.Log("ObjectManager : J'existe !");
    }

    public override void OnNetworkSpawn() 
    {
        //ClearObjectList();
        if (IsServer) { isSynced = true; }
        if (!IsServer) { InitObjectManagerRpc(); }
    }


    //Ask Server for ObjectList infos
    [Rpc(SendTo.Server, RequireOwnership = false)]
    void InitObjectManagerRpc(RpcParams rpcParams = default)
    {
        var clientId = rpcParams.Receive.SenderClientId;
        //Debug.Log($"Object Manager : Initialising ObjectList with {ObjectIdList.Count} items for client {clientId}");
        foreach (var obj in ObjectIdList) 
        {
            //Debug.Log($"Object Manager : Sending {obj.ToString()} to client {clientId}");
            if (obj.Value)
            {
                AddObjectToListRpc(obj.Key, obj.Value, NetworkManager.Singleton.RpcTarget.Single(clientId, RpcTargetUse.Temp)); 
            }
        }

        SendSynchronizedEventRpc(NetworkManager.Singleton.RpcTarget.Single(clientId, RpcTargetUse.Temp));
    }

    //Fire synchronized event on client
    [Rpc(SendTo.SpecifiedInParams)]
    void SendSynchronizedEventRpc(RpcParams rpcParams) 
    {
        //Debug.Log($"Object Manager : Je suis synchronisé");
        isSynced = true;
        if (ObjectManagerSynchronizedEvent != null) { ObjectManagerSynchronizedEvent(); } 
    }



    //Add Object to list
    [Rpc(SendTo.SpecifiedInParams)]
    void AddObjectToListRpc(int id, NetworkObjectReference nor, RpcParams rpcParams) {
        AddObjectToList((GameObject) nor, id); }
    
    public static int AddObjectToList(GameObject go, int id = -1)
    {
        if (NetworkManager.Singleton.IsServer) 
        {
            if (ObjectIdList.ContainsValue(go))
            {
                id = GetObjectId(go);
                //Debug.Log($"{go.name} is already in the list, with id {id}"); 
                return id; 
            }

            if (id == -1) { id = nextId; nextId++; }
            OM.AddObjectToListRpc(id, go, NetworkManager.Singleton.RpcTarget.NotServer);
        }

        ObjectIdList.Add(id, go);
        ReverseObjectIdList.Add(go, id);
        //Debug.Log($"ObjectManager : Added {go.name} with ObjectId : {id}");
        return id;
    }



    //Remove Object from list
    [Rpc(SendTo.SpecifiedInParams)]
    void RemoveObjectFromListRpc(NetworkObjectReference nor, RpcParams rpcParams) {
        RemoveObjectFromList((GameObject) nor); }

    public static void RemoveObjectFromList(GameObject go)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            OM.RemoveObjectFromListRpc(go, NetworkManager.Singleton.RpcTarget.NotServer);
        }

        int id = GetObjectId(go);
        ObjectIdList.Remove(id);
        ReverseObjectIdList.Remove(go);
        //Debug.Log($"ObjectManager : Removed {go.name} with ObjectId : {id}");
    }


    public static void ClearObjectList() 
    {
        nextId = 0;
        ObjectIdList.Clear(); 
        ReverseObjectIdList.Clear();
        //Debug.Log($"ObjectList : List has been cleared");
    }


    public static GameObject GetObjectById(int id) 
    {
        if (ObjectIdList.TryGetValue(id, out GameObject go)) { return go; }
        Debug.Log($"{id} has no associated object"); return null;
    }


    public static int GetObjectId(GameObject go) 
    { 
        if (ReverseObjectIdList.TryGetValue(go, out int id)) { return id; }
        Debug.Log($"{go.name} is not in object list"); return -1;
    }



    public static int GetNextId() { return nextId; }

}
