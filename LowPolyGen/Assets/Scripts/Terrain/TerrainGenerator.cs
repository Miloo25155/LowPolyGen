using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class TerrainGenerator : MonoBehaviour
{
    public const int chunkSize = 240;

    public int seed;
    public Vector2 offset;
    float[,] noiseMap;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colorSettingsFoldout;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshData meshData;
    Mesh mesh;
    MeshCollider meshCollider;

    [HideInInspector]
    public ColorGenerator colorGenerator;

    public int trianglesLenght;


    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    void Start()
    {
        //GenerateTerrain();
    }

    public void RequestMapData(Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(callback);
        };

        new Thread(threadStart).Start();
    }

    void MapDataThread(Action<MapData> callback)
    {
        MapData mapData = GetMapData();
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }


    public void RequestMeshData(MapData mapData, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, callback);
        };

        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, Action<MeshData> callback)
    {
        MeshData meshData = GetMeshData(mapData);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }


    private void Update()
    {
        if(mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    private MeshData GetMeshData(MapData mapData)
    {
        MeshData meshData = MeshDataGenerator.GenerateMeshDataFromHeightMap(chunkSize, mapData.heightMap, shapeSettings);

        if (shapeSettings.flatShading)
        {
            meshData = MeshDataGenerator.ConstructFlatShadedMeshData(meshData);
        }

        return meshData;
    }

    private MapData GetMapData()
    {
        float[,] heightMap = noiseMap = Noise.GenerateNoiseMap(chunkSize, shapeSettings, seed, offset);
        return new MapData(heightMap);
    }

    public void GenerateTerrain()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    private void Initialize()
    {
        #region Init mesh components
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        #endregion

        colorGenerator = new ColorGenerator();
        colorGenerator.UpdateColorSettings(colorSettings);

        noiseMap = Noise.GenerateNoiseMap(chunkSize, shapeSettings, seed, offset);
    }

    private void GenerateMesh()
    {
        noiseMap = Noise.GenerateNoiseMap(chunkSize, shapeSettings, seed, offset);
        MapData mapData = new MapData(noiseMap);

        meshData = GetMeshData(mapData);
        colorGenerator.UpdateElevation(meshData.elevationMinMax);

        mesh = meshData.CreateMesh();

        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = colorSettings.material;
        meshCollider.sharedMesh = mesh;

        trianglesLenght = mesh.triangles.Length;
    }

    public void GenerateColors()
    {
        colorGenerator.UpdateColors();
    }

    public void OnShapeSettingsUpdated()
    {
        Initialize();
        GenerateMesh();
    }

    public void OnColorSettingsUpdated()
    {
        Initialize();
        GenerateColors();
    }

    public void ClearTerrain()
    {
        mesh = new Mesh();
        Destroy(GetComponent<MeshRenderer>());
        Destroy(GetComponent<MeshFilter>());
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback ?? throw new ArgumentNullException(nameof(callback));
            this.parameter = parameter;
        }
    }
}
