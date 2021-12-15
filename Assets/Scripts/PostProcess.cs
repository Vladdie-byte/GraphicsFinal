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
        Matrix4x4 colorMatrix = new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0), new Vector4(0, 0, 1, 0), new Vector4(0, 0, 0, 0));

        Vector4 R = new Vector4(colors.x, 0, 0, 0);
        Vector4 G = new Vector4(0, colors.y, 0, 0);
        Vector4 B = new Vector4(0, 0, colors.z, 0);

        Matrix4x4 S = new Matrix4x4(R, G, B, new Vector4(0, 0, 0, 0));


        Matrix4x4 T = new Matrix4x4(new Vector4(0.31399022f, 0.15537241f, 0.01775239f, 0),
                                    new Vector4(0.63951294f, 0.75789446f, 0.10944209f, 0),
                                    new Vector4(0.04649755f, 0.08670142f, 0.87256922f, 0),
                                    new Vector4(0, 0, 0, 1));

        Matrix4x4 Tinverse = new Matrix4x4(new Vector4(5.47221206f, -1.1252419f, 0.02980165f, 0),
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
        Vector4 W = new Vector4(1, 1, 1, 0);


        if ((colors.x != 0 || colors.y != 0) && (colors.x != 0 || colors.z != 0) && (colors.z != 0 || colors.y != 0))
        {
            if (colors.x != 1)
            {
                Vector4 blueBase = T * B;

                float q1 = (1 - colors.x) * (blueBase.x * W.z - W.x * blueBase.z) / (blueBase.y * W.z - W.y * blueBase.z);
                float q2 = (1 - colors.x) * (blueBase.x * W.y - W.x * blueBase.y) / (blueBase.z * W.y - W.z * blueBase.y);


                colorMatrix.SetRow(0, new Vector4(colors.x, q1, q2, 0));
                Debug.Log($"{blueBase.x},{blueBase.y},{blueBase.z},{blueBase.w},-----------------------------,{q1},{q2}");
            }
            else
            {
                colorMatrix.SetRow(0, R);
            }
            if (colors.y != 1)
            {
                Vector4 blueBase = T * B;

                float q1 = (1 - colors.y) * (blueBase.y * W.z - W.y * blueBase.z) / (blueBase.x * W.z - W.y * blueBase.z);
                float q2 = (1 - colors.y) * (blueBase.y * W.y - W.x * blueBase.x) / (blueBase.z * W.y - W.z * blueBase.x);

                colorMatrix.SetRow(1, new Vector4(q1, colors.y, q2, 0));
            }
            else
            {
                colorMatrix.SetRow(1, G);
            }
            if (colors.z != 1)
            {
                Vector4 redBase = T * R;

                float q1 = (1 - colors.z) * (redBase.z * W.z - W.x * redBase.x) / (redBase.y * W.z - W.y * redBase.x);
                float q2 = (1 - colors.z) * (redBase.z * W.y - W.x * redBase.y) / (redBase.x * W.y - W.z * redBase.y);

                colorMatrix.SetRow(2, new Vector4(q2, q1, colors.z, 0));
            }
            else
            {
                colorMatrix.SetRow(2, B);
            }
        }
        else if (colors.x != 0 && colors.y != 0 && colors.z != 0)
        {
            colorMatrix = new Matrix4x4(Vector4.zero, Vector4.zero, Vector4.zero, Vector4.zero);
        }
        else
        {
            if (colors.x > 0)
            {
                Vector4 MonoColor = new Vector4(0, 0, 0, 0);
                colorMatrix = new Matrix4x4(new Vector4(colors.x, colors.x, colors.x, 0), MonoColor, MonoColor, new Vector4(0, 0, 0, 1));
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
    }
}