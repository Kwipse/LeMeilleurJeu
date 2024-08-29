# LeMeilleurJeu

Ben c'est le meilleur jeu quoi


# INTRO

Bienvenue dans le meilleur jeu.


# RACCOURCIS/FONCTIONALITES


## General

Tab : Echange le mode de jeu entre FPS et RTS


## FPS

ZQSD : Mouvement FPS

Espace : Saut

Clic gauche : Tir

Clic droit : Tir altenatif


## RTS

Le RTS est en mode selection par defaut.

### General

- Echap : Retourne en mode selection
- B : Entre en mode construction de batiments
- C : Entre en mode création d'unité
- Maj : Permet d'enchainer les ordres de construction/creation d'unités


### Camera

- ZQSD : Translation de la camera
- Clic Molette : Pivot autour de la pos visée

### Selection

- Clic Gauche : Selection d'unités/batiments
- Clic Droit : Ordonne le déplacement des unités selectionnees
- Maj : Permet d'enchainer plusieurs ordres de déplacement aux unités
- S : Utilise le skill des unités

### Mode construction de batiments

- Molette haut/bas : Choisit le blueprint a construire
- Clic Gauche : Construction du batiment choisi
- W : Spawn le 1er batiment
- X : Spawn le 2e batiment

### Mode Création d'unités

- W : Spawn la 1er unité du batiment
- X : Spawn la 2nd unité du batiment


# TAGS

Unity donne a chaque GameObject un tag, qu'on peut initialiser dans l'éditeur.
Plein de fonctionnalités ne marcherons pas si les tags ne sont pas corrects.

```csharp
GameObject go;
string tag = go.tag;
```

## Moteur de jeu

- ground : Tag réservé au sol, principalement utilisé pour les constructions RTS
- Player : Pour les objets joueurs (FPS/RTS). 
- Unit : Pour les unités qui ne sont pas des joueurs (unités RTS, ...)
- Arme : Pour les armes
- Projectile : Pour les projectiles
- Building : Pour les batiments

## Couleurs

- TeamColor : Les objets avec ce tag seront colorés avec la couleur de l'équipe
- PlayerColor : Les objets avec ce tag seront colorés avec la couleur de l'équipe


# SYSTEMES

Pour faciliter la création d'objets, on peut ajouter des fonctionnalites a un prefab.
La plupart de ces systemes implémentent des scriptables objects, ce qui nous permet de créer facilement des assets dans l'editeur (Dans la fenetre "Project" > Clic-droit > Create > FaisTonChoix)

Systemes a ajouter comme component :
- WeaponSystem
- HealthSystem
- UnitSpawnerSystem
- BuilderSystem

Systemes a instancier par script :
- AnimationSystem 
- UI (FPSUI/RTSUI)
- SelectionSystem (RTSSelection)
- MovementSystem (FPSMovement)
- Ressource (AmmoSystem, GoldSystem)


## WeaponSystem

Ce script donne un systeme d'armes a l'objet qui le possède. 
Par defaut, l'unité n'aura pas d'arme.
Dans l'editeur, vous pouvez créer/recuperer un nouveau WeaponSystemAsset pour donner des armes par defaut.

```csharp
    public void AddAvailableWeapon(GameObject weaponPrefab) 
    public void RemoveAvailableWeapon(GameObject weaponPrefab) 

    public void AddAvailableAmmo(string ammoType, int maxAmmo) 
    public void RemoveAvailableAmmo(string ammoType) 
    public bool IsAmmoAvailable(string ammoType) 
    public AmmoSystem GetBackpackAmmo(string ammoType) 

    public void AddAmmoInBackpack(int amount) 
    public void SetBackpackMaxAmmo(int maxAmount) 
    public void RemoveAmmoFromBackpack(int amount) 
    public bool IsEnoughBackpackAmmo(int testAmount) 

    public void UpdateHandlesIK() //Call this in a fixedUpdate to Update weapon IKs positions

    public GameObject GetCurrentWeapon() 
    public int GetCurrentWeaponNumber() 
    public Arme GetCurrentWeaponScript() 
    public AmmoSystem GetCurrentWeaponAmmo() 
    public AmmoSystem GetCurrentBackpackAmmo() 
    public string GetCurrentAmmoType() 

    public void EquipNextWeapon() 
    public void EquipPreviousWeapon() 
    public void EquipWeaponNumber(int weaponNumber) 
    public void EquipWeapon(GameObject weaponPrefab) 
    public void UnequipWeapon() 

    public void ShootWeapon() 
    public void ShootAltWeapon() 
    public void ReloadWeapon() 
```


## HealthSystem 

Ce script donne un systeme de vie a l'objet qui le possède. 

```csharp
//Faire perdre des points de vie a un objet
GameObject cible;
cible.GetComponent<HealthSystem>()?.LoosePv(int dmg)
```


## UnitSpawnerSystem

Ce script donne un systeme de création d'unité à l'objet qui le possède.
Dans l'editeur, vous pouvez ajouter des unités à la liste des unités disponibles par defaut .

```csharp
    public class UnitSpawnerSystem : NetworkBehaviour 

    //Unités disponibles
    public List<GameObject> AvailableUnits; //A definir initialement dans l'editeur
    public void AddAvailableUnit(GameObject unitPrefab) 
    public void RemoveAvailableUnit(GameObject unitPrefab) 
    public void ClearAvailableUnitList() 

    //Spawners disponibles
    //Par defaut, tous les objets appartenant au UnitSpawner avec le tag "UnitSpawner" seront disponibles.
    public void AddAvailableSpawner(GameObject spawnerObject) 
    public void RemoveAvailableSpawner(GameObject spawnerObject) 
    public void ClearAvailableSpawnerList() 

    //Rally Point
    public void MoveRallyPoint(Vector3 pos) 
    public Vector3 GetRallyPoint() 

    //Spawn fonctions
    //si on ne specifie pas de spawner, alors on utilise la liste des spawners 
    //disponibles et les options de spawn pour décider du spawn
    public void SpawnUnit(GameObject unitPrefab, GameObject specificSpawner = null) 
    public void SpawnUnitByName(string unitName, GameObject specificSpawner = null)
    public void SpawnUnitByIndex(int unitIndex, GameObject specificSpawner = null)

    //Options de spawn
    public bool isMatchUnitToSpawner = true; //si un spawner contient le nom de l'unité, on spawn dessus
```


## BuilderSystem

Component qui donne un systeme de construction de batiments.
Dans l'editeur, vous pouvez ajouter des batiments à la liste des batiments disponibles par defaut .

```csharp
    public class BuilderSystem : NetworkBehaviour 

    //A definir initialement dans l'editeur
    public List<GameObject> AvailableBuildings; //Gestion de la liste des batiments disponibles
    public void AddAvailableBuilding(GameObject buildingPrefab) 
        public void RemoveAvailableBuilding(GameObject buildingPrefab) 

    //Accessible via code
    [HideInInspector] public GameObject currentBlueprint;
    [HideInInspector] public bool isBlueprintAllowed = true;
    [HideInInspector] public bool keepBlueprintOnConstruction = false;


    //Gestion des blueprints
    public void SelectBlueprint(int blueprintPrefabNumber = 0) //default to the first available building
    public void SelectNextBlueprint() 
    public void SelectPreviousBlueprint() 
    public void ClearBlueprint() 
    public void MoveBlueprintToPosition(Vector3 position) 
    public void RotateBlueprintToQuaternion(Quaternion rotation) 

    //Construction des batiments
    public void ConstructCurrentBlueprint() 
    public void ConstructBuilding(int buildingNumber, Vector3 position, Quaternion rotation) 
```

RdN : Un objet avec un BuilderSystem, qui n'est pas controlé par un joueur, n'a pas tellement de raison d'utiliser les blueprints.


## SelectionSystem

Component qui donne un systeme de selection a un objet.

 ```csharp
    public class SelectionSystem : NetworkBehaviour

    //
    [HideInInspector] public List<GameObject> selection = new List<GameObject>();

    //Options de selection, a définir dans l'editeur ou par code
    public bool isAddingToSelection = false;
    public bool canSelectOwnedObjects = true;
    public bool canSelectNotOwnedObjects = true;
    public bool canSelectBuilding = true;
    public bool canSelectUnit = true;
    public bool canSelectWeapon = true;

    //Events
    public delegate void SelectEvent(GameObject go);
    public event SelectEvent ObjectAddedToSelectionEvent;
    public event SelectEvent ObjectRemovedFromSelectionEvent;

    //Fonctions de selection
    public void SelectObject(GameObject toSelect, bool forceAddSelection = false) 
    public void SelectList(List<GameObject> toSelectList) 
    public void SelectInBox(Vector3 p1, Vector3 p2) 
    public void StartBoxSelection(Vector3 startPoint) {
    public void UpdateBoxSelection(Vector3 updatePoint) {
    public void EndBoxSelection(Vector3 endPoint) {
    public void DeselectObject(GameObject go) 
    public void RemoveListFromSelection(List<GameObject> goList) {
    public void ClearSelection() 

    //Recuperer les infos de la selection
    public int GetSelectionCount() {
    public List<GameObject> GetSelection() {

    //Orders to selection
    public void OrderSelectedUnitsToMove(Vector3 pos) 
    public void OrderSelectedBuildingsToMoveRallyPoint(Vector3 pos) 
    public void OrderSelectedSpawnersToCreateUnit(int unitIndex)
```

Usage :

```csharp

using systems;

public class ExempleClass : 
{
    SelectionSystem SS;

    void Awake()
    {
        SS = GetComponent<SelectionSystem>(); 
    }

    void Start()
    {
        //Optionellement, s'abonner aux events de modification de la selection, 
        SS.ObjectAddedToSelectionEvent += OnObjectAddedToSelection;
        SS.ObjectRemovedFromSelectionEvent += OnObjectRemovedFromSelection;
    }

    void OnObjectAddedToSelection() 
    {
        //Code lorsqu'un objet est ajouté a la selection
    }

    void OnObjectRemovedFromSelection()
    {
        //Code lorsqu'un objet est ajouté a la selection
    }

    void OnDestroy()
    {
        //Se desabonner des events pour eviter de leak de la memoire
        SS.ObjectAddedToSelectionEvent -= OnObjectAddedToSelection;
        SS.ObjectRemovedFromSelectionEvent -= OnObjectRemovedFromSelection;
    }
}

```


# MANAGERS

Pour faciliter le code, il y a des objets préplacés dans la scène, qui possedent des fonctions accessibles dans tout le projet.


## OBJECT MANAGER

Gere la liste des objets :

```csharp
//Syntaxe d'appel
ObjectManager.fonctionAUtiliser();

public static GameObject GetObjectById(int id) 
public static int GetObjectId(GameObject go) 
public static int AddObjectToList(GameObject go, int id = -1)
public static void RemoveObjectFromList(GameObject go)
public static void ClearObjectList() 
```


## SPAWN MANAGER

Gere le spawn et despawn des objets :

```csharp
//Syntaxe d'appel
SpawnManager.fonctionAUtiliser();

public static void DestroyObject(GameObject go)
public static void SpawnObject(GameObject go, Vector3 SpawnLocation, Quaternion SpawnRotation)
public static void SpawnObject(string PrefabName, Vector3 SpawnLocation, Quaternion SpawnRotation)
public static void SpawnProjectile(GameObject projectilePrefab, GameObject weapon, int initialForce = 0) 
public static void SpawnProjectile(GameObject projectilePrefab, GameObject weapon, Transform altGunpoint, int initialForce = 0) 
public static void SpawnProjectile(GameObject projectilePrefab, GameObject weapon, Vector3 position, Quaternion rotation, int initialForce = 0) 
public static void SpawnWeapon(GameObject weaponPrefab, GameObject weaponHolder)
public static void SpawnUnit(GameObject unitPrefab, Vector3 spawnPosition, Quaternion spawnRotation = default(Quaternion), Vector3 rallyPosition = default(Vector3)) 
public static void SpawnUnit(string unitPrefabName, Vector3 spawnPosition, Quaternion spawnRotation = default(Quaternion), Vector3 rallyPosition = default(Vector3)) 
public static void SpawnPlayer(string PlayerPrefabName, Vector3 SpawnLocation)
public static void DestroyPlayer(GameObject go)
```


## TEAM MANAGER

Gere les équipes :

```csharp
//Syntaxe d'appel
TeamManager.fonctionAUtiliser();

public static void SetTeam(ulong clientId, int teamId) 
public static int GetTeam(ulong clientId) 
public static bool AreObjectsEnnemies(GameObject go1, GameObject go2)
```


## COLOR MANAGER

Gère les couleurs des objets en fonction du joueur et de son équipe :

```csharp
//Syntaxe d'appel
ColorManager.fonctionAUtiliser();

public static void SetPlayerMaterial(int playerId, int matId) 
public static void SetTeamMaterial(int teamId, int matId) 
public static void SetObjectColors(GameObject objectToColor)
public static void SetBlueprintColor(GameObject go, bool Allowed)
public static void SetPlayerColors(ulong clientId)
public static void DrawBox(Vector3 p1, Vector3 p2)
public static void DrawBounds(Bounds b)
```

## PLAYER MANAGER

Gère les infos des joueurs 

```csharp
//Syntaxe d'appel
PlayerManager.fonctionAUtiliser();

public static void SetPlayerObjectID(ulong clientId, int playerObjectId)
public static int GetPlayerObjectID(ulong clientID)
public static GameObject GetPlayerObject(ulong clientID)
public static Camera GetPlayerCamera(ulong clientID)
public static int GetPlayerTeam(ulong clientID)
```

# HERITAGE


## Abstract 

Toute classe avec une/des methodes abstract doit etre une classe abstraite.
Une classe abstraite ne peut qu'être héritée.
La classe héritante devra implementer les fonctions avec le mot clef "abstract".

Classe abstraite :

```csharp
public abstract class ClasseAbstraite
{
    void Start()
    {
        AbstractFonction();
    }

    public abstract void AbstractFonction;
}
```

Classe héritante :

```csharp
public class ClasseHeritante : ClasseAbstraite
{
    public override void AbstractFonction()
    {
        //Contenu de la fonction
    }

}
```


## Virtual

Une methode avec le mot clef "virtual" aura un comportement par défaut décrit dans la classe mère, mais peut être redéfinie dans la classe héritante.
Dans la classe héritante, on peut aussi appeler la methode virtuelle de la classe mere pour avoir les deux comportements.

Classe mère :

```csharp
public void class ClasseMereAvecVirtual
{
    void Start()
    {
        VirtualFonction();
    }

    public virtual void VirtualFonction()
    {
        //Comportement par défaut
    }
}
```

Classe héritante :

```csharp
public class ClasseHeritante : ClasseAbstraite
{
    public override void VirtualFonction()
    {
        //Ajouter cette ligne si vous voulez quand meme executer la methode de la classe mere
        base.VirtualFonction();

        //Contenu de la fonction
    }

}
```


## Nos classes abstraites

### Arme : 

Donne les proprietes d'arme a un objet.
La classe héritante devra implémenter l'action a realiser lors du tir, et optionellement du tir alternatif.

```csharp
public abstract void OnShoot;
public virtual void OnShootAlt() {};
```

### Unit :

Donne les proprietes d'unité a un objet.
La classe héritante devra implémenter l'attaque de l'unité.

```csharp
public abstract void AttackAction; 
```

### Projectile :

Donne les proprietes de projectile à un objet.
La classe héritante devra implémenter le comportement lors d'une collision.

```csharp
public abstract void OnProjectileCollision;
```

### RTSBuilding :
Donne les proprietes de batiment à un objet.




# CREER UN NOUVEL OBJET


## Général 


### Scripts Awake() & Start()

```csharp
//Si vous voulez un Awake()
public override void Awake()
{
    base.Awake();
}

//Si vous voulez un Start()
public override void Start()
{
    base.Start();
}
```


### Renderers

Les components "Renderer" seront placés dans les objets-enfants :
Les Objets-Enfants avec un renderer seront colorés si ils ont le tag "PlayerColor" ou "TeamColor".

- NouvelObjet : 
    - Objet-Enfant 1 : 'renderer' + tag "PlayerColor" -> Colore l'objet avec la couleur du joueur. 
    - Objet-Enfant 2 : 'renderer' + tag "PlayerColor" -> Colore l'objet avec la couleur de l'équipe du joueur.  
    - Objet-Enfant 3 : 'renderer' -> Colore l'objet avec la couleur assignée dans le renderer.


## Arme

Dans le dossier Prefabs/Armes, créer un prefab et un script pour votre arme.
Configurer le tag en "Arme".
Assigner le script a votre arme, remplir les infos dans l'éditeur.

```csharp
using UnityEngine;
using classes;

public class NouvelleArme : Arme
{	
    //Obligatoire
    public override void OnShoot()
    {
        //Executé lors du tir
    }

    //Optionnel
    public override void OnShootAlt()
    {
        //Executé lors du tir alternatif
    }
}
```


## Projectile

Dans le dossier Prefabs/Projectiles, créer un prefab et un script pour votre projectile.
Configurer le tag en "Projectile".
Assigner le script a votre projectile.

```csharp
using UnityEngine;
using classes;

public class NouveauProjectile : Projectile 
{	
    //Obligatoire
    public override void OnProjectileCollision(GameObject target)
    {
        //Executé lors du tir alternatif
    }
}
```


## Units

Dans le dossier Prefabs/Sbires, créer un prefab et un script pour votre unité.
Configurer le tag en "Unit".
Assigner le script a votre unité, remplir les infos dans l'éditeur.

```csharp
using UnityEngine;
using classes;

public class NouvelleUnite : Unit
{	
    //Obligatoire
    public override void AttackAction()
    {
        //Executé lorsque l'unité attaque
    }
}
```

## Batiments 

Dans le dossier Prefabs/Batiments, créer un prefab et un script pour votre batiment.
Configurer le tag en "Building".
Assigner le script a votre batiment, remplir les infos dans l'éditeur.


# Notes

Start sans unity hub :
Creer un .txt, dans lequel il faut adapter la ligne suivante
start "" "D:\Logiciels\Unity\Unity Editors\2022.3.5f1\Editor\Unity.exe" -projectpath "D:\Logiciels\Unity\Projets\LeMeilleurJeu"
renommer le fichier en .bat
???
profit

blender to unity :
S'assurer que les pos, rot et scales soient à 0 (object mode -> A (select all) -> Ctrl + a -> Apply all transforms)
sauvegarder le .blend dans LeMeilleurJeu/Assets/Ressources/BlenderImports

Recuperer toutes les animations d'un .blend:
"I FIGURED IT OUT. Apparently Unity can recognise animations set up this way but as of version 2019.3 it’s disabled by default and there’s no way to enable it within the editor. In order to enable it, you must navigate to /Data/Tools/ in your installation directory (I installed it using the default path so for me it was C:\Program Files\2019.3.0f6\Editor\Data), open Unity-BlenderToFBX.py and change bake_anim_use_all_actions=False, (it should be on line 43) to bake_anim_use_all_actions=True,. This lets unity properly recognise your actions."


Ragdolls :
Il y a un component "Articulation Body", qui pourrait aider a automatiser le process de ragdoll
