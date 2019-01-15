using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator
{
    ColorSettings settings;

    Texture2D texture;
    const int textureResolution = 50;

    public void UpdateColorSettings(ColorSettings settings)
    {
        this.settings = settings;

        if(texture == null)
        {
            texture = new Texture2D(textureResolution, 1);
        }
    }

    public void UpdateElevation(MinMax elevationMinMax)
    {
        settings.material.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
    }

    public void UpdateColors()
    {
        Color[] colors = new Color[textureResolution];

        for (int i = 0; i < textureResolution; i++)
        {
            Color tintColor = settings.tint;

            colors[i] = settings.gradient.Evaluate(i / (textureResolution - 1f));// * (1 - biome.tintPercent) + tintColor * biome.tintPercent;
        }

        texture.SetPixels(colors);
        texture.Apply();
        settings.material.SetTexture("_texture", texture);
    }
}
