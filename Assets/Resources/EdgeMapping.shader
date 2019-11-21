Shader "Unlit/EdgeMapping"
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

			sampler2D _MainTex;
			float4    _MainTex_ST;
			float4    _MainTex_TexelSize;
			

			// =====================================================

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex); 
				return o;
			}
			

			fixed4 frag (v2f i) : SV_Target
			{ 
				float4  p = tex2D(_MainTex, i.uv);
			    if (abs(dot(p,(0.25).xxxx) - 1.) < 0.001) discard;

				float scale = 2.;

				float4 A = tex2D(_MainTex, i.uv.xy + _MainTex_TexelSize.xy*float2(-1,  1)*scale);
				float4 C = tex2D(_MainTex, i.uv.xy + _MainTex_TexelSize.xy*float2( 1,  1)*scale);
				float4 F = tex2D(_MainTex, i.uv.xy + _MainTex_TexelSize.xy*float2(-1, -1)*scale);
				float4 H = tex2D(_MainTex, i.uv.xy + _MainTex_TexelSize.xy*float2( 1, -1)*scale);
			    
				float normDis   = 1. - 0.5*(dot(A.xyz, H.xyz) + dot(C.xyz, F.xyz));
				float depthDis  = (1. - 0.5*abs(A.w - H.w))*(1. - 0.5*abs(A.w - H.w));
				      depthDis *= (1. - 0.5*abs(C.w - F.w))*(1. - 0.5*abs(C.w - F.w));
					  depthDis  = 1. - depthDis;           
				return float4(normDis, depthDis, depthDis*normDis,1.);
			}
			// =====================================================
			ENDCG
		}

		// ============================================================================================
		Pass
			{
				Blend One One
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

			sampler2D _MainTex;
			float4    _MainTex_ST;

			// =====================================================

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}


			fixed4 frag(v2f i) : SV_Target
			{
				return  tex2D(_MainTex, i.uv);
			}
				// =====================================================
				ENDCG
			}

	}
}
