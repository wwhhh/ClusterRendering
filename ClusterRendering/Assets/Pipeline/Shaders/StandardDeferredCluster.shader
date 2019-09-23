Shader "Instanced/deferredCluster"
{
    Properties
    {
		_Tint("Tint", Color) = (1, 1, 1, 1)
		_MainTex("Albedo", 2D) = "white" {}

		[NoScaleOffset] _NormalMap("Normals", 2D) = "bump" {}
		_BumpScale("Bump Scale", Float) = 1

		[NoScaleOffset] _MetallicMap("Metallic", 2D) = "white" {}
		[Gamma] _Metallic("Metallic用于Specular与Albedo插值", Range(0, 1)) = 0
		_Smoothness("Smoothness", Range(0, 1)) = 0.1

		[NoScaleOffset] _OcclusionMap("Occlusion", 2D) = "white" {}
		_OcclusionStrength("Occlusion Strength", Range(0, 1)) = 1

		[NoScaleOffset] _EmissionMap("Emission", 2D) = "black" {}
		_Emission("Emission", Color) = (0, 0, 0)

		[NoScaleOffset] _DetailMask("Detail Mask", 2D) = "white" {}
		_DetailTex("Detail Albedo", 2D) = "gray" {}
		[NoScaleOffset] _DetailNormalMap("Detail Normals", 2D) = "bump" {}
		_DetailBumpScale("Detail Bump Scale", Float) = 1

		_AlphaCutoff("Alpha Cutoff", Range(0, 1)) = 0.5
    }

	CGINCLUDE

	#define BINORMAL_PER_FRAGMENT
	#define DEFERRED_PASS

	ENDCG

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_cluster
            #pragma fragment frag_deferredLighting
			#pragma target 4.5

			#include "CGINC/Cluster.cginc"
			#include "CGINC/DeferredRender.cginc"

            ENDCG
        }
    }
}