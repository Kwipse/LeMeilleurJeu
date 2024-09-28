using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

[Serializable]
public class Edge
{
    public Vector3 start;
    public Vector3 end;

    public Vector3 startUp;
    public Vector3 endUp;

    public float length;
    public Quaternion facingNormal;
    public bool facingNormalCalculated = false;

    public Vector3 triNormal;


    public Edge(Vector3 startPoint, Vector3 endPoint)
    {
        start = startPoint;
        end = endPoint;
    }
}

[Serializable]
public class Link
{
    public Vector3 position;
    public Edge edge;

    public Link(Vector3 position, Edge edge = null)
    {
        this.position = position;
        this.edge = edge;
    }
}


public class NavMeshLinks_AutoPlacer : MonoBehaviour
{
    public Transform linkPrefab;
    public Transform OnewayLinkPrefab;
    public float tileWidth = 5f;

    [Header("OffMeshLinks")]
    public float maxJumpHeight = 3f;
    public float maxJumpDist = 5f;
    public LayerMask raycastLayerMask = -1;
    public float sphereCastRadius = 1f;

    //how far over to move spherecast away from navmesh edge to prevent detecting the same edge
    public float cheatOffset = .25f;

    //how high up to bump raycasts to check for walls (to prevent forming links through walls)
    public float wallCheckYOffset = 0.5f;

    [Header("EdgeNormal")] public bool invertFacingNormal = false;
    public bool dontAllignYAxis = false;


    //private Dictionary<Vector3, Vector3> edgeLinks = new Dictionary<Vector3, Vector3>(); //Dict of linkPos-normal
    private List<Edge> edges = new List<Edge>();
    private List<Link> edgeLinks = new List<Link>();

    private float agentRadius = 2;

    private Vector3 ReUsableV3;
    private Vector3 offSetPosY;

    public bool isCalculatingNewEdges = false;


    void Start()
    {
        edges.Clear();
        edgeLinks.Clear();
        agentRadius = NavMesh.GetSettingsByIndex(0).agentRadius;
    }


    public async void UpdateLinks()
    {
        if (isCalculatingNewEdges) {
            Debug.Log($"Link generation already taking place");
            return; }

        isCalculatingNewEdges = true;
        //Debug.Log($"Generating Links");


        var tr = NavMesh.CalculateTriangulation(); 
        Vector3[] vertices = tr.vertices;
        int[] triangles = tr.indices;
        if (vertices.Length == 0) {isCalculatingNewEdges = false; return; } //le reste se plante si le navmesh est vide
        

        List<Edge> sampledEdges = await Task.Run(() => sampledEdges = CalcEdges(vertices,triangles));
        List<Edge> newEdges = OperateEdges(EdgeOptions.merge, edges, sampledEdges);
        List<Edge> allEdges = OperateEdges(EdgeOptions.merge, edges, sampledEdges);
        List<Edge> conservedEdges = OperateEdges(EdgeOptions.uniquesWithDuplicates, edges, sampledEdges);
        List<Edge> movingEdges = OperateEdges(EdgeOptions.substractions, edges, conservedEdges);

        Debug.Log($"{edges.Count} currentEdges, {sampledEdges.Count} sampled edges, {conservedEdges.Count} conserved edges, {movingEdges.Count} moving edges");

        //find edges changes
        List<Edge> addedEdges = OperateEdges(EdgeOptions.additions, newEdges, edges);
        List<Edge> removedEdges = OperateEdges(EdgeOptions.substractions, newEdges, edges);
        Debug.Log($"{removedEdges.Count} edges to remove");

        List<Link> removedLinks = GetLinksInEdges(removedEdges);
        List<Link> addedLinks = await Task.Run(() => CreateEdgeLinks(addedEdges, tileWidth));

        Debug.Log($"Links updated : {edges.Count}->{sampledEdges.Count} (-{removedEdges.Count}/+{addedEdges.Count}) edges, {edgeLinks.Count} (-{removedLinks.Count}/+{addedLinks.Count}) edge links");

        if (addedEdges.Count != 0) {
            edges = edges.Concat(addedEdges).ToList();
            edgeLinks = edgeLinks.Concat(addedLinks).ToList(); }

        if (removedEdges.Count != 0) {
            edges = edges.Except(removedEdges).ToList();
            edgeLinks = edgeLinks.Except(removedLinks).ToList(); }


        isCalculatingNewEdges = false;
    }


    //Edges
    List<Link> GetLinksInEdges(List<Edge> edges)
    {
        List<Link> links = new List<Link>();
        foreach (var link in edgeLinks) {
            foreach (var edge in edges) {
                if (link.edge == edge) {
                    links.Add(link); } } }
        return links;
    }

    private List<Edge> CalcEdges(Vector3[] vertices, int[] triangles)
    {
        List<Edge> calcEdges = new List<Edge>();

        for (int i = 0; i < triangles.Length - 1; i += 3)
        {
            Vector3 a = vertices[triangles[i]];
            Vector3 b = vertices[triangles[i+1]];
            Vector3 c = vertices[triangles[i+2]];
            Vector3 triangleNormal = Vector3.Cross(a-b,c-b).normalized;

            Edge ab = CreateTriangleEdge(a,b,triangleNormal);
            Edge bc = CreateTriangleEdge(b,c,triangleNormal);
            Edge ac = CreateTriangleEdge(c,a,triangleNormal);

            if (ab != null) { calcEdges.Add(ab); }
            if (bc != null) { calcEdges.Add(bc); }
            if (ac != null) { calcEdges.Add(ac); }
        }

        calcEdges = OperateEdges(EdgeOptions.uniques, calcEdges);

        //Debug.Log($"Sampling edges : {calcEdges.Count} edges");
        return calcEdges;
    }



    private enum EdgeOptions { additions, substractions, allDuplicates, uniquesWithDuplicates, uniques, merge }
    private List<Edge> OperateEdges(EdgeOptions options, List<Edge> edges1, List<Edge> edges2 = null)
    {
        List<Edge> concatEdges = edges2 != null ? edges1.Concat(edges2).ToList() : edges1;

        //Added and removed
        if (options == EdgeOptions.additions) { return edges1.Except(edges2).ToList(); }
        if (options == EdgeOptions.substractions) { return edges2.Except(edges1).ToList(); }

        //Merge
        if (options == EdgeOptions.merge)
        {
            List<Edge> mergedEdges = new List<Edge>();
            foreach (Edge concatEdge in concatEdges)
            {
                bool isEdgeMerged = false;
                foreach (Edge mergedEdge in mergedEdges) {
                    if (AreEdgesEqual(concatEdge, mergedEdge)) {
                        isEdgeMerged = true; } }
                if (isEdgeMerged) { continue; }
                mergedEdges.Add(concatEdge);
            }
            return mergedEdges;
        }

        //Duplicates, uniques
        List<Edge> uniqueEdges = new List<Edge>();
        List<Edge> allDuplicatesEdges = new List<Edge>();
        List<Edge> uniqueDuplicatesEdges = new List<Edge>();
        foreach (Edge existingEdge in concatEdges)
        {
            bool isDuplicate = false;
            foreach (Edge evalEdge in concatEdges) {
                if (existingEdge == evalEdge) { continue; } 
                if (!AreEdgesEqual(existingEdge, evalEdge)) { continue; } 

                //Manage duplicates
                allDuplicatesEdges.Add(existingEdge); 
                isDuplicate = true;
                if (uniqueDuplicatesEdges.Contains(evalEdge)) { break; }
                uniqueDuplicatesEdges.Add(existingEdge);
                break; }

            if (isDuplicate) { continue; }

            //Manage uniques
            uniqueEdges.Add(existingEdge);
            uniqueDuplicatesEdges.Add(existingEdge);
        }

        //Debug.Log($"Compare Edge : {allDuplicatesEdges.Count} duplicates, {uniqueDuplicatesEdges.Count} uniques duplicates, {uniqueEdges.Count} uniques");
        if (options == EdgeOptions.allDuplicates) { return allDuplicatesEdges; }
        if (options == EdgeOptions.uniques) { return uniqueEdges; }
        if (options == EdgeOptions.uniquesWithDuplicates) { return uniqueDuplicatesEdges; }



        return null;
    }

    private bool AreEdgesEqual(Edge edge1, Edge edge2) {
        if (Vector3.Distance(edge1.start, edge2.start) + Vector3.Distance(edge1.end, edge2.end) < 0.1f) { return true; }
        if (Vector3.Distance(edge1.start, edge2.end) + Vector3.Distance(edge1.end, edge2.start) < 0.1f) { return true; }
        return false; }

    private Edge CreateTriangleEdge(Vector3 p1, Vector3 p2, Vector3 triangleNormal) {
        Edge newEdge = new Edge(p1, p2);
        newEdge.triNormal = Vector3.Cross(triangleNormal, p2 - p1).normalized;
        newEdge.length = Vector3.Distance( newEdge.start, newEdge.end);
        return newEdge; }



    //Links
    private List<Link> CreateEdgeLinks(List<Edge> edgeList, float lenghtBetweenLinks)
    {
        if (edgeList.Count == 0) { return null; }
        if (lenghtBetweenLinks == 0) { return null; }

        List<Link> links = new List<Link>();
        float distanceToNextTile = lenghtBetweenLinks;

        foreach (Edge edge in edgeList)
        {
            // no tiles because too close
            if (distanceToNextTile > edge.length) {
                distanceToNextTile -= edge.length;
                continue; }

            int tilesCountWidth = 1 + Mathf.FloorToInt((edge.length-distanceToNextTile) / lenghtBetweenLinks);
            float remainingEdgeDistance = (edge.length-distanceToNextTile) % lenghtBetweenLinks;

            //Debug.Log($"Edge of lenght {edge.length}, Tiles : {tilesCountWidth} with Width : {lenghtBetweenLinks}, DistanceToNextTile : {distanceToNextTile}, RemainingEdgeDistance {remainingEdgeDistance}");

            for (int columnN = 0; columnN < tilesCountWidth; columnN++)
            {
                float edgeLerp = distanceToNextTile + (edge.length-distanceToNextTile) * columnN / tilesCountWidth;
                Vector3 placePos = Vector3.Lerp(edge.start, edge.end, edgeLerp/edge.length);
                //Debug.Log($"Adding the {columnN+1}/{tilesCountWidth} edge link at ({edgeLerp}/{edge.length}), at pos {placePos}");
                links.Add(new Link(placePos, edge));
            }

            distanceToNextTile = lenghtBetweenLinks - remainingEdgeDistance;
        }

        return links;
    }

    public void ClearLinks()
    {
        List<NavMeshLink> navMeshLinkList = GetComponentsInChildren<NavMeshLink>().ToList();
        while (navMeshLinkList.Count > 0)
        {
            GameObject obj = navMeshLinkList[0].gameObject;
            if (obj != null) DestroyImmediate(obj);
            navMeshLinkList.RemoveAt(0);
        }
    }

    void ManageLinks()
    {
        //Debug.Log($"Managing {edgeLinks.Count} potential links");
        foreach (var link in edgeLinks) {


            //Drop down links
            //CheckPlacePos(linkPos,link.Value);
            if (Physics.Raycast(link.position + link.edge.triNormal * agentRadius * 5, Vector3.down, out RaycastHit downHit, (float) maxJumpHeight))
            {
                //Debug.DrawLine(linkPos, linkPos + linkNormal * agentRadius * 5, Color.white, 10f);
                //Debug.DrawLine(linkPos + linkNormal * agentRadius * 5, downHit.point, Color.white, 10f);

                if (NavMesh.SamplePosition(downHit.point, out NavMeshHit navmeshDownHit, 1f, NavMesh.AllAreas))
                {

                }
            }


            //Alternative links
            //CheckPlacePosHorizontal(linkPos,link.Value);


            //Between links
            foreach (Link otherLink in edgeLinks) {

                float distance = Vector3.Distance(link.position, otherLink.position);
                if (distance > maxJumpDist) { continue; } //Distance check
                if (link.position == otherLink.position) { continue; } //Don't link.position with self
                if (Physics.Raycast(link.position, (otherLink.position - link.position), out RaycastHit hit2, distance)) { continue; } //Don't link if something between
                if (!NavMesh.Raycast(link.position, otherLink.position, out NavMeshHit hit , NavMesh.AllAreas)) { continue; } //Don't link inside navmesh

                //Debug.DrawLine (linkPos, otherLink.position, Color.yellow, 10f );
            } }
    }


    //void OnDrawGizmos()
    //{
    //    if (isCalculatingNewEdges) { return; }

    //    foreach (var edge in edges)
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawLine(edge.start, edge.end);
    //    }

    //    int i = 0;
    //    int interval = 2;
    //    foreach (var link in edgeLinks)
    //    {
    //        i++;
    //        if (i % interval == 0) { continue; }

    //        //edge links 
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawCube(link.position, new Vector3(1,1,1));

    //        ////forward edge links
    //        //Gizmos.color = Color.white;
    //        //Gizmos.DrawCube(link.position + link.edge.triNormal * agentRadius * 5, new Vector3(1,1,1));

    //        //drop down links
    //        if (Physics.Raycast(link.position + link.edge.triNormal * agentRadius * 5, Vector3.down, out RaycastHit downHit, (float) maxJumpHeight)) {
    //            if (NavMesh.SamplePosition(downHit.point, out NavMeshHit navmeshDownHit, 1f, NavMesh.AllAreas)) {
    //                Gizmos.color = Color.white;
    //                Gizmos.DrawCube(navmeshDownHit.position, new Vector3(1,1,1)); } }


    //    }

    //}


    void CalcNormals()
    {
        foreach (Edge edge in edges)
        {
            if (!edge.facingNormalCalculated) {

                edge.facingNormal = Quaternion.LookRotation(Vector3.Cross(edge.end - edge.start, Vector3.up));

                if (edge.startUp.sqrMagnitude > 0)
                {
                    var vect = Vector3.Lerp(edge.endUp, edge.startUp, 0.5f) - Vector3.Lerp(edge.end, edge.start, 0.5f);
                    edge.facingNormal = Quaternion.LookRotation(Vector3.Cross(edge.end - edge.start, vect));

                    //FIX FOR NORMALs POINTING DIRECT TO UP/DOWN
                    float triggerAngle = 0.999f;
                    if (Mathf.Abs(Vector3.Dot(Vector3.up, (edge.facingNormal * Vector3.forward).normalized)) >
                            triggerAngle)
                    {
                        edge.startUp += new Vector3(0, 0.1f, 0);
                        vect = Vector3.Lerp(edge.endUp, edge.startUp, 0.5f) - Vector3.Lerp(edge.end, edge.start, 0.5f);
                        edge.facingNormal = Quaternion.LookRotation(Vector3.Cross(edge.end - edge.start, vect));
                    }
                }

                if (dontAllignYAxis)
                {
                    edge.facingNormal = Quaternion.LookRotation(
                            edge.facingNormal * Vector3.forward,
                            Quaternion.LookRotation(edge.end - edge.start) * Vector3.up
                            );
                }
                edge.facingNormalCalculated = true;
            }
            if (invertFacingNormal) edge.facingNormal = Quaternion.Euler(Vector3.up * 180) * edge.facingNormal;
        }
    }

    bool CheckPlacePos(Vector3 pos, Quaternion normal)
    {
        bool result = false;

        Vector3 startPos = pos + normal * Vector3.forward * agentRadius * 5;
        Vector3 endPos = startPos - Vector3.up * maxJumpHeight * 1.1f;

        Debug.DrawLine ( pos + Vector3.right * 0.2f, endPos, Color.white, 10 );
        Debug.DrawLine ( startPos, endPos, Color.white, 10 );


        NavMeshHit navMeshHit;
        RaycastHit raycastHit = new RaycastHit();
        if (Physics.Linecast(startPos, endPos, out raycastHit, raycastLayerMask.value,
                    QueryTriggerInteraction.Ignore))
        {
            if (NavMesh.SamplePosition(raycastHit.point, out navMeshHit, 1f, NavMesh.AllAreas))
            {
                //Debug.DrawLine( pos, navMeshHit.position, Color.black, 15 );

                if (Vector3.Distance(pos, navMeshHit.position) > 1.1f)
                {
                    //added these 2 line to check to make sure there aren't flat horizontal links going through walls
                    Vector3 calcV3 = (pos - normal * Vector3.forward * 0.02f);
                    if ((calcV3.y - navMeshHit.position.y) > 1f)
                    {

                        //SPAWN NAVMESH LINKS
                        Transform spawnedTransf = Instantiate(
                                linkPrefab.transform,
                                //pos - normal * Vector3.forward * 0.02f,
                                calcV3,
                                normal
                                ) as Transform;

                        var nmLink = spawnedTransf.GetComponent<NavMeshLink>();
                        nmLink.startPoint = Vector3.zero;
                        nmLink.endPoint = nmLink.transform.InverseTransformPoint(navMeshHit.position);
                        nmLink.UpdateLink();

                        spawnedTransf.SetParent(transform);
                    }
                }
            }
        }

        return result;
    }

    bool CheckPlacePosHorizontal(Vector3 pos, Quaternion normal)
    {
        bool result = false;

        Vector3 startPos = pos + normal * Vector3.forward * agentRadius * 2;
        Vector3 endPos = startPos - normal * Vector3.back * maxJumpDist * 1.1f;
        // Cheat forward a little bit so the sphereCast doesn't touch this ledge.
        Vector3 cheatStartPos = LerpByDistance(startPos, endPos, cheatOffset);
        //Debug.DrawRay(endPos, Vector3.up, Color.blue, 5);
        //Debug.DrawLine ( cheatStartPos , endPos, Color.white, 5 );
        //Debug.DrawLine(startPos, endPos, Color.white, 5);


        NavMeshHit navMeshHit;
        RaycastHit raycastHit = new RaycastHit();

        //calculate direction for Spherecast
        ReUsableV3 = endPos - startPos;
        // raise up pos Y value slightly up to check for wall/obstacle
        offSetPosY = new Vector3(pos.x, (pos.y + wallCheckYOffset), pos.z);
        // ray cast to check for walls
        if (!Physics.Raycast(offSetPosY, ReUsableV3, (maxJumpDist/2), raycastLayerMask.value))
        {
            //Debug.DrawRay(pos, ReUsableV3, Color.yellow, 15);
            Vector3 ReverseRayCastSpot = (offSetPosY + (ReUsableV3));
            //now raycast back the other way to make sure we're not raycasting through the inside of a mesh the first time.
            if (!Physics.Raycast(ReverseRayCastSpot, -ReUsableV3, (maxJumpDist+1), raycastLayerMask.value))
            {
                //Debug.DrawRay(ReverseRayCastSpot, -ReUsableV3, Color.red, 15);
                //Debug.DrawRay(ReverseRayCastSpot, -ReUsableV3, Color.red, 15);

                //if no walls 1 unit out then check for other colliders using the Cheat offset so as to not detect the edge we are spherecasting from.
                if (Physics.SphereCast(cheatStartPos, sphereCastRadius, ReUsableV3, out raycastHit, maxJumpDist, raycastLayerMask.value, QueryTriggerInteraction.Ignore))
                    //if (Physics.Linecast(startPos, endPos, out raycastHit, raycastLayerMask.value, QueryTriggerInteraction.Ignore))
                {
                    Vector3 cheatRaycastHit = LerpByDistance(raycastHit.point, endPos, .2f);
                    if (NavMesh.SamplePosition(cheatRaycastHit, out navMeshHit, 1f, NavMesh.AllAreas))
                    {
                        //Debug.Log("Success");
                        //Debug.DrawLine( pos, navMeshHit.position, Color.black, 15 );

                        if (Vector3.Distance(pos, navMeshHit.position) > 1.1f)
                        {
                            //SPAWN NAVMESH LINKS
                            Transform spawnedTransf = Instantiate(
                                    OnewayLinkPrefab.transform,
                                    pos - normal * Vector3.forward * 0.02f,
                                    normal
                                    ) as Transform;

                            var nmLink = spawnedTransf.GetComponent<NavMeshLink>();
                            nmLink.startPoint = Vector3.zero;
                            nmLink.endPoint = nmLink.transform.InverseTransformPoint(navMeshHit.position);
                            nmLink.UpdateLink();

                            spawnedTransf.SetParent(transform);
                        }
                    }
                }
            }
        }
        return result;
    }
    
    //Just a helper function I added to calculate a point between normalized distance of two V3s
    public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x) { return x * Vector3.Normalize(B - A) + A; }
}







#if UNITY_EDITOR

[CustomEditor( typeof( NavMeshLinks_AutoPlacer ) )]
[CanEditMultipleObjects]
public class NavMeshLinks_AutoPlacer_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if( GUILayout.Button( "Generate" ) )
        {
            foreach ( var targ in targets )
            {
                ( ( NavMeshLinks_AutoPlacer ) targ ).UpdateLinks();
            }
        }

        if ( GUILayout.Button ( "ClearLinks" ) )
        {
            foreach ( var targ in targets )
            {
                ( (NavMeshLinks_AutoPlacer)targ ).ClearLinks();
            }
        }
    }

}

#endif
