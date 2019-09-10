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
            #pragma vertex vert_cluster
            #pragma fragment frag
			#pragma target 4.5

			#include "UnityCG.cginc"
			#include "UnityDeferredLibrary.cginc"
			#include "UnityPBSLighting.cginc"
			#include "UnityStandardUtils.cginc"
			#include "UnityGBuffer.cginc"
			#include "UnityStandardBRDF.cginc"
			#include "CGINC/Cluster.cginc"

			float4x4 _InvVP;
			float3 _CurLightDir;
			float3 _CurLightColor;

            fixed4 frag (clusterdata i) : SV_Target
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