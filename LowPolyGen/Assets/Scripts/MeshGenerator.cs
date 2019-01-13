using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{
    int xSize;
    int zSize;

    public MeshGenerator(int xSize, int zSize)
    {
        this.xSize = xSize;
        this.zSize = zSize;
    }

    public Mesh GenerateMesh(bool useNoise, float[,] heighMap)
    {
        float topLeftX = (xSize - 1) / -2f;
        float topLeftZ = (zSize - 1) / 2f;

        MeshData meshData = new MeshData(xSize, zSize);

        int vertexIndex = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                Vector3 point = new Vector3(topLeftX + x, 0, topLeftZ - z);
                float noiseValue = useNoise ? heighMap[x,z] : 0;
                point.y = noiseValue;

                meshData.vertices[vertexIndex] = point;
                meshData.uvs[vertexIndex] = new Vector2(x / (float)xSize, z / (float)zSize);

                if(x < xSize - 1 && z < zSize - 1)
                {
                    meshData.AddTriangles(vertexIndex, vertexIndex + xSize + 1, vertexIndex + xSize);
                    meshData.AddTriangles(vertexIndex + xSize + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData.CreateMesh();
    }

    class MeshData{
        public Vector3[] vertices;
        public Vector2[] uvs;
        public int[] triangles;

        int triangleIndex;

        public MeshData(int xSize, int zSize)
        {
            vertices = new Vector3[xSize * zSize];
            uvs = new Vector2[xSize * zSize];
            triangles = new int[(xSize - 1) * (zSize - 1) * 6];

            triangleIndex = 0;
        }

        public void AddTriangles(int x, int y, int z)
        {
            triangles[triangleIndex] = x;
            triangles[triangleIndex + 1] = y;
            triangles[triangleIndex + 2] = z;

            triangleIndex += 3;
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}
