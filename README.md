# LeMeilleurJeu
Ben c'est le meilleur jeu quoi









SPAWN MANAGER

Pour faire spawn/despawn des objets, plus besoin d'ajouter un component SpawnManager a chaque objet, on peut faire un appel de n'importe où avec la commande :

    SpawnManager.spawner.fonctionAUtiliser();

Pour éviter d'avoir a taper toute la ligne a chaque fois, on peut utiliser l'astuce suivante :

Déclaration :

    SpawnManager SM;

Initialisation :

    SM = (SpawnManager) SpawnManager.spawner;

Utilisation :

    SM.fonctionAUtiliser();


Liste des fonctions du SpawnManager :

Spawn()
DestroyObject()
SpawnPlayer()
DestroyPlayer()



