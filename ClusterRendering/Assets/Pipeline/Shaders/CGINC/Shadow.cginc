#ifndef SHADOW
#define SHADOW

float4x4 _ShadowMatrixVP;
Texture2D _Shadowmap;
SamplerComparisonState sampler_Shadowmap;

float GetShadow(float4 shadowCameraWorldPos)
{
	float3 projCoords = shadowCameraWorldPos.xyz / shadowCameraWorldPos.w;
	projCoords = projCoords * 0.5 + 0.5;

	// 硬件自动PCF
	float shadow = _Shadowmap.SampleCmpLevelZero(sampler_Shadowmap, projCoords.xy, projCoords.z);
	return shadow;
}

float4 GetShadowCameraWorldPos(float4 worldPos) 
{
	float4 shadowWorldPos = mul(_ShadowMatrixVP, worldPos);
	return shadowWorldPos;
}

#endif