using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshDataGenerator
{
    public static int baseLevelOfDetail = 3;

    public static MeshData GenerateFlatMeshData(int chunkSize, int levelOfDetail)
    {
        float topLeftX = (chunkSize - 1) / -2f;
        float topLeftZ = (chunkSize - 1) / 2f;

        int meshSimplificationIncrement = levelOfDetail + baseLevelOfDetail;

        int verticesPerLine = (chunkSize - 1) / meshSimplificationIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);

        int vertexIndex = 0;
        for (int z = 0; z < chunkSize; z += meshSimplificationIncrement)
        {
            for (int x = 0; x < chunkSize; x += meshSimplificationIncrement)
            {
                Vector3 point = new Vector3(topLeftX + x, 0, topLeftZ - z);

                meshData.vertices[vertexIndex] = point;
                meshData.uvs[vertexIndex] = new Vector2(x / (float)chunkSize, z / (float)chunkSize);

                if (x < chunkSize - 1 && z < chunkSize - 1)
                {
                    meshData.AddTriangles(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangles(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }

    public static MeshData GenerateMeshDataFromHeightMap(int chunkSize, int levelOfDetail, float[,] noiseMap, ShapeSettings settings)
    {
        float topLeftX = (chunkSize - 1) / -2f;
        float topLeftZ = (chunkSize - 1) / 2f;

        int meshSimplificationIncrement =  levelOfDetail + baseLevelOfDetail;

        int verticesPerLine = (chunkSize - 1) / meshSimplificationIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);

        float heightMultiplicator = settings.heightMultiplicator;
        AnimationCurve heightCurve = new AnimationCurve(settings.heightCurve.keys);

        int vertexIndex = 0;
        for (int z = 0; z < chunkSize; z += meshSimplificationIncrement)
        {
            for (int x = 0; x < chunkSize; x += meshSimplificationIncrement)
            {
                float noiseValue = heightCurve.Evaluate(noiseMap[x, z]) * heightMultiplicator;
                Vector3 point = new Vector3(topLeftX + x, noiseValue, topLeftZ - z);

                meshData.elevationMinMax.AddValue(noiseValue);

                meshData.vertices[vertexIndex] = point;
                meshData.uvs[vertexIndex] = new Vector2(x / (float)chunkSize, z / (float)chunkSize);

                if (x < chunkSize - 1 && z < chunkSize - 1)
                {
                    meshData.AddTriangles(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangles(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
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
