using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static Mesh GenerateTerrain(MeshData meshData, float[,] noiseMap, ShapeSettings settings, ref MinMax elevationMinMax)
    {
        int chunkSize = noiseMap.GetLength(0) / settings.resolution;

        int nbOfVertexPerLine = (int)Mathf.Sqrt(meshData.vertices.Length);

        Vector3[] vertices = meshData.vertices;

        float heightMultiplicator = settings.heightMultiplicator;
        AnimationCurve heightCurve = settings.heightCurve;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 currentVertex = vertices[i];

            int x = i % chunkSize;
            int y = i / chunkSize;

            float noiseValue = heightCurve.Evaluate(noiseMap[x, y]);
            currentVertex.y += noiseValue * heightMultiplicator;

            elevationMinMax.AddValue(currentVertex.y);

            vertices[i] = currentVertex;
        }
        meshData.vertices = vertices;

        return meshData.CreateMesh();
    }

    public static Mesh ConstructFlatShadedMesh(Mesh mesh)
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

        return mesh;
    }
}
