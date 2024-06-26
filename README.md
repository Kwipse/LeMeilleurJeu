﻿# LeMeilleurJeu

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

### Mode construction de batiments

- Molette haut/bas : Choisit le blueprint a construire
- Clic Gauche : Construction du batiment choisi

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

Pour faciliter la création d'objets, on peut ajouter des fonctionnalites a un prefab


## HealthSystem 

Ce script donne un systeme de vie a l'objet qui le possède. 
Il suffit de placer ce component sur un objet.

```csharp
//Faire perdre des points de vie a un objet
GameObject cible;
cible.GetComponent<HealthSystem>().LoosePv(int dmg)
```


## UnitSpawnerSystem

Ce script donne un systeme de création d'unité à l'objet qui le possède.
Dans l'editeur, vous pouvez ajouter des unités à la liste pour que le batiment puisse les produire.

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


## Nos classes

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


# MANAGERS

Pour faciliter le code, il y a des objets préplacés dans la scène, qui possedent des fonctions accessibles dans tout le projet.


## SPAWN MANAGER

Gere le spawn et despawn des objets :

```csharp
//Syntaxe d'appel
SpawnManager.fonctionAUtiliser();

//Crée un nouvel objet, qui appartient au client qui l'appelle
SpawnObject(GameObject Prefab, Vector3 SpawnLocation, Quaternion SpawnRotation);
SpawnObjectByName(string PrefabName, Vector3 SpawnLocation, Quaternion SpawnRotation);

//Crée une explosion
SpawnExplosion(Vector3 position, int size, int unitDmg, int buildingDmg, float duration, int outwardForce);

//Crée un projectile
//initialForce donne une impulsion en avant au projectile
SpawnProjectile(GameObject projectilePrefab, Vector3 position, Quaternion rotation, int initialForce = 0);

//Crée et assigne une arme a un objet
SpawnWeapon(GameObject weaponPrefab, GameObject weaponHolder)

//Détruit un objet
DestroyObject(GameObject go);

//Détruit l'objet joueur, et en crée un nouveau
//PlayerPrefabName = "FPSPlayer" || "RTSPlayer"
SpawnPlayer(string PlayerPrefabName, Vector3 SpawnLocation);

//Détruit l'objet joueur
DestroyPlayer();
```


## TEAM MANAGER

Gere les équipes :

```csharp
//Syntaxe d'appel
TeamManager.fonctionAUtiliser();

//Assigne un joueur à une équipe
SetTeam(ulong clientId, int teamId);

//Retourne l'équipe du joueur
int team = GetTeam(ulong clientId);
```


## COLOR MANAGER

Gère les couleurs des objets en fonction du joueur et de son équipe :

```csharp
//Syntaxe d'appel
ColorManager.fonctionAUtiliser();

//Assigne un material a un joueur
SetPlayerMaterial(int playerId, int matId);

//Assigne un material a une équipe
SetTeamMaterial(int teamId, int matId);

//Applique les couleurs de joueur et d'équipe à
//un objet, en fonction des infos joueur/team
SetObjectColors(GameObject objectToColor)
```


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

