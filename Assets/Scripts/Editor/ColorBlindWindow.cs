using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ColorBlindWindow : EditorWindow
{
    private Vector3 colors = new Vector3(1,1,1);
    private bool groupEnabled;

    [MenuItem ("Window/MY EYES")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ColorBlindWindow));
        Debug.Log("!cry");
    }

    public void OnGUI()
    {
        GUILayout.Label("Base Settings",EditorStyles.boldLabel);
        if (GUILayout.Button("RED BLIND"))
        {
            colors = new Vector3(0,1,1);
        }
        if (GUILayout.Button("GREEN BLIND"))
        {
            colors = new Vector3(1,0,1);
        }
        if (GUILayout.Button("BLUE BLIND"))
        {
            colors = new Vector3(1,1,0);
        }
        if (GUILayout.Button("RED MONOCHROMATIC"))
        {
            colors = new Vector3(1,0,0);
        }
        if (GUILayout.Button("GREEN MONOCHROMATIC"))
        {
            colors = new Vector3(0,1,0);
        }
        if (GUILayout.Button("BLUE MONOCHROMATIC"))
        {
            colors = new Vector3(0,0,1);
        }
        groupEnabled = EditorGUILayout.BeginToggleGroup ("Color Scales", groupEnabled);
            colors.x = EditorGUILayout.Slider("Red", colors.x,0,1);
            colors.y = EditorGUILayout.Slider("Green", colors.y,0,1);
            colors.z = EditorGUILayout.Slider("Blue", colors.z,0,1);
        EditorGUILayout.EndToggleGroup();
    }

    private Vector4 calculateRatios()
    {
        return new Vector4();
    }
}
