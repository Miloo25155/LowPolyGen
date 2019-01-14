using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ShapeSettings : ScriptableObject
{
    public int xSize = 100;
    public int zSize = 100;

    public int baseHeight = 0;

    [Range(1, 10)]
    public int resolution = 1;

    public bool flatMesh = false;
    public bool flatShading = false;
    public float heightMultiplicator = 10;

    public NoiseSettings noiseSettings;

    //public NoiseLayer[] noiseLayers;
    //[System.Serializable]
    //public class NoiseLayer
    //{
    //    public bool enabled = true;
    //    public bool useFirstLayerAsMask;
    //    public NoiseSettings noiseSettings;
    //}
}
