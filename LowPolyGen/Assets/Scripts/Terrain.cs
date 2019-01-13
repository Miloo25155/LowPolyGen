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

    MeshGenerator meshGenerator;

    public int trianglesLenght;

    float[,] heightMap;

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
    }

    public void GenerateTerrain()
    {
        meshGenerator = new MeshGenerator(shapeSettings.xSize, shapeSettings.zSize);

        heightMap = Noise.GenerateNoiseMap(shapeSettings.xSize, shapeSettings.zSize,
                                            shapeSettings.noiseSettings.scale, shapeSettings.noiseSettings.octaves,
                                            shapeSettings.noiseSettings.persistence, shapeSettings.noiseSettings.lacunarity,
                                            shapeSettings.heightMultiplicator);

        mesh = meshGenerator.GenerateMesh(true, heightMap);

        if (shapeSettings.flatShading)
        {
            ConstructFlatShadedMesh();
        }

        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = colorSettings.material;
        meshCollider.sharedMesh = mesh;

        trianglesLenght = mesh.triangles.Length;
    }

    public void OnShapeSettingsUpdated()
    {
        Initialize();
        GenerateTerrain();
    }

    public void OnColorSettingsUpdated()
    {

    }

    public void ConstructFlatShadedMesh()
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

    //public void UpdateUVs(ColorGenerator colorGenerator)
    //{
    //    for (int y = 0; y < resolution; y++)
    //    {
    //        for (int x = 0; x < resolution; x++)
    //        {
    //            int i = x + y * resolution;
    //            Vector2 percent = new Vector2(x, y) / (resolution - 1);
    //            Vector3 pointOnUnitCube = localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB;
    //            Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

    //            uvs[i] = new Vector2(colorGenerator.BiomePercentFromPoint(pointOnUnitSphere), 0);
    //        }
    //    }
    //}

    public void ClearTerrain()
    {
        mesh = new Mesh();
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
}
