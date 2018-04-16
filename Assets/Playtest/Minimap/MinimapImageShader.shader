Shader "Hidden/MinimapImageShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DotSize("DotSize", Float) = 0.01
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
			float _DotSize;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				
				float sqrd = (i.uv.x - 0.5)*(i.uv.x - 0.5) + (i.uv.y - 0.5) * (i.uv.y - 0.5);

				if (sqrd < _DotSize * _DotSize) {
					return col * 0.2 + fixed4(1, 0.05, 0, 0.8);
				}

				fixed x = i.uv.x;
				fixed y = i.uv.y;

				if (y > 0.5 && y > x && y > 1.0 - x) {
					return col + fixed4(0, 0.1, 0, 0);
				}

				return col;
			}
			ENDCG
		}
	}
}
