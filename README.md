﻿# LeMeilleurJeu
Ben c'est le meilleur jeu quoi



-SYSTEMES-

Pour faciliter la création d'objets, on peut ajouter des fonctionnalites a un prefab

HealthSystem :

Ce script donne un systeme de vie a l'objet qui le possède. 
Il suffit de placer ce component sur un objet.

    //Faire perdre des points de vie a un objet
    GameObject cible;
    cible.GetComponent<HealthSystem>().LoosePv(int dmg)


-ABSTRACT CLASSES-

Une classe abstraite ne peut qu'être héritée.
La classe héritante devra implementer les fonctions avec le mot clef "abstract".

Classe abstraite :

    public abstract class ClasseAbstraite
    {
        void Start()
        {
            AbstractFonction();
        }

        public abstract void AbstractFonction;
    }

Classe héritante :

    using AbstractClasses;

    public class ClasseHeritante : ClasseAbstraite
    {
        public override void AbstractFonction()
        {
            //Contenu de la fonction
        }

    }


Nos classes abstraites 

RTSUnit :

Donne les proprietes d'unité RTS a un objet.
La classe héritante devra implémenter l'attaque de l'unité.

    public abstract void AttackAction; 


Projectile :

Donne les proprietes de projectile à un objet.
La classe héritante devra implémenter le comportement lors d'une collision.

    public abstract void OnProjectileCollision;


RTSBuilding :
Donne les proprietes de batiment à un objet.



-MANAGERS-

Pour faciliter le code, il y a des objets préplacés dans la scène, qui possedent des fonctions accessibles dans tout le projet.


SPAWN MANAGER

Gere le spawn et despawn des objets :

    //Syntaxe d'appel
    SpawnManager.fonctionAUtiliser();

    //Crée un nouvel objet, qui appartient au client qui l'appelle
    SpawnObject(GameObject Prefab, Vector3 SpawnLocation, Quaternion SpawnRotation);
    SpawnObjectByName(string PrefabName, Vector3 SpawnLocation, Quaternion SpawnRotation);
    
    //Crée une explosion
    SpawnExplosion();

    //Détruit un objet
    DestroyObject(GameObject go);

    //Détruit l'objet joueur, et en crée un nouveau
    //PlayerPrefabName = "FPSPlayer" || "RTSPlayer"
    SpawnPlayer(string PlayerPrefabName, Vector3 SpawnLocation);

    //Détruit l'objet joueur
    DestroyPlayer();


TEAM MANAGER

Gere les équipes :

    //Syntaxe d'appel
    TeamManager.fonctionAUtiliser();

    //Assigne un joueur à une équipe
    SetTeam(ulong clientId, int teamId);

    //Retourne l'équipe du joueur
    int team = GetTeam(ulong clientId);


COLOR MANAGER

Gère les couleurs des objets en fonction du joueur et de son équipe :

    //Syntaxe d'appel
    ColorManager.fonctionAUtiliser();

    //Assigne un material a un joueur
    SetPlayerMaterial(int playerId, int matId);

    //Assigne un material a une équipe
    SetTeamMaterial(int teamId, int matId);

    //Applique les couleurs de joueur et d'équipe à
    //un objet, en fonction des infos joueur/team
    SetObjectColors(GameObject objectToColor)


SERVER MANAGER :

Vu qu'on va mettre uniquement ce que le serveur doit faire en début de jeu, il n'y a pas de fonction accessible.

