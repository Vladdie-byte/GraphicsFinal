using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcess : MonoBehaviour
{

    private Material _material;
    public Shader _shader;
    private Matrix4x4 colorRatioMatrix;

    // Start is called before the first frame update
    void Start()
    {
        _material = new Material(_shader);
        colorRatioMatrix = Matrix4x4.identity;
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //Set uniform variables here
        //member varibles, private, etc.
        _material.SetMatrix("ColorScaleMatrix", colorRatioMatrix);

        Graphics.Blit(source, destination, _material);
    }

    public void calculateRatios(Vector3 colors)
    {
        Matrix4x4 colorMatrix = new Matrix4x4(new Vector4(1,0,0,0), new Vector4(0, 1, 0, 0), new Vector4(0, 0, 1, 0), new Vector4(0, 0, 0, 0));

        Vector4 R = new Vector4(colors.x, 0, 0, 0);
        Vector4 G = new Vector4(0, colors.y, 0, 0);
        Vector4 B = new Vector4(0, 0, colors.z, 0);

        Matrix4x4 S = new Matrix4x4(R,G,B,new Vector4(0,0,0,0));

        
        Matrix4x4 T = new Matrix4x4(new Vector4(0.31399022f, 0.15537241f, 0.01775239f, 0),
                                    new Vector4(0.63951294f, 0.75789446f, 0.10944209f, 0),
                                    new Vector4(0.04649755f, 0.08670142f, 0.87256922f, 0),
                                    new Vector4(0, 0, 0, 1));

        Matrix4x4 Tinverse = new Matrix4x4(new Vector4(5.47221206f, -1.1252419f, 0.02980165f,0),
                                           new Vector4(-4.6419601f, 2.29317094f, -0.19318073f, 0),
                                           new Vector4(0.16963708f, -0.1678952f, 1.16364789f, 0),
                                           new Vector4(0, 0, 0, 1));
        
        /*
        Matrix4x4 T = new Matrix4x4(new Vector4(0.31399022f, 0.63951294f, 0.04649755f, 0), 
                                    new Vector4(0.15537241f, 0.75789446f, 0.08670142f, 0), 
                                    new Vector4(0.01775239f, 0.10944209f, 0.87256922f, 0), 
                                    new Vector4(0, 0, 0, 0));
        Matrix4x4 Tinverse = new Matrix4x4(new Vector4( 5.47221206f, -4.6419601f, 0.16963708f ,0), 
                                           new Vector4( -1.1252419f, 2.29317094f, -0.1678952f ,0), 
                                           new Vector4( 0.02980165f, -0.19318073f, 1.16364789f ,0), 
                                           new Vector4(0,0,0,0));
        */
        Vector4 W = new Vector4(1,1,1,0);


        if ((colors.x != 0 || colors.y != 0) && (colors.x != 0 || colors.z != 0) && (colors.z != 0 || colors.y != 0))
        {
            if (colors.x != 1)
            {
                Vector4 blueBase = T * B;

                float q1 = (1 - colors.x) * (blueBase.x * W.z - W.x * blueBase.z) / (blueBase.y * W.z - W.y * blueBase.z);
                float q2 = (1 - colors.x) * (blueBase.x * W.y - W.x * blueBase.y) / (blueBase.z * W.y - W.z * blueBase.y);


                colorMatrix.SetRow(0, new Vector4(colors.x, q1, q2 , 0));
                Debug.Log($"{blueBase.x},{blueBase.y},{blueBase.z},{blueBase.w},-----------------------------,{q1},{q2}");
            }
            else
            {
                colorMatrix.SetRow(0,R);
            }
            if (colors.y != 1)
            {
                Vector4 blueBase = T * B;

                float q1 = (1 - colors.y) * (blueBase.y * W.z - W.y * blueBase.z) / (blueBase.x * W.z - W.y * blueBase.z);
                float q2 = (1 - colors.y) * (blueBase.y * W.y - W.x * blueBase.x) / (blueBase.z * W.y - W.z * blueBase.x);

                colorMatrix.SetRow(1,new Vector4 ( q1, colors.y, q2 , 0));
            }
            else
            {
                colorMatrix.SetRow(1,G);
            }
            if (colors.z != 1)
            {
                Vector4 redBase = T * R;

                float q1 = (1 - colors.z) * (redBase.z * W.z - W.x * redBase.x) / (redBase.y * W.z - W.y * redBase.x);
                float q2 = (1 - colors.z) * (redBase.z * W.y - W.x * redBase.y) / (redBase.x * W.y - W.z * redBase.y);

                colorMatrix.SetRow(2,new Vector4 ( q2, q1, colors.z , 0));
            }
            else
            {
                colorMatrix.SetRow(2,B);
            }
        }
        else if (colors.x != 0 && colors.y != 0 && colors.z != 0)
        {
            colorMatrix = new Matrix4x4(Vector4.zero,Vector4.zero,Vector4.zero,Vector4.zero);
        }
        else
        {
            if (colors.x > 0)
            {
                Vector4 MonoColor = new Vector4(0, 0, 0, 0);
                colorMatrix = new Matrix4x4(new Vector4(colors.x,colors.x,colors.x,0), MonoColor, MonoColor, new Vector4(0, 0, 0, 1));
            }
            else if (colors.y > 0)
            {
                Vector4 MonoColor = new Vector4(0, 0, 0, 0);
                colorMatrix = new Matrix4x4(MonoColor, new Vector4(colors.y, colors.y, colors.y, 0), MonoColor, new Vector4(0, 0, 0, 1));
            }
            else if (colors.z > 0)
            {
                Vector4 MonoColor = new Vector4(0, 0, 0, 0);
                colorMatrix = new Matrix4x4(MonoColor, MonoColor, new Vector4(colors.z, colors.z, colors.z, 0), new Vector4(0, 0, 0, 1));
            }
        }
        Matrix4x4 temporary = Tinverse * colorMatrix;
        colorRatioMatrix = temporary * T;
        Debug.Log(colorRatioMatrix);
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
        Matrix3x1 newMatrix = new Matrix3x1(0, 0, 0);
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

    public void MatrixMultiplicationf(float f)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Matrix[i, j] *= f;
            }
        }
    }
}

public struct Matrix3x1
{
    public float[] Matrix;
    public Matrix3x1(float R, float G, float B)
    {
        Matrix = new float[3] { R, G, B };
    }
    public void MatrixMultiplicationf(float f)
    {
        for (int i = 0; i < 3; i++)
        {
            Matrix[i] *= f;

        }
    }
}

