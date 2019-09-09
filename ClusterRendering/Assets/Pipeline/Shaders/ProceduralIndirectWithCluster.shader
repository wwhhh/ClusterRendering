Shader "Instanced/ProcedualWithCluster"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma target 4.5

			#include "UnityCG.cginc"
			#include "UnityDeferredLibrary.cginc"
			#include "UnityPBSLighting.cginc"
			#include "UnityStandardUtils.cginc"
			#include "UnityGBuffer.cginc"
			#include "UnityStandardBRDF.cginc"
			#include "CGINC/Cluster.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float3 wpos : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 tangent : TEXCOORD2;
				float2 uv0 : TEXCOORD3;
            };

			float4x4 _InvVP;
			float3 _CurLightDir;
			float3 _CurLightColor;

            v2f vert (uint vertexID : SV_VertexID, uint instanceID : SV_InstanceID)
            {
				Point p = GetPoint(vertexID, instanceID);

				v2f o;
				o.vertex = mul(UNITY_MATRIX_VP, float4(p.vertex, 1.0f));
				o.wpos = p.vertex;
				o.normal = p.normal;
				o.tangent = p.tangent;
				o.uv0 = p.uv0;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float4 wpos = float4(i.wpos, 1);
				float3 viewDir = normalize(_WorldSpaceCameraPos - wpos);
				float3 eyeVec = normalize(wpos.xyz - _WorldSpaceCameraPos);

                return float4(i.normal, 1);
            }
            ENDCG
        }
    }
}