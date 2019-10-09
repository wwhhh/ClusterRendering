#ifndef SHADOW
#define SHADOW

#define SAMPLE_COUNT 128

#include "CGINC/Random.cginc"

float4x4 _ShadowMatrixVP;
Texture2D _Shadowmap;
SamplerState sampler_Shadowmap;
float _ShadowmapSize;
float _ShadowBias;
float _ShadowSoftValue;

float GetDirLightShadow(float4 worldPos)
{
	float4 shadowPos = mul(_ShadowMatrixVP, worldPos);
	shadowPos.xyz /= shadowPos.w;

	float2 textureCoordinates = (shadowPos.xy * 0.5 + 0.5);
	float2 seed = textureCoordinates;

	float atten = 0;
	float curDepth = shadowPos.z + _ShadowBias;

	for (int i = 0; i < SAMPLE_COUNT; i++) {
		seed = hash22(seed) * 2 - 1;
		float4 encode = _Shadowmap.Sample(sampler_Shadowmap, textureCoordinates + seed * _ShadowSoftValue / 1024);
		atten += DecodeFloatRGBA(encode) > curDepth;
	}

	atten /= SAMPLE_COUNT;
	return atten;
}

#endif