using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Game Manager
public class GameManager : NetworkBehaviour
{
    static bool isGameSynced;
    static int syncNb;

    static List<SyncedBehaviour> behavioursToSync = new List<SyncedBehaviour>();
    static List<SyncedBehaviour> behavioursWaitingSync = new List<SyncedBehaviour>();


    static GameManager GM;
    void Awake() 
    {
        GM = this;
        isGameSynced = false;
        Debug.Log("GameManager : J'existe !");

        behavioursToSync = new List<SyncedBehaviour>();
        behavioursWaitingSync = new List<SyncedBehaviour>();
        syncNb = 0;
    }

    public static void AddBehaviourToSync(SyncedBehaviour sb) { behavioursToSync.Add(sb); }
    public static void AddBehaviourToAfterSync(SyncedBehaviour sb) { behavioursWaitingSync.Add(sb); }
    public static void OnBehaviourSynchronized(SyncedBehaviour sb) { GM.StartCoroutine(CheckForGameSync(sb)); }

    public static IEnumerator CheckForGameSync(SyncedBehaviour sb)
    {
        yield return new WaitForSeconds(1);
        syncNb ++;
        behavioursToSync.Remove(sb);
        Debug.Log($"GameManager : {sb.GetType().Name} est synchronizé ({syncNb}/{behavioursToSync.Count + syncNb})");

        //Debug.Log($"[{string.Join(",", behavioursToSync)}]");

        if (behavioursToSync.Count == 0) {
            EndGameSynchronization(); }
    }


    static void InitializeGameSynchronization()
    {
        Debug.Log($"GameManager : INITIALISATION DE LA SYNCHRONISATION");
        isGameSynced = true;
        foreach(var b in behavioursToSync) {
            Debug.Log($"{b.GetType().Name} : Initializing sync"); 
            b.InitializeBeforeSync(); }
    }

    static void StartGameSynchronization()
    {
        Debug.Log($"GameManager : COMMENCEMENT DE LA SYNCHRONISATION");
        isGameSynced = true;
        foreach(var b in behavioursToSync) {
            Debug.Log($"{b.GetType().Name} : Starting sync"); 
            b.StartSync(); }
    }

    static void EndGameSynchronization()
    {
        Debug.Log($"GameManager : LE JEU EST SYNCHRONISE");
        isGameSynced = true;
        foreach(var b in behavioursWaitingSync) {
            Debug.Log($"{b.GetType().Name} : Game is synced"); 
            b.StartAfterGameSync(); }
    }


    public static bool isGameSynchronized()
    {
        return isGameSynced;
    }
}
