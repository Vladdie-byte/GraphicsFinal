using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcess : MonoBehaviour
{

    private Material _material;
    public Shader _shader;
    private Matrix4x4 colorMatrix;
    private float percent;

    // Start is called before the first frame update
    void Start()
    {
        _material = new Material(_shader);
        colorMatrix = Matrix4x4.identity;
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //Set uniform variables here
        //member varibles, private, etc.
        _material.SetMatrix("ColorScaleMatrix", colorMatrix);
        _material.SetFloat("maxColor", percent);

        Graphics.Blit(source,destination,_material);
    }

    public void setColorMatrix(Matrix4x4 mat, float value)
    {
        colorMatrix=mat;
        percent=value;
    }

}
