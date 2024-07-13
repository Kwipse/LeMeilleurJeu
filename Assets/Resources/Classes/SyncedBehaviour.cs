using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//Sync Interfaces
public interface ISyncBeforeGame { } //Pour les objets qui doivent etre synchro avant le jeu
public interface IWaitForGameSync { } //Pour les objets qui doivent attendre la synchro


//Sync Behaviour
[RequireComponent(typeof(NetworkObject))]
public class SyncedBehaviour : NetworkBehaviour
{

    //Called before OnNetworkSpawn()
    protected override void OnSynchronize<T>(ref BufferSerializer<T> serializer)
    {

    }

    //N'oubliez pas de mettre "base.OnNetworkSpawn();" dans la classe dérivée si vous overridez
    public override void OnNetworkSpawn()
    {
        //If game is not synced yet, call the object sync methods
        if (!GameManager.isGameSynchronized())
        {
            if (this is ISyncBeforeGame) {
                GameManager.AddBehaviourToSync(this); //Tell GameManager to wait for the object to sync
                //Debug.Log($"{this.GetType()} : Initialization before Sync");
                InitializeBeforeSync();
                //Debug.Log($"{this.GetType().Name} : Starting object sync");
                StartSync();
            }

            if (this is IWaitForGameSync) {
                GameManager.AddBehaviourToAfterSync(this); //Tell GameManager to signal on gamesync
                //Debug.Log($"{this.GetType().Name} : Waiting for game sync");
            }
        }

        //If game is already synced, call the object StartAfterGameSync() method
        if (GameManager.isGameSynchronized())
        {
            if (this is ISyncBeforeGame) {
                //Debug.Log($"{this.GetType().Name} : Can't sync after game sync !");
                }

            if (gameObject.GetComponent<IWaitForGameSync>() != null) {
                //Debug.Log($"{this.GetType().Name} : Starting after game sync");
                StartAfterGameSync(); }
        }

    }


    //Methodes a implementer au besoin dans la classe dérivée
    public virtual void InitializeBeforeSync() { }
    public virtual void StartSync() { EndSync(); }
    public virtual void StartAfterGameSync() { }

    //Call this from the synced object to signal end of sync
    public void EndSync() 
    {
        //Debug.Log($"{this.GetType().Name} : Ending sync");
        GameManager.OnBehaviourSynchronized((SyncedBehaviour)this);
    }
}
