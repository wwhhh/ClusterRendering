#ifndef SHADOW
#define SHADOW

float4x4 _ShadowMatrixVP;
float _ShadowmapSize;
Texture2D _Shadowmap;
SamplerState sampler_Shadowmap;

float4 GetShadowCameraWorldPos(float4 worldPos) 
{
	float4 shadowWorldPos = mul(_ShadowMatrixVP, worldPos);
	return shadowWorldPos;
}

float GetShadow(float4 worldPos) 
{
	float4 ndcpos = GetShadowCameraWorldPos(worldPos);
	ndcpos.xyz /= ndcpos.w;
	float3 uvpos = ndcpos * 0.5 + 0.5;

	float4 shadowmap = _Shadowmap.Sample(sampler_Shadowmap, uvpos.xy);
	float shadowCameraDepth = DecodeFloatRGBA(shadowmap);

	return step(uvpos.z, shadowCameraDepth);
}

#endif