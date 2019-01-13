using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Terrain))]
public class PlanetEditor : Editor
{
    Terrain terrain;
    Editor shapeEditor;
    Editor colorEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed)
            {
                terrain.GenerateTerrain();
            }
        }

        if (GUILayout.Button("Generate Terrain"))
        {
            terrain.GenerateTerrain();
        }

        if (GUILayout.Button("Clear Terrain"))
        {
            terrain.ClearTerrain();
        }

        DrawSettingsEditor(terrain.shapeSettings, terrain.OnShapeSettingsUpdated, ref terrain.shapeSettingsFoldout, ref shapeEditor);
        DrawSettingsEditor(terrain.colorSettings, terrain.OnColorSettingsUpdated, ref terrain.colorSettingsFoldout, ref colorEditor);
    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (foldout)
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed)
                    {
                        if (onSettingsUpdated != null)
                        {
                            onSettingsUpdated();
                        }
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        terrain = (Terrain)target;
    }
}