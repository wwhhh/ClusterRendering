#ifndef STANDARD_PBR
#define STANDARD_PBR

#include "UnityCG.cginc"

half4 StandardPBR(half3 diffColor, half3 specColor, half oneMinusReflectivity, half smoothness,
	float3 normal, float3 viewDir,
	UnityLight light) 
{
	float roughness =  (1 - smoothness);
	float3 halfDir = Unity_SafeNormalize(float3(light.dir) + viewDir);
	half nv = abs(dot(normal, viewDir));
	half nl = saturate(dot(normal, light.dir));
	float nh = saturate(dot(normal, halfDir));
	half lv = saturate(dot(light.dir, viewDir));
	half lh = saturate(dot(light.dir, halfDir));

	half diffuseTerm = DisneyDiffuse(nv, nl, lh, roughness) * nl;

	half3 color = diffColor * (light.color * diffuseTerm) + diffColor * 0.02f;

	return half4(color, 1);

}

#endif