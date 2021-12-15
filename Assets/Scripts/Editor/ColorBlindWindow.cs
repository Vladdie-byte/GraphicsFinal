using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Mathematics;

namespace Unity.Mathematics
{
    public class ColorBlindWindow : EditorWindow
    {
        [SerializeField] PostProcess CameraPostProcessing;

        private static Vector3 colors = new Vector3(1, 1, 1);
        private static Vector3 colors2 = new Vector3(1, 1, 1);
        private bool groupEnabled = false;
        [MenuItem("Window/Color Blindness")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ColorBlindWindow));
            colors2 = colors;
        }

        public void OnGUI()
        {
            CameraPostProcessing = Camera.main.GetComponentInParent<PostProcess>();

            GUILayout.Label("Trichromatism", EditorStyles.boldLabel);
            if (GUILayout.Button("protanomalous"))
            {
                colors = new Vector3(.33f, 1, 1);
            }
            if (GUILayout.Button("deuteranomalous"))
            {
                colors = new Vector3(1, .33f, 1);
            }
            if (GUILayout.Button("tritanomalous"))
            {
                colors = new Vector3(1, 1, .33f);
            }
            GUILayout.Label("Dichromatism", EditorStyles.boldLabel);
            if (GUILayout.Button("protanopia"))
            {
                colors = new Vector3(0, 1, 1);
            }
            if (GUILayout.Button("deuteranopia"))
            {
                colors = new Vector3(1, 0, 1);
            }
            if (GUILayout.Button("tritanopia"))
            {
                colors = new Vector3(1, 1, 0);
            }
            GUILayout.Label("Monochromatism", EditorStyles.boldLabel);
            if (GUILayout.Button("RED"))
            {
                colors = new Vector3(1, 0, 0);
            }
            if (GUILayout.Button("GREEN"))
            {
                colors = new Vector3(0, 1, 0);
            }
            if (GUILayout.Button("BLUE"))
            {
                colors = new Vector3(0, 0, 1);
            }
            groupEnabled = EditorGUILayout.BeginToggleGroup("Color Scales", groupEnabled);
            colors.x = EditorGUILayout.Slider("Red", colors.x, 0, 1);
            colors.y = EditorGUILayout.Slider("Green", colors.y, 0, 1);
            colors.z = EditorGUILayout.Slider("Blue", colors.z, 0, 1);
            EditorGUILayout.EndToggleGroup();
            if (colors2 != colors)
            {
                CameraPostProcessing.calculateRatios(colors);
            }
            colors2 = colors;
        }


    }
}
