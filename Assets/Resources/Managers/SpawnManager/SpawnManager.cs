using UnityEngine;
using Unity.Netcode;
using classes;
using systems;

public class SpawnManager : NetworkBehaviour
{
    static GameObject ExpPrefab;
    static GameObject Explosion;
    static Explosion ExpStats;

    //Enable global access to static 
    //functions as SpawnManager.function()
    static SpawnManager SM;
    void Awake()
    { 
        SM = this; 
        //Debug.Log("SpawnManager : J'existe !");
    }
    
    void Start()
    {
        PrefabManager.LoadAllPrefabs();
    }

    public override void OnNetworkSpawn() 
    {
        if (IsServer)
        {
            ExpPrefab = PrefabManager.GetPrefab("Explosion");
        }
    }

    

    //SPAWN OBJECT
    public static void SpawnObject(GameObject go, Vector3 SpawnLocation, Quaternion SpawnRotation)
        {SM.SpawnObjectByNameServerRpc(go.name, SpawnLocation, SpawnRotation);}

	//SPAWN OBJECT BY NAME
	public static void SpawnObjectByName(string PrefabName, Vector3 SpawnLocation, Quaternion SpawnRotation)
		{SM.SpawnObjectByNameServerRpc(PrefabName, SpawnLocation, SpawnRotation);}
		
	[ServerRpc(RequireOwnership = false)]
	void SpawnObjectByNameServerRpc(string PrefabName, Vector3 SpawnLocation, Quaternion SpawnRotation, ServerRpcParams serverRpcParams = default)
	{
		var clientId = serverRpcParams.Receive.SenderClientId;
        GameObject go = PrefabManager.GetPrefab(PrefabName);

		go = Instantiate(go, SpawnLocation, SpawnRotation);
		go.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
	}


    //SPAWN EXPLOSION
    public static void SpawnExplosion(
            Vector3 position,
            int size = 10,
            int unitDmg = 0,
            int buildingDmg = 0,
            float duration = 0.25f,
            int outwardForce = 1)
    {
        SM.SpawnExplosionServerRPC(position, size, unitDmg, buildingDmg, duration, outwardForce);
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnExplosionServerRPC(
            Vector3 position, 
            int size,
            int unitDmg,
            int buildingDmg,
            float duration,
            int outwardForce,
            ServerRpcParams serverRpcParams = default)
    {

		var clientId = serverRpcParams.Receive.SenderClientId;

        Explosion = Instantiate(ExpPrefab, position, Quaternion.identity);
        ExpStats = Explosion.GetComponent<Explosion>();

        Explosion.transform.localScale = new Vector3 (size, size, size);

        ExpStats.ExplosionSize = size;
        ExpStats.damageToUnit = unitDmg;
        ExpStats.damageToBuilding = buildingDmg;
        ExpStats.ExplosionDuration = duration;
        ExpStats.outwardForce = outwardForce;

		Explosion.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        //Debug.Log($"SpawnManager : Spawning explosion at {position}");
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
            RpcParams serverRpcParams = default)
    {


		var clientId = serverRpcParams.Receive.SenderClientId;
        GameObject weapon = weaponNor;
        GameObject projectile = Instantiate(PrefabManager.GetPrefab(projectileName), position, rotation);

        Projectile projectileStats = projectile.GetComponent<Projectile>();
        projectileStats.initialForce = initialForce;

        projectile.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);

        SetupProjectileRpc(projectile, weapon);
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    void SetupProjectileRpc(NetworkObjectReference projectileNor, NetworkObjectReference weaponNor)
    {
        GameObject projectile = projectileNor;
        GameObject weapon = weaponNor;

        if (projectile.GetComponent<NetworkObject>().IsOwner)
        {
            projectile.GetComponent<Projectile>().SetWeapon(weapon);
        }

    }
    


    //SPAWN WEAPON
    public static void SpawnWeapon(GameObject weaponPrefab, GameObject weaponHolder)
    {
        //Debug.Log($"Client : J'ai donné ordre de spawn {weaponPrefab.name} pour {weaponHolder.name}");
        SM.SpawnWeaponServerRpc(weaponPrefab.name, weaponHolder);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    void SpawnWeaponServerRpc(
            string weaponName, 
            NetworkObjectReference weaponHolder,
            RpcParams rpcParams = default)
    {
        GameObject holder = weaponHolder;
        //Debug.Log($"Server : J'ai recu ordre de spawn {weaponName} pour {holder.name}");
        GameObject weapon = Instantiate(PrefabManager.GetPrefab(weaponName), Vector3.zero , Quaternion.identity);
        weapon.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        SetupSpawnedWeaponClientRpc(weapon, holder, RpcTarget.Single(rpcParams.Receive.SenderClientId, RpcTargetUse.Temp));
    }
    
    [Rpc(SendTo.SpecifiedInParams)]
    void SetupSpawnedWeaponClientRpc(
            NetworkObjectReference weaponNor, 
            NetworkObjectReference holderNor,
            RpcParams rpcParams
            )
    {
        NetworkObject holderNo = holderNor;
        WeaponSystem ws = holderNo.gameObject.GetComponent<WeaponSystem>();
        GameObject weapon = weaponNor;

        ws.SetupEquippedWeapon(weapon);
        //Debug.Log($"Client : {holderNo.gameObject.name} a equippé {weapon.name}");
    }




    //SPAWN UNIT
    public static void SpawnUnit(
            GameObject unitPrefab, Vector3 spawnPosition,
            Quaternion spawnRotation = default(Quaternion), Vector3 rallyPosition = default(Vector3)) 
    { 
        SM.SpawnUnitByNameServerRpc(unitPrefab.name, spawnPosition, spawnRotation, rallyPosition);
    }

    public static void SpawnUnitByName(
            string unitPrefabName, Vector3 spawnPosition,
            Quaternion spawnRotation = default(Quaternion), Vector3 rallyPosition = default(Vector3))
    { 
        SM.SpawnUnitByNameServerRpc(unitPrefabName, spawnPosition, spawnRotation, rallyPosition);
    }

	[ServerRpc(RequireOwnership = false)]
	void SpawnUnitByNameServerRpc(
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
            go.GetComponent<Unit>()?.MoveUnitToPos(rallyPosition, false);
        }
    }



    //DESTROY
	public static void DestroyObject(GameObject go)
        {
            if (go) SM.DestroyServerRPC(go);
        }

	[ServerRpc(RequireOwnership = false)]
	void DestroyServerRPC(NetworkObjectReference nor, ServerRpcParams serverRpcParams = default)	
    {
		var clientId = serverRpcParams.Receive.SenderClientId;

        NetworkObject no = nor;
        
        if (!no) {
            Debug.Log($"SpawnManager : The object you are trying to destroy does not exist"); 
            return;
        }

        no.Despawn();
        //Debug.Log($"SpawnManager : I despawned {no.name} for client {clientId}");
    }


        

	//SPAWN PLAYER
	public static void SpawnPlayer(string PlayerPrefabName, Vector3 SpawnLocation)
		{SM.SpawnPlayerServerRPC(PlayerPrefabName, SpawnLocation);}
		
	[ServerRpc(RequireOwnership = false)]
	void SpawnPlayerServerRPC(string PlayerPrefabName, Vector3 SpawnLocation, ServerRpcParams serverRpcParams = default)
	{
		var clientId = serverRpcParams.Receive.SenderClientId;
		
		//Destroy current player if it exist
		GameObject go = PlayerList.PlayerListinstance.GetPlayerObject(clientId);
		//Debug.Log("PlayerId :" + clientId + " - go :" + go);
		if (go != null) 
            DestroyPlayer(go);
		
		//Instantiate and spawn
		go = Instantiate(PrefabManager.GetPrefab(PlayerPrefabName), SpawnLocation, Quaternion.identity);
		go.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
		
		//Register in Player List
		PlayerList.PlayerListinstance.AddPlayerObject(clientId,go);
	}
	
	
	//DESTROY PLAYER
    public static void DestroyPlayer(GameObject go)
        {if (go) SM.DestroyPlayerServerRPC(go);}
    
	[ServerRpc(RequireOwnership = false)]
	void DestroyPlayerServerRPC(NetworkObjectReference nor, ServerRpcParams serverRpcParams = default)
	{ 
        GameObject go = nor;
        NetworkObject no = nor;
        var playerId = PlayerList.PlayerListinstance.GetPlayerId(go);

        no.Despawn();
        PlayerList.PlayerListinstance.RemovePlayerObject((ulong) playerId);
	}
	
}
