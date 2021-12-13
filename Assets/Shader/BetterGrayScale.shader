Shader "Unlit/GrayScale"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			Matrix ColorScaleMatrix;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
			//fixed4 redOnly = fixed4(col.y,col.x,col.z,col.w);
			//float intensity = (col.x * 0.299) + (col.y * 0.587) + (col.z * 0.114);
			//fixed4 bandw = fixed4(intensity,intensity,intensity,col.w);
			fixed4 bandw = fixed4((col.x * ColorScaleMatrix[0][0] + col.y * ColorScaleMatrix[0][1] + col.z * ColorScaleMatrix[0][2]), (col.x * ColorScaleMatrix[1][0] + col.y * ColorScaleMatrix[1][1] + col.z * ColorScaleMatrix[1][2]), (col.x * ColorScaleMatrix[2][0] + col.y * ColorScaleMatrix[2][1] + col.z * ColorScaleMatrix[2][2]), col.w);
			// apply fog
			//UNITY_APPLY_FOG(i.fogCoord, col);
			return bandw;
		}
		ENDCG
	}
	}
}
