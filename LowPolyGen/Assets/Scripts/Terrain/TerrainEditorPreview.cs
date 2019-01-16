using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainEditorPreview: MonoBehaviour
{
    public int seed;
    public Vector2 offset;
    [Range(0, 6)]
    public int editorLevelOfDetail;
    float[,] noiseMapEditor;

    MeshFilter meshFilterEditor;
    MeshRenderer meshRendererEditor;
    MeshData meshDataEditor;
    Mesh meshEditor;
    MeshCollider meshColliderEditor;

    static TerrainGenerator terrainGenerator;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colorSettingsFoldout;

    public int trianglesLenght;

    public void GenerateTerrain()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    private void Initialize()
    {
        terrainGenerator = FindObjectOfType<TerrainGenerator>();

        shapeSettings = terrainGenerator.shapeSettings;
        colorSettings = terrainGenerator.colorSettings;

        terrainGenerator.colorGenerator = new ColorGenerator();
        terrainGenerator.colorGenerator.UpdateColorSettings(colorSettings);

        #region Init mesh components
        meshFilterEditor = GetComponent<MeshFilter>();
        if (meshFilterEditor == null)
        {
            meshFilterEditor = gameObject.AddComponent<MeshFilter>();
        }

        meshRendererEditor = GetComponent<MeshRenderer>();
        if (meshRendererEditor == null)
        {
            meshRendererEditor = gameObject.AddComponent<MeshRenderer>();
        }

        meshColliderEditor = GetComponent<MeshCollider>();
        if (meshColliderEditor == null)
        {
            meshColliderEditor = gameObject.AddComponent<MeshCollider>();
        }
        #endregion

        noiseMapEditor = Noise.GenerateNoiseMap(TerrainGenerator.chunkSize, shapeSettings, seed, offset);
    }

    private void GenerateMesh()
    {
        noiseMapEditor = Noise.GenerateNoiseMap(TerrainGenerator.chunkSize, shapeSettings, seed, offset);
        MapData mapData = new MapData(noiseMapEditor);

        meshDataEditor = terrainGenerator.GetMeshData(mapData, editorLevelOfDetail);
        terrainGenerator.colorGenerator.UpdateElevation(meshDataEditor.elevationMinMax);

        meshEditor = meshDataEditor.CreateMesh();

        meshFilterEditor.sharedMesh = meshEditor;
        meshRendererEditor.sharedMaterial = colorSettings.material;
        meshColliderEditor.sharedMesh = meshEditor;

        trianglesLenght = meshEditor.triangles.Length;
    }

    public void GenerateColors()
    {
        terrainGenerator.colorGenerator.UpdateColors();
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
        meshEditor = new Mesh();
        DestroyImmediate(GetComponent<MeshRenderer>());
        DestroyImmediate(GetComponent<MeshFilter>());
        DestroyImmediate(GetComponent<MeshCollider>());
    }

}
