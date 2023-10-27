# LeMeilleurJeu
Ben c'est le meilleur jeu quoi



-SYSTEMES-

Pour faciliter la création d'objets, on peut ajouter des scripts pour différents systèmes.

HEALTHSYSTEM

Ce script donne un systeme de vie a l'objet qui le possède. 
Il suffit de placer ce component sur un objet.

    //Faire perdre des points de vie a un objet
    GameObject cible;
    cible.GetComponent<HealthSystem>().LoosePv(int dmg)

BATIMENTSYSTEM

Ce script donne un systeme de blueprint a l'objet qui le possède :
En plus de ce component, l'objet requiert un Rigidbody et un Collider.

UNITSYSTEM

Ce script donne un systeme d'unité à l'objet qui le possède :
En plus de ce component, l'objet requiert un NavMeshAgent.




-MANAGERS-

Pour faciliter le code, il y a des objets préplacés dans la scène, qui possedent des fonctions accessibles dans tout le projet.


SPAWN MANAGER

Gere le spawn et despawn des objets :

    //Syntaxe d'appel
    SpawnManager.fonctionAUtiliser();

    //Crée un nouvel objet, qui appartient au client qui l'appelle
    SpawnObject(GameObject Prefab, Vector3 SpawnLocation, Quaternion SpawnRotation);
    SpawnObjectByName(string PrefabName, Vector3 SpawnLocation, Quaternion SpawnRotation);

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

