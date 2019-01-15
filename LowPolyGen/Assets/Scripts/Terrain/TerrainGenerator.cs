using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    ColorGenerator colorGenerator = new ColorGenerator();

    public int trianglesLenght;

    MinMax elevationMinMax;

    void Start()
    {
        GenerateTerrain();
    }

    private void Initialize()
    {
        #region Init mesh components
        meshFilter = GetComponent<MeshFilter>();
        if(meshFilter == null)
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

        colorGenerator.UpdateColorSettings(colorSettings);

        noiseMap = Noise.GenerateNoiseMap(chunkSize, shapeSettings, seed, offset);

        elevationMinMax = new MinMax();
    }

    public void GenerateTerrain()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    public void GenerateMesh()
    {
        meshData = MeshDataGenerator.GenerateFlatMeshData(chunkSize, shapeSettings.resolution);
        mesh = MeshGenerator.GenerateTerrain(meshData, noiseMap, shapeSettings, ref elevationMinMax);

        if (shapeSettings.flatShading)
        {
            mesh = MeshGenerator.ConstructFlatShadedMesh(mesh);
        }

        colorGenerator.UpdateElevation(elevationMinMax);

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
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
}
