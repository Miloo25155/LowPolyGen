using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    [Range(15, 100)]
    public float scale = 1;
    [Range(1, 10)]
    public int octaves = 1;
    [Range(0, 10)]
    public float lacunarity = 2f;
    [Range(0, 1)]
    public float persistence = 0.5f;
}
