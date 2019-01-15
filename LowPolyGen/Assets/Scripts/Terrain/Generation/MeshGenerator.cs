using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    //public static Mesh GenerateTerrain(MeshData meshData, float[,] noiseMap, ShapeSettings settings)
    //{
    //    int chunkSize = noiseMap.GetLength(0) / settings.resolution;

    //    int nbOfVertexPerLine = (int)Mathf.Sqrt(meshData.vertices.Length);

    //    Vector3[] vertices = meshData.vertices;

    //    float heightMultiplicator = settings.heightMultiplicator;
    //    AnimationCurve heightCurve = settings.heightCurve;

    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        Vector3 currentVertex = vertices[i];

    //        int x = i % chunkSize;
    //        int y = i / chunkSize;

    //        float noiseValue = heightCurve.Evaluate(noiseMap[x, y]);
    //        currentVertex.y += noiseValue * heightMultiplicator;

    //        meshData.elevationMinMax.AddValue(currentVertex.y);

    //        vertices[i] = currentVertex;
    //    }
    //    meshData.vertices = vertices;

    //    return meshData.CreateMesh();
    //}
}
