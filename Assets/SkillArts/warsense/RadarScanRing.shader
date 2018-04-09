Shader "Hidden/RadarScanRing"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_ringColor("Color", Color) = (0.6, 0.0, 0.0, 0.8)
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
				float3 ray : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _ringColor;
			uniform float radius;
			uniform float thickness;
			uniform float3 ringCenter;
			uniform float3 cameraPositionWS;
			uniform float4x4 cameraToWorldMatrix;
			uniform float4x4 _FrustumCornersES;
			uniform sampler2D _CameraDepthTexture;
			uniform float4 _MainTex_TexelSize;


			v2f vert (appdata v)
			{
				v2f o;

				half index = v.vertex.z;
				v.vertex.z = 0.1;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv.y = 1 - o.uv.y;
				#endif

				// Get the eyespace view ray (normalized)
				o.ray = _FrustumCornersES[(int)index].xyz;
				o.ray = mul(cameraToWorldMatrix, o.ray);
				return o;
			}
			

			fixed4 frag (v2f i) : SV_Target
			{
				float3 rd = normalize(i.ray.xyz);
				float3 ro = cameraPositionWS;

				float2 duv = i.uv;
				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					duv.y = 1 - duv.y;
				#endif
				
				float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, duv).r);
				depth *= length(i.ray.xyz);

				float3 v = ro + rd * depth;

				fixed4 col = tex2D(_MainTex, i.uv);
				
				float dx = v.x - ringCenter.x;
				float dz = v.z - ringCenter.z;
				//float d = abs(sqrt(dx*dx + dz*dz) - radius);
				//float r = d / thickness;
				//r = min(r*r*r, 1.0);
				//fixed4 add = lerp(_ringColor, float4(0, 0, 0, 0), r);
				float r = sqrt(dx*dx + dz*dz) / radius;
				if (r > 1.0) 
					r = 0.0;
				r = pow(r, 4);
				fixed4 add = lerp(float4(0, 0, 0, 0), _ringColor, r);
				
				return fixed4(col*(1.0-add.w) + add.xyz * add.w, 1.0);
			}
			ENDCG
		}
	}
}
