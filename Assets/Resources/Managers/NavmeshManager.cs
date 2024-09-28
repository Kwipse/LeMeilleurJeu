using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class NavmeshManager : MonoBehaviour
{
    static AsyncOperation lastNavmeshUpdate;
    static NavMeshSurface navmeshSurface;
    static bool isUpdating;
    static bool isDirty;

    static NavMeshLinks_AutoPlacer navmeshLinker;

    static NavmeshManager NM;
    void Awake()
    {
        NM = this;
    }

    void Start()
    {
        GameObject sol = GameObject.Find("Sol");
        navmeshSurface = sol.GetComponent<NavMeshSurface>();
        navmeshLinker = sol.GetComponent<NavMeshLinks_AutoPlacer>();

        isUpdating = false;
        //UpdateNavmesh();
    }

    public static void UpdateNavmesh()
    {
        if (isUpdating && !isDirty)
        {
            //Debug.Log($"NavmeshManager : Navmesh is dirty");
            isDirty = true;
        }

        if (!isUpdating)
        {
            isUpdating = true;
            isDirty = false;
            lastNavmeshUpdate = navmeshSurface.UpdateNavMesh(navmeshSurface.navMeshData);
            lastNavmeshUpdate.completed += OnNavmeshUpdated;
        }
    }


    static void OnNavmeshUpdated(AsyncOperation updateOperation)
    {
        isUpdating = false;
        //Debug.Log($"NavmeshManager : Navmesh updated");
        updateOperation.completed -= OnNavmeshUpdated;
        if (isDirty)
        {
            UpdateNavmesh(); 
            return;
        }

        //if (!navmeshLinker.isCalculatingNewEdges) { navmeshLinker.UpdateLinks(); }
        
    }

    //nms.UpdateNavMesh(nms.navMeshData);

    //Bounds b = new Bounds(gameObject.transform.position, new Vector3(500,500,500));
    //List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
    //NavMeshBuilder.CollectSources(null, 1, NavMeshCollectGeometry.PhysicsColliders, 0, true,  new List<NavMeshBuildMarkup>(), false, sources);
    //NavMeshBuilder.UpdateNavMeshDataAsync(nms.navMeshData, nms.GetBuildSettings(), sources, b);

    //Debug.Log($"{sources.Count}");
    //foreach (var s in sources)
    //{
    //    if (s.sourceObject) {
    //        Debug.Log($"{s.sourceObject.name}"); }
    //}

    //nmlinker.Generate();
    //gameObject.GetComponent<NavMeshLinks_AutoPlacer>().Generate();
}
