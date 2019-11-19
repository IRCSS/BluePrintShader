Shader "Unlit/DepthPeeler"
{

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			// =====================================================
			#pragma vertex   vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			// =====================================================

			struct appdata
			{
			//  ------ ------   ----------
				float4 vertex : POSITION;
				float2 uv     : TEXCOORD0;
				float4 normal : NORMAL;
			//  ------ ------   ----------
			};

			struct v2f
			{
			//  ------ ------   ----------
				float2 uv      : TEXCOORD0;
				float4 vertex  : SV_POSITION;
				float4 proj    : TEXCOORD2;
				half3  wNormal : TEXCOORD1;
			//  ------ ------   ----------
			};

			sampler2D _PreviusLayer;
			
			// =====================================================
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv     = v.uv;
				o.proj   = ComputeScreenPos(o.vertex);
				o.proj.z = COMPUTE_DEPTH_01;

				o.wNormal = UnityObjectToWorldNormal(v.normal);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col    = tex2D(_PreviusLayer, i.proj.xy/i.proj.w);
			    clip(i.proj.z - (col.b + 0.00001));
			           col.b  = i.proj.z;
					   
					   col.rg = normalize(i.wNormal).xy;
					
				return col;
			}

			// =====================================================
			ENDCG
		}
	}
}
