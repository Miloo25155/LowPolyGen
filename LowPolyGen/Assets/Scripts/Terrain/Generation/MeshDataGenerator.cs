using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshDataGenerator
{
    public static MeshData GenerateFlatMeshData(int chunkSize, int resolution)
    {
        //implement resolution here : equivalent to grid cell size
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
}
