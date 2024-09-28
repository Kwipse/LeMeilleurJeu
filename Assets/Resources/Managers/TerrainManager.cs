using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainManager : MonoBehaviour
{
    public bool voxelMesh = false;

    public int worldSize = 10;
    public int worldHeight = 3;
    public int chunkSize = 16;

    public int noiseSeed = 12345;
    [Range(0f,255f)] public float noiseStrenght = 32f;
    [Range(0f,255f)] public float noiseFrequency = 32f;

    [Range(0.01f,1f)] public float alwaysGroundHeight = 0f;
    [Range(0.01f,1f)] public float alwaysAirHeight = 1f;


    private static Dictionary<Chunk, Vector3Int> chunks;
    private static Dictionary<Vector3Int, Chunk> chunksCoord;
    public Material VoxelMaterial;
    public static float[,] noise2DArray;

    public static Voxel[,,] allVoxels;
    public static Voxel emptyVoxel;

    static GameObject sol;


    public static TerrainManager TM { get; private set; }
    void Awake()
    {
        TM = this;
        sol = GameObject.Find("Sol");
    }

    public static bool GetVoxelMesh() { return TM.voxelMesh; }
    public static int GetWorldSize() { return TM.worldSize; }
    public static int GetWorldHeight() { return TM.worldHeight; }
    public static int GetChunkSize() { return TM.chunkSize; }
    public static float GetNoiseStrenght() { return TM.noiseStrenght; }
    public static float GetNoiseFrequency() { return TM.noiseFrequency; }
    public static float GetAlwaysGroundHeight() { return TM.alwaysGroundHeight; }
    public static float GetAlwaysAirHeight() { return TM.alwaysAirHeight; }
    public static Material GetVoxelMaterial() { return TM.VoxelMaterial; }

    void Start()
    {
        Noise.Seed = noiseSeed;
        allVoxels = new Voxel[((2*TM.worldSize)-1)*TM.chunkSize , TM.worldHeight*TM.chunkSize , ((2*TM.worldSize)-1)*TM.chunkSize];
        noise2DArray = Noise.Calc2D(allVoxels.GetLength(0) + 1 , allVoxels.GetLength(2) + 1, TM.noiseFrequency * 0.0001f); //les +1 servent a eviter le mur chelou au bout du monde
        chunksCoord = new Dictionary<Vector3Int, Chunk>();
        chunks = new Dictionary<Chunk, Vector3Int>();

        GenerateWorld();
    }

    private void GenerateWorld()
    {
        //Create Chunks
        for (int x = 1-TM.worldSize; x < TM.worldSize; x++) {
            for (int y = 0; y < TM.worldHeight; y++) {
                for (int z = 1-TM.worldSize; z < TM.worldSize; z++) {
                    Vector3 chunkPosition = new Vector3(x * TM.chunkSize, y * TM.chunkSize, z * TM.chunkSize);
                    GameObject newChunkObject = new GameObject($"Chunk_{x}_{y}_{z}");
                    newChunkObject.transform.position = chunkPosition;
                    newChunkObject.transform.parent = sol.transform;

                    Chunk newChunk = newChunkObject.AddComponent<Chunk>();
                    chunksCoord.Add(new Vector3Int(x,y,z), newChunk);
                    chunks.Add(newChunk, new Vector3Int(x,y,z));
                    newChunk.Initialize();
                    newChunk.gameObject.tag = "ground";
                    newChunk.gameObject.layer = 8; } } }

        Debug.Log($"{chunks.Count} Chunks are initialized");

        Debug.Log($"AllVoxels[{allVoxels.GetLength(0)},{allVoxels.GetLength(1)},{allVoxels.GetLength(2)}]");

        //Populate allVoxels[]
        foreach (var c in chunks)
        {
            Chunk chunk = c.Key;
            Vector3Int chunkCoord = c.Value;

            Vector3Int startingCoord = chunkCoord * TM.chunkSize;
            int horizontalOffset = (TM.worldSize-1) * TM.chunkSize;

            int ax = startingCoord.x + horizontalOffset ;
            int ay = startingCoord.y ;
            int az = startingCoord.z + horizontalOffset ;

            for (int x = 0 ; x < TM.chunkSize ; x++) {
                for (int y = 0 ; y < TM.chunkSize ; y++) {
                    for (int z = 0 ; z < TM.chunkSize ; z++) {
                        allVoxels[ax+x,ay+y,az+z] = chunk.voxels[x,y,z]; }} }

            //Debug.Log($"AllVoxels[{ax},{ay},{az}] to AllVoxels[{ax+TM.chunkSize},{ay+TM.chunkSize},{az+TM.chunkSize}] populated"); 
        }

        ////Check allVoxels
        //int nbActiveVoxel = 0;
        //for (int x = 0 ; x < allVoxels.GetLength(0) ; x++) {
        //    for (int y = 0 ; y < allVoxels.GetLength(1) ; y++) {
        //        for (int z = 0 ; z < allVoxels.GetLength(2) ; z++) {

        //            try {
        //                nbActiveVoxel += allVoxels[x,y,z].isActive ? 1 : 0; }
        //            catch {
        //                Debug.Log($"AllVoxel[{x},{y},{z}] is fucked"); }
        //        }}}
        //          
        //Debug.Log($"AllVoxel has {nbActiveVoxel} active voxels");

        //Generate terrain
        foreach (var c in chunks)
        {
            c.Key.UpdateMesh();
        }


    }


    //Modify Navmesh
    public static void AddGroundInCube(Vector3 position, float size)
    {
        foreach(Voxel v in GetVoxelsInCube(position, size)) {
            v.isActive = true;
            v.type = Voxel.VoxelType.Ground; }

        foreach (Chunk c in GetChunksInCube(position, size + 4)) {
            c.UpdateMesh(); }
    }


    public static void RemoveGroundInCube(Vector3 position, float size)
    {
        foreach(Voxel v in GetVoxelsInCube(position, size)) {
            v.isActive = false;
            v.type = Voxel.VoxelType.Air; }

        foreach (Chunk c in GetChunksInCube(position, size + 4)) {
            c.UpdateMesh(); }
    }

    public static void AddGroundInSphere(Vector3 position, float size)
    {
        foreach(Voxel v in GetVoxelsInCube(position, size))
        {
            if (Vector3.Distance(v.position, position) > size/2) { continue; }
            v.isActive = true;
            v.type = Voxel.VoxelType.Ground;
        }

        foreach (Chunk c in GetChunksInCube(position, size + 4)) {
            c.UpdateMesh(); }
    }

    public static void RemoveGroundInSphere(Vector3 position, float size)
    {
        foreach(Voxel v in GetVoxelsInCube(position, size))
        {
            if (Vector3.Distance(v.position, position) > size/2) { continue; }
            v.isActive = false;
            v.type = Voxel.VoxelType.Air;
        }

        foreach (Chunk c in GetChunksInCube(position, size + 4)) {
            c.UpdateMesh(); }
    }

    public static List<Voxel> GetVoxelsInCube(Vector3 center, float size)
    {
       List<Voxel> voxels = new List<Voxel>();

       int vx1 = Mathf.FloorToInt((center.x - size/2));
       int vy1 = Mathf.FloorToInt((center.y - size/2));
       int vz1 = Mathf.FloorToInt((center.z - size/2));

       int vx2 = 1+Mathf.FloorToInt((center.x + size/2));
       int vy2 = 1+Mathf.FloorToInt((center.y + size/2));
       int vz2 = 1+Mathf.FloorToInt((center.z + size/2));
       
       for (int x = vx1; x < vx2; x++) {
           for (int y = vy1; y < vy2; y++) {
               for (int z = vz1; z < vz2; z++) {
                   if (x < (1-TM.worldSize)*TM.chunkSize || z < (1-TM.worldSize)*TM.chunkSize || y < 0) { continue; }
                   if (x >=  TM.worldSize  *TM.chunkSize || z >=  TM.worldSize  *TM.chunkSize || y > TM.worldHeight * TM.chunkSize) { continue; }
                   voxels.Add(GetVoxel(new Vector3(x,y,z)));
               } } }

       return voxels;
    }

    public static List<Chunk> GetChunksInCube(Vector3 center, float size)
    {
       List<Chunk> chunks = new List<Chunk>();

       int cx1 = Mathf.FloorToInt((center.x - size/2)/TM.chunkSize);
       int cy1 = Mathf.FloorToInt((center.y - size/2)/TM.chunkSize);
       int cz1 = Mathf.FloorToInt((center.z - size/2)/TM.chunkSize);

       int cx2 = 1+Mathf.FloorToInt((center.x + size/2)/TM.chunkSize);
       int cy2 = 1+Mathf.FloorToInt((center.y + size/2)/TM.chunkSize);
       int cz2 = 1+Mathf.FloorToInt((center.z + size/2)/TM.chunkSize);


       for (int x = cx1; x < cx2; x++) {
           for (int y = cy1; y < cy2; y++) {
               for (int z = cz1; z < cz2; z++) {
                   if (x <= -TM.worldSize || y < 0 || z <= -TM.worldSize) { continue; }
                   if (x >= TM.worldSize || y > TM.worldHeight || z >= TM.worldSize) { continue; }
                   chunks.Add(chunksCoord[new Vector3Int(x,y,z)]); 
               } } }

       return chunks;

    }




    //Chunks
    public static Chunk GetChunk(Voxel voxel) { return GetChunk(GetChunkCoordinates(voxel.position)); }
    public static Chunk GetChunk(Vector3 globalPosition) { return GetChunk(GetChunkCoordinates(globalPosition)); }
    public static Chunk GetChunk(Vector3Int coord)       { return chunksCoord[coord]; }
    public static Vector3Int GetChunkCoordinates(Chunk chunk) { return chunks[chunk]; }
    public static Vector3Int GetChunkCoordinates(Vector3 globalPosition) {
        return new Vector3Int(
                Mathf.FloorToInt(globalPosition.x/TM.chunkSize),
                Mathf.FloorToInt(globalPosition.y/TM.chunkSize),
                Mathf.FloorToInt(globalPosition.z/TM.chunkSize)); }


    //Voxels
    public static Vector3Int GetVoxelCoordinates(Voxel voxel) { return GetVoxelCoordinates(voxel.position); }
    public static Vector3Int GetVoxelCoordinates(Vector3Int localPosition, Chunk chunk) { return GetChunkCoordinates(chunk)*TM.chunkSize + localPosition; }
    public static Vector3Int GetVoxelCoordinates(Vector3 globalPosition) {
        return new Vector3Int(
                Mathf.RoundToInt(globalPosition.x),
                Mathf.RoundToInt(globalPosition.y),
                Mathf.RoundToInt(globalPosition.z)); }


    public static Voxel GetVoxel(Vector3Int localPosition, Voxel voxel)
    {
        Vector3Int coord = voxel.allVoxelsCoord + localPosition;
        if (coord.x<0 || coord.y<0 || coord.z<0 ||
                coord.x >= allVoxels.GetLength(0) || coord.y >= allVoxels.GetLength(1) || coord.z >= allVoxels.GetLength(2))
        {
            return null;
        }

        return allVoxels[coord.x,coord.y,coord.z];
    }

    public static Voxel GetVoxel(Vector3Int localPosition, Chunk chunk) { return GetVoxel(GetVoxelCoordinates(localPosition, chunk)); }
    public static Voxel GetVoxel(Vector3 globalPosition) { return GetVoxel(GetVoxelCoordinates(globalPosition)); }
    public static Voxel GetVoxel(Vector3Int coord)
    {
        coord = GetPositiveCoord(coord);
        int x = coord.x;
        int y = coord.y;
        int z = coord.z;

        if (x<0 || y<0 || z<0 ||
            x >= allVoxels.GetLength(0) || y >= allVoxels.GetLength(1) || z >= allVoxels.GetLength(2))
        {
            return null;
        }

        return allVoxels[x,y,z];
    }

    public static bool isVoxelActive(Vector3Int localPosition, Voxel voxel)
    {
        Vector3Int coord = voxel.allVoxelsCoord + localPosition;

        if (coord.x<0 || coord.y<0 || coord.z<0 ||
                coord.x >= allVoxels.GetLength(0) || coord.y >= allVoxels.GetLength(1) || coord.z >= allVoxels.GetLength(2))
        {
            //Debug.Log($"Voxel {coord} is out of bounds"); 
            return false;
        }

        //Debug.Log($"Voxel {coord} is in bounds, isActive : {allVoxels[coord.x,coord.y,coord.z].isActive}"); 
        return allVoxels[coord.x,coord.y,coord.z].isActive;

    }

    public static Vector3Int GetPositiveCoord(Vector3Int coord)
    {
        int x = coord.x + TM.chunkSize * (TM.worldSize-1);
        int y = coord.y;
        int z = coord.z + TM.chunkSize * (TM.worldSize-1);
        return new Vector3Int(x,y,z);
    }

    public static float GetGlobalNoiseValue(float globalX, float globalZ)
    {
        globalX += TM.chunkSize * (TM.worldSize-1);
        globalZ += TM.chunkSize * (TM.worldSize-1);

        int x = Mathf.RoundToInt(globalX);
        int z = Mathf.RoundToInt(globalZ);

        try
        {
            return noise2DArray[x,z];
        }
        catch
        {
            Debug.Log($"Noise at ({globalX},{globalZ}) cannot be found");
            return 0;
        }
    }
}


