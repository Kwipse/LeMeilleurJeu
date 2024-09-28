using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Voxel
{
    public Chunk chunk;
    public Vector3Int chunkCoord;

    public Vector3 position;
    public Vector3Int localCoord;
    public Vector3Int globalCoord;
    public Vector3Int allVoxelsCoord;

    public bool isActive;
    public VoxelType type; // Using the VoxelType enum


    public enum VoxelType
    {
        Air,    
        Ground,
    }


    public Voxel(Vector3Int globalCoord, VoxelType type, bool isActive)
    {
        this.globalCoord = globalCoord;
        this.position = globalCoord; // * voxelSize
        this.type = type;
        this.isActive = isActive;

        this.chunk = TerrainManager.GetChunk(this.position);
        this.chunkCoord = TerrainManager.GetChunkCoordinates(chunk);

        this.localCoord = globalCoord - (chunkCoord * TerrainManager.GetChunkSize());
        this.allVoxelsCoord = TerrainManager.GetPositiveCoord(globalCoord);

    }
}       

public class Chunk : MonoBehaviour
{
    public Voxel[,,] voxels;
    public Bounds bounds;
    private bool voxelMesh = TerrainManager.GetVoxelMesh();
    private int chunkSize = TerrainManager.GetChunkSize();
    private int worldHeight = TerrainManager.GetWorldHeight();
    private int worldSize = TerrainManager.GetWorldSize();
    private float noiseStrenght = TerrainManager.GetNoiseStrenght();
    private float alwaysGroundHeight = TerrainManager.GetAlwaysGroundHeight();
    private float alwaysAirHeight = TerrainManager.GetAlwaysAirHeight();


    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    private Mesh mesh;
    private List<int> triangles = new List<int>();
    private List<Vector3> vertices = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>(); //Face stuff

    //MarchingCube 
    float[,] noiseValues;
    float[,] groundValues;
    float[,,] pointValues;
    float isoLevel;



    public void Initialize() //called from VoxelWorld
    {
        voxels = new Voxel[chunkSize, chunkSize, chunkSize];
        noiseValues = new float[chunkSize, chunkSize];
        groundValues = new float[chunkSize, chunkSize];
        pointValues = new float[chunkSize+1, chunkSize+1, chunkSize+1];
        mesh = new Mesh();
        meshFilter = gameObject.AddComponent<MeshFilter>(); 
        meshRenderer = gameObject.AddComponent<MeshRenderer>(); 
        meshCollider = gameObject.AddComponent<MeshCollider>(); 

        InitializeVoxels();
    }

    private void InitializeVoxels()
    {
        for (int x = 0 ; x < chunkSize ; x++) {
            for (int z = 0 ; z < chunkSize ; z++)
            {
                noiseValues[x,z] = TerrainManager.GetGlobalNoiseValue(transform.position.x + x , transform.position.z + z)/256;
                groundValues[x,z] = noiseValues[x,z] * noiseStrenght;

                for (int y = 0 ; y < chunkSize ; y++)
                {
                    Vector3Int globalCoord = (TerrainManager.GetChunkCoordinates(this)*chunkSize) + new Vector3Int(x,y,z);
                    Voxel.VoxelType type = DetermineVoxelType(groundValues[x,z], transform.position.y + y);
                    voxels[x,y,z] = new Voxel(globalCoord, type, type != Voxel.VoxelType.Air);

                }
            } }
    }

    private Voxel.VoxelType DetermineVoxelType(float groundValue, float voxelHeight)
    {
        float minGroundHeight = worldHeight * chunkSize * alwaysGroundHeight;
        float maxGroundHeight = worldHeight * chunkSize * alwaysAirHeight;

        //Not between min/max
        if (voxelHeight <= minGroundHeight) { return Voxel.VoxelType.Ground; }
        if (voxelHeight >= maxGroundHeight) { return Voxel.VoxelType.Air; }

        //Between min/max
        if ((voxelHeight <= groundValue)) {
            return Voxel.VoxelType.Ground; }
        else {
            return Voxel.VoxelType.Air; }
    }



    public async void UpdateMesh()
    {

        if (!voxelMesh)
        {
            CalculatePointValuesFromNoise();
        }
        if (voxelMesh)
        {
            await Task.Run(() => CalculatePointValuesFromVoxels());
        }


        vertices.Clear();
        triangles.Clear();
        uvs.Clear();

        await Task.Run(() => March());
        //March();

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        //mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        meshRenderer.material = TerrainManager.GetVoxelMaterial();

        //check for mesh with no faces or it will error
        if (vertices.Count == 0) {
            meshCollider.sharedMesh = null; }
        if (vertices.Count > 0 && vertices.Count < 10) {
            if (vertices[0] == vertices[1]) { meshCollider.sharedMesh = null; } }
        if (vertices.Count >= 10) {
            meshCollider.sharedMesh = mesh; }

        //Debug.Log($"building mesh for chunk {TerrainManager.GetChunkCoordinates(this)}, with {vertices.Count} vertices");
        NavmeshManager.UpdateNavmesh();
    }



    public void CalculatePointValuesFromVoxels(int depth = 2)
    {
        int nbValues = (int) Mathf.Pow(1 + 2*depth, 3) - 1;

        //For each voxel
        for (int x = 0; x <= chunkSize ; x++) {
            for (int z = 0; z <= chunkSize ; z++) {
                for (int y = 0; y <= chunkSize ; y++) {
                    pointValues[x,y,z] = 0;

                    Voxel currentVoxel = TerrainManager.GetVoxel(new Vector3Int(x,y,z),this);
                    if (currentVoxel == null) { continue; }


                    //if all direct neighbours are the same, set pointValue to it
                    float depht1 = 0;
                    depht1 += ProcessVoxel(x, y + 1, z); // Top
                    depht1 += ProcessVoxel(x, y - 1, z); // Bottom
                    depht1 += ProcessVoxel(x - 1, y, z); // Left
                    depht1 += ProcessVoxel(x + 1, y, z); // Right
                    depht1 += ProcessVoxel(x, y, z + 1); // Front
                    depht1 += ProcessVoxel(x, y, z - 1); // Back
                    if (depht1 == 0)  { pointValues[x,y,z] = 0f; continue; }
                    if (depht1 == 6)  { pointValues[x,y,z] = 1f; continue; }

                    //Debug.Log($"Voxel {currentVoxel.globalCoord}, all coord {currentVoxel.allVoxelsCoord}");

                    //for each adjacent voxel
                    for (int ax = -depth; ax <= depth ; ax++) {
                        for (int ay = -depth; ay <= depth ; ay++) {
                            for (int az = -depth; az <= depth ; az++) {

                                //float distance = Vector3.Distance(new Vector3(x,y,z), new Vector3(x+ax,y+ay,z+az));
                                //if (distance == 0) { continue; }
                                //pointValues[x,y,z] += ProcessVoxel(x+ax, y+ay, z+az)/(nbValues);
                                pointValues[x,y,z] += TerrainManager.isVoxelActive(new Vector3Int(ax,ay,az), currentVoxel)?1f/(nbValues):0;

                            }}}

                }}}

        isoLevel = 0.5f;
    }


    float ProcessVoxel(int x, int y, int z)
    {

        //Voxel outside chunk
        if (x < 0 || y < 0 || z < 0 || x >= chunkSize || y >= chunkSize || z >= chunkSize) {
            Voxel voxel = TerrainManager.GetVoxel(new Vector3Int(x,y,z), this);
            return voxel != null ? (voxel.isActive ? 1 : 0) : 0;
        }

        //Voxel inside chunk
        else {
            return voxels[x,y,z].isActive ? 1 : 0; }
        
    }



    void CalculatePointValuesFromNoise()
    {
        float minGroundHeight = worldHeight * chunkSize * alwaysGroundHeight;
        float maxGroundHeight = worldHeight * chunkSize * alwaysAirHeight;

        for (int x = 0; x <= chunkSize ; x++) {
            for (int z = 0; z <= chunkSize ; z++) {

                float noise = TerrainManager.GetGlobalNoiseValue(transform.position.x + x , transform.position.z + z)/256;
                float groundLevel = noise * noiseStrenght;

                for (int y = 0; y <= chunkSize ; y++) {

                    float globalY = transform.position.y + y;
                    pointValues[x,y,z] = (groundLevel - globalY) / chunkSize;
                    if (globalY > maxGroundHeight) { pointValues[x,y,z] = Mathf.Min(pointValues[x,y,z], 0); }
                    if (globalY < minGroundHeight) { pointValues[x,y,z] = Mathf.Max(pointValues[x,y,z], 1); }

                    isoLevel = 0.5f;
                }}}
    }

    void March()
    {
        for (int x = 0; x < chunkSize ; x++) {
            for (int z = 0; z < chunkSize ; z++) {
                for (int y = 0; y < chunkSize ; y++) {

                    Vector3 localPos = new Vector3(x,y,z);


                    // Set values at the corners of the cube
                    float[]cubeValues=new float[]{
                        pointValues[x,y,z+1],
                        pointValues[x+1,y,z+1],
                        pointValues[x+1,y,z],
                        pointValues[x,y,z],
                        pointValues[x,y+1,z+1],
                        pointValues[x+1,y+1,z+1],
                        pointValues[x+1,y+1,z],
                        pointValues[x,y+1,z]
                    };


                    // Find the triangulation index (NdN : ceci est une dinguerie a base de bits)
                    int cubeIndex = 0;
                    if (cubeValues[0] < isoLevel) cubeIndex |= 1;
                    if (cubeValues[1] < isoLevel) cubeIndex |= 2;
                    if (cubeValues[2] < isoLevel) cubeIndex |= 4;
                    if (cubeValues[3] < isoLevel) cubeIndex |= 8;
                    if (cubeValues[4] < isoLevel) cubeIndex |= 16;
                    if (cubeValues[5] < isoLevel) cubeIndex |= 32;
                    if (cubeValues[6] < isoLevel) cubeIndex |= 64;
                    if (cubeValues[7] < isoLevel) cubeIndex |= 128;


                    // Triangulate
                    int[] edges = MarchingCubesTables.triTable[cubeIndex]; // Get the intersecting edges
                    for (int i = 0; edges[i] != -1; i += 3)
                    {
                        int e00 = MarchingCubesTables.edgeConnections[edges[i]][0];
                        int e01 = MarchingCubesTables.edgeConnections[edges[i]][1];

                        int e10 = MarchingCubesTables.edgeConnections[edges[i+1]][0];
                        int e11 = MarchingCubesTables.edgeConnections[edges[i+1]][1];

                        int e20 = MarchingCubesTables.edgeConnections[edges[i+2]][0];
                        int e21 = MarchingCubesTables.edgeConnections[edges[i+2]][1];

                        Vector3 a = Interp(MarchingCubesTables.cubeCorners[e00], cubeValues[e00], MarchingCubesTables.cubeCorners[e01], cubeValues[e01]) + localPos;
                        Vector3 b = Interp(MarchingCubesTables.cubeCorners[e10], cubeValues[e10], MarchingCubesTables.cubeCorners[e11], cubeValues[e11]) + localPos;
                        Vector3 c = Interp(MarchingCubesTables.cubeCorners[e20], cubeValues[e20], MarchingCubesTables.cubeCorners[e21], cubeValues[e21]) + localPos; 

                        AddTriangle(a,b,c);
                    }
                }
            }
        }
    }

    void AddTriangle(Vector3 a, Vector3 b, Vector3 c) {
        int triIndex = triangles.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        triangles.Add(triIndex);
        triangles.Add(triIndex + 1);
        triangles.Add(triIndex + 2);
    }

    Vector3 Interp(Vector3 edgeVertex1, float valueAtVertex1, Vector3 edgeVertex2, float valueAtVertex2) {
        return (edgeVertex1 + (isoLevel - valueAtVertex1) * (edgeVertex2 - edgeVertex1) / (valueAtVertex2 - valueAtVertex1));
    }






    //FACES
    public void IterateVoxels()
    {
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                for (int z = 0; z < chunkSize; z++) {

                    if (voxels == null) { Debug.Log($"voxels is null"); continue; } //Quit on voxels not initialised
                    if (!voxels[x,y,z].isActive) { continue; } //Quit on voxel inactive


                    //Determine visible faces
                    bool[] facesVisible = new bool[6];
                    facesVisible[0] = IsFaceVisible(x, y + 1, z); // Top
                    facesVisible[1] = IsFaceVisible(x, y - 1, z); // Bottom
                    facesVisible[2] = IsFaceVisible(x - 1, y, z); // Left
                    facesVisible[3] = IsFaceVisible(x + 1, y, z); // Right
                    facesVisible[4] = IsFaceVisible(x, y, z + 1); // Front
                    facesVisible[5] = IsFaceVisible(x, y, z - 1); // Back

                    for (int i = 0; i < facesVisible.Length; i++) {
                        if (facesVisible[i]) { AddFaceData(x, y, z, i); } }
                }
            } 
        }
    }


    private bool IsFaceVisible(int x, int y, int z)
    {
        //Border Faces
        if (x == -1 || y == -1 || z == -1 || x == chunkSize || y == chunkSize || z == chunkSize)
        {
            Vector3 globalPos = transform.position + new Vector3(x,y,z);
            Chunk neighborChunk = TerrainManager.GetChunk(globalPos);

            if (neighborChunk == null) { return true; }

            Vector3 localPos = neighborChunk.transform.InverseTransformPoint(globalPos);
            return !neighborChunk.IsVoxelActiveAt(localPos);
        }

        //Inside Faces
        else
        {
            return !voxels[x,y,z].isActive;
        }

    }
 

    public bool IsVoxelActiveAt(Vector3 localPosition)
    {
        localPosition = localPosition / chunkSize;

       int x = (int) localPosition.x;
       int y = (int) localPosition.y;
       int z = (int) localPosition.z;

        // Check if the indices are within the bounds of the voxel array
        if (x >= 0 && y >= 0 && z >= 0  &&  x < chunkSize && y < chunkSize && z < chunkSize)
        {
            return voxels[x,y,z].isActive;
        }

        Debug.Log($"Voxel out of bounds");
        return false;
    }

    private void AddFaceData(int x, int y, int z, int faceIndex)
    {
        if (faceIndex == 0) // Top Face
        {
            vertices.Add(new Vector3(x,     y + 1, z    ));
            vertices.Add(new Vector3(x,     y + 1, z + 1)); 
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            vertices.Add(new Vector3(x + 1, y + 1, z    )); 
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(0, 1));
        }

        if (faceIndex == 1) // Bottom Face
        {
            vertices.Add(new Vector3(x,     y, z    ));
            vertices.Add(new Vector3(x + 1, y, z    )); 
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x,     y, z + 1)); 
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));
        }

        if (faceIndex == 2) // Left Face
        {
            vertices.Add(new Vector3(x, y,     z    ));
            vertices.Add(new Vector3(x, y,     z + 1));
            vertices.Add(new Vector3(x, y + 1, z + 1));
            vertices.Add(new Vector3(x, y + 1, z    ));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(0, 1));
        }

        if (faceIndex == 3) // Right Face
        {
            vertices.Add(new Vector3(x + 1, y,     z + 1));
            vertices.Add(new Vector3(x + 1, y,     z    ));
            vertices.Add(new Vector3(x + 1, y + 1, z    ));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));
        }

        if (faceIndex == 4) // Front Face
        {
            vertices.Add(new Vector3(x,     y,     z + 1));
            vertices.Add(new Vector3(x + 1, y,     z + 1));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            vertices.Add(new Vector3(x,     y + 1, z + 1));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 1));
        }

        if (faceIndex == 5) // Back Face
        {
            vertices.Add(new Vector3(x + 1, y,     z    ));
            vertices.Add(new Vector3(x,     y,     z    ));
            vertices.Add(new Vector3(x,     y + 1, z    ));
            vertices.Add(new Vector3(x + 1, y + 1, z    ));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 0));

        }


        //Add triangles to list
        int vertCount = vertices.Count;

        // First triangle
        triangles.Add(vertCount - 4);
        triangles.Add(vertCount - 3);
        triangles.Add(vertCount - 2);

        // Second triangle
        triangles.Add(vertCount - 4);
        triangles.Add(vertCount - 2);
        triangles.Add(vertCount - 1);
    }


}
