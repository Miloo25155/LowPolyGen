using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{
    int xSize;
    int zSize;
    int resolution;
    int baseHeight;

    float heightMultiplicator;

    float[,] heightMap;

    public MinMax elevationMinMax;

    public void UpdateMeshSettings(ShapeSettings settings)
    {
        this.xSize = settings.xSize;
        this.zSize = settings.zSize;
        this.resolution = settings.resolution;
        this.heightMultiplicator = settings.heightMultiplicator;
        this.baseHeight = settings.baseHeight;

        elevationMinMax = new MinMax();
        heightMap = Noise.GenerateNoiseMap(settings);
    }

    public Mesh GenerateMesh(bool flatMesh)
    {
        //implement resolution here : equivalent to grid cell size
        int xResSize = xSize / resolution;
        int zResSize = zSize / resolution;

        float topLeftX = (xSize - 1) / -2f;
        float topLeftZ = (zSize - 1) / 2f;

        MeshData meshData = new MeshData(xResSize, zResSize);

        int vertexIndex = 0;
        for (int z = 0; z < xResSize; z++)
        {
            for (int x = 0; x < zResSize; x++)
            {
                int xRes = x * resolution;
                int zRes = z * resolution;

                Vector3 point = new Vector3(topLeftX + xRes, 0, topLeftZ - zRes);
                float noiseValue = flatMesh ? 0 : heightMap[xRes, zRes] * heightMultiplicator;
                point.y = noiseValue + baseHeight;

                elevationMinMax.AddValue(point.y);

                meshData.vertices[vertexIndex] = point;
                meshData.uvs[vertexIndex] = new Vector2(xRes / (float)xSize, zRes / (float)zSize);

                if(x < xResSize - 1 && z < zResSize - 1)
                {
                    meshData.AddTriangles(vertexIndex, vertexIndex + xResSize + 1, vertexIndex + xResSize);
                    meshData.AddTriangles(vertexIndex + xResSize + 1, vertexIndex, vertexIndex + 1);
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
