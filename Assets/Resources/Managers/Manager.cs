using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace managers
{
    //Interfaces
    public interface ISyncBeforeGame //Pour les managers qui doivent etre synchro avant le jeu
    { 
        void InitializeBeforeSync() { }
        void StartSync() { EndSync(); }
        void EndSync() 
        {
            //GameManager.OnManagerSynchronized(); 
        }
    }

    public interface IWaitForGameSync //Pour les managers qui doivent attendre la synchro
    {
        void OnGameSynchronized() { }
    }


    //Manager abstract class
    public abstract class Manager : NetworkBehaviour
    {

        public override void OnNetworkSpawn()
        {
            //Debug.Log($"{this.GetType()} : Initialization before Sync");
            InitializeBeforeSync();

            //If game is not synced yet, call the manager sync methods
            if (!GameManager.isGameSynchronized())
            {
                if (this is ISyncBeforeGame) {
                    //GameManager.AddManagerToSync(this); //Tell GameManager to wait for the manager to sync
                    Debug.Log($"{this.GetType().Name} : Starting manager sync");
                    StartSync(); }

                if (this is IWaitForGameSync) {
                    //GameManager.AddManagerToAfterSync(this); //Tell GameManager to signal on gamesync
                    Debug.Log($"{this.GetType().Name} : Waiting for game sync");
                }

            }

            //If game is already synced, call the manager OnGameSynchronized() method
            if (GameManager.isGameSynchronized())
            {
                if (gameObject.GetComponent<IWaitForGameSync>() != null) {
                    //Debug.Log($"{this.GetType().Name} : Game is already synced");
                    OnGameSynchronized(); }
            }

        }


        //Methodes a implementer dans le manager
        public virtual void InitializeBeforeSync() { }
        public virtual void StartSync() { }
        public virtual void OnGameSynchronized() { }

        //Call this in manager to signal the end of sync to the gamemanager
        public void EndManagerSync() 
        {
            //Debug.Log($"{this.GetType().Name} : Ending sync");
            //GameManager.OnManagerSynchronized((Manager)this);
        }
    }
    
}
