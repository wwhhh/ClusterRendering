#ifndef SHADOW
#define SHADOW

#define SAMPLE_COUNT 128

#include "CGINC/Random.cginc"
#pragma require 2darray

UNITY_DECLARE_TEX2DARRAY(_Shadowmap);

Texture2D _Shadowmap0; SamplerState sampler_Shadowmap0;
Texture2D _Shadowmap1; SamplerState sampler_Shadowmap1;
Texture2D _Shadowmap2; SamplerState sampler_Shadowmap2;
Texture2D _Shadowmap3; SamplerState sampler_Shadowmap3;

float4x4 _ShadowMatrixVP;
float _ShadowmapSize;
float _ShadowBias;
float _ShadowSoftValue;

float GetDirLightShadow(float4 worldPos)
{
	float4 shadowPos = mul(_ShadowMatrixVP, worldPos);
	shadowPos.xyz /= shadowPos.w;

	float2 shadowUV = (shadowPos.xy * 0.5 + 0.5);
	float2 seed = shadowUV;

	float atten = 0;
	float curDepth = shadowPos.z + _ShadowBias;

	float ShadowMapDistance = UNITY_SAMPLE_TEX2DARRAY(_Shadowmap, float3(shadowUV, 0));

	float4 encode0 = _Shadowmap0.Sample(sampler_Shadowmap0, shadowUV);
	//float4 encode1 = _Shadowmap1.Sample(sampler_Shadowmap1, shadowUV);
	//float4 encode2 = _Shadowmap2.Sample(sampler_Shadowmap2, shadowUV);
	//float4 encode3 = _Shadowmap3.Sample(sampler_Shadowmap3, shadowUV);
	atten += DecodeFloatRGBA(encode0);
	//atten += DecodeFloatRGBA(encode1);
	//atten += DecodeFloatRGBA(encode2);
	//atten += DecodeFloatRGBA(encode3);
	
	//atten /= 4;

	return atten;
}

#endif