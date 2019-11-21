Shader "Unlit/CombineOnScreen"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
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
		    //  ------ ------   ----------
			};

			struct v2f
			{
		    //  ------ ------   ----------
				float2 uv     : TEXCOORD0;
				float4 vertex : SV_POSITION;
			//  ------ ------   ----------
			};

			// =====================================================
			sampler2D _MainTex;
			sampler2D _ForwadPass;
			float4    _MainTex_ST;
			float     _ForwardPassContribution;

			// =====================================================

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv     = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 edgeMap     = tex2D(_MainTex,    i.uv);
				fixed4 forwardPass = tex2D(_ForwadPass, i.uv);
				
				
				return  edgeMap + forwardPass* _ForwardPassContribution;
				return  lerp( float4(1.,1.,1.,1.), float4(0.,0.,0.,0.), 1.-edgeMap.z);
			}

			// =====================================================
			ENDCG
		}
	}
}
