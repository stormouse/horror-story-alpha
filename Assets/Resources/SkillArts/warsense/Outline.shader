// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Outlined/Cutout" 
{
	Properties
	{
		_Color("Color", Color) = (1.0, 0.0, 0.0)
		_EdgePow("EdgePow", float) = 3.0
	}

	SubShader{
		Tags { "Queue"="Geometry+100" }
		LOD 200
		
		Cull Off
		ZWrite Off
		ZTest Always
		Blend SrcAlpha One // Additive blending
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			/* -- input struct in UnityCG.cginc --
				struct appdata_base
				{
					float4 vertex   : POSITION;  // The vertex position in model space.
					float3 normal   : NORMAL;    // The vertex normal in model space.
					float4 texcoord : TEXCOORD0; // The first UV coordinate.
				};
			 */

			struct v2f {
				float4 pos : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			uniform float3 _Color;
			uniform float _EdgePow;

			v2f vert(appdata_base v){
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.normal = mul(UNITY_MATRIX_MV, normalize(v.normal));
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				float r = 1 - abs(i.normal.z);
				r = pow(r, _EdgePow);
				fixed4 col = fixed4(_Color, r);
				return col;
			}

			ENDCG
		}
	}
}