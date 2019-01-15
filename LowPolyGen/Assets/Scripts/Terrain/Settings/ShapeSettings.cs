using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ShapeSettings : ScriptableObject
{
    [Range(3, 10)]
    public int resolution = 3;

    public bool flatMesh = false;
    public bool flatShading = false;
    public float heightMultiplicator = 10;
    public AnimationCurve heightCurve;

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
