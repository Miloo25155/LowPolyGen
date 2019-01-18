using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    [Range(15, 450)]
    public float scale = 1;
    [Range(1, 8)]
    public int octaves = 1;
    [Range(1, 5)]
    public float lacunarity = 2f;
    [Range(0, 1)]
    public float persistence = 0.5f;

    [Range(0.2f, 2)]
    public float globalAttenuationFactor = 1.5f;
}
