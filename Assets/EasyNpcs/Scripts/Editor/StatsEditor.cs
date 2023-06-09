using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AI_Package;

[CustomEditor(typeof(AI_Stats))]
public class StatsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        AI_Stats stats = (AI_Stats)target;
        SerializedProperty weapon = serializedObject.FindProperty("assignedWeapon");
        if (weapon.intValue == 1)
        {
            stats.projectile = (Projectile)EditorGUILayout.ObjectField("Projectile", stats.projectile, typeof(Projectile), true);
            stats.launchHight = EditorGUILayout.FloatField("Launch Hight", stats.launchHight);
        }
    }
}
