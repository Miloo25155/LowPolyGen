using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public ShapeSettings shapeSettings;
    public WaterSettings waterSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool waterSettingsFoldout;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshData meshData;
    Mesh mesh;
    MeshCollider meshCollider;

    static TerrainEditorPreview terrainEditor;

    public int trianglesLenght;

    void Start()
    {
        GenerateWater();
    }

    private void Initialize()
    {
        terrainEditor = FindObjectOfType<TerrainEditorPreview>();

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
    }

    public void GenerateWater()
    {
        Initialize();
        GenerateMesh();
    }

    public void GenerateMesh()
    {
        meshData = MeshDataGenerator.GenerateFlatMeshData(TerrainGenerator.chunkSize, terrainEditor.editorLevelOfDetail);

        mesh = meshData.CreateMesh();

        if (shapeSettings.flatShading)
        {
            meshData = MeshDataGenerator.ConstructFlatShadedMeshData(meshData);
        }

        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = waterSettings.material;
        meshCollider.sharedMesh = mesh;

        trianglesLenght = mesh.triangles.Length;
    }

    public void GenerateSurface()
    {
    }

    public void OnShapeSettingsUpdated()
    {
        Initialize();
        GenerateMesh();
    }

    public void OnWaterSettingsUpdated()
    {
        Initialize();
        GenerateSurface();
    }

    public void ClearWater()
    {
        mesh = new Mesh();
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
}
