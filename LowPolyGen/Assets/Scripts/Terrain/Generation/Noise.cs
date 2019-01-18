using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public enum NormalizeMode { Local, Global };

    public static float[,] GenerateNoiseMap(int chunkSize, ShapeSettings settings, int seed, Vector2 offset, NormalizeMode normalizeMode)
    {
        float scale = settings.noiseSettings.scale;
        int octaves = settings.noiseSettings.octaves;
        float persistence = settings.noiseSettings.persistence;
        float lacunarity = settings.noiseSettings.lacunarity;

        float[,] noiseMap = new float[chunkSize, chunkSize];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float amplitude = 1;
        float frequency = 1;
        float maxPossibleHeight = 0;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistence;
        }

        if(scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfChunkSize = chunkSize / 2f;

        for (int z = 0; z < chunkSize; z++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfChunkSize + octaveOffsets[i].x) / scale * frequency;
                    float sampleZ = (z - halfChunkSize + octaveOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }
                if(noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                } else if(noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x, z] = noiseHeight;
            }
        }

        float globalAttenuationFactor = settings.noiseSettings.globalAttenuationFactor;

        for (int z = 0; z < chunkSize; z++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                if(normalizeMode == NormalizeMode.Local)
                {
                    noiseMap[x, z] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, z]);
                }
                else
                {
                    float normalizeHeight = ((noiseMap[x, z] + 1) / 2f) / maxPossibleHeight / globalAttenuationFactor;
                    noiseMap[x, z] = normalizeHeight;
                }

            }
        }

        return noiseMap;
    }
}
