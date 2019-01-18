using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ShapeSettings : ScriptableObject
{
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
