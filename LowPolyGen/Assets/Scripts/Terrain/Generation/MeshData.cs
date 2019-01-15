using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
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
