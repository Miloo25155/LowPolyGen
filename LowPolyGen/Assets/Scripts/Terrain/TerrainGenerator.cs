using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class TerrainGenerator : MonoBehaviour
{
    public const int chunkSize = 241;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;


    [HideInInspector]
    public ColorGenerator colorGenerator = new ColorGenerator();


    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();


    public MeshData GetMeshData(MapData mapData, int lod)
    {
        MeshData meshData = MeshDataGenerator.GenerateMeshDataFromHeightMap(chunkSize, lod, mapData.heightMap, shapeSettings);

        if (shapeSettings.flatShading)
        {
            meshData = MeshDataGenerator.ConstructFlatShadedMeshData(meshData);
        }

        return meshData;
    }
    public MapData GetMapData(Vector2 center, int seed, Vector2 offset)
    {
        float[,] heightMap = Noise.GenerateNoiseMap(chunkSize, shapeSettings, seed, center + offset);
        return new MapData(heightMap);
    }


    public void RequestMapData(Vector2 center, int seed, Vector2 offset, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(center, seed, offset, callback);
        };

        new Thread(threadStart).Start();
    }
    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, lod, callback);
        };

        new Thread(threadStart).Start();
    }


    void MapDataThread(Vector2 center, int seed, Vector2 offset, Action<MapData> callback)
    {
        MapData mapData = GetMapData(center, seed, offset);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }
    void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData = GetMeshData(mapData, lod);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    private void Start()
    {
        colorGenerator.UpdateColorSettings(colorSettings);
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

    public void GenerateColors()
    {
        colorGenerator.UpdateColors();
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
