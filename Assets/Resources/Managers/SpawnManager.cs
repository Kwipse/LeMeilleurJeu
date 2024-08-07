using UnityEngine;
using Unity.Netcode;

public class SpawnManager : SyncedBehaviour, ISyncBeforeGame
{
    //Enable global access to static functions as SpawnManager.function()
    static SpawnManager SM;
    void Awake()
    { 
        SM = this; 
        //Debug.Log("SpawnManager : J'existe !");
    }

    public override void InitializeBeforeSync()
    {
        PrefabManager.LoadAllPrefabs();
    }



    //Basic Spawn functions (executed by server)
    int ServerSpawnPrefab(string prefabName, Vector3 spawnPos, Quaternion spawnRot, ulong clientId) {
        return ServerSpawnPrefab(PrefabManager.GetPrefab(prefabName), spawnPos, spawnRot, clientId); }

    int ServerSpawnPrefab(GameObject prefab, Vector3 spawnPos, Quaternion spawnRot, ulong clientId)
    {
        GameObject go;
        int id;

        go = Instantiate(prefab, spawnPos, spawnRot);
        go.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        //Debug.Log($"SpawnManager : Spawned {go.name}, for client {clientId}"); 

        id = ObjectManager.AddObjectToList(go);
        return id;
    }

    void ServerDespawnObject(GameObject go)
    {
        if (go) {
            ObjectManager.RemoveObjectFromList(go);
            go.GetComponent<NetworkObject>().Despawn(); }
    }



    //DESTROY OBJECT
    public static void DestroyObject(GameObject go) {
        if (go.GetComponent<NetworkObject>().IsSpawned) { SM.DestroyObjectRpc(go); } }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    void DestroyObjectRpc(NetworkObjectReference nor)	{ ServerDespawnObject((GameObject) nor); }



    //SPAWN OBJECT
    public static void SpawnObject(GameObject go, Vector3 SpawnLocation, Quaternion SpawnRotation)
    {SpawnObject(go.name, SpawnLocation, SpawnRotation);}

    public static void SpawnObject(string PrefabName, Vector3 SpawnLocation, Quaternion SpawnRotation)
    {SM.SpawnObjectServerRpc(PrefabName, SpawnLocation, SpawnRotation);}

    [ServerRpc(RequireOwnership = false)]
    void SpawnObjectServerRpc(string PrefabName, Vector3 SpawnLocation, Quaternion SpawnRotation, ServerRpcParams serverRpcParams = default)
    {
        int objectId = ServerSpawnPrefab(PrefabName, SpawnLocation, SpawnRotation, serverRpcParams.Receive.SenderClientId);
    }



    //SPAWN PROJECTILE
    public static void SpawnProjectile(GameObject projectilePrefab, GameObject weapon, int initialForce = 0) {
        Transform gunpoint = weapon.GetComponent<Arme>().GetGunpointTransform();
        SM.SpawnProjectileRpc(projectilePrefab.name, weapon, gunpoint.position, gunpoint.rotation, initialForce); }

    public static void SpawnProjectile(GameObject projectilePrefab, GameObject weapon, Transform altGunpoint, int initialForce = 0) {
        SM.SpawnProjectileRpc(projectilePrefab.name, weapon, altGunpoint.position, altGunpoint.rotation, initialForce); }

    public static void SpawnProjectile(GameObject projectilePrefab, GameObject weapon, Vector3 position, Quaternion rotation, int initialForce = 0) {
        SM.SpawnProjectileRpc(projectilePrefab.name, weapon, position, rotation, initialForce); }


    [Rpc(SendTo.Server, RequireOwnership = false)]
    void SpawnProjectileRpc(
            string projectileName,
            NetworkObjectReference weaponNor,
            Vector3 position,
            Quaternion rotation,
            int initialForce,
            RpcParams rpcParams = default)
    {
        int projectileId = ServerSpawnPrefab(projectileName, position, rotation, rpcParams.Receive.SenderClientId);
        GameObject projectile = ObjectManager.GetObjectById(projectileId);

        SetupProjectileRpc(projectile, weaponNor);
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    void SetupProjectileRpc(NetworkObjectReference projectileNor, NetworkObjectReference weaponNor)
    {
        GameObject projectile = projectileNor;
        GameObject weapon = weaponNor;
        if (projectile.GetComponent<NetworkObject>().IsOwner) {
            //Projectile projectileStats = projectile.GetComponent<Projectile>();
            //projectileStats.initialForce = initialForce;
            projectile.GetComponent<Projectile>().FireProjectile(weapon); }
    }



    //SPAWN WEAPON
    public static void SpawnWeapon(GameObject weaponPrefab, GameObject weaponHolder)
    {
        //Debug.Log($"Client : J'ai donné ordre de spawn {weaponPrefab.name} pour {weaponHolder.name}");
        SM.SpawnWeaponRpc(weaponPrefab.name, weaponHolder);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    void SpawnWeaponRpc(
            string weaponPrefabName, 
            NetworkObjectReference holderNor,
            RpcParams rpcParams = default)
    {
        int weaponId = ServerSpawnPrefab(weaponPrefabName, Vector3.zero , Quaternion.identity, rpcParams.Receive.SenderClientId);
        GameObject holder = holderNor;
        holder.GetComponent<WeaponSystem>().currentWeaponId.Value = weaponId;
        //SyncWeaponRpc(holderNor, weaponId);
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    void SyncWeaponRpc(NetworkObjectReference holderNor, int weaponId)
    {
        GameObject holder = holderNor;
        Debug.Log($"weaponHolder : {holder}, weaponId : {weaponId}");
        //holder.GetComponent<IWeaponizable>()?.WS.UpdateWeapon(weaponId);
    }




    //SPAWN UNIT
    public static void SpawnUnit(
            GameObject unitPrefab, Vector3 spawnPosition,
            Quaternion spawnRotation = default(Quaternion), Vector3 rallyPosition = default(Vector3)) 
    { 
        SpawnUnit(unitPrefab.name, spawnPosition, spawnRotation, rallyPosition);
    }

    public static void SpawnUnit(
            string unitPrefabName, Vector3 spawnPosition,
            Quaternion spawnRotation = default(Quaternion), Vector3 rallyPosition = default(Vector3))
    { 
        SM.SpawnUnitServerRpc(unitPrefabName, spawnPosition, spawnRotation, rallyPosition);
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnUnitServerRpc(
            string PrefabName, Vector3 SpawnLocation,
            Quaternion SpawnRotation = default(Quaternion), Vector3 rallyPosition = default(Vector3),
            ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        GameObject go = Instantiate(PrefabManager.GetPrefab(PrefabName), SpawnLocation, SpawnRotation);
        go.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);

        //if (rallyPosition != default(Vector3))
        SM.MoveUnitToRallyPointClientRPC(go, rallyPosition);
    }

    [ClientRpc]
    void MoveUnitToRallyPointClientRPC(
            NetworkObjectReference nor, Vector3 rallyPosition,
            ClientRpcParams clientRpcParams = default)
    {
        NetworkObject no = nor;
        GameObject go = nor;

        //Debug.Log($"SpawnManager : Received order to move {go.name} to rally point {rallyPosition}");
        if (no.IsOwner)
        {
            go.GetComponent<Unit>()?.MoveOrder(rallyPosition, false);
        }
    }



    //SPAWN PLAYER
    public static void SpawnPlayer(string PlayerPrefabName, Vector3 SpawnLocation)
    {SM.SpawnPlayerRPC(PlayerPrefabName, SpawnLocation);}

    [Rpc(SendTo.Server, RequireOwnership = false)]
    void SpawnPlayerRPC(string PlayerPrefabName, Vector3 SpawnLocation, RpcParams rpcParams = default)
    {
        var clientId = rpcParams.Receive.SenderClientId;

        //Destroy current obj if it exist
        if (PlayerManager.GetPlayerObjectID(clientId) > 0) {
            DestroyPlayer(PlayerManager.GetPlayerObject(clientId)); }

        //Spawn player & Register in Player List
        int playerObjectId = ServerSpawnPrefab(PlayerPrefabName, SpawnLocation, Quaternion.identity, clientId);
        PlayerManager.SetPlayerObjectID(clientId, playerObjectId);
    }


    //DESTROY PLAYER
    public static void DestroyPlayer(GameObject go)
    {
        if (go) { SM.DestroyPlayerRPC(go); }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    void DestroyPlayerRPC(NetworkObjectReference nor, RpcParams rpcParams = default)
    { 
        GameObject go = nor;
        ServerDespawnObject(go);
        //var playerId = PlayerManager.GetPlayerId(go);
        //PlayerManager.RemovePlayerObject((ulong) playerId);
    }

}

