using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Water))]
public class WaterEditor : Editor
{
    Water water;
    Editor shapeEditor;
    Editor waterEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed)
            {
                water.GenerateWater();
            }
        }

        if (GUILayout.Button("Generate water"))
        {
            water.GenerateWater();
        }

        if (GUILayout.Button("Clear water"))
        {
            water.ClearWater();
        }

        DrawSettingsEditor(water.shapeSettings, water.OnShapeSettingsUpdated, ref water.shapeSettingsFoldout, ref shapeEditor);
        DrawSettingsEditor(water.waterSettings, water.OnWaterSettingsUpdated, ref water.waterSettingsFoldout, ref waterEditor);
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
        water = (Water)target;
    }
}
