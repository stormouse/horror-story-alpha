Shader "Hidden/CircleMask"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Mask("Mask", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _Mask;
			uniform float4 _MainTex_TexelSize;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 duv = i.uv;
				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					duv.y = 1 - duv.y;
				#endif
				fixed4 col = tex2D(_MainTex, duv);
				col.rgb *= tex2D(_Mask, duv).r;
				
				return col;
			}
			ENDCG
		}
	}
}
