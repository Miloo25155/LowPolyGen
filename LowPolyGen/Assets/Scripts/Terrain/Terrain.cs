using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colorSettingsFoldout;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    Mesh mesh;
    MeshCollider meshCollider;

    MeshGenerator meshGenerator = new MeshGenerator();
    ColorGenerator colorGenerator = new ColorGenerator();

    public int trianglesLenght;

    public MinMax elevationMinMax;

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

        meshGenerator.UpdateMeshSettings(shapeSettings);
        colorGenerator.UpdateColorSettings(colorSettings);
    }

    public void GenerateTerrain()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    public void GenerateMesh()
    {
        mesh = meshGenerator.GenerateMesh(shapeSettings.flatMesh);

        if (shapeSettings.flatShading)
        {
            ConstructFlatShadedMesh();
        }

        colorGenerator.UpdateElevation(meshGenerator.elevationMinMax);

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

    private void ConstructFlatShadedMesh()
    {
        int triLength = mesh.triangles.Length;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector2[] uvs = mesh.uv;

        Vector3[] flatVertices = new Vector3[triLength];
        Vector2[] flatUvs = new Vector2[triLength];

        for (int i = 0; i < triLength; i++)
        {
            flatVertices[i] = vertices[triangles[i]];
            flatUvs[i] = uvs[triangles[i]];
            triangles[i] = i;
        }

        mesh.vertices = flatVertices;
        mesh.triangles = triangles;
        mesh.uv = flatUvs;
        mesh.RecalculateNormals();
    }

    public void ClearTerrain()
    {
        mesh = new Mesh();
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
}
