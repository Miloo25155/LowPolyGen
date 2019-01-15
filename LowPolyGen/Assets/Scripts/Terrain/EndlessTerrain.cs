using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float maxViewDist = 300;
    public Transform viewer;

    public static Vector2 viewerPosition;
    static TerrainGenerator terrainGenerator;
    int chunkSize;
    int chunksVisibileInViewDist;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    void Start()
    {
        terrainGenerator = FindObjectOfType<TerrainGenerator>();
        terrainGenerator.colorGenerator = new ColorGenerator();
        terrainGenerator.colorGenerator.UpdateColorSettings(terrainGenerator.colorSettings);

        chunkSize = TerrainGenerator.chunkSize;
        chunksVisibileInViewDist = Mathf.RoundToInt(maxViewDist / chunkSize);
    }

    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks()
    {
        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunksVisibileInViewDist; yOffset <= chunksVisibileInViewDist; yOffset++)
        {
            for (int xOffset = -chunksVisibileInViewDist; xOffset <= chunksVisibileInViewDist; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    if (terrainChunkDictionary[viewedChunkCoord].IsVisible())
                    {
                        terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
                    }
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform, terrainGenerator.colorSettings.material));
                }
            }
        }
    }

    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        MeshCollider meshCollider;

        public TerrainChunk(Vector2 coord, int size, Transform parent, Material terrainMaterial)
        {
            position = coord * size;
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            bounds = new Bounds(position, Vector2.one * size);

            meshObject = new GameObject("Terrain chunk");

            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshRenderer.material = terrainMaterial;

            meshFilter = meshObject.AddComponent<MeshFilter>();

            meshCollider = meshObject.AddComponent<MeshCollider>();

            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;

            SetVisible(false);

            terrainGenerator.RequestMapData(OnMapDataReceived);
        }
        void OnMeshDataReceived(MeshData meshData)
        {
            Mesh mesh = meshData.CreateMesh();
            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
            terrainGenerator.GenerateColors();
        }

        void OnMapDataReceived(MapData mapData)
        {
            terrainGenerator.RequestMeshData(mapData, OnMeshDataReceived);
        }

        public void UpdateTerrainChunk()
        {
            float viewDistFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewDistFromNearestEdge <= maxViewDist;
            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }
}
