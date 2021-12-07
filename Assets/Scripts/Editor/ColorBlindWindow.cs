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

        private Vector3 colors = new Vector3(0, 0, 0);
        private bool groupEnabled;
        [MenuItem("Window/MY EYES")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ColorBlindWindow));
        }

        public void OnGUI()
        {
            CameraPostProcessing = Camera.main.GetComponentInParent<PostProcess>();

            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            if (GUILayout.Button("RED BLIND"))
            {
                colors = new Vector3(0, 1, 1);
                CameraPostProcessing.setColorMatrix(calculateRatios());
            }
            if (GUILayout.Button("GREEN BLIND"))
            {
                colors = new Vector3(1, 0, 1);
                CameraPostProcessing.setColorMatrix(calculateRatios());
            }
            if (GUILayout.Button("BLUE BLIND"))
            {
                colors = new Vector3(1, 1, 0);
                CameraPostProcessing.setColorMatrix(calculateRatios());
            }
            if (GUILayout.Button("RED MONOCHROMATIC"))
            {
                colors = new Vector3(1, 0, 0);
                CameraPostProcessing.setColorMatrix(calculateRatios());
            }
            if (GUILayout.Button("GREEN MONOCHROMATIC"))
            {
                colors = new Vector3(0, 1, 0);
                CameraPostProcessing.setColorMatrix(calculateRatios());
            }
            if (GUILayout.Button("BLUE MONOCHROMATIC"))
            {
                colors = new Vector3(0, 0, 1);
                CameraPostProcessing.setColorMatrix(calculateRatios());
            }
            groupEnabled = EditorGUILayout.BeginToggleGroup("Color Scales", groupEnabled);
            colors.x = EditorGUILayout.Slider("Red", colors.x, 0, 1 );
            colors.y = EditorGUILayout.Slider("Green", colors.y, 0, 1);
            colors.z = EditorGUILayout.Slider("Blue", colors.z, 0, 1);
            EditorGUILayout.EndToggleGroup();

            if (GUILayout.Button("Update"))
            {
                CameraPostProcessing.setColorMatrix(calculateRatios());
            }
        }

        public void OnSlider()
        {
            CameraPostProcessing.setColorMatrix(calculateRatios());
        }

        private Matrix4x4 calculateRatios()
        {
            Matrix3x3 colorMatrix = new Matrix3x3(new float[3, 3] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } });

            Matrix3x1 R = new Matrix3x1(colors.x, 0, 0);
            Matrix3x1 G = new Matrix3x1(0, colors.y, 0);
            Matrix3x1 B = new Matrix3x1(0, 0, colors.z);

            Matrix3x3 S = new Matrix3x3(new float[3, 3] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } });
            S.newColor(R.Matrix, 0);
            S.newColor(G.Matrix, 1);
            S.newColor(B.Matrix, 2);

            Matrix3x3 T = new Matrix3x3(new float[3, 3] { { 0.31399022f, 0.63951294f, 0.04649755f }, { 0.15537241f, 0.75789446f, 0.08670142f }, { 0.01775239f, 0.10944209f, 0.87256922f } });
            Matrix3x3 Tinverse = new Matrix3x3(new float[3, 3] { { 5.47221206f, -4.6419601f, 0.16963708f }, { -1.1252419f, 2.29317094f, -0.1678952f }, { 0.02980165f, -0.19318073f, 1.16364789f } });
            Matrix3x1 W = new Matrix3x1(1, 1, 1);

            
            if ((colors.x != 0 || colors.y != 0) && (colors.x != 0 || colors.z != 0) && (colors.z != 0 || colors.y != 0))
            {
                if (colors.x != 1)
                {
                    Matrix3x1 blueBase = T.MatrixMultiplication3x1(new Matrix3x1(0, 0, 1));

                    float q1 = (1 - colors.x) * (blueBase.Matrix[0] * W.Matrix[2] - W.Matrix[0] * blueBase.Matrix[2]) / (blueBase.Matrix[1] * W.Matrix[2] - W.Matrix[1] * blueBase.Matrix[2]);
                    float q2 = (1 - colors.x) * (blueBase.Matrix[0] * W.Matrix[1] - W.Matrix[0] * blueBase.Matrix[1]) / (blueBase.Matrix[2] * W.Matrix[1] - W.Matrix[2] * blueBase.Matrix[1]);

                    
                    colorMatrix.newColor(new float[] { colors.x, q1, q2 }, 0);
                }
                else
                {
                    colorMatrix.newColor(R.Matrix, 0);
                }
                if (colors.y != 1)
                {
                    Matrix3x1 blueBase = T.MatrixMultiplication3x1(new Matrix3x1(0, 0, 1));

                    float q1 = (1 - colors.y) * (blueBase.Matrix[1] * W.Matrix[2] - W.Matrix[1] * blueBase.Matrix[2]) / (blueBase.Matrix[0] * W.Matrix[2] - W.Matrix[1] * blueBase.Matrix[2]);
                    float q2 = (1 - colors.y) * (blueBase.Matrix[1] * W.Matrix[1] - W.Matrix[0] * blueBase.Matrix[0]) / (blueBase.Matrix[2] * W.Matrix[1] - W.Matrix[2] * blueBase.Matrix[0]);

                    colorMatrix.newColor(new float[] { q1, colors.y, q2 }, 1);
                }
                else
                {
                    colorMatrix.newColor(G.Matrix, 1);
                }
                if (colors.z != 1)
                {
                    Matrix3x1 redBase = T.MatrixMultiplication3x1(new Matrix3x1(1, 0, 0));

                    float q1 = (1 - colors.z) * (redBase.Matrix[2] * W.Matrix[2] - W.Matrix[0] * redBase.Matrix[0]) / (redBase.Matrix[1] * W.Matrix[2] - W.Matrix[1] * redBase.Matrix[0]);
                    float q2 = (1 - colors.z) * (redBase.Matrix[2] * W.Matrix[1] - W.Matrix[0] * redBase.Matrix[1]) / (redBase.Matrix[0] * W.Matrix[1] - W.Matrix[2] * redBase.Matrix[1]);

                    colorMatrix.newColor(new float[] { q2, q1, colors.z }, 2);
                }
                else
                {
                    colorMatrix.newColor(B.Matrix, 2);
                }
            }
            else if (colors.x != 0 && colors.y != 0 && colors.z != 0)
            {
                //full colorblind
                //(col.x * 0.299) + (col.y * 0.587) + (col.z * 0.114);
            }
            else
            {
                Matrix3x3 temp = new Matrix3x3();
                if (colors.x != 0)
                {
                    temp = Tinverse.MatrixMultiplication3x3(new Matrix3x3(new float[3, 3] { { colors.x, 0f, 0f }, { colors.x, 0f, 0f }, { colors.x, 0f, 0f } }));
                    temp = temp.MatrixMultiplication3x3(T);
                }
                else if (colors.y != 0)
                {
                    temp = Tinverse.MatrixMultiplication3x3(new Matrix3x3(new float[3, 3] { { 0f, colors.y, 0f }, { 0f, colors.y, 0f }, { 0f, colors.y, 0f } }));
                }
                else if (colors.z != 0)
                {
                    temp = Tinverse.MatrixMultiplication3x3(new Matrix3x3(new float[3, 3] { { 0f, 0f, colors.z, }, { 0f, 0f, colors.z, }, { 0f, 0f, colors.z, } }));
                }
                temp = temp.MatrixMultiplication3x3(T);
                //colorMatrix = convertMatrix(new Matrix3x1(temp.Matrix[0, 0], temp.Matrix[1, 1], temp.Matrix[2, 2]));
            }

            Debug.Log(colorMatrix.StringThing());

            Matrix3x3 temporary = Tinverse.MatrixMultiplication3x3(colorMatrix);
            temporary = temporary.MatrixMultiplication3x3(T);
            return convertMatrix(temporary);
            
        }
        private Matrix4x4 convertMatrix(Matrix3x3 mat)
        {
            Matrix4x4 newMat = new Matrix4x4();
            newMat.m00 = mat.Matrix[0, 0];
            newMat.m01 = mat.Matrix[0, 1];
            newMat.m02 = mat.Matrix[0, 2];
            newMat.m10 = mat.Matrix[1, 0];
            newMat.m11 = mat.Matrix[1, 1];
            newMat.m12 = mat.Matrix[1, 2];
            newMat.m20 = mat.Matrix[2, 0];
            newMat.m21 = mat.Matrix[2, 1];
            newMat.m22 = mat.Matrix[2, 2];
            newMat.m33 = 1f;
            return newMat;
        }
        private Matrix4x4 convertMatrix(Matrix3x1 mat)
        {
            Matrix4x4 newMat = new Matrix4x4();
            newMat.m00 = mat.Matrix[0];
            newMat.m11 = mat.Matrix[1];
            newMat.m22 = mat.Matrix[2];
            newMat.m33 = 1f;
            return newMat;
        }
    }

    public struct Matrix3x3
    {
        public float[,] Matrix;
        public Matrix3x3(float[,] values)
        {
            Matrix = values;
        }

        public void newColor(float[] colorRow, int colorIndex)
        {
            for (int i = 0; i < 3; i++)
            {
                Matrix[colorIndex, i] = colorRow[i];
            }
        }

        public float[] getColor(int colorIndex)
        {
            float[] color = new float[] { Matrix[colorIndex, 0], Matrix[colorIndex, 1], Matrix[colorIndex, 2] };
            return color;
        }

        public string StringThing()
        {
            return $"[{Matrix[0, 0]},{Matrix[0, 1]},{Matrix[0, 2]}],[{Matrix[1, 0]},{Matrix[1, 1]},{Matrix[1, 2]}],[{Matrix[2, 0]},{Matrix[2, 1]},{Matrix[2, 2]}]";
        }

        public Matrix3x3 MatrixMultiplication3x3(Matrix3x3 otherMatrix)
        {
            Matrix3x3 newMatrix = new Matrix3x3(new float[3, 3] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } });
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    float temp = 0;
                    for (int k = 0; k < 3; k++)
                    {
                        temp += Matrix[i, k] * otherMatrix.Matrix[k, j];
                    }
                    newMatrix.Matrix[i, j] = temp;
                }
            }
            return newMatrix;
        }
        public Matrix3x1 MatrixMultiplication3x1(Matrix3x1 otherMatrix)
        {
            Matrix3x1 newMatrix = new Matrix3x1(0,0,0);
            for (int i = 0; i < 3; i++)
            {
                float temp = 0;
                for (int k = 0; k < 3; k++)
                {
                    temp += Matrix[i, k] * otherMatrix.Matrix[k];
                }
                newMatrix.Matrix[i] = temp;
            }
            return newMatrix;
        }
    }

    public struct Matrix3x1
    {
        public float[] Matrix;
        public Matrix3x1(float R, float G, float B)
        {
            Matrix = new float[3] { R, G, B };
        }
    }
}
