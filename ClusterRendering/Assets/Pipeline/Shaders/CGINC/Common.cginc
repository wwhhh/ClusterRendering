#ifndef COMMON
#define COMMON

#include "UnityCG.cginc"

float4 _Tint;
sampler2D _MainTex, _DetailTex, _DetailMask;
float4 _MainTex_ST, _DetailTex_ST;

sampler2D _NormalMap, _DetailNormalMap;
float _BumpScale, _DetailBumpScale;

sampler2D _MetallicMap;
float _Metallic;
float _Smoothness;

sampler2D _OcclusionMap;
float _OcclusionStrength;

sampler2D _EmissionMap;
float3 _Emission;

float _AlphaCutoff;

struct Interpolators {
	float4 pos : SV_POSITION;
	float4 uv : TEXCOORD0;
	float3 normal : TEXCOORD1;

#if defined(BINORMAL_PER_FRAGMENT)
	float4 tangent : TEXCOORD2;
#else
	float3 tangent : TEXCOORD2;
	float3 binormal : TEXCOORD3;
#endif
	float3 worldPos : TEXCOORD4;
};

#endif