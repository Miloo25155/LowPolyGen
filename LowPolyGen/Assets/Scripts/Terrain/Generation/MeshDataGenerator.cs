using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshDataGenerator
{
    public static MeshData GenerateFlatMeshData(int chunkSize, int resolution)
    {
        int chunkSizeResolution = chunkSize / resolution;

        float topLeftX = (chunkSize - 1) / -2f;
        float topLeftZ = (chunkSize - 1) / 2f;

        MeshData meshData = new MeshData(chunkSizeResolution, chunkSizeResolution);

        int vertexIndex = 0;
        for (int z = 0; z < chunkSizeResolution; z++)
        {
            for (int x = 0; x < chunkSizeResolution; x++)
            {
                int xRes = x * resolution;
                int zRes = z * resolution;

                Vector3 point = new Vector3(topLeftX + xRes, 0, topLeftZ - zRes);

                meshData.vertices[vertexIndex] = point;
                meshData.uvs[vertexIndex] = new Vector2(xRes / (float)chunkSize, zRes / (float)chunkSize);

                if (x < chunkSizeResolution - 1 && z < chunkSizeResolution - 1)
                {
                    meshData.AddTriangles(vertexIndex, vertexIndex + chunkSizeResolution + 1, vertexIndex + chunkSizeResolution);
                    meshData.AddTriangles(vertexIndex + chunkSizeResolution + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }

    public static MeshData GenerateMeshDataFromHeightMap(int chunkSize, float[,] noiseMap, ShapeSettings settings)
    {
        int resolution = settings.resolution;
        int chunkSizeResolution = chunkSize / resolution;

        float topLeftX = (chunkSize - 1) / -2f;
        float topLeftZ = (chunkSize - 1) / 2f;

        MeshData meshData = new MeshData(chunkSizeResolution, chunkSizeResolution);

        float heightMultiplicator = settings.heightMultiplicator;
        AnimationCurve heightCurve = new AnimationCurve(settings.heightCurve.keys);

        int vertexIndex = 0;
        for (int z = 0; z < chunkSizeResolution; z++)
        {
            for (int x = 0; x < chunkSizeResolution; x++)
            {
                int xRes = x * resolution;
                int zRes = z * resolution;

                float noiseValue = heightCurve.Evaluate(noiseMap[x, z]) * heightMultiplicator;
                Vector3 point = new Vector3(topLeftX + xRes, noiseValue, topLeftZ - zRes);

                meshData.elevationMinMax.AddValue(noiseValue);

                meshData.vertices[vertexIndex] = point;
                meshData.uvs[vertexIndex] = new Vector2(xRes / (float)chunkSize, zRes / (float)chunkSize);

                if (x < chunkSizeResolution - 1 && z < chunkSizeResolution - 1)
                {
                    meshData.AddTriangles(vertexIndex, vertexIndex + chunkSizeResolution + 1, vertexIndex + chunkSizeResolution);
                    meshData.AddTriangles(vertexIndex + chunkSizeResolution + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }

    public static MeshData ConstructFlatShadedMeshData(MeshData meshData)
    {
        Vector3[] vertices = meshData.vertices;
        int[] triangles = meshData.triangles;
        Vector2[] uvs = meshData.uvs;

        Vector3[] flatVertices = new Vector3[triangles.Length];
        Vector2[] flatUvs = new Vector2[triangles.Length];

        for (int i = 0; i < triangles.Length; i++)
        {
            flatVertices[i] = vertices[triangles[i]];
            flatUvs[i] = uvs[triangles[i]];
            triangles[i] = i;
        }

        meshData.vertices = flatVertices;
        meshData.uvs = flatUvs;
        meshData.triangles = triangles;

        return meshData;
    }
}
